using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{


    [ApiController]
    [Route("Pregitor")]

    public class HomeController : ControllerBase
    {

        [HttpPost("Preregitor")]
        public IActionResult Preregitor([FromBody] Preregitor_Request data)
        {
            var result = ClsSQLDT.Preregitor(data);
            if (result.status == "Error")
            {
                return BadRequest(new { status = "Error", message = result.message });
            }

            return Ok(new {status = "Success", message = result.message });

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login_Request data)
        {
            var result = ClsSQLDT.Login_JWT(data);
            if(result.status == "Error")
            {
                return BadRequest(new { status = "Error", message = "ผู้ใช้นี้มีอยู่แล้ว" });
            }

            return Ok(new { status = "Success", message = data.username, toekn = result.token });
        }


    }
}
