
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FactsController : ControllerBase
{
    
    private readonly FactService _factService;

    public FactsController(FactService fact)
    {
        _factService = fact;
    }

    [HttpGet("random")]
    public IActionResult GetRandomFact()
    {
        
        var fact = _factService.GetRandomFact();
        return Ok(fact);

    }

}