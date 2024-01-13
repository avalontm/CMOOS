using System;
using System.Windows.Media;


namespace NES
{
    public class GameRender
    {
        NES NES;

        // Setup background color to use with Alpha
        Color colorBG;

        public void InitializeGame()
        {
            colorBG = new Color(0, 0, 255);
        }

        public unsafe void WriteBitmap(byte[] byteToWrite, Color XColor)
        {
            lock (this)
            {
                fixed (int* ptr = App.ScreenBuf.RawData)
                {
                    for (int i = 0; i < (App.ScreenBuf.Width * App.ScreenBuf.Height); i++) ptr[i] = (int)XColor.ToArgb();
                }
                
                int w = 0;
                int h = 0;

                for (int i = 0; i < byteToWrite.Length; i += 4)
                {
                    Color color = Color.FromArgb(byteToWrite[i + 3], byteToWrite[i + 2], byteToWrite[i + 1], byteToWrite[i + 0]);
                    if (color.A != 0)
                    {
                        App.ScreenBuf.RawData[App.ScreenBuf.Width * h + w] = (int)color.ToArgb();
                    }
                    color.Dispose();
                    //
                    w++;
                    //256*240
                    if (w == 256)
                    {
                        w = 0;
                        h++;
                    }
                }
            }
        }

        public GameRender(NES formObject)
        {
            NES = formObject;
            InitializeGame();
        }
    }
}