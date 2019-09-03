using Microsoft.AspNetCore.Mvc;
using Models;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API
{
    [Route("api/[controller]")]
    public class MyAPIController : Controller
    {
        [HttpGet]
        public ActionResult<TestResponse> Get([FromQuery]int number, [FromQuery]string text)
        {
            try
            {
                return Ok(new TestResponse()
                {
                    intValue = number,
                    stringValue = text
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult<TestResponse> Post([FromBody]TestRequestBody information)
        {
            try
            {
                return Ok(new TestResponse()
                {
                    intValue = information.i,
                    stringValue = information.s
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
