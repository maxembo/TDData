using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TDData.DTO;
using TDData.Services;

namespace TDData.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AddressService _addressService;
    private readonly IMapper _mapper;
    private readonly ILogger<AddressController> _logger;

    public AddressController(IMapper mapper, AddressService addressService, ILogger<AddressController> logger)
    {
        _addressService = addressService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            _logger.LogWarning($"BadRequest: {nameof(address)} cannot be empty.");
            return BadRequest("Address is empty");
        }
        
        _logger.LogInformation("Get request: {Address}", address);
        
        var content = await _addressService.Get(address);
        if (content?.Result == null)
        {
            _logger.LogInformation($"{nameof(address)} not found");
            return NotFound("Address not found");
        }
        
        var response = _mapper.Map<AddressDto>(content);
        
        _logger.LogInformation($"{nameof(address)} successfully get");
        
        return Ok(response);
    }
}