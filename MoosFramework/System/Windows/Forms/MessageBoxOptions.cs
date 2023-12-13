using System;
using System.Collections.Generic;

using System.Text;

namespace System.Windows.Forms
{
    [Flags]
    public enum MessageBoxOptions
    {
        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        ServiceNotification = 0,

        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        DefaultDesktopOnly = 1,

        /// <summary>
        ///  Specifies that the message box text is right-aligned.
        /// </summary>
        RightAlign = 2,

        /// <summary>
        ///  Specifies that the message box text is displayed with Rtl reading order.
        /// </summary>
        RtlReading = 3,
    }

}
