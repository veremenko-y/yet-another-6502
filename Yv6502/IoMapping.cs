using System.Runtime.CompilerServices;

namespace Yv6502
{
    internal class IoMapping
    {
        public IoMapping(ushort start, ushort end, IoEvent handler)
        {
            StartAddress = start;
            EndAddress = end;
            Handler = handler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(ushort address)
        {
            return StartAddress <= address && address <= EndAddress;
        }

        public ushort StartAddress { get; private set; }
        public ushort EndAddress { get; private set; }
        public IoEvent Handler { get; set; }
    }
}
