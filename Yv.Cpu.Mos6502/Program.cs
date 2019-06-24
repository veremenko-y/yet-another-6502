using System;
using System.Collections.Generic;
using System.IO;

namespace NesTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var cpu = new Cpu();
            //var rom = File.ReadAllBytes(@"..\..\6502_functional_test.bin");
            var rom = File.ReadAllBytes(@"..\..\osi_bas.bin");
            var ram = new byte[0x8000];
            //cpu.ram = rom;
            cpu.RegisterIo(0x0000, 0x7fff, (write, addr, value) =>
            {
                if (write)
                {
                    ram[addr] = value;
                }
                return ram[addr];
            });
            cpu.RegisterIo(0xC000, 0xffff, (write, addr, value) =>
            {
                addr = (ushort)(addr - 0xC000);
                if (write)
                {
                    rom[addr] = value;
                    return value;
                }
                else
                {
                    return rom[addr];
                }
            });
            cpu.RegisterIo(0xA000, (write, addr, value) =>
            {
                var res = !Console.KeyAvailable ? 0x01 : 0;
                return (byte)(res | 0x02);
            });
            cpu.RegisterIo(0xA001, (write, addr, value) =>
            {
                if (write)
                {
                    Console.Write((char)value);
                }
                else
                {
                    while (!Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        value = (byte)key.KeyChar;
                        return value;
                        //value = (byte)Console.Read();
                    }
                }
                return value;
            });
            //cpu.RegisterIo(0xf001, (write, addr, value) =>
            //{
            //    Console.Write((char)value);
            //    return value;
            //});
            //cpu.RegisterIo(0xf004, (write, addr, value) =>
            //{
            //    char c = (char)Console.Read();
            //    return (byte)c;
            //});
            var traces = new Queue<string>();
            cpu.TraceCallback += (addr, value, member) =>
            {
                var trace = string.Format("{0} {1:x4}: {2:x2}", member, addr, value);
                //Console.WriteLine(trace);
                traces.Enqueue(trace);
                while (traces.Count > 128)
                    traces.Dequeue();
            };
            //Array.Copy(rom, cpu.ram, rom.Length);

            cpu.regs.PcLow = 0x00;
            cpu.regs.PcHigh = 0x04;
            ushort prev = 0;
            int count = 0;
            while (true)
            {
                var regs = cpu.regs.ToString();
                cpu.Step();
                var command = "\t" + cpu.command.ToString();
                for (var tabs = command.Length / 4; 3 - tabs > 0; tabs++)
                {
                    command += "\t";
                }
                command += " " + regs;
                cpu.OnTraceCallback(0, (byte)cpu.command, command);
                if ((byte)cpu.command == 0xFF)
                {
                    Fail(cpu, traces);
                    break;
                }
                //if (Console.KeyAvailable)
                //{
                //    var key = Console.ReadKey();
                //    if (key.Key == ConsoleKey.D1)
                //    {
                //        Fail(cpu, traces);
                //    }
                //    else if (key.Key == ConsoleKey.D2)
                //    {
                //        MemoryDump(cpu);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                ushort pc = (ushort)((cpu.regs.PcHigh << 8) + cpu.regs.PcLow);
                if (pc == 0x400)
                {
                    Console.WriteLine("Success");
                    break;
                }
                if (pc != prev)
                {
                    prev = pc;
                    count = 0;
                }
                else
                {
                    count++;
                    if (count > 3)
                    {
                        Fail(cpu, traces);
                        Console.WriteLine("Trap break");
                        break;
                    }
                }
                //Thread.Sleep(100);
            }
            Console.ReadLine();
        }

        private static void Fail(Cpu cpu, Queue<string> traces)
        {
            Console.WriteLine("--------------Dump-----------------");
            while (traces.Count > 0)
                Console.WriteLine(traces.Dequeue());
            MemoryDump(cpu);
            Console.WriteLine("-----------------------------------");
        }

        private static void MemoryDump(Cpu cpu)
        {
            Console.WriteLine("--------------Memory Dump----------");
            for (var i = 0; i < 0x20; i++)
            {
                Console.Write("{0:x4}:", i * 0x10);
                for (var j = 0; j < 0x10; j++)
                {
                    if (j == 8)
                        Console.Write(" |");
                    Console.Write(" {0:x2}", cpu.GetByte((ushort)(i * 0x10 + j)));
                }
                Console.WriteLine();
            }
        }

        private static byte ConsoleOutput(bool write, ushort addr, byte val)
        {
            if (write) Console.Write((char)val);
            return val;
        }
    }
}
