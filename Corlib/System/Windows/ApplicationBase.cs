using System.Runtime.InteropServices;

namespace System.Windows
{
    public class IApplicationBase
    {
        [DllImport("ApplicationCreate")]
        public static extern void ApplicationCreate(IntPtr handler);

        string _executablePath;
        public string ExecutablePath
        {
            get
            {
                return _executablePath;
            }
        }

        public IApplicationBase()
        {
            ApplicationCreate(this.GetHandle());
        }

        public void SetExecutablePath(string path)
        {
            if (string.IsNullOrEmpty(_executablePath))
            {
                _executablePath = path;
                Console.WriteLine($"[ExecutablePath] {_executablePath}");
            }
        }
    }
}
