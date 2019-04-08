using System.Runtime.InteropServices;

namespace TailwindTraders.Mobile.Droid.ThirdParties.Camera
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorUnion
    {
        [FieldOffset(0)]
        private uint value;
        [FieldOffset(0)]
        private byte b;
        [FieldOffset(1)]
        private byte g;
        [FieldOffset(2)]
        private byte r;
        [FieldOffset(3)]
        private byte a;

        public ColorUnion(uint value)
            : this(0, 0, 0, 0)
        {
            this.value = value;
        }

        public ColorUnion(byte r, byte g, byte b)
            : this(r, g, b, 255)
        {
        }

        public ColorUnion(byte r, byte g, byte b, byte a)
        {
            this.value = 0;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public uint Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }

        public byte R
        {
            get
            {
                return this.r;
            }

            set
            {
                this.r = value;
            }
        }

        public byte G
        {
            get
            {
                return this.g;
            }

            set
            {
                this.g = value;
            }
        }

        public byte B
        {
            get
            {
                return this.b;
            }

            set
            {
                this.b = value;
            }
        }

        public byte A
        {
            get
            {
                return this.a;
            }

            set
            {
                this.a = value;
            }
        }
    }
}