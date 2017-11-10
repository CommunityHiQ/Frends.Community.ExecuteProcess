using System;
using NUnit.Framework;
using System.IO;
using System.Threading;

namespace Frends.Community.ExecuteProcess.Tests
{
    [TestFixture]
    public class ExecuteTests
    {
        private readonly string _testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ExecTests\");
        private readonly string _process = Environment.ExpandEnvironmentVariables(@"%windir%\system32\cmd.exe");

        [SetUp]
        public void Setup()
        {
            if (!Directory.Exists(_testDir))
                Directory.CreateDirectory(_testDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        [Test]
        public void TestExecuteScript()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test1.txt");

            var args = new[]
            {
                new ExecuteProcessCommand.Argument { Name = "/C", Value = "echo testi >> " + testFileWithPath }
            };
            var input = new ExecuteProcessCommand.Input {ScriptPath = _process,  Arguments = args, WaitForResponse =  true, TimeoutMS = 6000 };

            ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(File.Exists(testFileWithPath)); // Tests if the file is created

            Assert.AreEqual(File.ReadAllText(testFileWithPath), "testi " + Environment.NewLine); // Tests that the cmd.exe is only executed once
        }

        [Test]
        public void TestExecuteMultipleArgs()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test2.txt");

            var args = new []
            {
                new ExecuteProcessCommand.Argument { Name = "/C", Value = "set" },
                new ExecuteProcessCommand.Argument { Name = "/A", Value = "(1+10)" },
                new ExecuteProcessCommand.Argument { Name = ">>", Value = testFileWithPath }
            };

            var input = new ExecuteProcessCommand.Input { ScriptPath = _process, Arguments = args, WaitForResponse = true, TimeoutMS = 6000 };

            ExecuteProcessCommand.ExecuteProcess(input);

            Assert.AreEqual(File.ReadAllText(testFileWithPath), "11");
        }

        [Test]
        public void TestExecuteResult()
        {
            var args = new []
            {
                new ExecuteProcessCommand.Argument { Name = "/C", Value = "set" },
                new ExecuteProcessCommand.Argument { Name = "/A", Value = "(1111+2222)" }
            };

            var input = new ExecuteProcessCommand.Input { ScriptPath = _process, Arguments = args, WaitForResponse = true, TimeoutMS = 6000 };

            var result = ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(result.Result.Contains("3333"));
        }

        [Test]
        public void TestExecuteScriptsAsync()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test3.txt");
         
            var args = new[]
            {
                new ExecuteProcessCommand.Argument { Name = "/C", Value = "set" },
                new ExecuteProcessCommand.Argument { Name = "/A (1+10) >>", Value = testFileWithPath}
            };
            var input = new ExecuteProcessCommand.Input { ScriptPath = _process, Arguments = args, WaitForResponse = false };
            var result = ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(result.Status);
            Thread.Sleep(100);
            Assert.AreEqual(File.ReadAllText(testFileWithPath), "11");
        }
    }
}
