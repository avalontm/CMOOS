using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Explorer.Managers
{
    public static class WindowManager
    {
        public static List<Window> Windows = new List<Window>();
        public static Window Focus { private set; get; }


        public static void Update()
        {
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].OnUpdate();
            }
        }

        public static void Draw()
        {
            for(int i=0; i < Windows.Count; i++)
            {
                Windows[i].OnDraw();
            }
        }
    }
}
