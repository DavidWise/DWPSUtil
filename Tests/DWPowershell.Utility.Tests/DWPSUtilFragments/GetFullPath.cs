using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StaticAbstraction;
using StaticAbstraction.IO.Mocks;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class GetFullPath
    {
        private IStaticAbstraction _diskManager = null;

        [SetUp]
        public void setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
            DWPSUtils._diskManager = _diskManager;
        }



        [TestCase("H:\\some\\folder", "H:\\some\\folder")]
        [TestCase("\\\\some\\folder", "\\\\some\\folder")]
        public void GetFullPath_Existing_matches(string testValue, string expectedValue)
        {
            string basePath = "J:\\junk\\path";
            var result = DWPSUtils.GetFullPath(basePath, testValue);
            Assert.AreEqual(expectedValue, result);
        }

        [TestCase("H:\\some\\folder", null, "H:\\some\\folder")]
        [TestCase("H:\\some\\folder", "", "H:\\some\\folder")]
        [TestCase("H:\\some\\folder", " ", "H:\\some\\folder")]
        [TestCase("H:\\some\\folder", "\t", "H:\\some\\folder")]
        [TestCase("H:\\some\\folder", " \t ", "H:\\some\\folder")]
        public void GetFullPath_whitespace_path_matches_basePath(string basePath, string fileName, string expectedValue)
        {
            var combined = string.IsNullOrWhiteSpace(fileName) ? basePath : new StaticAbstractionWrapper().Path.Combine(basePath, fileName);
            _diskManager.Path.Combine(basePath, fileName).Returns(combined);
            var info = new MockFileInfo { FullName = combined };
            _diskManager.NewFileInfo(combined).Returns(info);
            var result = DWPSUtils.GetFullPath(basePath, fileName);
            Assert.AreEqual(expectedValue, result);
            _diskManager.Path.Received(0).Combine(Arg.Any<string>(), Arg.Any<string>());
            _diskManager.Received(0).NewFileInfo(Arg.Any<string>());
        }

        [TestCase("H:\\some\\folder", "SomeName", "H:\\some\\folder\\SomeName")]
        [TestCase("H:\\some\\folder", "Some\\Name", "H:\\some\\folder\\Some\\Name")]
        public void GetFullPath_valid_path(string basePath, string fileName, string expectedValue)
        {
            var combined = new StaticAbstractionWrapper().Path.Combine(basePath, fileName);
            _diskManager.Path.Combine(basePath, fileName).Returns(combined);

            var info = new MockFileInfo { FullName = combined };
            _diskManager.NewFileInfo(combined).Returns(info);

            var result = DWPSUtils.GetFullPath(basePath, fileName);
            Assert.AreEqual(expectedValue, result);
            _diskManager.Path.Received(1).Combine(basePath, fileName);
            _diskManager.Received(1).NewFileInfo(combined);
        }

    }
}
