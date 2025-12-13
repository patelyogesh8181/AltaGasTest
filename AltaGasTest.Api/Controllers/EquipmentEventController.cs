using AltaGasTest.Data.Entities;
using AltaGasTest.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AltaGasTest.Api.Controllers
{
    /// <summary>
    /// API controller for managing equipment data and processing Equipment events.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EquipmentEventController : ControllerBase
    {
        private readonly IEquipmentEventRepository _equipmentEventRepository;
        private readonly ILogger<EquipmentEventController> _logger;
        public EquipmentEventController(
            IEquipmentEventRepository equipmentEventRepository,
            ILogger<EquipmentEventController> logger)
        {
            _equipmentEventRepository = equipmentEventRepository ?? throw new ArgumentNullException(nameof(equipmentEventRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves Equipment events based on equipmentId.
        /// </summary>
        /// <returns>List of Equipment events.</returns>
        [HttpGet("{equipmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EquipmentEvent>>> GetEquipments(string equipmentId)
        {
            try
            {
                _logger.LogInformation("Fetching all Equipment events");
                var equipmentEvents = await _equipmentEventRepository.GetAsync(equipmentId);
                return Ok(equipmentEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Equipment events");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An error occurred while retrieving Equipment events." });
            }
        }
    }
}
