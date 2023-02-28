using System.Collections.Generic;

namespace Moos.Framework.Controls
{
    [ContentProperty(nameof(Children))]
    public abstract partial class Layout : List<IView>
    { 
        readonly List<IView> _children = new();
        public List<IView> Children => this;

    }
}
