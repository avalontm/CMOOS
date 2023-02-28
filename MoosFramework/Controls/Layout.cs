using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public abstract partial class Layout : UIElementCollection
    { 
        UIElementCollection _children = new();
        public UIElementCollection Children { get { return _children; } set { _children = value; } }

    }
}
