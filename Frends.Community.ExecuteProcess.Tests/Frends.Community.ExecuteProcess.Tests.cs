using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Frends.Community.ExecuteProcess.Tests
{
    [TestFixture]
    public class Tests
    {
        private readonly string _testDir = Path.Combine(Path.GetTempPath(), @"ExecTests"+DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        private readonly string _inputFile = "file8kb.txt";
        private readonly string _process = Environment.ExpandEnvironmentVariables(@"%windir%\system32\cmd.exe");

        [SetUp]
        public void Setup()
        {
            if (!Directory.Exists(_testDir))
            {
                Directory.CreateDirectory(_testDir);

                File.WriteAllText(Path.Combine(_testDir, _inputFile), new string('a', 8 * 1024+5));
            }
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
        [Order(1)]
        public void ExecuteScript()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test1.txt");

            var args = new[]
            {
                new Argument { Name = "/C", Value = "echo testi >>" + testFileWithPath }
            };
            var input = new Input { ScriptPath = _process, Arguments = args, WaitForResponse = true, TimeoutSeconds = 6 };

            ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(File.Exists(testFileWithPath)); // Tests if the file is created

            Assert.AreEqual(File.ReadAllText(testFileWithPath), "testi " + Environment.NewLine); // Tests that the cmd.exe is only executed once
        }

        [Test]
        [Order(2)]
        public void ExecuteMultipleArgs()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test2.txt");

            var args = new[]
            {
                new Argument { Name = "/C", Value = "set" },
                new Argument { Name = "/A", Value = "(1+10)" },
                new Argument { Name = ">>", Value = testFileWithPath }
            };

            var input = new Input { ScriptPath = _process, Arguments = args, WaitForResponse = true, TimeoutSeconds = 6 };

            ExecuteProcessCommand.ExecuteProcess(input);

            Assert.AreEqual(File.ReadAllText(testFileWithPath), "11");
        }

        [Test]
        [Order(3)]
        public void ExecuteResult()
        {
            var args = new[]
            {
                new Argument { Name = "/C", Value = "set" },
                new Argument { Name = "/A", Value = "(1111+2222)" }
            };

            var input = new Input { ScriptPath = _process, Arguments = args, WaitForResponse = true, TimeoutSeconds = 6 };

            var result = ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(result.Result.Contains("3333"));
        }

        [Test]
        [Order(4)]
        public void ExecuteScriptsAsync()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test3.txt");

            var args = new[]
            {
                new Argument { Name = "/C", Value = "set" },
                new Argument { Name = "/A (1+10) >>", Value = testFileWithPath}
            };
            var input = new Input { ScriptPath = _process, Arguments = args, WaitForResponse = false };
            var result = ExecuteProcessCommand.ExecuteProcess(input);

            Assert.IsTrue(result.Status);
            Thread.Sleep(100);
            Assert.AreEqual(File.ReadAllText(testFileWithPath), "11");
        }

        [Test]
        [Order(5)]
        public void RunMultipleArgs()
        {
            var testFileWithPath = Path.Combine(_testDir, @"test4.txt");

            var args = new[]
            {
                new Argument { Name = "/C", Value = "set" },
                new Argument { Name = "/A", Value = "(1+10)" },
                new Argument { Name = ">>", Value = testFileWithPath }
            };

            var input = new RunProcessParameters { FileName = _process, Arguments = args  };
            var options = new RunProcessOptions { KillProcessAfterTimeout = false, TimeoutSeconds = 30, RedirectStandardInput=false };

            ExecuteProcessCommand.RunProcess(input, options);

            Assert.AreEqual(File.ReadAllText(testFileWithPath), "11");
        }

        private ActualValueDelegate<object> TestBaseTimeoutKill(bool optionsKillProcess)
        {
            var args = new[]
            {
                new Argument { Name = "/C", Value = "timeout 10 /nobreak >NUL" }
            };

            var input = new RunProcessParameters { FileName = _process, Arguments = args };
            var options = new RunProcessOptions { KillProcessAfterTimeout = optionsKillProcess, TimeoutSeconds = 5, RedirectStandardInput = false };

            return () => ExecuteProcessCommand.RunProcess(input, options);
        }

        [Test]
        [Order(6)]
        public void TimeoutNoKillProcess()
        {
            ActualValueDelegate<object> test = TestBaseTimeoutKill(false);
            Assert.That(test, Throws.TypeOf<TimeoutException>());
        }

        [Test]
        [Order(7)]
        public void TimeoutKillProcess()
        {
            ActualValueDelegate<object> test = TestBaseTimeoutKill(true);
            Assert.That(test, Throws.TypeOf<TimeoutException>());
        }

        [Test]
        [Order(8)]
        // Test possible STDOUT buffer sync problems
        public void FillSTDOUT()
        {
            var testFileWithPath = Path.Combine(_testDir, _inputFile);

            var args = new[]
            {
                new Argument { Name = "/C", Value = $"type {testFileWithPath}"  }
            };

            var input = new RunProcessParameters { FileName = _process, Arguments = args };
            var options = new RunProcessOptions { KillProcessAfterTimeout = false, TimeoutSeconds = 30, RedirectStandardInput = false };

            var output = ExecuteProcessCommand.RunProcess(input, options);

            Assert.IsTrue(output.Output.Length >=8096+5);
            Assert.IsTrue(output.Output[1234] == 'a');
        }


        [Test]
        [Order(9)]
        // Test possible STDOUT buffer sync problems
        public void FillSTDOUTTimeout30secsKillProcess()
        {
            ActualValueDelegate<object> test = TestBufferTimeoutKill();
            Assert.That(test, Throws.TypeOf<TimeoutException>());
        }

        private ActualValueDelegate<object> TestBufferTimeoutKill()
        {
            var testFileWithPath = Path.Combine(_testDir, _inputFile);

            var args = new[]
            {
                new Argument { Name = "/C", Value = $"type {testFileWithPath} && timeout 60 /nobreak >NUL"  },
            };

            var input = new RunProcessParameters { FileName = _process, Arguments = args };
            var options = new RunProcessOptions { KillProcessAfterTimeout = true, TimeoutSeconds = 30, RedirectStandardInput = false };
            return () => ExecuteProcessCommand.RunProcess(input, options);
        }
    }

}
