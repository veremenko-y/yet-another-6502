using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace NesTest
{
    static class Program
    {
        static void Main(string[] args)
        {
            var cpu = new Cpu();
            var rom = File.ReadAllBytes("6502_functional_test.bin");
            cpu.ram = rom;
            //Array.Copy(rom, 8, cpu.ram, 0, rom.Length - 8);
            cpu.regs.PcLow = 0x00;
            cpu.regs.PcHigh = 0x10;
            while (true)
            {
                var regs = cpu.regs.ToString();
                cpu.Step();
                //Console.Write("\t\t");
                Console.Write(cpu.command);
                Console.Write("\t\t");
                Console.WriteLine(regs);
                //Thread.Sleep(100);
            }
        }

        private static byte ConsoleOutput(bool write, ushort addr, byte val)
        {
            if (write) Console.Write((char)val);
            return val;
        }
    }
}
