using System.Runtime.CompilerServices;

namespace Yv6502
{
    public struct IoEventArgs
    {
        private bool read;
        private byte value;

        public bool ReadSuccess { get; private set; }
        public bool IsRead { get => read; }
        public bool IsWrite { get => !read; }
        public ushort Address { get; set; }
        public byte Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.value = value; ReadSuccess = true; }
        }

        public IoEventArgs(bool read, ushort address, byte value)
        {
            this.read = read;
            this.value = value;
            Address = address;
            ReadSuccess = false;
        }

        public IoEventArgs(bool read, ushort address)
        {
            this.read = read;
            value = 0;
            Address = address;
            ReadSuccess = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoEventArgs Read(ushort address)
        {
            return new IoEventArgs(true, address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoEventArgs Write(ushort address, byte value)
        {
            return new IoEventArgs(false, address, value);
        }
    }
}
