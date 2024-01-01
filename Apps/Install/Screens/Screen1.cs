using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Install.Screens
{
    public static class Screen1
    {

        public static void Draw()
        {
            Console.Clear();

            FontManager.font.DrawString(5, 5, "CMOOS 95 Setup", App.Gray.ToArgb());
            FontManager.font.DrawString(5, 20, "========================", App.Gray.ToArgb());

            FontManager.font.DrawString(15, 50, "Welcome to Setup.", App.Gray.ToArgb());

            FontManager.font.DrawString(15, 80, "The Setup program prepares CMOOS 95 to run on your computer.", App.Gray.ToArgb());

            FontManager.font.DrawString(15, 120, "* To set up MOOS now, press ENTER.", App.Gray.ToArgb());
            FontManager.font.DrawString(15, 140, "* To learn more about Setup before continue, press F1.", App.Gray.ToArgb());
            FontManager.font.DrawString(15, 160, "* To quit Setup without install CMOOS,press F3", App.Gray.ToArgb());


            FontManager.font.DrawString(15, App.screenHeight - 60, "To continue with Setup, press ENTER.", App.Gray.ToArgb());

            GDI.FillRectangle(0, App.screenHeight - 28, App.screenWidth, 32, App.Gray.ToArgb());
            FontManager.font.DrawString(5, App.screenHeight - 20, "ENTER=Continue   F1=Help    F3=Exit", App.Black.ToArgb());

            ConsoleKey key = Console.ReadKey();

            switch(key)
            {
                case ConsoleKey.Enter:
                    App.IndexScreen = 1;
                    break;
                case ConsoleKey.F1:
                    break;
                case ConsoleKey.F3:
                    App.IndexScreen = -1;
                    break;
            }
            
        }
    }
}
