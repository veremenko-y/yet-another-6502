using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NesTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var argsParser = new Arguments("Yv.Cpu.Most6502");
            argsParser.AddOption(new[] { "m", "machine" }, values: new[] { "test", "grants" }, description: "Machine name to be run");
            if (!argsParser.Parse(args))
                return;
            var machine = argsParser.Get("m");
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

    internal class Arguments
    {
        private List<(string[] options, string[] values, string description)> options = new List<(string[] options, string[] values, string description)>();
        private Dictionary<string[], string> parsed = new Dictionary<string[], string>();
        private string name;

        public Arguments(string name)
        {
            this.name = name;
        }

        public void AddOption(string[] options, string[] values = null, string description = null)
        {
            this.options.Add((options, values, description));
        }

        public bool Parse(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    var option = arg.Substring(1);
                    var value = "";
                    while (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        if (value == "")
                        {
                            value = args[i + 1];
                        }
                        else
                        {
                            value += " " + args[i + 1];
                        }
                        i++;
                    }
                    var defined = options.FirstOrDefault(o => o.options.Contains(option));
                    if (defined.options != null &&
                        (defined.values?.Contains(value) ?? true))
                    {
                        parsed.Add(defined.options, value);
                    }
                    else
                    {
                        PrintUsage();
                        return false;
                    }
                }
            }
            return true;
        }

        public string Get(string option)
        {
            var key = parsed.Keys.FirstOrDefault(i => i.Contains(option));
            if (key == null)
                return null;
            return parsed[key];
        }

        public void PrintUsage()
        {
            Console.WriteLine("Usage:");
            var usage = string.Join(
                " ",
                options.Select(i =>
                    "[" + string.Join(" | ", i.options.Select(j => "-" + j)) + "]" +
                    (i.values == null
                        ? ""
                        : " [" + string.Join(" | ", i.values) + "]")
                ));
            Console.WriteLine(name + " " + usage);
            foreach (var option in options)
            {
                Console.Write(string.Join(", ", option.options.Select(i => "-" + i)));
                Console.Write("\t\t");
                Console.Write(option.description ?? "");
            }
        }
    }
}
