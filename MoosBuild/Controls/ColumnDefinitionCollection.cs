using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moos.Framework.Controls
{
    public class ColumnDefinitionCollection : List<ColumnDefinition>
    {
        public GridLength Height { set; get; }

        public ColumnDefinitionCollection()
        {
            new List<ColumnDefinition>();
        }

    }
}
