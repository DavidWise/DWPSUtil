namespace DWPowerShell.Utility.Cmdlet
{
    public interface IDWPSCmdlet
    {
        string ScriptPath { get; }
        void WriteObject(object sendToPipeline);
        void WriteObject(object sendToPipeline, bool enumerateCollection);
    }
}
