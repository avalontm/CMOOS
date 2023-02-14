using System.Drawing;
using System.Moos;
using System.Moos.Controls;

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
            //button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Margin = new Thickness(5);
            //button.VerticalAlignment = VerticalAlignment.Top;
            button.X = 10;
            button.Y = 10;
            button.Width = 120;
            button.Height = 48;
            button.Background = Color.Blue;
            button.Command = new System.Moos.Input.ICommand(onButton_Click);
            button.CommandParameter = "parameters";
            button.Generate(this);

            this.Content = button;

            DataContext = this;
        }

        void onButton_Click(object obj)
        {
           NativeMethod.MessageBox("API", $"Mesage from api with parameter: {obj}");
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
