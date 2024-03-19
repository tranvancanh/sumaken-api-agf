using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sumaken_api_agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfLuggageStationRead : ControllerBase
    {
        // GET: api/<AgfLuggageStationRead>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AgfLuggageStationRead>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgfLuggageStationRead>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgfLuggageStationRead>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AgfLuggageStationRead>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
