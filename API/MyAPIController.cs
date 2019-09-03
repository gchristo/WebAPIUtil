using Microsoft.AspNetCore.Mvc;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API
{
    [Route("api/[controller]")]
    public class MyAPIController : Controller
    {
        [HttpGet]
        public TestResponse Get([FromQuery]int number, [FromQuery]string text)
        {
            return new TestResponse()
            {
                intValue = number,
                stringValue = text
            };
        }

        [HttpPost]
        public TestResponse Post([FromBody]TestRequestBody information)
        {
            return new TestResponse()
            {
                intValue = information.i,
                stringValue = information.s
            };
        }
    }
}
