using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum MessageBoxButtons
    {
        /// <summary>
        ///  Specifies that the message box contains an OK button.
        /// </summary>
        OK = 0,

        /// <summary>
        ///  Specifies that the message box contains OK and Cancel buttons.
        /// </summary>
        OKCancel = 1,

        /// <summary>
        ///  Specifies that the message box contains Abort, Retry, and Ignore buttons.
        /// </summary>
        AbortRetryIgnore = 2,

        /// <summary>
        ///  Specifies that the message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel = 3,

        /// <summary>
        ///  Specifies that the message box contains Yes and No buttons.
        /// </summary>
        YesNo = 4,

        /// <summary>
        ///  Specifies that the message box contains Retry and Cancel buttons.
        /// </summary>
        RetryCancel = 5,

        /// <summary>
        ///  Specifies that the message box contains Cancel, Try Again, and Continue buttons.
        /// </summary>
        CancelTryContinue = 6,
    }

}
