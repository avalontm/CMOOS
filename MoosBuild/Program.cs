using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlToCode;

namespace MoosBuild
{
    internal class Program
    {
        static string Root { set; get; }
        static List<string> Files { set; get; }

        static void Main(string[] args)
        {
            Console.WriteLine($"=================================================================");
            Console.WriteLine($"============= MOOS Mue Build V.1.0.0 (By AvalonTM) ==============");
            Console.WriteLine($"=================================================================");

            Files = new List<string>();

            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                string[] _args = arg.Split('=');

                if (_args.Length > 1)
                {
                    switch (_args[0])
                    {
                        case "-path":
                            {
                                Root = _args[1];
                            }
                            break;
                        case "-xaml":
                            {
                                string[] _files = _args[1].Split(new char[] { ';' });

                                if (_files.Length > 0)
                                {
                                    foreach (string _file in _files)
                                    {
                                        Files.Add(_file);
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            foreach (string file in Files)
            {
                string fileName = Path.Combine(Root, file);

                if (File.Exists(fileName))
                {
                    Console.WriteLine($"[Moos Xaml Build] {fileName}");
                    string xaml = File.ReadAllText(fileName);

                    if (xaml != null)
                    {
                        XamlToCodeConverter convert = new XamlToCodeConverter();
                       
                        string output = convert.Convert(xaml);
                        File.WriteAllText(fileName + ".generate.cs", output );
                        Console.WriteLine($" Result -> {output}");
                    }
                }
            }

        }
    }
}
