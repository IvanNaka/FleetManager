using FleetManager.Application.DTO;
using FleetManager.Domain.Services;
using FleetManager.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FleetManager.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IChassiService _chassiService;

        public VehicleController(IVehicleService vehicleService, IChassiService chassiService)
        {
            _vehicleService = vehicleService;
            _chassiService = chassiService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto dto)
        {
            try
            {
                var chassi = await _chassiService.AddChassiAsync(dto.ChassisNumber, dto.ChassisSeries);
                var vehicle = await _vehicleService.CreateVehicleAsync(chassi, dto.Type, dto.Color);
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{serie}/{number}")]
        public async Task<IActionResult> GetVehicle(string serie, uint number)
        {
            try
            {
                var vehicle = await _vehicleService.GetByChassisIdAsync(serie, number);
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{serie}/{number}/color")]
        public async Task<IActionResult> UpdateVehicleColor(string serie, uint number, [FromBody] UpdateVehicleColorDto dto)
        {
            try
            {
                var vehicle = await _vehicleService.UpdateVehicleColorAsync(serie, number, dto.NewColor);
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }
    }
}