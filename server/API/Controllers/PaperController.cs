using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaperController(IPaperService service) : ControllerBase
{
    
}