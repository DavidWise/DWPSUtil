using System;
using NUnit.Framework;
using NSubstitute;
using DWPowerShell.Utility.Cache.Dependency;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.Cache.Dependency
{
    [TestFixture]
    public class CacheDependencyCollectionTests
    {
        private IStaticAbstraction dm = null;
        private IDateTime dtp = null;
        private CacheDependencyCollection cln;

        [SetUp]
        public void TestSetup()
        {
            dtp = Substitute.For<IDateTime>();
            dm = Substitute.For<IStaticAbstraction>();
            dm.DateTime.Returns(dtp);
            dtp.Now.Returns(DateTime.Now);
            cln = new CacheDependencyCollection(dm);
        }


        [Test]
        public void SimpleDateTimeExpiration()
        {
            var expires = DateTime.Now.AddMinutes(5);

            var exp = new CacheDependencyDateTime(dtp, expires);
            cln.Add(exp);
            Assert.IsTrue(cln.Count == 1);
            Assert.IsFalse(cln.HasAnyExpired());
            Assert.IsFalse(cln.HasAnyChanged());

            dtp.Now.Returns(expires.AddMilliseconds(1));
            Assert.IsTrue(cln.Count == 1);
            Assert.IsTrue(cln.HasAnyExpired());
            Assert.IsTrue(cln.HasAnyChanged());
        }


        [Test]
        public void SlidingDateTimeExpiration()
        {
            var expiresIn = new TimeSpan(0,30,0);
            var expires = dtp.Now.Add(expiresIn);

            var exp = new CacheDependencyDateTime(dtp, expiresIn, true);
            cln.Add(exp);
            Assert.IsTrue(cln.Count == 1);
            Assert.IsFalse(cln.HasAnyExpired());
            Assert.IsFalse(cln.HasAnyChanged());
            var first = (CacheDependencyDateTime) cln[0];
            Assert.IsTrue(first.Expires == expires);

            // move a few minutes forward but within the expiration window
            var newTimeNow = DateTime.Now.AddSeconds(expiresIn.TotalSeconds/2);
            dtp.Now.Returns(newTimeNow);
            Assert.IsFalse(cln.HasAnyExpired());
            Assert.IsFalse(cln.HasAnyChanged());

            first = (CacheDependencyDateTime)cln[0];
            Assert.IsTrue(first.Expires == expires);

            var newExpires = newTimeNow.Add(expiresIn);
            cln.SlideExpiration();
            Assert.IsFalse(cln.HasAnyExpired());
            Assert.IsFalse(cln.HasAnyChanged());
            Assert.IsTrue(first.Expires == newExpires);

            var expiredTime = newExpires.AddMilliseconds(1);
            dtp.Now.Returns(expiredTime);
            Assert.IsTrue(cln.HasAnyExpired());
            Assert.IsTrue(cln.HasAnyChanged());
        }
    }
}
