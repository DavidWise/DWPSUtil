using NUnit.Framework;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class MakeBooleanTests
    {
        [TestCase("TRUE", true)]
        [TestCase("True", true)]
        [TestCase("yes", true)]
        [TestCase("on", true)]
        [TestCase("1", true)]
        [TestCase("-1", true)]
        [TestCase("y", true)]
        [TestCase("t", true)]
        public void MakeBoolean_positives(string value, bool expected)
        {
            Assert.AreEqual(DWPSUtils.MakeBoolean(value), expected);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("\t", false)]
        [TestCase("   ", false)]
        [TestCase("  \r ", false)]
        [TestCase("FALSE", false)]
        [TestCase("False", false)]
        [TestCase("No", false)]
        [TestCase("off", false)]
        [TestCase("0", false)]
        [TestCase("234", false)]
        [TestCase("-12312", false)]
        [TestCase("f", false)]
        [TestCase("n", false)]
        public void MakeBoolean_negatives(string value, bool expected)
        {
            Assert.AreEqual(DWPSUtils.MakeBoolean(value), expected);
        }

    }
}
