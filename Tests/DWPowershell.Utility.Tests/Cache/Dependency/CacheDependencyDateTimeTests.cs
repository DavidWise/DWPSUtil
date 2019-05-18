using System;
using NUnit.Framework;
using NSubstitute;
using DWPowerShell.Utility.Cache.Dependency;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.Cache.Dependency
{
    [TestFixture]
    public class CacheDependencyDateTimeTests
    {
        private IDateTime _dtp;

        [SetUp]
        public void Setup()
        {
            _dtp = Substitute.For<IDateTime>();
        }

        [Test]
        public void FixedExpirationCommon()
        {
            var now = DateTime.Now;
            var expires = now.AddMinutes(30);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expires);

            Assert.IsFalse(cache.HasExpired);
            Assert.IsFalse(cache.Sliding);
            Assert.IsTrue(cache.Expires == expires);
        }

        [Test]
        public void FixedExpirationHasExpired()
        {
            var now = DateTime.Now;
            var expires = now.AddMinutes(30);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expires);

            // now move the clock past the expiration
            _dtp.Now.Returns(expires.AddMilliseconds(1));

            Assert.IsTrue(cache.HasExpired);
            Assert.IsTrue(cache.Expires == expires);
            Assert.IsFalse(cache.Sliding);
        }

        [Test]
        public void FixedExpirationTimeSpanCommon()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0,30,0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn);

            Assert.IsFalse(cache.HasExpired);
            Assert.IsFalse(cache.Sliding);
            Assert.IsTrue(cache.Expires == expires);
        }

        [Test]
        public void FixedExpirationTimeSpanExpired()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0,30,0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn);
            _dtp.Now.Returns(expires.AddMilliseconds(1));

            Assert.IsTrue(cache.HasExpired);
            Assert.IsFalse(cache.Sliding);
            Assert.IsTrue(cache.Expires == expires);
        }

        [Test]
        public void FixedExpirationSlidingTimeSpanCommon()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0, 30, 0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn, true);

            Assert.IsFalse(cache.HasExpired);
            Assert.IsTrue(cache.Sliding);
            Assert.IsTrue(cache.Expires == expires);
        }

        [Test]
        public void FixedExpirationSlidingTimeSpanCommonExpired()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0, 30, 0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn, true);

            Assert.IsFalse(cache.HasExpired);
            Assert.IsTrue(cache.Sliding);
            Assert.IsTrue(cache.Expires == expires);

            _dtp.Now.Returns(expires.AddMilliseconds(1));
            Assert.IsTrue(cache.HasExpired);
        }


        [Test]
        public void FixedExpirationSlidingTimeSpanSlides()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0, 30, 0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn, true);

            Assert.IsTrue(cache.Expires == expires);

            var newNow = DateTime.Now.AddMinutes(5);
            _dtp.Now.Returns(newNow);

            cache.SlideExpiration();
            var newexpires = newNow.Add(expiresIn);
            Assert.IsFalse(cache.HasExpired);
            Assert.IsTrue(cache.Expires == newexpires);
        }


        [Test]
        public void FixedExpirationSlidingTimeSpanSlideAfterExpiration()
        {
            var now = DateTime.Now;
            var expiresIn = new TimeSpan(0, 30, 0);
            var expires = now.Add(expiresIn);

            _dtp.Now.Returns(now);
            var cache = new CacheDependencyDateTime(_dtp, expiresIn, true);

            Assert.IsTrue(cache.Expires == expires);

            var newNow = expires.AddMinutes(5);
            _dtp.Now.Returns(newNow);
            Assert.IsTrue(cache.HasExpired);

            cache.SlideExpiration();

            Assert.IsTrue(cache.HasExpired);
            Assert.IsTrue(cache.Expires == expires);
        }
    }
}
