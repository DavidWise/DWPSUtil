using System;
using System.Diagnostics;

namespace DWPowerShell.Utility.Abstraction.Process
{
    public interface IProcessManager
    {
        IProcessResult Execute(string command, string arguments, int timeoutInMs);
    }

    public class ProcessManager : IProcessManager
    {
        public IProcessResult Execute(string command, string arguments, int timeoutInMs)
        {
            var result = new ProcessResult {Command = command};
            var procStartInfo = new ProcessStartInfo(command, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,

                // Do not create the black window.
                CreateNoWindow = true
            };

            using (var proc = new System.Diagnostics.Process())
            {
                var start = DateTime.Now;
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit(timeoutInMs);
                // Get the output into a string
                result.Output = proc.StandardOutput.ReadToEnd();
                result.Errors = proc.StandardError.ReadToEnd();

                result.ElapsedMilliseconds = DateTime.Now.Subtract(start).TotalMilliseconds;
                // Display the command output
            }

            return result;
        }
    }
}
