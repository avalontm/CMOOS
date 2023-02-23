namespace System.Windows
{
    public partial class IApplicationBase
    {
        string _executablePath;
        public string ExecutablePath
        {
            get
            {
                return _executablePath;
            }
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
