using System;
using NUnit.Framework;
using NSubstitute;
using DWPowerShell.Utility.Cache.Dependency;
using StaticAbstraction.IO.Mocks;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.Cache.Dependency
{
    [TestFixture()]
    public class CacheDependencyFileTests
    {
        private string fullPath = null;
        private MockFileInfo fi = null;
        private IStaticAbstraction dm = null;

        [SetUp]
        public void TestSetup()
        {
            fullPath = "C:\\Some\\Folder\\With\\Files.txt";
            fi = new MockFileInfo()
            {
                Name = "Files.txt",
                FullName = fullPath,
                LastWriteTime = DateTime.Now.AddDays(-1),
                Exists = true
            };

            dm = Substitute.For<IStaticAbstraction>();
            dm.File.Exists(Arg.Any<string>()).Returns(fi.Exists);
            dm.NewFileInfo(Arg.Any<string>()).Returns(fi);
        }

        [Test]
        public void FileExists()
        {
            var dep = new CacheDependencyFile(dm, fullPath);

            Assert.IsTrue(dep.FullPath == fi.FullName);
            Assert.IsTrue(dep.Exists == fi.Exists);
            Assert.IsTrue(dep.LastModified == fi.LastWriteTime);
            Assert.IsFalse(dep.HasExpired);
            Assert.IsFalse(dep.HasChanged());

            dm.File.Exists(Arg.Any<string>()).Returns(false);
            Assert.IsFalse(dep.HasExpired);
            Assert.IsTrue(dep.HasChanged());
        }

        [Test]
        public void FileNotExists()
        {
            fi.Exists = false;
            dm.File.Exists(Arg.Any<string>()).Returns(false);

            var dep = new CacheDependencyFile(dm, fullPath);

            Assert.IsTrue(dep.LastModified == DateTime.MinValue);
            Assert.IsFalse(dep.HasExpired);
            Assert.IsFalse(dep.HasChanged());
        }

        [Test]
        public void FileExistAndTimeStampOlder()
        {
            var dep = new CacheDependencyFile(dm, fullPath);

            fi.LastWriteTime = fi.LastWriteTime.AddMinutes(-5);

            Assert.IsTrue(dep.LastModified < DateTime.Now);
            Assert.IsFalse(dep.HasExpired);
            Assert.IsTrue(dep.HasChanged());
        }

        [Test]
        public void FileExistAndTimeStampNewer()
        {
            var dep = new CacheDependencyFile(dm, fullPath);

            fi.LastWriteTime = DateTime.Now;

            Assert.IsTrue(dep.HasExpired);
            Assert.IsTrue(dep.HasChanged());
        }

        [Test]
        public void FileExistButWasDeleted()
        {
            var dep = new CacheDependencyFile(dm, fullPath);

            dm.File.Exists(Arg.Any<string>()).Returns(false);
            fi.Exists = false;

            Assert.IsFalse(dep.HasExpired);
            Assert.IsTrue(dep.HasChanged());
        }

        [Test]
        public void FileNotExistButAdded()
        {
            dm.File.Exists(Arg.Any<string>()).Returns(false);
            fi.Exists = false;
            var dep = new CacheDependencyFile(dm, fullPath);

            dm.File.Exists(Arg.Any<string>()).Returns(true);
            fi.Exists = true;
            fi.LastWriteTime = DateTime.Now;

            Assert.IsTrue(dep.HasExpired);
            Assert.IsTrue(dep.HasChanged());
        }
    }
}
