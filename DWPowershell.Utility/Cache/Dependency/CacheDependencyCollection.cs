using StaticAbstraction;
using System.Collections.Generic;
using System.Linq;

namespace DWPowerShell.Utility.Cache.Dependency
{
    public class CacheDependencyCollection : List<ICacheDependency>, ICacheSlidingDependency
    {
        private readonly IStaticAbstraction _diskManager;

        public CacheDependencyCollection() : this(null)
        {
        }

        public CacheDependencyCollection(IStaticAbstraction diskManager)
        {
            _diskManager = diskManager ?? new StaticAbstractionWrapper();
        }

        public void AddDependencyFiles(string[] filePaths)
        {
            if (filePaths == null || filePaths.Length < 1) return;

            foreach (var file in filePaths)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    var dep = new CacheDependencyFile(_diskManager, file);
                    this.Add(dep);
                }
            }
        }

        public bool HasAnyExpired()
        {
            return this.Any(x => x.HasExpired);
        }

        public bool HasAnyChanged()
        {
            if (this.HasAnyExpired()) return true;

            var deps = this.OfType<ICacheDependencyFile>();
            return deps.Any(x => x.HasChanged());
        }

        public void SlideExpiration()
        {
            var sliders = this.Where(x=> !x.HasExpired).OfType<ICacheSlidingDependency>();
            foreach (var item in sliders)
            {
                item.SlideExpiration();
            }
        }
    }
}
