using System;
using System.Collections.Generic;

using System.Text;

namespace System.Windows.Forms
{
    public enum MessageBoxIcon
    {
        /// <summary>
        ///  Specifies that the message box contain no symbols.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Specifies that the message box contains a hand symbol.
        /// </summary>
        Hand = 1,

        /// <summary>
        ///  Specifies that the message box contains a question mark symbol.
        /// </summary>
        Question = 2,

        /// <summary>
        ///  Specifies that the message box contains an exclamation symbol.
        /// </summary>
        Exclamation = 3,

        /// <summary>
        ///  Specifies that the message box contains an asterisk symbol.
        /// </summary>
        Asterisk = 4,

        /// <summary>
        ///  Specifies that the message box contains a hand icon. This field is constant.
        /// </summary>
        Stop = 5,

        /// <summary>
        ///  Specifies that the message box contains a hand icon.
        /// </summary>
        Error = 6,

        /// <summary>
        ///  Specifies that the message box contains an exclamation icon.
        /// </summary>
        Warning = 7,

        /// <summary>
        ///  Specifies that the message box contains an asterisk icon.
        /// </summary>
        Information = 8,
    }

}
