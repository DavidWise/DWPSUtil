namespace DWPowerShell.Utility.Cache.Dependency
{
    public interface ICacheDependency
    {
        bool HasExpired { get; }
    }

    public interface ICacheSlidingDependency
    {
        void SlideExpiration();
    }

    public interface ICacheDependencyFile : ICacheDependency
    {
        bool HasChanged();
    }
}