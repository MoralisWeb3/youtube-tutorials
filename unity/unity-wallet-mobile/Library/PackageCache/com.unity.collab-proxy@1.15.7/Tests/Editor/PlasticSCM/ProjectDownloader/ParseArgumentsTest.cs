using NUnit.Framework;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.ProjectDownloader;

namespace Unity.PlasticSCM.Tests.Editor.ProjectDownloader
{
    [TestFixture]
    class ParseArgumentsTest
    {
        [Test]
        public void TestParseCloudProject()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            args.Add("-cloudProject", "fpsmicrogame");

            Assert.AreEqual("fpsmicrogame", ParseArguments.CloudProject(args));
        }

        [Test]
        public void TestParseCloudOrganization()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            args.Add("-cloudOrganization", "D51E18A1-CA04-4E7C-A649-6FD2829E3223-danipen-unity");

            Assert.AreEqual("danipen-unity", ParseArguments.CloudOrganization(args));
        }

        [Test]
        public void TestParseProjectPath()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            args.Add("-createProject", @"c:\tmp\newproj");

            Assert.AreEqual(@"c:\tmp\newproj", ParseArguments.ProjectPath(args));
        }
    }
}
