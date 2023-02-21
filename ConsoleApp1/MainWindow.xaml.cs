using Moos.Framework;
using Moos.Framework.Controls;
using Moos.Framework.Input;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Image = Moos.Framework.Controls.Image;

namespace MoosApplication
{
    public partial class MainWindow : Window
    {
        private System.Globalization.CultureInfo EnglishCultureInfo = new System.Globalization.CultureInfo("en-us", false);

        int count = 0;

        public MainWindow()
        {
            InitializeComponent(); 

            //Window
            this.Title = "Moos Application";
            this.Width = 300;
            this.Height = 200;

            //Grid
            Grid _grid0 = new Grid(this);
            // ---------------------------
            RowDefinitionCollection _rowDefinitionCollection1 = _grid0.RowDefinitions;
            // ---------------------------
            RowDefinition _rowDefinition2 = new RowDefinition();
            _rowDefinitionCollection1.Add(_rowDefinition2);
            GridLengthConverter _gridLengthConverter = new GridLengthConverter();
            _rowDefinition2.Height = new GridLength(48, GridUnitType.Auto);
            // ---------------------------
            RowDefinition _rowDefinition3 = new RowDefinition();
            _rowDefinitionCollection1.Add(_rowDefinition3);
            _rowDefinition3.Height = new GridLength(1, GridUnitType.Star);
            // ---------------------------
            RowDefinition _rowDefinition4 = new RowDefinition();
            _rowDefinitionCollection1.Add(_rowDefinition4);
            _rowDefinition4.Height = new GridLength(40, GridUnitType.Pixel);

            _grid0.RowDefinitions = _rowDefinitionCollection1;

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

            _grid0.SetRow(button, 2);

            //Image
            Image image = new Image();
            image.Margin = new Thickness(5);
            image.Source = ImageSource.FromFile("MOOS-Logo.png");
            _grid0.SetRow(image, 1);

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
