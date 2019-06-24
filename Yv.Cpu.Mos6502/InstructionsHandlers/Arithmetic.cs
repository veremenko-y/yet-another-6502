using System;

namespace NesTest
{
    public partial class Cpu
    {
        private void Add(byte op)
        {
            ushort rt = (ushort)(regs.Ac + op + (regs.Is(Status.Carry) ? 1 : 0));
            regs.SetZero((byte)(rt & 0xFF));
            if (regs.Is(Status.Decimal))
            {
                throw new NotImplementedException();
            }
            else
            {
                regs.SetSign(rt);
                regs.SetOverflow(((regs.Ac ^ op) & 0x80) == 0 && (((regs.Ac ^ (byte)rt) & 0x80) != 0));
                regs.SetCarry(rt);
            }
            regs.Ac = (byte)rt;
        }

        private void Sbc(byte op)
        {
            ushort rt = (ushort)(regs.Ac - op - (!regs.Is(Status.Carry) ? 1 : 0));
            regs.SetSign(rt);
            regs.SetZero((byte)rt);
            regs.SetOverflow(((regs.Ac ^ op) & 0x80) != 0 && (((regs.Ac ^ (byte)rt) & 0x80) != 0));
            if (regs.Is(Status.Decimal))
            {
                throw new NotImplementedException();
            }
            regs.SetCarry(rt < 0x100);
            regs.Ac = (byte)rt;
        }

        private void Inc(ushort addr)
        {
            byte op = (byte)(GetByte(addr) + 1);
            regs.SetSign(op);
            regs.SetZero(op);
            SetByte(addr, op);
        }

        private void Inx()
        {
            regs.Xr = (byte)(regs.Xr + 1);
            regs.SetSign(regs.Xr);
            regs.SetZero(regs.Xr);
        }

        private void Iny()
        {
            regs.Yr = (byte)(regs.Yr + 1);
            regs.SetSign(regs.Yr);
            regs.SetZero(regs.Yr);
        }

        private void Dec(ushort addr)
        {
            byte op = (byte)(GetByte(addr) - 1);
            regs.SetSign(op);
            regs.SetZero(op);
            SetByte(addr, op);
        }

        private void Dex()
        {
            regs.Xr = (byte)(regs.Xr - 1);
            regs.SetSign(regs.Xr);
            regs.SetZero(regs.Xr);
        }

        private void Dey()
        {
            regs.Yr = (byte)(regs.Yr - 1);
            regs.SetSign(regs.Yr);
            regs.SetZero(regs.Yr);
        }

        private void Asl()
        {
            ushort rt = (ushort)(regs.Ac << 1);
            regs.SetSign(rt);
            regs.SetZero(rt);
            regs.SetCarry(rt);
            regs.Ac = (byte)rt;
        }

        private void Asl(ushort addr)
        {
            var op = GetByte(addr);
            ushort rt = (ushort)(op << 1);
            regs.SetSign(rt);
            regs.SetZero(rt);
            regs.SetCarry(rt);
            SetByte(addr, (byte)rt);
        }

        private void Lsr()
        {
            var op = regs.Ac;
            regs.SetCarry((byte)(op & 0x01) != 0);
            op = (byte)(op >> 1);
            regs.SetSign(op);
            regs.SetZero(op);
            regs.Ac = op;
        }

        private void Lsr(ushort addr)
        {
            var op = GetByte(addr);
            regs.SetCarry((byte)(op & 0x01) != 0);
            op = (byte)(op >> 1);
            regs.SetSign(op);
            regs.SetZero(op);
            SetByte(addr, op);
        }

        private void Rol()
        {
            ushort rt = (ushort)(regs.Ac << 1);
            if (regs.Is(Status.Carry))
            {
                rt = (ushort)(rt | 0x01);
            }
            regs.SetCarry(rt);
            regs.Ac = (byte)rt;
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
        }

        private void Rol(ushort addr)
        {
            var op = GetByte(addr);
            ushort rt = (ushort)(op << 1);
            if (regs.Is(Status.Carry))
            {
                rt = (ushort)(rt | 0x01);
            }
            regs.SetCarry(rt);
            op = (byte)rt;
            regs.SetSign(op);
            regs.SetZero(op);
            SetByte(addr, op);
        }

        private void Ror()
        {
            ushort rt = regs.Ac;
            if (regs.Is(Status.Carry))
            {
                rt = (ushort)(rt | 0x100);
            }
            regs.SetCarry((rt & 0x01) != 0);
            rt = (ushort)(rt >> 1);
            regs.Ac = (byte)rt;
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
        }

        private void Ror(ushort addr)
        {
            var op = GetByte(addr);
            ushort rt = op;
            if (regs.Is(Status.Carry))
            {
                rt = (ushort)(rt | 0x100);
            }
            regs.SetCarry((rt & 0x01) != 0);
            rt = (ushort)(rt >> 1);
            regs.SetSign(rt);
            regs.SetZero(rt);
            SetByte(addr, (byte)rt);
        }

        private void Bit(byte op)
        {
            var rt = (byte)(regs.Ac & op);
            regs.SetZero(rt);
            regs.SetSign(op);
            regs.SetOverflow((op & 0x40) != 0);
        }
    }
}
