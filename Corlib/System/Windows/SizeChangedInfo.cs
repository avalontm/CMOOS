using System.Windows.Controls;

namespace System.Windows
{
    public class SizeChangedInfo
    {
        public Position Rec { set; get; }

        public SizeChangedInfo()
        {
            Rec = new Position();
        }
    }
}
