using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            cpu.regs.PcHigh = 0x04;
            ushort prev = 0;
            int count = 0;
            while (true)
            {
                var regs = cpu.regs.ToString();
                cpu.Step();
                //Console.Write("\t\t");
                var command = string.Format("{0:x2} {1}", (byte)cpu.command, cpu.command);
                Console.Write(command);
                for (var tabs = command.Length / 4; 4 - tabs > 0; tabs++)
                {
                    Console.Write("\t");
                }

                Console.WriteLine(regs);
                if (Console.KeyAvailable) break;
                ushort pc = (ushort)((cpu.regs.PcHigh << 8) + cpu.regs.PcLow);
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
                        var index = (cpu.regs.PcHigh << 8) + cpu.regs.PcLow;
                        for (var i = index - 30; i < index; i++)
                        {
                            Console.Write("{0:x2} ", cpu.ram[i]);
                            if ((i - index - 30) % 8 == 0) Console.WriteLine();
                        }
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
