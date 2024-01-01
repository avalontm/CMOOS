using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Install.Screens
{
    public static class Screen2
    {
        static int selection = 0;

        public static void Draw()
        {
            Console.Clear();

            FontManager.font.DrawString(5, 5, "CMOOS 95 Setup", App.Gray.ToArgb());
            FontManager.font.DrawString(5, 20, "========================", App.Gray.ToArgb());

            FontManager.font.DrawString(15, 50, "Setup needs to configure the unallocated space on your", App.Gray.ToArgb());
            FontManager.font.DrawString(15, 70, "hard disk to prepare it for use with CMOOS. None of", App.Gray.ToArgb());
            FontManager.font.DrawString(15, 90, "your existing files will be affected.", App.Gray.ToArgb());

            FontManager.font.DrawString(15, 120, "To have Setup configure the space on your disk for your,", App.Gray.ToArgb());
            FontManager.font.DrawString(15, 140, "choose the recommended options.", App.Gray.ToArgb());


            //selection
            GDI.DrawRectangle(15, 180, App.screenWidth - 75, 60, App.Gray.ToArgb());

            onSelection();

            FontManager.font.DrawString(15, App.screenHeight - 60, "To accept the seleccion, press ENTER.", App.Gray.ToArgb());

            GDI.FillRectangle(0, App.screenHeight - 28, App.screenWidth, 32, App.Gray.ToArgb());
            FontManager.font.DrawString(5, App.screenHeight - 20, "ENTER=Continue   F1=Help    F3=Exit", App.Black.ToArgb());

            ConsoleKey key = Console.ReadKey();

            switch(key)
            {
                case ConsoleKey.Enter:
                    switch (selection)
                    {
                        case 0:
                            App.IndexScreen = 2;
                            break;

                        case 1:
                            App.IndexScreen = -1;
                            break;
                    }
                    break;
                case ConsoleKey.F1:
                    break;
                case ConsoleKey.F3:
                    App.IndexScreen = -1;
                    break;
                case ConsoleKey.Up:
                    selection--;
                    if (selection < 0)
                    {
                        selection = 1;
                    }
                    break;
                case ConsoleKey.Down:
                    selection++;
                    if (selection > 1)
                    {
                        selection = 0;
                    }
                    break;
            }
            
        }

        static void onSelection()
        {
            switch (selection)
            {
                case 0:
                    GDI.FillRectangle(20, 185, App.screenWidth - 85, 25, App.Gray.ToArgb());
                    FontManager.font.DrawString(25, 190, "Configure unallocated disk space (reommendaded).", App.Black.ToArgb());
                    FontManager.font.DrawString(25, 215, "Exit Setup.", App.Gray.ToArgb());
                    break;
                case 1:
                    GDI.FillRectangle(20, 210, App.screenWidth - 85, 25, App.Gray.ToArgb());
                    FontManager.font.DrawString(25, 190, "Configure unallocated disk space (reommendaded).", App.Gray.ToArgb());
                    FontManager.font.DrawString(25, 215, "Exit Setup.", App.Black.ToArgb());
                    break;
            }

           
        }
    }
}
