using System;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using DWPowerShell.Utility.Cache;
using StaticAbstraction;
using StaticAbstraction.IO;

namespace DWPowerShell.Utility.Tests.Cache
{
    [TestFixture]
    public class CacheContainerTests
    {
        private CacheContainer _cache;
        private IDateTime _dateTimeProvider;
        private IStaticAbstraction _diskManager;

        [SetUp]
        public void SetUp()
        {
            _dateTimeProvider = Substitute.For<IDateTime>();
            _diskManager = Substitute.For<IStaticAbstraction>();
            _diskManager.DateTime.Returns(_dateTimeProvider);
            _cache = new CacheContainer(_diskManager);
        }


        [Test]
        public void Initialization()
        {
            Assert.AreEqual(_cache.Length, 0);
            Assert.AreEqual(_cache.Keys.Length, 0);
        }


        [Test]
        public void Items_Add_and_Get()
        {
            _cache.Add("five", 5);
            _cache.Add("SIX", 6);

            var keys = _cache.Keys;
            Assert.AreEqual(_cache.Length, 2);
            Assert.AreEqual(keys.Length, 2);
            Assert.AreEqual(keys[0], "five");
            Assert.AreEqual(keys[1], "SIX");

            Assert.AreEqual(_cache["five"], 5);
            Assert.AreEqual(_cache["six"], 6);
            Assert.AreEqual(_cache["seven"], null);
        }


        [Test]
        public void Add_Exceptions()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _cache.Add(null, 2345); 
            });
            Assert.AreEqual(_cache.Length, 0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                _cache.Add("", 2345);
            });
            Assert.AreEqual(_cache.Length, 0);
        }


        [Test]
        public void Basic_expiration()
        {
            var fillerText = "Hello World";
            var item = new StringBuilder();
            item.Append(fillerText);

            var expires = DateTime.Now.AddMilliseconds(500);
            var key = " tokeN ";

            _dateTimeProvider.Now.Returns(DateTime.Now);
            _cache.Add(key, item, expires);

            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            // move past the expiration and try to get the item
            _dateTimeProvider.Now.Returns(expires.AddMilliseconds(1));
            ValidateObjectReturned<StringBuilder>(key, true);
        }


        [Test]
        public void Basic_expiration_TimeSpan_Sliding()
        {
            var fillerText = "Hello World";
            var item = new StringBuilder();
            item.Append(fillerText);

            var start = DateTime.Now;

            var expireSpan = new TimeSpan(0,0,0,0,500);
            var expires = start.Add(expireSpan);
            var key = " tokeN ";

            _dateTimeProvider.Now.Returns(start);
            _cache.Add(key, item, expireSpan, true);

            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            // move 250MS forward and get the object
            _dateTimeProvider.Now.Returns(start.AddMilliseconds(250));
            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            // move just past the original expiration and the object should still be in cache
            _dateTimeProvider.Now.Returns(expires.AddMilliseconds(1));
            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            // move just past the expiration window and the object should be gone
            _dateTimeProvider.Now.Returns(expires.Add(expireSpan).AddMilliseconds(2));
            ValidateObjectReturned<StringBuilder>(key, true);
        }


        [Test]
        public void Basic_expiration_TimeSpan()
        {
            var fillerText = "Hello World";
            var item = new StringBuilder();
            item.Append(fillerText);

            var start = DateTime.Now;

            var expireSpan = new TimeSpan(0, 0, 0, 0, 500);
            var expires = start.Add(expireSpan);
            var key = " tokeN ";

            _dateTimeProvider.Now.Returns(start);
            _cache.Add(key, item, expireSpan, true);

            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            // move 250MS forward and get the object
            _dateTimeProvider.Now.Returns(start.AddMilliseconds(250));
            ValidateObjectReturned<StringBuilder>(key, false, fillerText);
            
            // move just past the expiration window and the object should be gone
            _dateTimeProvider.Now.Returns(expires.Add(expireSpan).AddMilliseconds(1));
            ValidateObjectReturned<StringBuilder>(key, true);
        }


        [Test]
        public void With_File_Dependency()
        {
            var path = "C:\\Some\\path\\to\\file.txt";
            var key = "token";
            var fillerText = "Hello World!";
            var val = new StringBuilder();
            val.Append(fillerText);

            var lastWriteTime = DateTime.Now.AddMinutes(-60);
            var info = Substitute.For<IFileInfo>();
            info.LastWriteTime.Returns(lastWriteTime);

            _diskManager.File.Exists(path).Returns(true);
            _diskManager.NewFileInfo(path).Returns(info);
            _cache.Add(key, val, path);
            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            info.LastWriteTime.Returns(DateTime.Now);
            ValidateObjectReturned<StringBuilder>(key, true);

            // ensure that the object is really gone
            info.LastWriteTime.Returns(lastWriteTime);
            ValidateObjectReturned<StringBuilder>(key, true);
        }

        [Test]
        public void Get_With_Wrong_Type_Throws_Exception()
        {
            var key = "token";
            var fillerText = "Hello World!";
            var val = new StringBuilder();
            val.Append(fillerText);

            _cache.Add(key, val);
            ValidateObjectReturned<StringBuilder>(key, false, fillerText);

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                _cache.Get<CacheContainerTests>(key);
            });

            Assert.IsNotNull(ex?.Message);
            Assert.IsTrue(ex.Message.IndexOf("CacheContainerTests", StringComparison.CurrentCulture) >=0);
            Assert.IsTrue(ex.Message.IndexOf("StringBuilder", StringComparison.CurrentCulture) >=0);
        }


        protected void ValidateObjectReturned(string key, bool shouldBeNull, string expectedText = null)
        {
            var result = _cache[key];
            var objResult = _cache.Get(key);
            if (shouldBeNull)
            {
                Assert.IsNull(result);
                Assert.IsNull(objResult);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsNotNull(objResult);
                Assert.AreEqual(expectedText, result.ToString());
                Assert.AreEqual(expectedText, objResult.ToString());
            }
        }
        protected void ValidateObjectReturned<T>(string key, bool shouldBeNull, string expectedText = null) where T:class
        {
            ValidateObjectReturned(key, shouldBeNull, expectedText);
            var typedresult = _cache.Get<T>(key);
            if (shouldBeNull)
            {
                Assert.IsNull(typedresult);
            }
            else
            {
                Assert.IsNotNull(typedresult);
                Assert.AreEqual(expectedText, typedresult.ToString());
            }
        }

        [Test]
        public void Test_return_base_class_from_child_class()
        {
            var val = new ContainerMockChild {Name = "Some Name", Address = "Some Address"};
            var key = " tOken ";

            _cache.Add(key, val);
            var result = _cache.Get<ContainerMockChild>(key);
            Assert.IsNotNull(result);
            Assert.AreEqual(val.Name, result.Name);
            Assert.AreEqual(val.Address, result.Address);

            var baseResult = _cache.Get<ContainerMockBase>(key);
            Assert.IsNotNull(baseResult);
            Assert.AreEqual(val.Name, baseResult.Name);

            var baseIResult = _cache.Get<IContainerMockBase>(key);
            Assert.IsNotNull(baseIResult);
            Assert.AreEqual(val.Name, baseIResult.Name);

            var childIResult = _cache.Get<IContainerMockChild>(key);
            Assert.IsNotNull(childIResult);
            Assert.AreEqual(val.Name, childIResult.Name);
            Assert.AreEqual(val.Address, childIResult.Address);
        }

        //TODO: Test typed values, sliding expiration, file dependencies, etc.. unless those are tested in the /Dependency/ tests
    }

    interface IContainerMockBase
    {
        string Name { get; }
    }

    interface IContainerMockChild : IContainerMockBase
    {
        string Address { get; }
    }

    class ContainerMockBase : IContainerMockBase
    {
        public string Name { get; set; }
    }

    class ContainerMockChild : ContainerMockBase, IContainerMockChild
    {
        public string Address { get; set; }
    }
}
