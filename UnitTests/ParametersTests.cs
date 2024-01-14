using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessTextures;
using System;

namespace UnitTests
{
    [TestClass]
    public class ParametersTests
    {
        private string[] SplitCommandLineArgs(string s)
        {
            return s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        }

        [TestMethod("Input Output Directory")]
        public void TestInOutDirectory()
        {
            const string TestInputDirectory = "TestInputDirectory";
            const string TestOutputDirectory = "TestOutputDirectory";

            var Parameters1 = new Parameters(SplitCommandLineArgs($"-in {TestInputDirectory} -out {TestOutputDirectory}"));
            Assert.AreEqual(TestInputDirectory, Parameters1.InputDirectory);
            Assert.AreEqual(TestOutputDirectory, Parameters1.OutputDirectory);

            var Parameters2 = new Parameters(SplitCommandLineArgs($"--In {TestInputDirectory} --Out {TestOutputDirectory}"));
            Assert.AreEqual(TestInputDirectory, Parameters2.InputDirectory);
            Assert.AreEqual(TestOutputDirectory, Parameters2.OutputDirectory);
        }

        [TestMethod("Input Output Directory (quotas)")]
        public void TestInOutDirectoryWithQuotas()
        {
            var Parameters1 = new Parameters(SplitCommandLineArgs("-in \"g:\\Trainz\\ProcessTextures\\TestResources\\\" -out \"g:\\Trainz\\ProcessTextures\\TestResources\\\""));
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.InputDirectory);
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.OutputDirectory);
            Assert.IsFalse(Parameters1.GeneratePSD);
        }

        [TestMethod("Long directory name")]
        public void TestLongName()
        {
            string s = @"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body";
            string[] args = new string[2]
            {
                "-in",
                $"\"{s}\""
            };

            var Parameters = new Parameters(args);
            Assert.AreEqual(s, Parameters.InputDirectory);
        }

        [TestMethod("Test PSD 1")]
        public void TestPSD1()
        {
            var Parameters1 = new Parameters(SplitCommandLineArgs("-in \"g:\\Trainz\\ProcessTextures\\TestResources\\\" -out \"g:\\Trainz\\ProcessTextures\\TestResources\\\" -psd"));
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.InputDirectory);
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.OutputDirectory);
            
            Assert.IsTrue(Parameters1.GeneratePSD);
        }

        [TestMethod("Test PSD 2")]
        public void TestPSD2()
        {
            var Parameters1 = new Parameters(SplitCommandLineArgs("-psd -in \"g:\\Trainz\\ProcessTextures\\TestResources\\\" -out \"g:\\Trainz\\ProcessTextures\\TestResources\\\""));
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.InputDirectory);
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.OutputDirectory);

            Assert.IsTrue(Parameters1.GeneratePSD);
        }

        [TestMethod("Test PSD 3")]
        public void TestPSD3()
        {
            var Parameters1 = new Parameters(SplitCommandLineArgs("-in \"g:\\Trainz\\ProcessTextures\\TestResources\\\" -psd -out \"g:\\Trainz\\ProcessTextures\\TestResources\\\""));
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.InputDirectory);
            Assert.AreEqual(@"g:\Trainz\ProcessTextures\TestResources", Parameters1.OutputDirectory);

            Assert.IsTrue(Parameters1.GeneratePSD);
        }

        [TestMethod("Bad Parameters 1")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadParameters1()
        {
            new Parameters(SplitCommandLineArgs($"-in -out TestValue"));
        }

        [TestMethod("Bad Parameters 2")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadParameters2()
        {
            new Parameters(SplitCommandLineArgs($"-in TestValue -out"));
        }
    }
}
