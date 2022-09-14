namespace System.Text
{
    public abstract unsafe class Encoding
    {
        public static Encoding UTF8;
        public static Encoding ASCII;

        public abstract string GetString(byte* ptr);

        public byte[] GetBytes(string s)
        {
            byte[] buffer = new byte[s.Length];
            for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte)s[i];
            return buffer;
        }
    }
}
