using System;
using System.Diagnostics;
using System.IO;

namespace NesTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var argsParser = new Options("Yv.Cpu.Most6502");
            argsParser.AddOption(new[] { "m", "machine" }, values: new[] { "test", "grants" }, description: "Machine name to be run");
            //if (!argsParser.Parse(args))
            //    return;
            var machine = "test"; argsParser.Get("m");
            if (machine == "grants")
            {
                var rom = File.ReadAllBytes(@"..\..\osi_bas.bin");
                var simple6502 = new GrantsSimple6502(rom);
                simple6502.Reset();
                while (true)
                {
                    simple6502.Step();
                }
            }
            else if (machine == "test")
            {
                // use the tests with the report enabled
                var rom = File.ReadAllBytes(@"..\..\6502_functional_test.bin");
                var testMachine = new TestMachine(rom);
                // Before: 00:00:06.1502717
                var sw = new Stopwatch();
                sw.Start();
                while (testMachine.Running)
                {
                    testMachine.Step();
                }
                sw.Stop();
                Console.WriteLine();
                Console.WriteLine("Test finished {0}", sw.Elapsed);
            }
            else
            {
                argsParser.PrintUsage();
            }
        }
    }
}
