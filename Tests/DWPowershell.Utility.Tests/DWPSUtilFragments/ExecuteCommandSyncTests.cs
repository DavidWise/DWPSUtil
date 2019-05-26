using System;
using System.IO;
using NUnit.Framework;
using DWPowerShell.Utility.Abstraction.Process;
using NSubstitute;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class ExecuteCommandSyncTests
    {
        private IStaticAbstraction _diskManager;
        private IProcessManager _processManager;

        [SetUp]
        public void Setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
            _processManager = Substitute.For<IProcessManager>();

            DWPSUtils._diskManager = _diskManager;
            DWPSUtils._processManager = _processManager;
        }

        [Test]
        public void Bad_Parameter_Tests()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DWPSUtils.ExecuteCommandSync(_diskManager, null, "filler", "somePath");
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                DWPSUtils.ExecuteCommandSync(null, _processManager, "filler", "somePath");
            });
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(" \t ")]
        public void Bad_Command_Tests(string command)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, command, "somePath");
            });
        }

        [Test]
        public void Bad_Path_Tests()
        {
            var badPath = "f:\\bad\\path";

            _diskManager.Directory.Exists(badPath).Returns(false);

            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, "filler", badPath);
            });
        }


        [Test]
        public void Valid_Happy_Path_Tests()
        {
            var expectedCommand = "SomeAppName";

            DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, expectedCommand, null);

            _processManager.Received(1).Execute(Arg.Any<string>(), "/c " + expectedCommand, Arg.Any<int>());
        }


        [Test]
        public void Valid_Happy_Path_result_Tests()
        {
            var expectedCommand = "SomeAppName";
            var expectedResult = new ProcessResult
            {
                Output = "Sample Output",
                Errors = "There were Errors",
                Command = expectedCommand,
                ElapsedMilliseconds = 1234
            };
            _processManager.Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(expectedResult);

            var result = DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, expectedCommand, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, expectedResult);
        }

        [Test]
        public void Valid_Change_app_dir_Tests()
        {
            var currentFolder = "C:\\current\\folder\\here";
            var appFolder = "C:\\app\\folder\\there";

            _diskManager.Directory.Exists(appFolder).Returns(true);
            _diskManager.Directory.GetCurrentDirectory().Returns(currentFolder);

            DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, "filler", appFolder);

            _diskManager.Directory.Received(1).Exists(appFolder);
            _diskManager.Directory.Received(1).GetCurrentDirectory();
            _diskManager.Directory.Received(1).SetCurrentDirectory(appFolder);
            _diskManager.Directory.Received(1).SetCurrentDirectory(currentFolder);
        }
    }
}
