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

        public override string ToString()
        {
            return String.Format("A: {0:x2} Xr: {1:x2} Yr: {2:x2} Pc: {3:x2}{4:x2} Sp: {5} S: {6}", Ac, Xr, Yr, PcHigh,
                PcLow, Sp, Convert.ToString((byte)Status, 2).PadLeft(8, '0'));
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
        public void SetStatus(Status status)
        {
            Status = Status | status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetStatus(Status status)
        {
            Status = Status & ~status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsStatus(Status status)
        {
            return Status.HasFlag(status);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSign(ushort rt)
        {
            if (((byte)rt & 0X80) != 0)
            {
                SetStatus(Status.Sign);
            }
            else
            {
                ResetStatus(Status.Sign);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOverflow(bool condition)
        {
            if (condition)
            {
                SetStatus(Status.VOverflow);
            }
            else
            {
                ResetStatus(Status.VOverflow);
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
                SetStatus(Status.Carry);
            }
            else
            {
                ResetStatus(Status.Carry);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBreak(bool condition)
        {
            if (condition)
            {
                SetStatus(Status.Brk);
            }
            else
            {
                ResetStatus(Status.Brk);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInterrupt(bool condition)
        {
            if (condition)
            {
                SetStatus(Status.Interrupt);
            }
            else
            {
                ResetStatus(Status.Interrupt);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetZero(ushort op)
        {
            if ((byte)op == 0)
            {
                SetStatus(Status.Zero);
            }
            else
            {
                ResetStatus(Status.Zero);
            }
        }
    }
}