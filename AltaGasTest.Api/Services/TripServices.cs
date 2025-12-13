using AltaGasTest.Data.Entities;
using AltaGasTest.Data.Repository;
using System.Globalization;

namespace AltaGasTest.Api.Services
{
    /// <summary>
    /// Service for processing equipment events and converting them into trip records.
    /// Handles CSV parsing, timezone conversion, and trip creation logic.
    /// </summary>
    public class TripServices : ITripServices
    {
        private readonly ICanadianCityRepository _canadianCityRepository;
        private readonly ILogger<TripServices> _logger;

        // Event codes for equipment status
        private const string EventCodeReleased = "W";
        private const string EventCodePlaced = "Z";
        private const int MinimumCsvColumns = 4;

        public TripServices(
            IEquipmentEventRepository equipmentEventRepository,
            ICanadianCityRepository canadianCityRepository,
            ILogger<TripServices> logger)
        {
            _canadianCityRepository = canadianCityRepository ?? throw new ArgumentNullException(nameof(canadianCityRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes equipment events from a CSV file and creates trip records.
        /// </summary>
        /// <param name="file">CSV file with equipment events.</param>
        /// <returns>List of processed trips.</returns>
        public async Task<List<EquipmentEvent>> ProcessEquipmentEventsAsync(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            try
            {
                _logger.LogInformation("Starting equipment events processing for file: {FileName}", file.FileName);

                var canadianCities = await _canadianCityRepository.GetAllAsync();
                if (canadianCities == null || !canadianCities.Any())
                {
                    _logger.LogWarning("No Canadian cities found in database");
                    return new List<EquipmentEvent>();
                }

                var events = await ParseCsvEventsAsync(file, canadianCities.ToList());

                if (!events.Any())
                {
                    _logger.LogInformation("No valid events parsed from file");
                    return new List<EquipmentEvent>();
                }

                _logger.LogInformation("Successfully processed {EventCount}", events.Count);
                return events;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid CSV format in file: {FileName}", file.FileName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing equipment events from file: {FileName}", file.FileName);
                throw;
            }
        }

        /// <summary>
        /// Parses CSV events from the file stream.
        /// </summary>
        private async Task<List<EquipmentEvent>> ParseCsvEventsAsync(
            IFormFile file,
            List<CanadianCity> canadianCities)
        {
            var events = new List<EquipmentEvent>();

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                {
                    var headerLine = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        throw new InvalidOperationException("CSV file is empty or missing header.");
                    }

                    int lineNumber = 1;
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        var line = await reader.ReadLineAsync();

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        if (TryParseEventLine(line, canadianCities, out var equipmentEvent, lineNumber))
                        {
                            if (equipmentEvent != null)
                            {
                                events.Add(equipmentEvent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error reading CSV stream");
                throw;
            }

            return events;
        }

        /// <summary>
        /// Attempts to parse a single CSV line into an equipment event.
        /// </summary>
        private bool TryParseEventLine(
            string line,
            List<CanadianCity> canadianCities,
            out EquipmentEvent? equipmentEvent,
            int lineNumber)
        {
            equipmentEvent = null;

            var parts = line.Split(',');
            if (parts.Length < MinimumCsvColumns)
            {
                _logger.LogWarning("Line {LineNumber}: Insufficient columns. Expected {Expected}, found {Actual}",
                    lineNumber, MinimumCsvColumns, parts.Length);
                return false;
            }

            var equipmentId = parts[0].Trim();
            var eventCode = parts[1].Trim();
            var eventTimeLocalStr = parts[2].Trim();
            var cityIdStr = parts[3].Trim();

            // Validate event code
            if (!IsValidEventCode(eventCode))
            {
                _logger.LogDebug("Line {LineNumber}: Invalid event code '{EventCode}'", lineNumber, eventCode);
                return false;
            }

            // Validate city ID
            if (!int.TryParse(cityIdStr, out var cityId))
            {
                _logger.LogDebug("Line {LineNumber}: Invalid city ID '{CityId}'", lineNumber, cityIdStr);
                return false;
            }

            // Validate date/time
            if (!DateTime.TryParse(eventTimeLocalStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventLocal))
            {
                _logger.LogDebug("Line {LineNumber}: Invalid date/time '{DateTime}'", lineNumber, eventTimeLocalStr);
                return false;
            }

            // Find city and timezone
            var city = canadianCities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                _logger.LogDebug("Line {LineNumber}: City ID {CityId} not found", lineNumber, cityId);
                return false;
            }

            // Convert local time to UTC
            var eventUtc = ConvertLocalTimeToUtc(eventLocal, city.TimeZone);

            equipmentEvent = new EquipmentEvent
            {
                EquipmentId = equipmentId,
                EventCode = eventCode,
                EventTime = eventUtc,
                CityId = cityId
            };

            return true;
        }

        /// <summary>
        /// Converts local time to UTC using the specified timezone.
        /// </summary>
        private DateTime ConvertLocalTimeToUtc(DateTime localTime, string timeZone)
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var localUnspecified = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
                return TimeZoneInfo.ConvertTimeToUtc(localUnspecified, tz);
            }
            catch (TimeZoneNotFoundException ex)
            {
                _logger.LogError(ex, "Timezone '{TimeZone}' not found", timeZone);
                throw new InvalidOperationException($"Invalid timezone: {timeZone}", ex);
            }
        }

        /// <summary>
        /// Builds trip records from ordered equipment events.
        /// </summary>
        public Task<List<Trip>> BuildTripsFromEvents(List<EquipmentEvent> events)
        {
            var trips = new List<Trip>();

            foreach (var evt in events)
            {
                if (IsReleaseEvent(evt.EventCode))
                {
                    CreateNewTrip(trips, evt);
                }
                else if (IsPlacedEvent(evt.EventCode))
                {
                    CompleteTrip(trips, evt);
                }
            }

            return Task.FromResult(trips);
        }

        /// <summary>
        /// Creates a new trip record for a released event.
        /// </summary>
        private void CreateNewTrip(List<Trip> trips, EquipmentEvent evt)
        {
            var newTrip = new Trip
            {
                EquipmentId = evt.EquipmentId,
                OriginCityId = evt.CityId,
                StartUtc = evt.EventTime,
                DestinationCityId = null
            };

            trips.Add(newTrip);
            _logger.LogDebug("Created new trip for equipment {EquipmentId} from city {CityId}", evt.EquipmentId, evt.CityId);
        }

        /// <summary>
        /// Completes an existing trip with a placed event.
        /// </summary>
        private void CompleteTrip(List<Trip> trips, EquipmentEvent evt)
        {
            var existingTrip = trips.FirstOrDefault(t =>
                t.EquipmentId == evt.EquipmentId &&
                (t.DestinationCityId == 0 || t.DestinationCityId == null) &&
                t.EndUtc == default);

            if (existingTrip != null)
            {
                existingTrip.DestinationCityId = evt.CityId;
                existingTrip.EndUtc = evt.EventTime;
                existingTrip.TotalTripHours = (existingTrip.EndUtc - existingTrip.StartUtc).TotalHours;

                _logger.LogDebug("Completed trip for equipment {EquipmentId} to city {CityId}", evt.EquipmentId, evt.CityId);
            }
            else
            {
                _logger.LogWarning("Placed event for equipment {EquipmentId} has no corresponding release event", evt.EquipmentId);
            }
        }

        /// <summary>
        /// Determines if an event code represents a release (start) event.
        /// </summary>
        private bool IsReleaseEvent(string eventCode) =>
            eventCode.Equals(EventCodeReleased, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines if an event code represents a placed (end) event.
        /// </summary>
        private bool IsPlacedEvent(string eventCode) =>
            eventCode.Equals(EventCodePlaced, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Validates if an event code is recognized.
        /// </summary>
        private bool IsValidEventCode(string eventCode) =>
            eventCode.Equals(EventCodeReleased, StringComparison.OrdinalIgnoreCase) ||
            eventCode.Equals(EventCodePlaced, StringComparison.OrdinalIgnoreCase);
    }
}
