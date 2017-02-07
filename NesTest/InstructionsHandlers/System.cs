using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;

namespace NesTest
{
    public partial class Cpu
    {
        private void Brk()
        {
            clock += 7;
            regs.PcAdd(1);
            Push(regs.PcHigh);
            Push(regs.PcLow);
            regs.SetBreak(true);
            Push((byte)regs.Status);
            regs.SetInterrupt(true);
            regs.PcLow = GetByte(0xFE, 0xFF);
            regs.PcHigh = GetByte(0xFF, 0xFF);
        }

        private void Jmp(ushort addr)
        {
            regs.PcLow = (byte)addr;
            regs.PcHigh = (byte) (addr >> 8);
        }

        private void Jsr(ushort addr)
        {
            regs.PcSub(1);
            Push(regs.PcHigh);
            Push(regs.PcLow);
            regs.PcLow = (byte) addr;
            regs.PcHigh = (byte) (addr >> 8);
        }

        private void Rti()
        {
            regs.Status = (Status)  Pop();
            regs.PcLow = Pop();
            regs.PcHigh = Pop();
        }

        private void Rts()
        {
            regs.PcLow = Pop();
            regs.PcHigh = Pop();
            regs.PcAdd(1);
        }
    }
}
