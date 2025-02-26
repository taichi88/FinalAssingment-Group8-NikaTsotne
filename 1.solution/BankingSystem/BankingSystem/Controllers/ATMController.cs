using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ATMController : ControllerBase
    {
        //აქ უნდა დავაინჯექტო IATMService
        public readonly IATMService _iatmService;


        public ATMController(IATMService iATMService)
        {
            _iatmService = iATMService;
        }

        [HttpGet("authenticate")]
        
        public IActionResult Authenticate([FromBody] AuthenticationRequest request)

        {
            var authenticatedPerson = _atmService.Authenticate(request.CardNumber, request.PinCode);

            if (authenticatedPerson == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(authenticatedPerson);
        }
    }

}
