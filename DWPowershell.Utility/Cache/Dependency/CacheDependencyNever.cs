namespace DWPowerShell.Utility.Cache.Dependency
{
    public class CacheDependencyNever : ICacheDependency
    {
        public bool HasExpired
        {
            get { return false; }
        }
    }
}