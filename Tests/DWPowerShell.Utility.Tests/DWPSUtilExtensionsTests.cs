using System;
using NUnit.Framework;

namespace DWPowerShell.Utility.Tests
{
    [TestFixture]
    public class DWPSUtilExtensionsTests
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
            Assert.AreEqual(testValues, results);
        }

        [Test]
        public void TrimAll_invalid_emptyArray()
        {
            var testValues = new string[0];

            var results = testValues.TrimAll();
            CollectionAssert.IsEmpty(results);
            Assert.AreEqual(testValues, results);
        }

        [Test]
        public void TrimAll_invalid_char_emptyArray()
        {
            var splitChars = new char[0];
            var testValues = new string[] { "Hello", "There" };

            var results = testValues.TrimAll(splitChars);

            CollectionAssert.AreEqual(testValues, results);
            Assert.AreEqual(testValues, results);
        }

        [Test]
        public void TrimAll_invalid_char_nullArray()
        {
            char[] splitChars = null;
            var testValues = new string[] { "Hello", "There" };

            var results = testValues.TrimAll(splitChars);

            CollectionAssert.AreEqual(testValues, results);
            Assert.AreEqual(testValues, results);
        }


        [TestCase("Single Value", "Single Value", "")]
        [TestCase("Single Value", "Single Valu", "e")]
        [TestCase("Single Value", "ingle Valu", "Se")]
        [TestCase("Single Value", "Single Valu", "se")]
        [TestCase("Single Value", "ngle Val", "SsIiUuEe")]
        public void TrimAll_Chars_Tests(string values, string expected, string chars)
        {
            var splitDelim = ",";
            char[] splitChars = null;
            if (chars != null) splitChars = chars.ToCharArray();
            var source = values?.Split(splitDelim, StringSplitOptions.None);
            var expectedValues = expected?.Split(splitDelim, StringSplitOptions.None);

            var result = source.TrimAll(splitChars);

            Assert.AreEqual(result, expectedValues);
        }
    }
}
