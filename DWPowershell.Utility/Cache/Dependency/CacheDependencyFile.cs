using StaticAbstraction;
using System;

namespace DWPowerShell.Utility.Cache.Dependency
{
    public class CacheDependencyFile : ICacheDependencyFile
    {
        private readonly IStaticAbstraction _diskManager;
        public string FullPath { get; protected set; }
        public bool Exists { get; protected set; }
        public DateTime LastModified { get; protected set; }

        public CacheDependencyFile(string fullPath) : this(new StaticAbstractionWrapper(), fullPath)
        {
        }


        public CacheDependencyFile(IStaticAbstraction diskManager, string fullPath)
        {
            _diskManager = diskManager;
            if (string.IsNullOrWhiteSpace(fullPath)) throw new ArgumentNullException("fullPath is required");
            GetState(fullPath);
        }


        protected void GetState(string fullPath)
        {
            this.FullPath = fullPath;
            this.Exists = _diskManager.File.Exists(fullPath);
            if (this.Exists)
            {
                var info = _diskManager.NewFileInfo(fullPath);
                this.LastModified = info.LastWriteTime;
            }
            else
            {
                this.LastModified = DateTime.MinValue;
            }
        }



        /// <summary>
        /// indicates if there was *any* change in the state of a file (created/deleted/timestamp)
        /// </summary>
        /// <returns>true if the specified file has changed in any way, false if not</returns>
        public bool HasChanged()
        {
            var hasChanged = false;
            var existsNow = _diskManager.File.Exists(this.FullPath);

            hasChanged = existsNow != this.Exists;

            if (!hasChanged && existsNow)
            {
                var info = _diskManager.NewFileInfo(this.FullPath);
                hasChanged = this.LastModified != info.LastWriteTime;
            }

            return hasChanged;
        }

        /// <summary>
        /// Indicates if a files timestamp has been updated since the file was put in cache.  This only applies if the file existed originally
        /// and has been changed with a later timestamp
        /// </summary>
        public bool HasExpired
        {
            get
            {
                var result = false;
                if (_diskManager.File.Exists(this.FullPath))
                {
                    var info = _diskManager.NewFileInfo(this.FullPath);
                    result = this.LastModified < info.LastWriteTime;
                }

                return result;
            }
        }
    }
}