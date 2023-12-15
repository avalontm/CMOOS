using Moos.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Explorer.Controls
{
    public class Separator : Widget
    {

        public Separator()
        {

        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            IsVisible = true;
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            if(IsLoaded && IsVisible)
            {
                GDI.DrawLine(X, Y, X + Width, Y , 0xFF868a8e);
                GDI.DrawLine(X, Y+1, X + Width, Y +1, 0xFFFFFFFF);
            }
        }
    }
}
