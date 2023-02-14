using MOOS;
using MOOS.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows
{

    public enum WindowStartupLocation
    {
        Manual = 0,
        CenterScreen = 1,
        CenterOwner = 2
    }

    public abstract class Window : Widget
    {
        public string Title { set; get; }
        public WindowStartupLocation WindowStartupLocation { get; set; }
        public Widget Focus { set; get; }
        Button CloseButton;

        Widget _content;
        public Widget Content
        {
            set
            {
                _content = value;
                _content.Parent = this;
            }
            get
            {
                return _content;
            }
        }

        public bool Visible
        {
            set
            {
                _visible = value;
                OnSetVisible(value);
            }
            get
            {
                return _visible;
            }
        }

        public Window() : base()
        {
            this.Visible = false;
            X= 0;
            Y= 0;
            Width = 300;
            Height = 150;
            onInitWindow();
        }

        public Window(int x, int y, int width, int height)
        {
            this.Visible = false;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            onInitWindow();
        }

        void onInitWindow()
        {
            Background = Brushes.White;

            CloseButton = new Button();
            CloseButton.Width = 28;
            CloseButton.Height = 28;
            CloseButton.Command = new Data.Binding();
            CloseButton.Command.Source = new ICommand(onClose);
            CloseButton.Content = "X";
            CloseButton.Background = new Brush(0xd9d9d9);
            CloseButton.HighlightBackground = new Brush(0xde4343);

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Focus = this;
            WindowManager.Childrens.Add(this);
        }

        public void ShowDialog()
        {
            OnLoaded();
            onWindowStartupLocation();
            WindowManager.MovetoTop(this);
            this.Visible = true;
        }

        public virtual void OnLoaded()
        { 
        
        }

        void onWindowStartupLocation()
        {
            switch (this.WindowStartupLocation)
            {
                case WindowStartupLocation.Manual:
                    break;
                case WindowStartupLocation.CenterOwner:

                    break;
                case WindowStartupLocation.CenterScreen:
                    X = (Framebuffer.Width / 2) - (this.Width / 2);
                    Y = (Framebuffer.Height / 2) - (this.Height / 2);
                    break;
            }

        }

        public bool IsUnderMouse()
        {
            if (Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y && Control.MousePosition.Y < Y + Height) return true;
            return false;
        }

        public bool _visible;

        public virtual void OnSetVisible(bool value) { }

        public int BarHeight = 40;

        bool Move;
        int OffsetX;
        int OffsetY;
        public int Index { get => WindowManager.Childrens.IndexOf(this); }

        public virtual void OnInput()
        {

            if (Control.MouseButtons == MouseButtons.Left)
            {
                if (
                    !WindowManager.HasWindowMoving &&
                    Control.MousePosition.X > CloseButtonX && Control.MousePosition.X < CloseButtonX + WindowManager.CloseButton.Width &&
                    Control.MousePosition.Y > CloseButtonY && Control.MousePosition.Y < CloseButtonY + WindowManager.CloseButton.Height
                )
                {
                    this.OnClose();
                    return;
                }
                if (!WindowManager.HasWindowMoving && !Move && Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y - BarHeight && Control.MousePosition.Y < Y)
                {
                    WindowManager.MovetoTop(this);

                    if (WindowManager.FocusWindow == this)
                    {
                        Move = true;
                        WindowManager.HasWindowMoving = true;
                        OffsetX = Control.MousePosition.X - X;
                        OffsetY = Control.MousePosition.Y - Y;
                    }
                }
            }
            else
            {
                Move = false;
                WindowManager.HasWindowMoving = false;
            }

            if (Move)
            {
                X = Control.MousePosition.X - OffsetX;
                Y = Control.MousePosition.Y - OffsetY;
            }
        }

        private int CloseButtonX => X + Width + 2 - (BarHeight / 2) - (CloseButton.Width / 2);
        private int CloseButtonY => Y - BarHeight + (BarHeight / 2) - (CloseButton.Height / 2);

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.Visible)
            {
                if (Content != null)
                {
                    Content.OnUpdate();
                }

                CloseButton.X = CloseButtonX; 
                CloseButton.Y = CloseButtonY;

                CloseButton.OnUpdate();
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if (this.Visible)
            {
                //WindowBar
                Framebuffer.Graphics.FillRectangle(X, Y - BarHeight, Width, BarHeight, 0xebebeb);

                if(!string.IsNullOrEmpty(Title))
                {
                    WindowManager.font.DrawString(X + (Width / 2) - ((WindowManager.font.MeasureString(Title)) / 2), Y - (BarHeight / 2) - (WindowManager.font.FontSize / 4), Title, 0xFF000000);
                }

                //Window Content
                Framebuffer.Graphics.FillRectangle(X, Y, Width, Height, Background.Value);

                if (Content != null)
                {
                    Content.OnDraw();
                }

                if (BorderBrush != null)
                {
                    DrawBorder();
                }

                CloseButton.OnDraw();

            }
        }

        void onClose(object obj)
        {
            OnClose();
        }

        public virtual void OnClose()
        {
            this.Visible = false;

            if (Content != null)
            {
                Content.Dispose();
            }

            if (WindowManager.Childrens.Remove(this))
            {
               // this.Dispose();
            }
        }

    }
}
