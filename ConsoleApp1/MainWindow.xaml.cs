using Moos.Framework;
using Moos.Framework.Controls;
using Moos.Framework.Input;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Image = Moos.Framework.Controls.Image;

namespace MoosApplication
{
    public partial class MainWindow : Window
    {
        int count = 0;

        public MainWindow()
        {
            InitializeComponent(); 
        }

        void OnCounterClicked(object obj)
        {
            Button button = (Button)obj;

            if (button != null)
            {
                count++;

                if (count == 1)
                    button.Text = $"Clicked {count} time";
                else
                    button.Text = $"Clicked {count} times";
            }

            byte[] data = Encoding.UTF8.GetBytes("Hola mundo!");
            File.WriteAllBytes("home/moos/Desktop/files/file.txt", data);
            data.Dispose();
        }

    }
}
