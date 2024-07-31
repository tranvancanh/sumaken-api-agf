using NLog;
using NLog.Targets;
using System.Diagnostics;

namespace Sumaken_Api_Agf.Commons
{
    public class SchedulingTask
    {
        private int MostRecentDays = 365;

        private readonly ILogger<SchedulingTask> _logger;
        public SchedulingTask(ILogger<SchedulingTask> logger)
        {
            _logger = logger;
            _logger.LogInformation("Nlog is started to Scheduling Task");
        }

        public void ExecuteImmediately()
        {
            // Your task logic here
            try
            {
                Debug.WriteLine("Immediately Task Executing!");
                // Use NLog to log messages
                _logger.LogInformation("Immediately Task Executing!");

                // Your task logic here
                var nlogFileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().ToList();
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                for (var i = 0; i < nlogFileTarget.Count; i++)
                {
                    var target = nlogFileTarget[i];
                    var logFileName = target.FileName.Render(dummyEventInfo);
                    var logPath = Path.GetDirectoryName(logFileName);
                    if (!Directory.Exists(logPath))
                    {
                        continue;
                    }
                    Debug.WriteLine($"Log Path {i} : {logPath}");
                    DirectoryInfo dir = new DirectoryInfo(logPath); //Assuming Test is your Folder
                    FileInfo[] files = dir.GetFiles("*.log"); //Getting log files
                    var dtCreateLimit = DateTime.Now.AddDays(-MostRecentDays);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fFirstTime = file.CreationTime;
                            var fLastTime = file.LastWriteTime;
                            if (fLastTime < dtCreateLimit)
                            {
                                file.Delete();
                                _logger.LogInformation($"File {file.Name} is deleted!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"File {file.Name} cannot be deleted!");
                            _logger.LogError($"Exception Message : {ex.Message}!");
                        }
                    }
                }

                Debug.WriteLine("Immediately Task Executed!");
                _logger.LogInformation("Immediately Task is Executed!");
            }
            catch (Exception)
            {
                Debug.WriteLine("Immediately Task Error!");
                _logger.LogError("Immediately Task Error!");
            }
        }

        public void ExecuteMinutelyAsync()
        {
            // Your task logic here
            try
            {
                Debug.WriteLine("Minutely Task Executing!");
                // Use NLog to log messages
                _logger.LogInformation("Minutely Task Executing!");

                // Your task logic here
                var nlogFileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().ToList();
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                for (var i = 0; i < nlogFileTarget.Count; i++)
                {
                    var target = nlogFileTarget[i];
                    var logFileName = target.FileName.Render(dummyEventInfo);
                    var logPath = Path.GetDirectoryName(logFileName);
                    if (!Directory.Exists(logPath))
                    {
                        continue;
                    }
                    Debug.WriteLine($"Log Path {i} : {logPath}");
                    DirectoryInfo dir = new DirectoryInfo(logPath); //Assuming Test is your Folder
                    FileInfo[] files = dir.GetFiles("*.log"); //Getting log files
                    var dtCreateLimit = DateTime.Now.AddDays(-MostRecentDays);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fFirstTime = file.CreationTime;
                            var fLastTime = file.LastWriteTime;
                            if (fLastTime < dtCreateLimit)
                            {
                                file.Delete();
                                _logger.LogInformation($"File {file.Name} is deleted!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"File {file.Name} cannot be deleted!");
                            _logger.LogError($"Exception Message : {ex.Message}!");
                        }
                    }
                }

                Debug.WriteLine("Minutely Task Executed!");
                _logger.LogInformation("Minutely Task is Executed!");
            }
            catch (Exception)
            {
                Debug.WriteLine("Minutely Task Error!");
                _logger.LogError("Minutely Task Error!");
            }
        }

        public async Task ExecuteDailyAsync()
        {
            try
            {
                Debug.WriteLine("Daily Task Executing!");
                _logger.LogInformation("Daily Task Executing!");

                // Your task logic here
                var nlogFileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().ToList();
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                for (var i = 0; i < nlogFileTarget.Count; i++)
                {
                    var target = nlogFileTarget[i];
                    var logFileName = target.FileName.Render(dummyEventInfo);
                    var logPath = Path.GetDirectoryName(logFileName);
                    if (!Directory.Exists(logPath))
                    {
                        continue;
                    }
                    Debug.WriteLine($"Log Path {i} : {logPath}");
                    DirectoryInfo dir = new DirectoryInfo(logPath); //Assuming Test is your Folder
                    FileInfo[] files = dir.GetFiles("*.log"); //Getting log files
                    var dtCreateLimit = DateTime.Now.AddDays(-MostRecentDays);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fFirstTime = file.CreationTime;
                            var fLastTime = file.LastWriteTime;
                            if (fLastTime < dtCreateLimit)
                            {
                                file.Delete();
                                _logger.LogInformation($"File {file.Name} is deleted!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"File {file.Name} cannot be deleted!");
                            _logger.LogError($"Exception Message : {ex.Message}!");
                        }
                    }
                }

                Debug.WriteLine("Daily Task Executed!");
                _logger.LogInformation("Daily Task is Executed!");
            }
            catch (Exception)
            {
                Debug.WriteLine("Daily Task Error!");
                _logger.LogError("Daily Task Error!");
            }
            await Task.CompletedTask;
        }

        public async Task ExecuteMonthlyAsync()
        {
            try
            {
                Debug.WriteLine("Monthly Task Executing!");
                _logger.LogInformation("Monthly Task Executing!");

                // Your task logic here
                var nlogFileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().ToList();
                var dummyEventInfo = new LogEventInfo { TimeStamp = DateTime.UtcNow };
                for (var i = 0; i < nlogFileTarget.Count; i++)
                {
                    var target = nlogFileTarget[i];
                    var logFileName = target.FileName.Render(dummyEventInfo);
                    var logPath = Path.GetDirectoryName(logFileName);
                    if (!Directory.Exists(logPath))
                    {
                        continue;
                    }
                    Debug.WriteLine($"Log Path {i} : {logPath}");
                    DirectoryInfo dir = new DirectoryInfo(logPath); //Assuming Test is your Folder
                    FileInfo[] files = dir.GetFiles("*.log"); //Getting log files
                    var dtCreateLimit = DateTime.Now.AddDays(-MostRecentDays);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fFirstTime = file.CreationTime;
                            var fLastTime = file.LastWriteTime;
                            if (fLastTime < dtCreateLimit)
                            {
                                file.Delete();
                                _logger.LogInformation($"File {file.Name} is deleted!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"File {file.Name} cannot be deleted!");
                            _logger.LogError($"Exception Message : {ex.Message}!");
                        }
                    }
                }

                Debug.WriteLine("Monthly Task Executed!");
                _logger.LogInformation("Monthly Task is Executed!");
            }
            catch (Exception)
            {
                Debug.WriteLine("Monthly Task Error!");
                _logger.LogError("Monthly Task Error!");
            }
            await Task.CompletedTask;
        }

    }
}
