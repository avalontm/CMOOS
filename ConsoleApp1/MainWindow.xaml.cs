using Moos.Framework.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace MoosApplication
{
    public partial class MainWindow : Window
    {
        int count = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        void OnCounterClicked(object sender)
        {
            count++;

            if (count == 1)
                btnClick.Text = $"Clicked {count} time";
            else
                btnClick.Text = $"Clicked {count} times";
        }

        void onClick(object sender, EventArgs e)
        {
            btnClick.Text = "onClick";
        }

    }
}
