using Moos.Framework;
using Moos.Framework.Controls;
using Moos.Framework.Input;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace MoosApplication
{
    public partial class MainWindow : Window
    {
        int count = 0;

        public MainWindow()
        {
            InitializeComponent(); 

            //Window
            this.Title = "Moos Application";
            this.Width = 300;
            this.Height = 200;

            //Button 
            Button button = new Button(this);
            button.Text = "Click Me";
            button.Margin = new Thickness(5);
            button.X = 10;
            button.Y = 10;
            button.Width = 120;
            button.Height = 48;
            button.Background = Color.FromArgb(0xFF549dc4);
            button.Foreground = Color.FromArgb(0xFFf5f5f5);
            button.Command = new ICommand(OnCounterClicked);
            button.CommandParameter = button;
            button.HorizontalOptions = LayoutOptions.Center;
            button.VerticalOptions= LayoutOptions.Center;
            DataContext = this;
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
        }

    }
}
