using Moos.Framework.Controls;
using System.IO;
using System.Text;
using System.Windows;

namespace MoosApplication
{
    public partial class MainWindow : UIMainWindow
    {
        int count = 0;

        public MainWindow() : base()
        {
          
        }

        void OnCounterClicked(object obj)
        {
            Button button = (Button)obj;

            if (button != null)
            {
                count++;

                if (count == 1)
                    button.Content = $"Clicked {count} time";
                else
                    button.Content = $"Clicked {count} times";
            }

        }

    }
}
