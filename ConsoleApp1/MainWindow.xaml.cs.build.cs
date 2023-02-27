
private void Get()
{
    window.Class = "MoosApplication.MainWindow";
    window.Ignorable = "d";
    window.Title = "MainWindow";
    window.Height = "450";
    window.Width = "800";
    button.Text = "Click me";
    button.Command = "{Binding OnCounterClicked}";
    button.CommandParameter = "{Binding }";
    grid.Children = button;
    window.Content = grid;
    return window;
}
