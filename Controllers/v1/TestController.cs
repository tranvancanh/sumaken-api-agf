﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sumaken_Api_Agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private static Timer timer;
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var env = string.Empty;
#if DEBUG
       env = "Debug";
#else
            env = "Productions";
#endif
            var dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            return new string[] { "value1", "value2", $"Env: {env}", $"日時: {dateTimeNow}" };
        }

        // GET api/<ValuesController>/5
        [HttpGet]
        [Route("TimerCallback")]
        public IActionResult TimerCallback()
        {
            // Create a TimerCallback delegate that points to the method you want to execute
            TimerCallback timerCallback = new TimerCallback(DoSomething);

            // Create a Timer that calls the TimerCallback delegate every 5 seconds
            timer = new Timer(timerCallback, null, 60000, Timeout.Infinite);

            // Wait for user input to exit
            Debug.WriteLine("Press any key to exit...");

            Debug.WriteLine("Timer started. Waiting for 60 seconds...");

            // Đảm bảo chương trình không kết thúc ngay lập tức

            return Ok("Create a Timer that calls the TimerCallback delegate every 60 seconds");
        }

        static void DoSomething(object state)
        {
            // Method to be executed
            Debug.WriteLine("Executing DoSomething method...");

            // Dispose the timer to release resources when done
            timer.Dispose();
        }


        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpGet]
        [Route("SessionId")]
        public IActionResult SessionId()
        {
            // Get the request ID from the HttpContext
            var requestId = HttpContext.TraceIdentifier;
            // Retrieve session ID
            var sessionId = HttpContext?.Session?.Id;
            if(sessionId != null)
            {
                // You can now use the requestId as needed
                // For example, you can log it or include it in the response
                return Ok($"Session ID: {sessionId}");
            }
            else
            {
                return Ok($"Request ID: {requestId}");
            }
           
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
