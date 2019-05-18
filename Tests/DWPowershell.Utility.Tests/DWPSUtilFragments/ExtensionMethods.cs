using System;
using NUnit.Framework;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class ExtensionMethodTests
    {
        [TestCase("one,two,THRee,four", "one,two,THRee,four", ",")]
        [TestCase("one , two,\rTHRee\n,\t   four\t  ", "one,two,THRee,four", ",")]
        [TestCase("one", "one", ",")]
        [TestCase("", "", ",")]
        [TestCase(null, null, ",")]
        public void TrimAll_Arrays_Tests(string values, string expected, string delim)
        {
            var splitDelim = new string[] { delim };
            var source = values?.Split(splitDelim, StringSplitOptions.None);
            var expectedValues = expected?.Split(splitDelim, StringSplitOptions.None);

            var result = source.TrimAll();

            Assert.AreEqual(result, expectedValues);
        }

        [Test]
        public void TrimAll_valid_tests()
        {
            var testValues = new string[] { "One", null, "", "\t", " ", " \t ", "tWo" };
            var expectedValues = new string[] { "One", null, "", "", "", "", "tWo" };

            var results = testValues.TrimAll();
            CollectionAssert.AreEqual(results, expectedValues);
        }

        [Test]
        public void TrimAll_invalid_null()
        {
            string[] testValues = null;

            var results = testValues.TrimAll();
            Assert.IsNull(results);
        }

        [Test]
        public void TrimAll_invalid_emptyArray()
        {
            var testValues = new string[0];

            var results = testValues.TrimAll();
            CollectionAssert.IsEmpty(results);
        }

    }
}
