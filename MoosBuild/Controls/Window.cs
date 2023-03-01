using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Content))]
    public class Window : ContentControl
    {
        public string Title { set; get; }
        public int Height { set; get; }
        public int Width { set; get; }

        public object Content { set; get; }
    }
}
