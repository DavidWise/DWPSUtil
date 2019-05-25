using DWPowerShell.Utility.Cache.Dependency;
using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWPowerShell.Utility.Cache
{
    public class CacheContainer : ICacheContainer
    {
        protected Dictionary<string, ICacheItem> _entries = null;
        protected IStaticAbstraction _diskManager = new StaticAbstractionWrapper();

        public CacheContainer()
        {
            _entries = new Dictionary<string, ICacheItem>(StringComparer.InvariantCultureIgnoreCase);
        }

        public CacheContainer(IStaticAbstraction diskManager) : this()
        {
            if (diskManager != null) _diskManager = diskManager;
        }

        public void Add(string key, object value)
        {
            var expires = new CacheDependencyNever();

            AddItem(key, value, expires);
        }

        public void Add(string key, object value, DateTime expires)
        {
            var expireAction = new CacheDependencyDateTime(_diskManager.DateTime, expires);
            AddItem(key, value, expireAction);
        }

        public void Add(string key, object value, TimeSpan expiresIn, bool sliding=false)
        {
            var expireAction = new CacheDependencyDateTime(_diskManager.DateTime, expiresIn, sliding);
            AddItem(key, value, expireAction);
        }

        public void Add(string key, object value, string dependencyFile) 
        {
            Add(key, value, new string[] { dependencyFile });
        }

        public void Add(string key, object value, string[] dependencyFiles)
        {
            var cacheObj = new CacheItem(value);

            foreach (var file in dependencyFiles)
            {
                var dep = new CacheDependencyFile(_diskManager, file);
                cacheObj.Dependencies.Add(dep);
            }
            AddItem(key, cacheObj);
        }


        protected void AddItem(string key, object value, ICacheDependency expireAction)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            var cacheObj = new CacheItem(value, expireAction);

            if (ContainsKey(key))
                _entries[key] = cacheObj;
            else
                _entries.Add(key, cacheObj);
        }

        protected void AddItem(string key, CacheItem value)
        {
            var prepKey = new PreparedKey(key);
            if (!prepKey.IsValid) throw new ArgumentNullException(nameof(key));

            if (ContainsKey(prepKey))
                _entries[prepKey.Prepared] = value;
            else
                _entries.Add(prepKey.Prepared, value);
        }


        public bool ContainsKey(string key)
        {
            var prepKey = new PreparedKey(key);
            return ContainsKey(prepKey);
        }

        private bool ContainsKey(PreparedKey key)
        {
            return key != null && key.IsValid && _entries.ContainsKey(key.Prepared);
        }



        public object this[string key]
        {
            get => this.Get(key);
            set => this.Add(key, value);
        }


        public object Get(string key)
        {
            object result = null;
            var prepKey = new PreparedKey(key);
            if (!ContainsKey(prepKey)) return null;

            var item = _entries[prepKey.Prepared];
            if (item.HasExpired)
                _entries.Remove(prepKey.Prepared);
            else
                result = item.Value;

            return result;
        }

        public T Get<T>(string key) where T : class
        {
            T result = default(T);
            var obj = this.Get(key);
            if (obj != null)
            {
                if (obj is T)
                    result = obj as T;
                else
                    throw new ArgumentException(
                        $"Requested cache key '{key}' attempted to resolve to type '{typeof(T)}' but was '{obj.GetType().Name}'");
            }

            return result;
        }

        public void Remove(string key)
        {
            var prepKey = new PreparedKey(key);
            if (!ContainsKey(prepKey)) return;
            _entries.Remove(prepKey.Prepared);
        }

        public string[] Keys => _entries.Keys.ToArray();
        public int Length => _entries.Count;
    }


    internal class PreparedKey
    {
        public string Raw { get; set; }
        public string Prepared { get; set; }
        public bool IsValid { get; set; }

        public PreparedKey(string rawKey)
        {
            Prepared = rawKey == "" ? null : rawKey;
            Raw = rawKey;
            IsValid = !string.IsNullOrEmpty(Prepared);
        }
    }
}
