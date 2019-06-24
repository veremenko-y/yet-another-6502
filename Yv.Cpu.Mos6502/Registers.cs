using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace NesTest
{
    public struct Registers
    {
        public byte Ac;
        public byte Xr;
        public byte Yr;
        public byte PcLow;
        public byte PcHigh;
        public byte Sp;
        public Status Status;
        private bool BFlag;

        public override string ToString()
        {
            return String.Format("A: 0x{0:x2}(b{7}) Xr: 0x{1:x2} Yr: 0x{2:x2} Pc: 0x{3:x2}{4:x2} Sp: 0x{5} S: b{6}",
                Ac, Xr, Yr, PcHigh,
                PcLow, Sp,
                Convert.ToString((byte)Status, 2).PadLeft(8, '0'),
                Convert.ToString((byte)Ac, 2).PadLeft(8, '0'));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PcAdd(byte op)
        {
            ushort rt = (ushort)(PcLow + op);
            if (rt > 0xFF)
            {
                PcHigh++;
                PcLow = (byte)rt;
            }
            else
            {
                PcLow = (byte)rt;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PcSub(byte op)
        {
            ushort rt = (ushort)(PcLow - op);
            if (rt > 0xFF)
            {
                PcHigh--;
                PcLow = (byte)rt;
            }
            else
            {
                PcLow = (byte)rt;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Status status)
        {
            if (status == Status.Brk) BFlag = true;
            Status = Status | status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(Status status)
        {
            if (status == Status.Brk) BFlag = false;
            Status = Status & ~status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Is(Status status)
        {
            if(status == Status.Brk) return BFlag;
            return Status.HasFlag(status);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSign(ushort rt)
        {
            if (((byte)rt & 0X80) != 0)
            {
                Set(Status.Sign);
            }
            else
            {
                Reset(Status.Sign);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOverflow(bool condition)
        {
            if (condition)
            {
                Set(Status.VOverflow);
            }
            else
            {
                Reset(Status.VOverflow);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCarry(ushort rt)
        {
            SetCarry(rt > 0xFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCarry(bool condition)
        {
            if (condition)
            {
                Set(Status.Carry);
            }
            else
            {
                Reset(Status.Carry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBreak(bool condition)
        {
            if (condition)
            {
                Set(Status.Brk);
            }
            else
            {
                Reset(Status.Brk);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInterrupt(bool condition)
        {
            if (condition)
            {
                Set(Status.Interrupt);
            }
            else
            {
                Reset(Status.Interrupt);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetZero(ushort op)
        {
            if ((byte)op == 0)
            {
                Set(Status.Zero);
            }
            else
            {
                Reset(Status.Zero);
            }
        }
    }
}