using System.Diagnostics;

namespace System.Windows
{
    public class MessageBox : Window
    {
        string _message;

        public MessageBox()
        {
            X = 0;
            Y = 0;
            Width = 400;
            Height = 75;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public override void OnDraw()
        {
            if (this._message != null)
            {
                this.Width = WindowManager.font.MeasureString(_message);
            }

            if (this.Width < 400)
            {
                this.Width = 400;
            }

            base.OnDraw();

            if (this._message != null)
            {
                WindowManager.font.DrawString(X + (Width / 2) - ((WindowManager.font.MeasureString(_message)) / 2), (Y + (Height / 2)) -(WindowManager.font.FontSize / 2), _message, Foreground.Value);
            }
        }

        void SetText(string text, string title) 
        {
            this.Title = title;
            this._message = text;
        }

        public static void Show(string text, string title)
        {
            MessageBox frm = new MessageBox();
            frm.SetText(text, title);
            frm.ShowDialog();
        }

        public override void OnClose()
        {
            this._message.Dispose();
            base.OnClose();
        }
    }
}
