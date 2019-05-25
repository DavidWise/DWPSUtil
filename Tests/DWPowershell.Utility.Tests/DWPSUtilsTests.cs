using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using StaticAbstraction.IO;
using StaticAbstraction.IO.Mocks;
using StaticAbstraction.Reflection;
using System;

namespace DWPowerShell.Utility.Tests
{
    [TestFixture]
    public class DWPSUtilsTests
    {
        private IStaticAbstraction _diskManager = null;

        [SetUp]
        public void setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
            DWPSUtils._diskManager = _diskManager;
        }


        [TestCase("C:\\junk", "C:\\junk\\")]
        [TestCase("C:\\junk\\", "C:\\junk\\")]
        [TestCase("C:", "C:\\")]
        [TestCase("C:\\", "C:\\")]
        public void ForceTrailingSlash_Valid(string compare, string expected)
        {
            var result = DWPSUtils.ForceTrailingSlash(compare);
            Assert.IsTrue(result == expected);
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("   \t ")]
        public void ForceTrailingSlash_Invalid(string compare)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var result = DWPSUtils.ForceTrailingSlash(compare);
            });
        }
 
        
        [TestCase("C:\\Users\\Public\\Libraries", "C:\\Users\\Public", "..")]
        [TestCase("C:\\Users\\", "C:\\Users\\Public", ".\\Public")]
        [TestCase("C:\\Users\\", "C:\\Users\\Public\\", ".\\Public\\")]
        [TestCase("C:\\Users", "C:\\Users\\Public", ".\\Public")]
        [TestCase("C:\\Users", "C:\\Users\\Public\\", ".\\Public\\")]
        public void BuildRelativePath_basic(string source, string target, string expected)
        {
            _diskManager.NewDirectoryInfo(source).Returns(new MockDirectoryInfo { Exists = true });
            _diskManager.NewFileInfo(source).Returns(new MockFileInfo { Exists = false });

            _diskManager.NewDirectoryInfo(target).Returns(new MockDirectoryInfo { Exists = true });
            _diskManager.NewFileInfo(target).Returns(new MockFileInfo { Exists = false });

            var relPath = DWPSUtils.BuildRelativePath(source, target);
            Assert.AreEqual(expected, relPath);
        }

        [TestCase("C:\\Users\\Public\\Libraries\\junk.file", "C:\\Users\\Public", "..")]
        [TestCase("C:\\Users\\Public\\Libraries\\junk.file", "C:\\Users\\Public\\", "..\\")]
        public void BuildRelativePath_basic_file(string source, string target, string expected)
        {
            _diskManager.NewDirectoryInfo(source).Returns(new MockDirectoryInfo { Exists = false});
            _diskManager.NewFileInfo(source).Returns(new MockFileInfo { Exists = true });

            _diskManager.NewDirectoryInfo(target).Returns(new MockDirectoryInfo { Exists = true });
            _diskManager.NewFileInfo(target).Returns(new MockFileInfo { Exists = false });

            var relPath = DWPSUtils.BuildRelativePath(source, target);
            Assert.AreEqual(expected, relPath);
        }

        [TestCase("C:\\Users\\Public\\Libraries", "C:\\Users\\Public\\junk.file", "..\\junk.file")]
        public void BuildRelativePath_basic_targetfile(string source, string target, string expected)
        {
            _diskManager.NewDirectoryInfo(source).Returns(new MockDirectoryInfo { Exists = true });
            _diskManager.NewFileInfo(source).Returns(new MockFileInfo { Exists = false });

            _diskManager.NewDirectoryInfo(target).Returns(new MockDirectoryInfo { Exists = false });
            _diskManager.NewFileInfo(target).Returns(new MockFileInfo { Exists = true });

            var relPath = DWPSUtils.BuildRelativePath(source, target);
            Assert.AreEqual(expected, relPath);
        }


        // Need to find a better way to test this
        ////[TestCase("C:\\Users", "P:\\Corporate")] // had to comment this out as I apparently lost access to this drive...
        //[TestCase("C:\\Users", "\\\\SomeShare\\swapps")]
        //public void BuildRelativePath_cross_volume(string source, string target)
        //{
        //    var ex = Assert.Throws<ArgumentException>(() => {
        //        var relPath = PSUtils.BuildRelativePath(source, target);
        //    });

        //    Assert.AreEqual(ex.Message, "Paths must have a common prefix");
        //}


        [Test]
        public void GetAssemblyFolder_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => { DWPSUtils.GetAssemblyFolder((IAssemblyInstance)null); });
            Assert.AreEqual(ex.ParamName, "assembly");
        }


        [TestCase("F:\\Bad\\Path\\To", "F:\\Bad\\Path\\To\\")]
        [TestCase("F:\\Bad\\Path\\To\\", "F:\\Bad\\Path\\To\\")]
        public void GetAssemblyFolder_Path(string path, string expected)
        {
            var asm = Substitute.For<IAssemblyInstance>();
            asm.Location.Returns($"{path.Trim('\\')}\\Library.dll");

            var info = Substitute.For<IFileInfo>();
            info.DirectoryName.Returns(path);

            _diskManager.NewFileInfo(Arg.Any<string>()).Returns(info);

            var result = DWPSUtils.GetAssemblyFolder(asm);
            Assert.AreEqual(result, expected);
        }


        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase(" ", false)]
        [TestCase("\t", false)]
        [TestCase(" \t", false)]
        [TestCase("\\\\server\\path", true)]
        [TestCase("R:\\folder\\path", true)]
        [TestCase("Drive:\\folder\\path", false)]
        [TestCase(":\\folder\\path", false)]
        public void IsFullPath_common(string value, bool expected)
        {
            Assert.AreEqual(DWPSUtils.IsFullPath(value), expected);
        }


        [TestCase("H:\\some\\folder", "H:\\some\\folder\\")]
        [TestCase("H:\\some\\folder\\", "H:\\some\\folder\\")]
        [TestCase("H:", "H:\\")]
        [TestCase("H:\\", "H:\\")]
        public void CurrentFolder_simple(string value, string expected)
        {
            _diskManager.Directory.GetCurrentDirectory().Returns(value);

            var result = DWPSUtils.CurrentFolder;
            Assert.AreEqual(result, expected);
        }



        [TestCase("H:\\some\\folder", "H_some_folder")]
        [TestCase("H:\\some/folder>target|result", "H_some_folder_target_result")]
        [TestCase("Test Value*.txt", "Test_Value_.txt")]
        [TestCase("Test Value.?x?", "Test_Value._x_")]
        public void MakeFileSystemSafe_tests(string testValue, string expectedValue)
        {
            var result = DWPSUtils.MakeFileSystemSafe(testValue);
            Assert.AreEqual(expectedValue, result);
        }


    }
}
