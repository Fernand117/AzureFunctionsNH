using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class CompnayFunction
    {
        private readonly ILogger _logger;

        public CompnayFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CompnayFunction>();
        }

        [Function("CompnayFunction")]
        public void Run([TimerTrigger("0 */10 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class DataIoTDevice
    {
        public int Id { get; set; }
        public string name { get; set; }
        public float Temperature { get; set; }
        public float Variable { get; set; }
    }
}
