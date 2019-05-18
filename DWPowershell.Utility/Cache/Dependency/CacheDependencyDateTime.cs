using System;
using StaticAbstraction;

namespace DWPowerShell.Utility.Cache.Dependency
{
    public class CacheDependencyDateTime : ICacheDependency, ICacheSlidingDependency
    {
        protected DateTime _timeCreated = DateTime.Now;
        protected TimeSpan _slidingWindow = TimeSpan.MinValue;
        private readonly IDateTime _dateTimeProvider;

        public DateTime Expires { get; protected set; }
        public bool Sliding { get; protected set; }

        public CacheDependencyDateTime(DateTime expires) : this(new StAbDateTime(), expires) { }

        public CacheDependencyDateTime(IDateTime dateTimeProvider, DateTime expires)
        {
            _dateTimeProvider = dateTimeProvider;
            this.Expires = expires;
        }

        public CacheDependencyDateTime(TimeSpan expiresIn, bool sliding = false) : this(new StAbDateTime(), expiresIn, sliding) { }
        public CacheDependencyDateTime(IDateTime dateTimeProvider, TimeSpan expiresIn, bool sliding = false)
        {
            _dateTimeProvider = dateTimeProvider;
            this._slidingWindow = expiresIn;
            this.Expires = _dateTimeProvider.Now.Add(expiresIn);
            this.Sliding = sliding;
        }

        public bool HasExpired =>  _dateTimeProvider.Now > this.Expires;
        

        public void SlideExpiration()
        {
            if (this.Sliding  && !HasExpired) this.Expires = _dateTimeProvider.Now.Add(this._slidingWindow);
        }
    }
}