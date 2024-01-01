using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Install.Screens
{
    public static class Screen3
    {
        static int progress = 0;

        public static void Draw()
        {
            Console.Clear();

            int max = App.screenWidth - 60;
            int progressWidth = (int)((progress / 100.0) * (max-10));

            FontManager.font.DrawString(15, App.screenHeight - 32, $"{progress}% complete", App.Gray.ToArgb());

            GDI.DrawRectangle(15, App.screenHeight - 75, max, 32, App.Gray.ToArgb());
            GDI.FillRectangle(20, App.screenHeight - 70, progressWidth, 22, App.Green.ToArgb());

            MoosNative.Sleep(50);

            if (progress < 100)
            {
                progress++;
            }


            if (progress == 100)
            {
                FontManager.font.DrawString(App.screenWidth - 200, App.screenHeight - 32, $"CONTINUE for exit", App.Gray.ToArgb());

                ConsoleKey key = Console.ReadKey();

                App.IndexScreen = -1;
            }
           
        }
    }
}
