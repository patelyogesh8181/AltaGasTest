using AltaGasTest.Api.Models;
using AltaGasTest.Api.Services;
using AltaGasTest.Data.Entities;
using AltaGasTest.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AltaGasTest.Api.Controllers
{
    /// <summary>
    /// API controller for managing trip data and processing trip events.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TripController : ControllerBase
    {
        private readonly ICanadianCityRepository _canadianCityRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IEquipmentEventRepository _equipmentEventRepository;
        private readonly ITripServices _tripServices;
        private readonly ILogger<TripController> _logger;

        private const string CsvFileExtension = ".csv";
        private const int MaxFileSizeBytes = 10485760; // 10 MB

        public TripController(
            ICanadianCityRepository canadianCityRepository,
            ITripRepository tripRepository,
            IEquipmentEventRepository equipmentEventRepository,
            ITripServices tripServices,
            ILogger<TripController> logger)
        {
            _canadianCityRepository = canadianCityRepository ?? throw new ArgumentNullException(nameof(canadianCityRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _equipmentEventRepository = equipmentEventRepository ?? throw new ArgumentNullException(nameof(equipmentEventRepository));
            _tripServices = tripServices ?? throw new ArgumentNullException(nameof(tripServices));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all Trips.
        /// </summary>
        /// <returns>List of Trips.</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
        {
            try
            {
                _logger.LogInformation("Fetching all Trips");
                var trips = await _tripRepository.GetAllAsync();
                return Ok(trips);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Fetching all Trips");
                return BadRequest(new { error = "Fetching all Trips: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trips");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An error occurred while retrieving trips." });
            }
        }

        /// <summary>
        /// Uploads and processes a CSV file containing equipment events.
        /// </summary>
        /// <param name="file">CSV file with equipment events.</param>
        /// <returns>Processing result with created trips.</returns>
        [HttpPost("upload-events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FileProcessingResult>> UploadEvents([FromForm] IFormFile? file)
        {
            try
            {
                if (!ValidateFile(file, out var validationError))
                {
                    _logger.LogWarning("File validation failed: {Error}", validationError);
                    return BadRequest(new { error = validationError });
                }

                _logger.LogInformation("Processing equipment events from file: {FileName}", file!.FileName);

                var events = await _tripServices.ProcessEquipmentEventsAsync(file);

                var trips = await _tripServices.BuildTripsFromEvents(events);

                if (!events.Any())
                {
                    _logger.LogInformation("No equipment event generated from file processing");
                    return Ok(new FileProcessingResult
                    {
                        Message = "File processed successfully, but no equipment event were generated.",
                        EquipmentEvents = events,
                        TripCount = 0
                    });
                }

                if (!trips.Any())
                {
                    _logger.LogInformation("No trips generated from file processing");
                    return Ok(new FileProcessingResult
                    {
                        Message = "File processed successfully, but no trips were generated.",
                        Trips = trips,
                        TripCount = 0
                    });
                }


                await _equipmentEventRepository.AddAsync(events);
                await _tripRepository.AddAsync(trips);

                _logger.LogInformation("Successfully processed {TripCount} trips from file: {FileName}",
                    trips.Count, file.FileName);

                return Ok(new FileProcessingResult
                {
                    Message = $"Events processed successfully. {trips.Count} trip(s) created.",
                    Trips = trips,
                    TripCount = trips.Count
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid CSV format in uploaded file");
                return BadRequest(new { error = "Invalid CSV format: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing file");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while processing the file." });
            }
        }

        /// <summary>
        /// Validates the uploaded file.
        /// </summary>
        private bool ValidateFile(IFormFile? file, out string errorMessage)
        {
            if (file == null)
            {
                errorMessage = "No file was uploaded.";
                return false;
            }

            if (file.Length == 0)
            {
                errorMessage = "The uploaded file is empty.";
                return false;
            }

            if (file.Length > MaxFileSizeBytes)
            {
                errorMessage = $"File size exceeds maximum allowed size of {MaxFileSizeBytes / 1024 / 1024} MB.";
                return false;
            }

            if (!file.FileName.EndsWith(CsvFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Only .csv files are supported.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
