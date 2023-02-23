using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics
{
    public partial class ProcessStartInfo
    {
        private string? _fileName;
        private string? _arguments;
        private string? _directory;
        private string? _userName;
        private string? _verb;

       // private Collection<string>? _argumentList;
        //private ProcessWindowStyle _windowStyle;

        //internal DictionaryWrapper? _environmentVariables;

        /// <devdoc>
        ///     Default constructor.  At least the <see cref='System.Diagnostics.ProcessStartInfo.FileName'/>
        ///     property must be set before starting the process.
        /// </devdoc>
        public ProcessStartInfo()
        {
        }

        /// <devdoc>
        ///     Specifies the name of the application or document that is to be started.
        /// </devdoc>
        public ProcessStartInfo(string fileName)
        {
            _fileName = fileName;
        }

        /// <devdoc>
        ///     Specifies the name of the application that is to be started, as well as a set
        ///     of command line arguments to pass to the application.
        /// </devdoc>
        public ProcessStartInfo(string fileName, string arguments)
        {
            _fileName = fileName;
            _arguments = arguments;
        }

        /// <devdoc>
        ///     Specifies the set of command line arguments to use when starting the application.
        /// </devdoc>
        [AllowNull]
        public string Arguments
        {
            get => _arguments ?? string.Empty;
            set => _arguments = value;
        }

        [AllowNull]
        public string FileName
        {
            get => _fileName ?? string.Empty;
            set => _fileName = value;
        }

        [AllowNull]
        public string WorkingDirectory
        {
            get => _directory ?? string.Empty;
            set => _directory = value;
        }

        public bool ErrorDialog { get; set; }
        public IntPtr ErrorDialogParentHandle { get; set; }

        [AllowNull]
        public string UserName
        {
            get => _userName ?? string.Empty;
            set => _userName = value;
        }

        [AllowNull]
        public string Verb
        {
            get => _verb ?? string.Empty;
            set => _verb = value;
        }
    }
}
