using System.IO;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction.Reflection;

namespace DWPowerShell.Utility.Tests.PSUtilFragments
{
    [TestFixture]
    public class EmbeddedResource_Tests
    {
        private IAssemblyInstance _assembly;
        private readonly string[] _resourceNames = new string[] {"Resource.Name.1", "Resource.Name.2", "Resource.Name.3"};

        private readonly string[] _resourceData = new string[]
        {
            BuildRepeatedString("String 1 says hi ", 200),
            BuildRepeatedString("a Word from string 2 ", 225),
            BuildRepeatedString("and now From string 3 ", 125)
        };


        protected  static string BuildRepeatedString(string value, int times)
        {
            var result = new StringBuilder();
            for (int i = 0 ; i < times;i++)
            {
                result.Append(value);
            }

            return result.ToString();
        }

        protected static Stream BuildStream(string value)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(value);
            return new MemoryStream(data);
        }

        [SetUp]
        public void Setup()
        {

            _assembly = Substitute.For<IAssemblyInstance>();
            _assembly.GetManifestResourceNames().Returns(_resourceNames);
            _assembly.GetManifestResourceStream(Arg.Is<string>(x=> _resourceNames[0].EndsWith(x))).Returns(BuildStream(_resourceData[0]));
            _assembly.GetManifestResourceStream(Arg.Is<string>(x=> _resourceNames[1].EndsWith(x))).Returns(BuildStream(_resourceData[1]));
            _assembly.GetManifestResourceStream(Arg.Is<string>(x=> _resourceNames[2].EndsWith(x))).Returns(BuildStream(_resourceData[2]));
        }


        [Test]
        public void GetResourceNames_Tests()
        {
            var names = DWPSUtils.GetEmbeddedResourceNames(_assembly);
            _assembly.Received(1).GetManifestResourceNames();
            CollectionAssert.AreEqual(_resourceNames, names);
        }


        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(8)]
        public void GetEmbeddedResource_valid_partialnames_Tests(int lengthOfPartial)
        {
            for (var i = 0; i < _resourceNames.Length; i++)
            {
                var last5 = _resourceNames[i].Substring(_resourceNames[i].Length - lengthOfPartial);
                var res = DWPSUtils.GetEmbeddedResource(_assembly, last5);
                Assert.AreEqual(_resourceData[i], res);
            }
        }


        [Test]
        public void GetEmbeddedResource_valid_fullnames_Tests()
        {
            for (var i = 0; i < _resourceNames.Length; i++)
            {
                var res = DWPSUtils.GetEmbeddedResource(_assembly, _resourceNames[i]);
                Assert.AreEqual(_resourceData[i], res);
            }
        }
    }
}
