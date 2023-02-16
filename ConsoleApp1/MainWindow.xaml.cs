using System.Drawing;
using System.Moos;
using System.Moos.Controls;
using System.Moos.Input;

namespace AppDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Create the Button 
            Button button = new Button();
            button.Content = "Click Me";
            button.Margin = new Thickness(5);
            button.X = 10;
            button.Y = 10;
            button.Width = 120;
            button.Height = 48;
            button.Background = Color.FromArgb(0x549dc4);
            button.Command = new ICommand(onButton_Click);
            button.CommandParameter = "parameters";
            button.Generate(this);

            this.Content = button;

            DataContext = this;
        }

        void onButton_Click(object obj)
        {
           Program.MessageBox("API", $"Mesage from api with parameter: {obj}");
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }
    }
}
