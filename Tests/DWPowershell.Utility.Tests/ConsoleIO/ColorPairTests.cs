using System;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWPowerShell.Utility.ConsoleIO;

namespace DWPowerShell.Utility.Tests.ConsoleIO
{
    [TestFixture]
    public class ColorPairTests
    {
        private IConsole _console = null;
        private ConsoleColor origFG = ConsoleColor.Cyan;
        private ConsoleColor origBG = ConsoleColor.Blue;

        [SetUp]
        public void TestSetup()
        {
            _console = Substitute.For<IConsole>();
            _console.BackgroundColor = origBG;
            _console.ForegroundColor = origFG;
        }


        [Test]
        public void NormalPath()
        {
            var fg = ConsoleColor.Green;
            var bg = ConsoleColor.DarkGray;

            var colors = new ColorPair(_console, fg, bg);
            // ensure that initial color state is preserved
            Assert.IsTrue(colors.OriginalBackground == _console.BackgroundColor);
            Assert.IsTrue(colors.OriginalForeground == _console.ForegroundColor);
            Assert.IsTrue(colors.Foreground == fg);
            Assert.IsTrue(colors.Background == bg);

            // ensure that the colors are applied
            colors.ApplyColors();
            Assert.IsTrue(_console.BackgroundColor == bg);
            Assert.IsTrue(_console.ForegroundColor == fg);

            // ensure that the colors are reset
            colors.ResetColors();
            Assert.IsTrue(_console.BackgroundColor == origBG);
            Assert.IsTrue(_console.ForegroundColor == origFG);
            Assert.IsTrue(colors.Foreground == fg);
            Assert.IsTrue(colors.Background == bg);
        }


        [Test]
        public void MissingForeColor()
        {
            ConsoleColor? fg = null;
            var bg = ConsoleColor.DarkGray;

            var colors = new ColorPair(_console, fg, bg);
            Assert.IsFalse(colors.Foreground.HasValue);

            // ensure that the colors are applied
            colors.ApplyColors();
            Assert.IsTrue(_console.BackgroundColor == bg);
            Assert.IsTrue(_console.ForegroundColor == origFG);

            // ensure that the colors are reset
            colors.ResetColors();
            Assert.IsTrue(_console.BackgroundColor == origBG);
            Assert.IsTrue(_console.ForegroundColor == origFG);
            Assert.IsTrue(colors.Foreground == fg);
            Assert.IsTrue(colors.Background == bg);
        }


        [Test]
        public void MissingBackColor()
        {
            ConsoleColor? fg = ConsoleColor.Yellow;
            ConsoleColor? bg = null;

            var colors = new ColorPair(_console, fg, bg);
            Assert.IsFalse(colors.Background.HasValue);

            // ensure that the colors are applied
            colors.ApplyColors();
            Assert.IsTrue(_console.BackgroundColor == origBG);
            Assert.IsTrue(_console.ForegroundColor == fg);

            // ensure that the colors are reset
            colors.ResetColors();
            Assert.IsTrue(_console.BackgroundColor == origBG);
            Assert.IsTrue(_console.ForegroundColor == origFG);
            Assert.IsTrue(colors.Foreground == fg);
            Assert.IsTrue(colors.Background == bg);
        }
    }
}
