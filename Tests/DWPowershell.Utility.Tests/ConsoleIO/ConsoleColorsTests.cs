using System;
using NUnit.Framework;
using DWPowerShell.Utility.ConsoleIO;

namespace DWPowerShell.Utility.Tests.ConsoleIO
{
    [TestFixture]
    public class ConsoleColorsTests
    {
        private string[] _validColorNames =
        {
            "Black", "DarkBlue", "DarkGreen", "DarkCyan", "DarkRed", "DarkMagenta", "DarkYellow", "Gray",
            "DarkGray", "Blue", "Green", "Cyan", "Red", "Magenta", "Yellow", "White"
        };

        [Test]
        public void EnsureValidResolution()
        {
            var clr = new ConsoleColors();
            Assert.That(clr.Names.Length, Is.EqualTo(16));

            Assert.IsTrue(clr["Black"] == ConsoleColor.Black);
            Assert.IsTrue(clr["DarkBlue"] == ConsoleColor.DarkBlue);
            Assert.IsTrue(clr["DarkGreen"] == ConsoleColor.DarkGreen);
            Assert.IsTrue(clr["DarkCyan"] == ConsoleColor.DarkCyan);
            Assert.IsTrue(clr["DarkRed"] == ConsoleColor.DarkRed);
            Assert.IsTrue(clr["DarkMagenta"] == ConsoleColor.DarkMagenta);
            Assert.IsTrue(clr["DarkYellow"] == ConsoleColor.DarkYellow);
            Assert.IsTrue(clr["Gray"] == ConsoleColor.Gray);
            Assert.IsTrue(clr["DarkGray"] == ConsoleColor.DarkGray);
            Assert.IsTrue(clr["Blue"] == ConsoleColor.Blue);
            Assert.IsTrue(clr["Green"] == ConsoleColor.Green);
            Assert.IsTrue(clr["Cyan"] == ConsoleColor.Cyan);
            Assert.IsTrue(clr["Red"] == ConsoleColor.Red);
            Assert.IsTrue(clr["Magenta"] == ConsoleColor.Magenta);
            Assert.IsTrue(clr["Yellow"] == ConsoleColor.Yellow);
            Assert.IsTrue(clr["White"] == ConsoleColor.White);
        }

        [Test]
        public void VerifyValidIgnoreCase()
        {
            var clr = new ConsoleColors();
            foreach (var name in _validColorNames)
            {
                var actual = clr[name];
                Assert.IsTrue(clr[name.ToLower()]==actual);
                Assert.IsTrue(clr[name.ToLower()]==actual);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("  \t ")]
        [TestCase("DarkPeach")]
        public void InvalidNames(string value)
        {
            var clr = new ConsoleColors();
            Assert.IsNull(clr[value]);
        }

        [TestCase(" BlaCk ")]
        [TestCase("\tyeLLow\r")]
        [TestCase("      white      ")]
        public void ValidNamesWithWhitespace(string value)
        {
            var clr = new ConsoleColors();
            Assert.IsNotNull(clr[value]);
        }
    }
}
