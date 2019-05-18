using System;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using StaticAbstraction.IO;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class GetDirectoryPathTests
    {
        private IStaticAbstraction _diskManager = null;

        [SetUp]
        public void Setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
            DWPSUtils._diskManager = _diskManager;
        }


        [TestCase(null)]
        [TestCase("")]
        public void GetDirectoryPath_common(string value)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => { DWPSUtils.GetDirectoryPath(value); });
            Assert.AreEqual(ex.ParamName, "path");
        }

        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase(" \t ")]
        [TestCase("G:\\Some\\Bad\\Path\\item.txt")]
        public void GetDirectoryPath_itemNotFound(string value)
        {
            _diskManager.File.Exists(Arg.Any<string>()).Returns(false);
            _diskManager.Directory.Exists(Arg.Any<string>()).Returns(false);

            var ex = Assert.Throws<ApplicationException>(() => { DWPSUtils.GetDirectoryPath(value); });
            Assert.IsNotNull(ex?.Message);
            if (value == "")
                Assert.IsTrue(ex.Message.IndexOf("''") > 0);
            else
                Assert.IsTrue(ex.Message.IndexOf(value) > 0);
        }

        [TestCase("G:\\folder", "file.txt", "G:\\folder\\")]
        [TestCase("G:\\folder", "file", "G:\\folder\\")]
        [TestCase("G:\\folder\\", "file.txt", "G:\\folder\\")]
        [TestCase("G:\\folder\\", "file", "G:\\folder\\")]
        public void GetDirectoryPath_existingFile(string folder, string fileName, string expected)
        {
            var filePath = folder.Trim('\\') + "\\" + fileName;
            _diskManager.File.Exists(filePath).Returns(true);
            var info = Substitute.For<IFileInfo>();
            info.DirectoryName.Returns(folder);
            _diskManager.NewFileInfo(filePath).Returns(info);

            var result = DWPSUtils.GetDirectoryPath(filePath);
            Assert.AreEqual(result, expected);
        }

        [TestCase("G:\\folder", "G:\\folder\\")]
        [TestCase("G:\\folder\\", "G:\\folder\\")]
        public void GetDirectoryPath_existingFolder(string folder, string expected)
        {
            _diskManager.File.Exists(Arg.Any<string>()).Returns(false);
            _diskManager.Directory.Exists(folder).Returns(true);
            var info = Substitute.For<IDirectoryInfo>();
            info.FullName.Returns(folder);

            _diskManager.NewDirectoryInfo(folder).Returns(info);

            var result = DWPSUtils.GetDirectoryPath(folder);
            Assert.AreEqual(result, expected);
        }
    }
}
