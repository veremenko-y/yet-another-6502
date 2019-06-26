using System;
using System.Collections.Generic;
using System.Linq;

namespace NesTest
{
    internal class Options
    {
        private List<(string[] options, string[] values, string description)> options = new List<(string[] options, string[] values, string description)>();
        private Dictionary<string[], string> parsed = new Dictionary<string[], string>();
        private string name;

        public Options(string name)
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
                    "[" + string.Join("|", i.options
                        .OrderBy(j=>j.Length)
                        .Select(j => "-" + j)
                        .FirstOrDefault()) +

                    (i.values == null
                        ? ""
                        : " <" + string.Join("|", i.values) + ">")

                    + "]"
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
