using Moos.Core.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace System.Xml
{
    public partial class XmlReader
    {
        Dictionary<string, Dictionary<string, string>> Nodes { set; get; } = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> node = new Dictionary<string, string>();

        string content { set; get; }
        string attributes { set; get; }
        public XmlNodeType NodeType { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }

        public string aName { get; private set; }
        public string aValue { get; private set; }

        public XmlReader()
        {
            NodeType = XmlNodeType.Element;
        }

        public bool Read()
        {
            if (!string.IsNullOrEmpty(content))
            {
                if (content.Length > 0)
                {
                    onElement();
                    return true;
                }
            }
            return false;
        }

        void onElement()
        {
            attributes = string.Empty;

            if ((int)content[0] == 10 || (int)content[0] == 13)
            {
                onNewLine();
                return;
            }

            if (count > 1 && !string.IsNullOrEmpty(Name))
            {
                onElementValue();
                return;
            }

            if (content.Substring(0, 2) == "</")
            {
                onEndElement();
                return;
            }

            int start = content.IndexOf("<");

            int end = content.IndexOf('>');

            if (content.Substring(start + 1, end).IndexOf('=') > 0)
            {
                int _start = content.IndexOf(' ');
                attributes = content.Substring(_start + 1, end);
                end = _start;
            }

            NodeType = XmlNodeType.Element;

            Name = content.Substring(start + 1, end);
            prevElement = Name;

            start = content.IndexOf('>');
            content = content.Substring(start + 1);
        }

        void onNewLine()
        {
            if ((int)content[0] == 10)
            {
                NodeType = XmlNodeType.Whitespace;
            }
            else if ((int)content[0] == 13)
            {
                NodeType = XmlNodeType.None;
            }
            Name = string.Empty;
            Value = string.Empty;
            content = content.Substring(1);
        }

        void onElementValue()
        {
            int start = 0;
            int end = content.IndexOf("</");

            if (content.Substring(0, 1) == "<")
            {
                start = content.IndexOf(">") + 1;
            }

            if (start < 0 || end < 0)
            {
                return;
            }

            NodeType = XmlNodeType.Text;
            prevElement = Name;
            Name = string.Empty;

            Value = content.Substring(start, end);
            content = content.Substring(end);
        }

        void onEndElement()
        {
            NodeType = XmlNodeType.EndElement;

            int start = content.IndexOf('/');
            int end = content.IndexOf('>', start);

            Name = content.Substring(start + 1, end);
            prevElement = Name;

            Value = string.Empty;
            content = content.Substring(end + 1);
        }


        public bool MoveToNextAttribute()
        {
            if (string.IsNullOrEmpty(attributes))
            {
                aValue = string.Empty;
                return false;
            }

            NodeType = XmlNodeType.Attribute;

            int start = 0;
            int end = attributes.IndexOf('=');

            if (end < 0)
            {
                aValue = string.Empty;
                return false;
            }

            aName = attributes.Substring(start, end);
            attributes = attributes.Substring(end + 1);
    
            start = attributes.IndexOf('"');
            attributes = attributes.Substring(start + 1);
            end = attributes.IndexOf('"');

            aValue = attributes.Substring(start, end);
            attributes = attributes.Substring(end + 1);

            return true;
        }

        static string root = "";
        static string prevElement = "";
        static int count = 0;

        public static XmlReader Create(string file)
        {
            XmlReader reader = new XmlReader();

            // Abrir el archivo XML en modo lectura
            byte[] archivo = File.ReadAllBytes(file);
          
            // Crear un lector de texto y leer todo el content del archivo
            StreamReader lector = new StreamReader(archivo);
            reader.content = lector.ReadToEnd();
          
            // Cerrar el archivo y el lector
            archivo.Dispose();
            lector.Close();

            // Analizar el contenido del archivo XML
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // Si es un elemento de apertura, imprimir el nombre de la etiqueta y sus atributos
                    case XmlNodeType.Element:

                       // Console.Write($"<{prevElement}");
                        while (reader.MoveToNextAttribute())
                        {
                            // Console.Write(" " + reader.Name + "=\"" + reader.Value + "\"");
                        }

                        if (string.IsNullOrEmpty(root))
                        {
                            root = reader.Name;

                        }

                       // Console.Write($">");
                        count++;

                        if (!string.IsNullOrEmpty(root))
                        {
                            reader.node.Add(root, prevElement);
                        }

                        break;
                    // Si es un elemento de cierre, imprimir el nombre de la etiqueta de cierre
                    case XmlNodeType.EndElement:

                        //Console.Write($"</{prevElement}>");
                        reader.Nodes.Add(prevElement, reader.node);
                        reader.node = new Dictionary<string, string>();

                        break;
                    // Si es texto, imprimir el valor del texto
                    case XmlNodeType.Text:
                        reader.node.Add(prevElement, reader.Value);
                       // Console.Write($" {reader.Value} ");

                        break;
                    case XmlNodeType.Whitespace:
                       // Console.Write(Environment.NewLine);
                        break;
                }
            }

            reader.content.Dispose();
            reader.attributes.Dispose();
            reader.node.Dispose();
            root.Dispose();
            prevElement.Dispose();

            return reader;
        }

        public Dictionary<string, string> GetNode(string node)
        {
            return Nodes[node];
        }

    }
    
}
