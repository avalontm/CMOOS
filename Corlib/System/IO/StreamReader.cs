namespace System.IO
{
    public class StreamReader
    {
        byte[] buffer;

        public StreamReader(byte[] data)
        {
            buffer = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = data[i];
            }

        }

        public string ReadToEnd()
        {
            string str = "";

            for (int i = 0; i < buffer.Length; i++)
            {
                str += (char)buffer[i];
            }

            return str;
        }

        public void Close()
        {
            buffer.Dispose();
        }
    }
}
