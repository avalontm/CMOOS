using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moos.Framework.Controls
{
    public class RowDefinitionCollection : List<RowDefinition>
    {
        public GridLength Height { set; get; }

        public RowDefinitionCollection()
        {
            new List<RowDefinition>();
        }

    }
}
