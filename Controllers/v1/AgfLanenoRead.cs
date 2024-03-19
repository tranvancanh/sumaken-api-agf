﻿using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sumaken_api_agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfLanenoRead : ControllerBase
    {
        // GET: api/<AgfLanenoRead>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AgfLanenoRead>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgfLanenoRead>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgfLanenoRead>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AgfLanenoRead>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
