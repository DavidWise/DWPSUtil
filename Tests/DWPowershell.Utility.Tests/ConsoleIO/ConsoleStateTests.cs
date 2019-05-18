using System;
using NSubstitute;
using NUnit.Framework;
using DWPowerShell.Utility.ConsoleIO;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.ConsoleIO
{
    [TestFixture]
    public class ConsoleStateTests
    {
        protected IConsole _console;
        protected ConsoleState _consoleState;

        [SetUp]
        public void Setup()
        {
            _console = Substitute.For<IConsole>();
            _console.CursorLeft.Returns(3);
            _console.CursorTop.Returns(10);
            _console.WindowTop.Returns(25);
            _console.BackgroundColor.Returns(ConsoleColor.DarkMagenta);
            _console.ForegroundColor.Returns(ConsoleColor.Red);

            _consoleState = new ConsoleState(_console);
        }

        [Test]
        public void InitTests()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var state = new ConsoleState(null);
            });
        }


        [Test]
        public void Verify_Initial_values()
        {
            Assert.AreEqual(_console.CursorLeft, _consoleState.CursorLeft);
            Assert.AreEqual(_console.CursorTop, _consoleState.CursorTop);
            Assert.AreEqual(_console.WindowTop, _consoleState.WindowTop);
            Assert.AreEqual(_console.BackgroundColor, _consoleState.BackColor);
            Assert.AreEqual(_console.ForegroundColor, _consoleState.ForeColor);
        }


        [Test]
        public void ResetColors_Tests()
        {
            _consoleState.ResetColors();
            _console.Received().ForegroundColor = _consoleState.ForeColor;
            _console.Received().BackgroundColor = _consoleState.BackColor;
        }


        [Test]
        public void ResetCursor_Tests()
        {
            _consoleState.ResetCursor();
            _console.Received().CursorTop = _consoleState.CursorTop;
            _console.Received().CursorLeft = _consoleState.CursorLeft;
        }

        [Test]
        public void ResetState_Tests()
        {
            ResetColors_Tests();
            ResetCursor_Tests();
        }
    }
}
