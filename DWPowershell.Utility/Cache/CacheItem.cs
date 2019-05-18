using System;
using DWPowerShell.Utility.Cache.Dependency;

namespace DWPowerShell.Utility.Cache
{
    public interface ICacheItem : ICacheDependency
    {
        CacheDependencyCollection Dependencies { get; }
        object Value { get; set; }
    }


    public class CacheItem : ICacheItem
    {
        protected object _value = null;
        public CacheDependencyCollection Dependencies { get; set; }

        public object Value
        {
            get
            {
                Dependencies.SlideExpiration();
                return _value;
            }
            set { _value = value; }
        }

        public CacheItem(object value)
        {
            this.Dependencies = new CacheDependencyCollection();
            this.Value = value;
        }
        public CacheItem(object value, ICacheDependency expires) : this(value)
        {
            if (expires == null) throw new ArgumentNullException(nameof(expires));
            this.Dependencies.Add(expires);
        }
        public CacheItem(object value, ICacheDependency[] expireItems) : this(value)
        {
            if (expireItems == null || expireItems.Length<1) throw new ArgumentNullException(nameof(expireItems));
            this.Dependencies.AddRange(expireItems);
        }

        public bool HasExpired
        {
            get { return this.Dependencies.HasAnyExpired() || this.Dependencies.HasAnyChanged(); }
        }
    }
}
