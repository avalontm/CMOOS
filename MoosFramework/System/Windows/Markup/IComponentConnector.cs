namespace System.Windows.Markup
{
    internal interface IComponentConnector
    {
        void Connect(int connectionId, object target);
        //
        // Resumen:
        //     Loads the compiled page of a component.
        void InitializeComponent();
    }
}
