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
            //cpu.ram = rom;
            //cpu.RegisterIo(0xff00, 0xffff, ConsoleOutput);
            Array.Copy(rom, cpu.ram, rom.Length);
            cpu.regs.PcLow = 0x00;
            cpu.regs.PcHigh = 0x10;
            ushort prev = 0;
            int count = 0;
            while (true)
            {
                var regs = cpu.regs.ToString();
                cpu.Step();
                //Console.Write("\t\t");
                Console.Write("{0:x2} {1}", (byte)cpu.command, cpu.command);
                Console.Write("\t\t");
                Console.WriteLine(regs);
                if (Console.KeyAvailable) break;
                ushort pc = (ushort) ((cpu.regs.PcHigh << 8) + cpu.regs.PcLow);
                if (pc != prev)
                {
                    prev = pc;
                    count = 0;
                }
                else
                {
                    count++;
                    if (count > 5)
                    {
                        Console.WriteLine("Trap break");
                        break;
                    }
                }
                //Thread.Sleep(100);
            }
            Console.ReadLine();
        }

        private static byte ConsoleOutput(bool write, ushort addr, byte val)
        {
            if (write) Console.Write((char)val);
            return val;
        }
    }
}
