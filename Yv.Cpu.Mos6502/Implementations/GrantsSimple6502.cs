using System;

namespace NesTest
{
    public class GrantsSimple6502 : Cpu
    {
        private char inputKey;
        private bool hasKey;

        public GrantsSimple6502(byte[] rom)
        {
            var ram = new byte[0x8000];
            RegisterIo(0x0000, 0x7fff, (write, addr, value) =>
            {
                if (write)
                {
                    ram[addr] = value;
                }
                return ram[addr];
            });
            RegisterIo(0xC000, 0xffff, (write, addr, value) =>
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
            RegisterIo(0xA000, (write, addr, value) =>
            {
                var res = hasKey ? 0x01 : 0;
                return (byte)(res | 0x02);
            });
            RegisterIo(0xA001, (write, addr, value) =>
            {
                if (write)
                {
                    Console.Write((char)value);
                }
                else
                {
                    hasKey = false;
                    value = (byte)inputKey;
                }
                return value;
            });
        }

        public new void Step()
        {
            if (Console.KeyAvailable)
            {
                hasKey = true;
                inputKey = Console.ReadKey(true).KeyChar;
            }
            base.Step();
        }
    }
}
