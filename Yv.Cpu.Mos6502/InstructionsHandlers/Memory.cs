using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;

namespace NesTest
{
    public partial class Cpu
    {
        private void Lda(byte op)
        {
            regs.SetSign(op);
            regs.SetZero(op);
            regs.Ac = op;
        }

        private void Ldx(byte op)
        {
            regs.SetSign(op);
            regs.SetZero(op);
            regs.Xr = op;
        }

        private void Ldy(byte op)
        {
            regs.SetSign(op);
            regs.SetZero(op);
            regs.Yr = op;
        }

        private void Sta(ushort addr)
        {
            SetByte(addr, regs.Ac);
        }

        private void Stx(ushort addr)
        {
            SetByte(addr, regs.Xr);
        }

        private void Sty(ushort addr)
        {
            SetByte(addr, regs.Yr);
        }

        private void Tax()
        {
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
            regs.Xr = regs.Ac;
        }

        private void Tay()
        {
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
            regs.Yr = regs.Ac;
        }

        private void Tsx()
        {
            regs.SetSign(regs.Sp);
            regs.SetZero(regs.Sp);
            regs.Xr = regs.Sp;
        }

        private void Txa()
        {
            regs.SetSign(regs.Xr);
            regs.SetZero(regs.Xr);
            regs.Ac = regs.Xr;
        }

        private void Txs()
        {
            regs.Sp = regs.Xr;
        }

        private void Tya()
        {
            regs.SetSign(regs.Yr);
            regs.SetZero(regs.Yr);
            regs.Ac = regs.Yr;
        }
    }
}
