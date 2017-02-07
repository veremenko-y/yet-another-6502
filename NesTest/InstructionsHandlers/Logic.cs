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
        private void And(byte op)
        {
            regs.Ac = (byte)(op & regs.Ac);
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
        }

        private void Ora(byte op)
        {
            regs.Ac = (byte) (op | regs.Ac);
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
        }

        private void Eor(byte op)
        {
            regs.Ac = (byte) (op ^ regs.Ac);
            regs.SetSign(regs.Ac);
            regs.SetZero(regs.Ac);
        }
    }
}
