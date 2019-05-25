using System;
using System.Management.Automation;

namespace DWPowerShell.Utility.Cmdlet
{

    public abstract class DWPSCmdletBase :PSCmdlet, IDWPSCmdlet
    {
        private string _scriptPath;

        public string ScriptPath {
            get
            {
                if (_scriptPath == null)
                {
                    try
                    {
                        _scriptPath = DWPSUtils.ForceTrailingSlash(base.GetUnresolvedProviderPathFromPSPath(".\\"));
                    } catch { }
                }

                if (_scriptPath == null) throw new ApplicationException("ScriptPath cannot be called prior to BeginProcessing()");

                return _scriptPath;
            }
        }


        public DWPSCmdletBase() : base()
        {

        }
    }
}
