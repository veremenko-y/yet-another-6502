using System;

namespace Yv6502
{
    [Flags]
    public enum Status
    {
        Sign = 0x80,
        VOverflow = 0x40,
        Na = 0x20,
        Brk = 0x10,
        Decimal = 0x08,
        Interrupt = 0x04,
        Zero = 0x02,
        Carry = 0x01
    }
}