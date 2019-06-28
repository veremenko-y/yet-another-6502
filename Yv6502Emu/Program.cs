using System;
using System.Diagnostics;
using System.IO;

namespace Yv6502Emu
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var argsParser = new Options("yv6502emu");
            argsParser.AddOption(
                new[] { "m", "machine" },
                allowedValues: new[] { "test", "grants" },
                description: "Machine name to be run",
                required: true);
            argsParser.AddOperand(
                description: "Path to the ROM file",
                required: true);
            if (!argsParser.Parse(args))
                return;
            var machine = argsParser.GetOption("m");
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
                while (testMachine.Running)
                {
                    testMachine.Step();
                }
            }
            else
            {
                argsParser.PrintUsage();
            }
        }
    }
}
