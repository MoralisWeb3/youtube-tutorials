using System.Collections.Generic;

using NUnit.Framework;
using Unity.PlasticSCM.Editor.ProjectDownloader;

namespace Unity.PlasticSCM.Tests.Editor.ProjectDownloader
{
    [TestFixture]
    public class CommandLineArgumentsTests
    {
        [Test]
        public void TestNullArguments()
        {
            string[] args = null;

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(0, parsedArgs.Count);
        }

        [Test]
        public void TestEmptyArguments()
        {
            string[] args = new string[]{ };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(0, parsedArgs.Count);
        }

        [Test]
        public void TestProgramArgumentOnly()
        {
            string[] args = new string[]
            {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe"
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(0, parsedArgs.Count);
        }

        [Test]
        public void TestSingleArgument()
        {
            string useHub = "-useHub";

            string[] args = new string[]
            {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                useHub
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(1, parsedArgs.Count);
            Assert.IsTrue(parsedArgs.ContainsKey(useHub));
            Assert.IsNull(parsedArgs[useHub]);
        }

        [Test]
        public void TestKeyValueArgument()
        {
            string createProjectKey = "-createProject";
            string createProjectValue = @"c:\tmp\newproj";

            string[] args = new string[]
            {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                createProjectKey,
                createProjectValue  };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(1, parsedArgs.Count);
            Assert.IsTrue(parsedArgs.ContainsKey(createProjectKey));
            Assert.AreEqual(createProjectValue, parsedArgs[createProjectKey]);
        }

        [Test]
        public void TestConsecutiveSingleArguments()
        {
            string useHub = "-useHub";
            string useIPC = "-useIPC";

            string[] args = new string[]
            {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                useHub,
                useIPC
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(2, parsedArgs.Count);

            Assert.IsTrue(parsedArgs.ContainsKey(useHub));
            Assert.IsNull(parsedArgs[useHub]);

            Assert.IsTrue(parsedArgs.ContainsKey(useIPC));
            Assert.IsNull(parsedArgs[useIPC]);
        }

        [Test]
        public void TestConsecutiveKeyValueArguments()
        {
            string createProjectKey = "-createProject";
            string createProjectValue = @"c:\tmp\newproj";

            string cloudEnvironmentKey = "-cloudEnvironment";
            string cloudEnvironmentValue = "production";

            string[] args = new string[] {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                createProjectKey,
                createProjectValue,
                cloudEnvironmentKey,
                cloudEnvironmentValue
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(2, parsedArgs.Count);

            Assert.IsTrue(parsedArgs.ContainsKey(createProjectKey));
            Assert.AreEqual(createProjectValue, parsedArgs[createProjectKey]);

            Assert.IsTrue(parsedArgs.ContainsKey(cloudEnvironmentKey));
            Assert.AreEqual(cloudEnvironmentValue, parsedArgs[cloudEnvironmentKey]);
        }

        [Test]
        public void TestArgumentsAreTrimmed()
        {
            string useHub = "    -useHub    ";
            string useHubTrimmed = "-useHub";

            string[] args = new string[] {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                useHub
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(1, parsedArgs.Count);

            Assert.IsTrue(parsedArgs.ContainsKey(useHubTrimmed));
            Assert.IsNull(parsedArgs[useHubTrimmed]);
        }

        [Test]
        public void TestDuplicatedArguments()
        {
            // we found cases (sign out/sign in/close unity/open project from the hub)
            // in which the hub passes duplicated arguments

            string projectPathKey = "-projectPath";
            string projectPathValue = @"c:\tmp\newproj";

            string[] args = new string[] {
                @"C:\Program Files\Unity\Hub\Editor\Unity 2020.2.1f1\Editor\unity.exe",
                projectPathKey,
                projectPathValue,
                projectPathKey,
                projectPathValue
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(1, parsedArgs.Count);

            Assert.IsTrue(parsedArgs.ContainsKey(projectPathKey));
            Assert.AreEqual(projectPathValue, parsedArgs[projectPathKey]);
        }

        [Test]
        public void TestRealExample()
        {
            string createProjectKey = "-createProject";
            string createProjectValue = @"c:\tmp\newproj";

            string useHub = "-useHub";
            string useIPC = "-useIPC";

            string cloudEnvironmentKey = "-cloudEnvironment";
            string cloudEnvironmentValue = "production";

            string cloudProjectKey = "-cloudProject";
            string cloudProjectValue = "fpsmicrogame";

            string cloudOrganizationKey = "- cloudOrganization";
            string cloudOrganizationValue = "D51E18A1-CA04-4E7C-A649-6FD2829E3223-danipen-unity";

            string accessTokenKey = "-accessToken";
            string accessTokenValue = "5k-CzPsncn9bGLI_uSZ91EhJW44Dcj1ShQPtKjCp2rA005f";

            string[] args = new string[] {
                @"C:\Program Files\Unity\Hub\Editor\2018.4.25f1\Editor\unity.exe",
                createProjectKey,
                createProjectValue,
                useHub,
                useIPC,
                cloudEnvironmentKey,
                cloudEnvironmentValue,
                cloudProjectKey,
                cloudProjectValue,
                cloudOrganizationKey,
                cloudOrganizationValue,
                accessTokenKey,
                accessTokenValue
            };

            Dictionary<string, string> parsedArgs = CommandLineArguments.Build(args);

            Assert.AreEqual(7, parsedArgs.Count);

            Assert.IsTrue(parsedArgs.ContainsKey(createProjectKey));
            Assert.AreEqual(createProjectValue, parsedArgs[createProjectKey]);

            Assert.IsTrue(parsedArgs.ContainsKey(useHub));
            Assert.IsNull(parsedArgs[useHub]);

            Assert.IsTrue(parsedArgs.ContainsKey(useIPC));
            Assert.IsNull(parsedArgs[useIPC]);

            Assert.IsTrue(parsedArgs.ContainsKey(cloudEnvironmentKey));
            Assert.AreEqual(cloudEnvironmentValue, parsedArgs[cloudEnvironmentKey]);

            Assert.IsTrue(parsedArgs.ContainsKey(cloudProjectKey));
            Assert.AreEqual(cloudProjectValue, parsedArgs[cloudProjectKey]);

            Assert.IsTrue(parsedArgs.ContainsKey(cloudOrganizationKey));
            Assert.AreEqual(cloudOrganizationValue, parsedArgs[cloudOrganizationKey]);

            Assert.IsTrue(parsedArgs.ContainsKey(accessTokenKey));
            Assert.AreEqual(accessTokenValue, parsedArgs[accessTokenKey]);
        }
    }
}
