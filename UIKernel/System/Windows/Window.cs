using Moos.Core.System.Windows;
using MOOS;
using MOOS.Driver;
using MOOS.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
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
        public WindowState WindowState { get; set; }
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
            WindowState = WindowState.Normal;
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
            if (!WindowManager.HasWindowsRegion && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height)) return true;
            return false;
        }

        public int BarHeight = 40;

        internal bool Move;
        int OffsetX;
        int OffsetY;
        public int Index { get => WindowManager.Childrens.IndexOf(this); }

        public virtual void OnInput()
        {
            if (IsVisible)
            {
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    if (CloseButton != null)
                    {
                        if (CloseButton.IsFocus)
                        {
                            return;
                        }
                    }

                    OnReSize();
                    onMove();
                    onRegion();
                }
                else
                {
                    Move = false;
                    WindowManager.HasWindowMoving = false;
                    WindowManager.HasWindowsRegion = false;
                    WindowManager.HasWindowResizing = false;
                }

                if (Move)
                {
                    X = Control.MousePosition.X - OffsetX;
                    Y = Control.MousePosition.Y - OffsetY;
                }
            }
        }

        //Window Bar
        void onMove()
        {
            if (WindowManager.HasWindowControl)
            {
                return;
            }

            if (Control.Clicked)
            {
                if (!WindowManager.HasWindowMoving && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > (Y - BarHeight) && Control.MousePosition.Y < Y)
                {
                    WindowManager.MovetoTop(this);

                    if (!WindowManager.HasWindowMoving)
                    {
                        if (WindowManager.FocusWindow == this)
                        {
                            Move = true;
                            WindowManager.HasWindowMoving = true;
                            OffsetX = Control.MousePosition.X - X;
                            OffsetY = Control.MousePosition.Y - Y;
                        }
                    }
                }
            }
        }

        //Window Content
        void onRegion()
        {
            if (Control.Clicked)
            {
                if (!WindowManager.HasWindowsRegion && Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                {
                    WindowManager.HasWindowsRegion = true;
                    WindowManager.MovetoTop(this);
                }
            }
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

                if (CloseButton != null)
                {
                    CloseButton.X = CloseButtonX;
                    CloseButton.Y = CloseButtonY;
                    CloseButton.OnUpdate();
                }

                OnEdges();
            }
        }


        int prevMouseX;
        int prevMouseY;

        int prevWidth;
        int prevHeight;
        bool isRightEdge;
        bool isBottomEdge;

        void OnEdges()
        {
            if (WindowState == WindowState.Normal)
            {
                if (WindowManager.FocusWindow != this)
                {
                    return;
                }

                if (WindowManager.HasWindowResizing)
                {
                    return;
                }

                isRightEdge = false;
                isBottomEdge = false;

                //LEFT
                if (Control.MousePosition.X > (X - 2) && Control.MousePosition.X < (X + 2) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                {
                    WindowManager.HasWindowFocusResizing = true;
                    CursorManager.State.Value = CursorState.Horizontal;

                    if (Control.Clicked)
                    {
                        if (!WindowManager.HasWindowResizing)
                        {
                            prevMouseX = Control.MousePosition.X;
                            prevWidth = this.Width;
                        }

                        isRightEdge = false;
                        WindowManager.HasWindowResizing = true;
                    }
                    return;
                }

                //RIGHT
                if (Control.MousePosition.X > ((X + Width) - 2) && Control.MousePosition.X < ((X + Width) + 2) && Control.MousePosition.Y > Y && Control.MousePosition.Y < (Y + Height))
                {
                    WindowManager.HasWindowFocusResizing = true;
                    CursorManager.State.Value = CursorState.Horizontal;

                    if (Control.Clicked)
                    {
                        if (!WindowManager.HasWindowResizing)
                        {
                            prevMouseX = Control.MousePosition.X;
                            prevWidth = this.Width;
                        }

                        isRightEdge = true;
                        WindowManager.HasWindowResizing = true;
                    }

                    return;
                }

                //TOP
                if (Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > ((Y - BarHeight) - 2) && Control.MousePosition.Y < ((Y - BarHeight) + 2))
                {
                    WindowManager.HasWindowFocusResizing = true;
                    CursorManager.State.Value = CursorState.Vertical;

                    if (Control.Clicked)
                    {
                        if (!WindowManager.HasWindowResizing)
                        {
                            prevMouseY = Control.MousePosition.Y;
                            prevHeight = this.Height;
                        }

                        isBottomEdge = false;
                        WindowManager.HasWindowResizing = true;
                    }

                    return;
                }

                //BOTTOM
                if (Control.MousePosition.X > X && Control.MousePosition.X < (X + Width) && Control.MousePosition.Y > ((Y + Height) - 2) && Control.MousePosition.Y < ((Y + Height) + 2))
                {
                    CursorManager.State.Value = CursorState.Vertical;

                    if (Control.Clicked)
                    {
                        if (!WindowManager.HasWindowResizing)
                        {
                            prevMouseY = Control.MousePosition.Y;
                            prevHeight = this.Height;
                        }

                        isBottomEdge = true;
                        WindowManager.HasWindowResizing = true;
                    }

                    return;
                }

                WindowManager.HasWindowFocusResizing = false;
            }
        }

        public virtual void OnReSize()
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                if (WindowManager.HasWindowResizing)
                {
                    if (CursorManager.State.Value == CursorState.Horizontal)
                    {
                        int diffX = Control.MousePosition.X - prevMouseX;

                        int diffWidth = ((isRightEdge) ? diffX : 0);

                        int newWidth = prevWidth + diffWidth;

                        if (!isRightEdge)
                        {
                            this.X = Control.MousePosition.X;
                            this.Width = newWidth - diffX ;
                        }
                        else
                        {
                            this.Width = newWidth;
                        }
                    }
                    else if (CursorManager.State.Value == CursorState.Vertical)
                    {
                        int diffY = Control.MousePosition.Y - prevMouseY;

                        int diffHeight = ((isBottomEdge) ? diffY : 0);

                        int newHeight = prevHeight + diffHeight;

                        if (!isBottomEdge)
                        {
                            this.Y = (Control.MousePosition.Y + this.BarHeight);
                            this.Height = newHeight - diffY;
                        }
                        else
                        {
                            this.Height = newHeight;
                        }
                    }
                }
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
                if (WindowManager.FocusWindow == this)
                {
                    WindowManager.FocusWindow = null;
                }
                OnUnloaded();
            }
        }

    }
}
