using MOOS;
using MOOS.Driver;
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

        public Window() : base()
        {
            this.IsVisible = false;
            X= 0;
            Y= 0;
            Width = 300;
            Height = 150;
            onInitWindow();
        }

        public Window(int x, int y, int width, int height)
        {
            this.IsVisible = false;
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
            CloseButton.Text = "X";
            CloseButton.Background = new Brush(0xd9d9d9);
            CloseButton.HighlightBackground = new Brush(0xde4343);
           
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Focus = this;
            WindowManager.Childrens.Add(this);
        }

        public void ShowDialog()
        {
            onWindowStartupLocation();
            WindowManager.MovetoTop(this);
            OnLoaded();
            this.IsVisible = true;
        }

        public override void OnLoaded()
        {
            CloseButton.OnLoaded();

            if (Content != null)
            {
                Content.OnLoaded();
            }

            base.OnLoaded();
        }

        public override void OnUnloaded()
        {
            if (Content != null)
            {
                Content.OnUnloaded();
            }

            CloseButton.OnUnloaded();

            base.OnUnloaded();
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

        public int BarHeight = 40;

        bool Move;
        int OffsetX;
        int OffsetY;
        public int Index { get => WindowManager.Childrens.IndexOf(this); }

        public virtual void OnInput()
        {
            if (IsVisible)
            {
                if (CloseButton != null)
                {
                    if (CloseButton.IsFocus)
                    {
                        return;
                    }
                }

                if (Control.MouseButtons == MouseButtons.Left)
                {
                    Debug.WriteLine($"[INDEX] {Index}");
                    if (!onOtherWindowFocus() && !WindowManager.HasWindowMoving && !Move &&Control.MousePosition.X > X && Control.MousePosition.X < X + Width && Control.MousePosition.Y > Y - BarHeight && Control.MousePosition.Y < Y)
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
        }

        bool onOtherWindowFocus()
        {
            if (!WindowManager.HasWindowMoving)
            {
                for (int i = Index + 1; i < WindowManager.Childrens.Count; i++)
                {
                    Window widget = WindowManager.Childrens[i];

                    if (Control.MousePosition.X > widget.X && Control.MousePosition.X < widget.X + widget.Width && Control.MousePosition.Y > widget.Y && Control.MousePosition.Y < widget.Y + widget.Height && !widget.Move)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        private int CloseButtonX => X + Width + 2 - (BarHeight / 2) - (CloseButton.Width / 2);
        private int CloseButtonY => Y - BarHeight + (BarHeight / 2) - (CloseButton.Height / 2);

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsVisible)
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

            if (this.IsVisible)
            {
                //WindowBar
                Framebuffer.Graphics.FillRectangle((X-1), (Y - BarHeight), (Width+1), BarHeight, 0xebebeb);

                if(!string.IsNullOrEmpty(Title))
                {
                    WindowManager.font.DrawString(X + (Width / 2) - ((WindowManager.font.MeasureString(Title)) / 2), Y - (BarHeight / 2) - (WindowManager.font.FontSize / 4), Title, 0xFF000000);
                }

                if (Background != null)
                {
                    //Window Content
                    Framebuffer.Graphics.FillRectangle(X, Y, Width, Height, Background.Value);
                }

                if (Content != null)
                {
                    Content.OnDraw();
                }

                if (BorderBrush != null)
                {
                    DrawBorder();
                }

                if (CloseButton != null)
                {
                    CloseButton.OnDraw();
                }
            }
        }

        void onClose(object obj)
        {
            OnClose();
        }

        public virtual void OnClose()
        {
            if (WindowManager.Childrens.Remove(this))
            {
                OnUnloaded();
            }
        }

    }
}
