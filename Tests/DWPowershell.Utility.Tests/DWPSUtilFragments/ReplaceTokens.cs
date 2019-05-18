using System;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class ReplaceTokens
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
        public void ReplaceTokens_Empty(string text)
        {
            var result = DWPSUtils.ReplaceTokens(text, null, null);
            Assert.IsTrue(result == string.Empty);
        }


        [Test]
        public void ReplaceTokens_Basic()
        {
            var source = " This    is      a Test! ";
            var tokens = new string[] { " " };
            var reptokens = new string[] { "_" };

            var result = DWPSUtils.ReplaceTokens(source, tokens, reptokens);
            Assert.IsTrue(result == source.Replace(" ", "_"));

            result = DWPSUtils.ReplaceTokens(source, tokens, reptokens, true);
            Assert.AreEqual("_This_is_a_Test!_", result);
        }


        [TestCase(null, null)]
        [TestCase(null, new string[0])]
        [TestCase(null, new string[] { "_", "-" })]
        [TestCase(new string[0], null)]
        [TestCase(new string[] { "_", "-" }, null)]
        public void ReplaceTokens_Invalid(string[] tokens, string[] replacements)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var result = DWPSUtils.ReplaceTokens("this is a test", tokens, replacements);
            });
        }


        [Test]
        public void ReplaceTokens_SimplePath()
        {
            var sourceText = "The pUrple fox swam through purplePURPLE water to paint the deck puRple";
            var tokens = new string[] { "purple" };
            var replaceWith = new string[] { "red" };
            var expected = "The red fox swam through red water to paint the deck red";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, true);

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void ReplaceTokens_SingleCharacter()
        {
            var sourceText = "PURpleThe pUrple fox swam through purplePURPLE water to paint the deck puRple";
            var tokens = new string[] { "purple" };
            var replaceWith = new string[] { "red" };
            var expected = "redThe red fox swam through red water to paint the deck red";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, true);

            Assert.AreEqual(result, expected);
        }


        [Test]
        public void ReplaceTokens_SingleCharacterTrimDuplicates()
        {
            var sourceText = "||1|2|||3|";
            var tokens = new string[] { "|" };
            var replaceWith = new string[] { "_" };
            var expected = "_1_2_3_";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, true);

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void ReplaceTokens_SingleCharacterKeepDuplicates()
        {
            var sourceText = "||1|2|||3|";
            var tokens = new string[] { "|" };
            var replaceWith = new string[] { "_" };
            var expected = "__1_2___3_";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, false);

            Assert.AreEqual(result, expected);
        }


        [Test]
        public void ReplaceTokens_ReplaceContainsToken()
        {
            var sourceText = "The red fox swam through red water to paint the deck red";
            var tokens = new string[] { " ", "i", "o" };
            var replaceWith = new string[] { "\\ /", "dig", "OO" };
            var expected = "The\\ /red\\ /fOOx\\ /swam\\ /thrOOugh\\ /red\\ /water\\ /tOO\\ /padignt\\ /the\\ /deck\\ /red";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, true);

            Assert.AreEqual(result, expected);
        }


        [Test]
        public void ReplaceTokens_RemovingCharacters()
        {
            var sourceText = "The red fox swam through red water to paint the deck red";
            var tokens = new string[] { " ", "i", "o" };
            var replaceWith = new string[] { "_", "", "" };
            var expected = "The_red_fx_swam_thrugh_red_water_t_pant_the_deck_red";

            var result = DWPSUtils.ReplaceTokens(sourceText, tokens, replaceWith, true);

            Assert.AreEqual(result, expected);
        }
    }
}
