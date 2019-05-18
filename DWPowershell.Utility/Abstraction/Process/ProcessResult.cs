namespace DWPowerShell.Utility.Abstraction.Process
{
    public interface IProcessResult
    {
        string Command { get; }
        string Output { get; }
        string Errors { get; }
        double ElapsedMilliseconds { get; }
    }

    public class ProcessResult : IProcessResult
    {
        public string Command { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }
        public double ElapsedMilliseconds { get; set; }
    }
}