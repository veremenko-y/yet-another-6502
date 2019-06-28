using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yv6502
{
    public partial class Cpu
    {
        private void Cmp(byte op)
        {
            ushort rt = (ushort)(regs.Ac - op);
            regs.SetCarry(rt < 0x100);
            regs.SetSign(rt);
            regs.SetZero((byte)rt);
        }

        private void Cpx(byte op)
        {
            ushort rt = (ushort)(regs.Xr - op);
            regs.SetCarry(rt < 0x100);
            regs.SetSign(rt);
            regs.SetZero((byte)rt);
        }

        private void Cpy(byte op)
        {
            ushort rt = (ushort)(regs.Yr - op);
            regs.SetCarry(rt < 0x100);
            regs.SetSign(rt);
            regs.SetZero((byte)rt);
        }
    }
}
