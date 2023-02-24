namespace System.Windows
{
    public class IApplicationBase
    {
        public IntPtr Handler { private set; get; }

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
            Handler = this.GetHandle(); 
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
