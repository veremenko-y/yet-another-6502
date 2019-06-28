using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yv6502Emu
{
    internal class Options
    {
        private List<(string[] options, bool required, string[] allowedValues, bool hasValue, string description)> options = new List<(string[] options, bool required, string[] allowedValues, bool hasValue, string description)>();
        private Dictionary<string[], string> parsed = new Dictionary<string[], string>();
        private string applicationName;
        private string operandDescription;
        private bool operandRequired;
        private bool operandMultiple;
        private bool operandAvailable;
        private string[] operandValues;

        internal void AddOperand(string description = null, bool required = false, bool allowMultiple = false)
        {
            operandAvailable = true;
            operandDescription = description;
            operandRequired = required;
            operandMultiple = allowMultiple;
        }

        public Options(string applicationName)
        {
            this.applicationName = applicationName;
        }

        public void AddOption(string[] options, bool required = false, string[] allowedValues = null, bool hasValue = false, string description = null)
        {
            if (allowedValues != null && hasValue == false)
                hasValue = true;
            this.options.Add((options, required, allowedValues, hasValue, description));
        }

        public bool Parse(string[] args)
        {
            var operandValuesList = new List<string>();
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    var option = arg.Substring(1);

                    var definition = options.FirstOrDefault(o => o.options.Contains(option));
                    if (definition.options == null)
                    {
                        PrintUsage();
                        return false;
                    }

                    string value;
                    if (!definition.hasValue)
                    {
                        value = null;
                    }
                    else
                    {
                        value = (i + 1) < args.Length ? args[i + 1] : string.Empty;
                        if (definition.allowedValues != null &&
                            !definition.allowedValues.Contains(value))
                        {
                            PrintUsage();
                            return false;
                        }
                        i++;
                    }

                    parsed.Add(definition.options, value);
                }
                else if (operandAvailable)
                {
                    if (!operandRequired ||
                        operandValuesList.Count == 0)
                    {
                        PrintUsage();
                        return false;
                    }
                    do
                    {
                        operandValuesList.Add(args[i++]);
                    }
                    while (operandMultiple && i < args.Length && !args[i].StartsWith("-"));

                }
                else
                {
                    PrintUsage();
                    return false;
                }
            }
            operandValues = operandValuesList.ToArray();
            return true;
        }

        public string GetOption(string option)
        {
            var key = parsed.Keys.FirstOrDefault(i => i.Contains(option));
            if (key == null)
                return null;
            return parsed[key];
        }

        public string GetOperand()
        {
            return operandValues.FirstOrDefault();
        }

        public string[] GetOperands()
        {
            return operandValues;
        }

        public void PrintUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage");
            sb.Append($"{applicationName}");
            if (operandAvailable)
            {
                sb.Append(" ");
                if (!operandRequired)
                    sb.Append("[");
                sb.Append("operand");
                if (operandMultiple)
                    sb.Append("...");
                if (!operandRequired)
                    sb.Append("]");
            }
            foreach (var option in options)
            {
                sb.Append(" ");
                if (!option.required)
                    sb.Append("[");
                sb.Append("-");
                sb.Append(option.options[0]);
                if (option.hasValue)
                {
                    sb.Append(" ");
                    if (option.allowedValues != null)
                    {
                        sb.Append("<").Append(string.Join("|", option.allowedValues)).Append(">");
                    }
                }
                if (!option.required)
                    sb.Append("]");
            }
            sb.AppendLine();
            if(operandAvailable)
            {
                sb.Append("operand\t\t").AppendLine(operandDescription ?? "");
            }
            foreach (var option in options)
            {
                sb.Append(string.Join(", ", option.options.Select(i => "-" + i)));
                sb.Append("\t\t");
                sb.AppendLine(option.description ?? "");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
