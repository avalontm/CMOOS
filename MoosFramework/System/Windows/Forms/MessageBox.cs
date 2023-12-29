using Moos.Framework.Fonts;
using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Controls;

namespace System.Windows.Forms
{
    public class MessageBox : Widget
    {
        public static MessageBox Instance = new MessageBox();
        string Text { set; get; }
        string Caption { set; get; }
        DialogResult Result = DialogResult.None;
        Image Icon { set; get; }
        Button[] Buttons;


        public MessageBox()
        {
            Background = Color.FromArgb(0xFFC3C7CB);
            Foreground = Color.Black;
            OnLoaded();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (IsLoaded && IsVisible)
            {

            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if(IsLoaded && IsVisible)
            {
                if (Background != null)
                {
                    GDI.FillRectangle(X, Y, Width, Height, Background.ARGB);
                }

                DrawTitleBar();
                DrawMessage();
                DrawButtons();
                DrawNormalBorder();
            }
        }

        void DrawTitleBar()
        {
            GDI.FillRectangle(X +2 , Y + 2, Width - 6, 28, 0xFF0000AA);

            if (!string.IsNullOrEmpty(Caption))
            {
                FontManager.font.DrawString(X + 5, (Y + 28 / 2) - (FontManager.font.Size / 2) + 4, Caption, 0xFFFFFFFF);
            }
        }

        void DrawMessage()
        {
            if (Icon != null)
            {
                GDI.DrawImage(X + 2, Y + 40, Icon, true);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                FontManager.font.DrawString(X + 35, Y + 48, Text, 0xFF000000);
            }
        }

        void DrawButtons()
        {
            if (Buttons != null)
            {
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].OnUpdate();
                    Buttons[i].OnDraw();
                }
            }
        }

        void DrawNormalBorder()
        {
            if (IsLoaded && IsVisible)
            {
                //white
                GDI.DrawLine(X, Y, X + Width, Y, 0xFFFFFFFF);
                GDI.DrawLine(X, Y, X, Y + Height - 2, 0xFFFFFFFF);

                //gray
                GDI.DrawLine(X + Width, Y, X + Width, Y + Height - 2, 0xFF868a8e);
                GDI.DrawLine(X - 2, Y + Height - 1, X + Width, Y + Height - 1, 0xFF868a8e);

                //black
                GDI.DrawLine(X + Width + 1, Y, X + Width + 1, Y + Height - 1, 0xFF000000);
                GDI.DrawLine(X - 2, Y + Height, X + Width, Y + Height, 0xFF000000);
            }
        }

        public static DialogResult Show(string? text, string? caption, EventHandler<object> confirm, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            Instance.Width = 350;
            Instance.Height = 150;

            Instance.X = ((GDI.GetWidth() /2) - (Instance.Width / 2));
            Instance.Y = ((GDI.GetHeight() / 2) - (Instance.Height / 2));

            Instance.Text = text;
            Instance.Caption = caption;
            
            if(buttons == MessageBoxButtons.YesNo)
            {
                Instance.Buttons = new Button[2];

                Instance.Buttons[0] = new Button();
                Instance.Buttons[0].Width = 64;
                Instance.Buttons[0].Height = 28;
                Instance.Buttons[0].X  = Instance.X + Instance.Width - (Instance.Buttons[0].Width *2) - 10;
                Instance.Buttons[0].Y = Instance.Y + Instance.Height - Instance.Buttons[0].Height - 5;
                Instance.Buttons[0].Text = "YES";
                Instance.Buttons[0].Clicked = onConfirm(confirm);
                Instance.Buttons[0].OnLoaded();

                Instance.Buttons[1] = new Button();
                Instance.Buttons[1].Width = 64;
                Instance.Buttons[1].Height = 28;
                Instance.Buttons[1].X = Instance.X + Instance.Width - Instance.Buttons[1].Width - 5;
                Instance.Buttons[1].Y = Instance.Y + Instance.Height - Instance.Buttons[1].Height - 5;
                Instance.Buttons[1].Text = "NO";
                Instance.Buttons[1].Clicked = onCancel;
                Instance.Buttons[1].OnLoaded();
            }
            else if(buttons == MessageBoxButtons.OKCancel)
            {
                Instance.Buttons = new Button[2];

                Instance.Buttons[0] = new Button();
                Instance.Buttons[0].Width = 64;
                Instance.Buttons[0].Height = 28;
                Instance.Buttons[0].X = Instance.X + Instance.Width - (Instance.Buttons[0].Width * 2) - 10;
                Instance.Buttons[0].Y = Instance.Y + Instance.Height - Instance.Buttons[0].Height - 5;
                Instance.Buttons[0].Text = "OK";
                Instance.Buttons[0].Clicked = onConfirm(confirm);
                Instance.Buttons[0].OnLoaded();

                Instance.Buttons[1] = new Button();
                Instance.Buttons[1].Width = 64;
                Instance.Buttons[1].Height = 28;
                Instance.Buttons[1].X = Instance.X + Instance.Width - Instance.Buttons[1].Width - 5;
                Instance.Buttons[1].Y = Instance.Y + Instance.Height - Instance.Buttons[1].Height - 5;
                Instance.Buttons[1].Text = "CANCEL";
                Instance.Buttons[1].Clicked = onCancel;
                Instance.Buttons[1].OnLoaded();
            }

            Instance.IsVisible = true;
            
            switch(icon)
            {
                case MessageBoxIcon.ShutDown:
                    Instance.Icon = PNG.FromFile("sys/media/menu_shutdown.png");
                    break;
                default:
                    //Instance.Icon = PNG.FromFile("sys/media/menu_shutdown.png");
                    break;
            }

            return Instance.Result;
        }

        static EventHandler<object> onConfirm(EventHandler<object> confirm)
        {
            Instance.IsVisible = false;
            return confirm;   
        }

        static void onCancel(object sender, object e)
        {
            Instance.IsVisible = false;
        }
    }
}
