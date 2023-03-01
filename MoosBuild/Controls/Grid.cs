using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public class Grid : ContentControl
    {
        public UIElementCollection Children { set; get; }
        public RowDefinitionCollection RowDefinitions { set; get; }

        public static void SetRow(UIElement element, int value)
        { 
        }
    }
}
