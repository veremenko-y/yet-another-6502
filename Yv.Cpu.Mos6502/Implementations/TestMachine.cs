using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesTest
{
    class TestMachine : Cpu
    {
        public bool Running { get; private set; }

        private ushort prev = 0;
        private int count = 0;

        public TestMachine(byte[] rom)
        {
            Running = true;
            RegisterIo(0x0000, 0xffff, (ref IoEventArgs e) =>
            {
                if (e.IsWrite)
                {
                    rom[e.Address] = e.Value;
                }
                else
                {
                    e.Value = rom[e.Address];
                }
            });
            RegisterIo(0xf001, (ref IoEventArgs e) =>
            {
                Console.Write((char)e.Value);
                if ((char)e.Value == 'R')
                    Running = false;
            });
            RegisterIo(0xf004, (ref IoEventArgs e) =>
            {
                //char c = (char)Console.Read();
                //e.Value = (byte)c;
            });
            regs.Pc = 0x0400;
        }

        public new void Step()
        {
            base.Step();
            ushort pc = regs.Pc;
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
                    MemoryDump();
                    Console.WriteLine("Trap break");
                    Running = false;
                    return;
                }
            }
        }

        private void MemoryDump()
        {
            Console.WriteLine("--------------Memory Dump----------");
            for (var i = 0; i < 0x20; i++)
            {
                Console.Write("{0:x4}:", i * 0x10);
                for (var j = 0; j < 0x10; j++)
                {
                    if (j == 8)
                        Console.Write(" |");
                    Console.Write(" {0:x2}", GetByte((ushort)(i * 0x10 + j)));
                }
                Console.WriteLine();
            }
        }
    }
}
