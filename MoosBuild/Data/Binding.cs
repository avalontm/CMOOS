using Moos.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Moos.Framework.Controls
{
    public class Binding: MarkupExtension
    {
        public string Path { set; get; }
        public object Source { set; get; }
        public string ElementName { set; get; }

        public Binding()
        {

        }

        public Binding(string path = "")
        {
            this.Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
