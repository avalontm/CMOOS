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
        public Dictionary<string, string> Elements = new Dictionary<string, string>();
        string content { set; get; }
        string attributes { set; get; }
        public XmlNodeType NodeType { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }

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
                    switch (content[0])
                    {
                        case '\n':
                            onWhitespace();
                            break;
                        default:
                            onElement();
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        void onElement()
        {
            attributes = string.Empty;

            if (!string.IsNullOrEmpty(Name))
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

            if (content.Substring(start +1, end).IndexOf('=') > 0)
            {
                int _start = content.IndexOf(' ');
                attributes = content.Substring(_start + 1, end);
                end = _start;
            }

            NodeType = XmlNodeType.Element;

            Name = content.Substring(start + 1, end);
    
            start = content.IndexOf('>');
            content = content.Substring(start + 1);
 
        }

        void onElementValue()
        {
            int start = 0;
            int end = content.IndexOf("</");

            if (end < 0)
            {
                return;
            }

            NodeType = XmlNodeType.Text;

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
            Value = string.Empty;
            content = content.Substring(end + 1);
        }

        void onWhitespace()
        {
            NodeType = XmlNodeType.Whitespace;
            content = content.Substring(1);
        }

        public bool MoveToNextAttribute()
        {
            if (string.IsNullOrEmpty(attributes))
            {
                Value = string.Empty;
                return false;
            }

            NodeType = XmlNodeType.Attribute;

            int start = 0;
            int end = attributes.IndexOf('=');

            if (end < 0)
            {
                Value = string.Empty;
                return false;
            }

            Name = attributes.Substring(start, end);
            attributes = attributes.Substring(end + 1);
    
            start = attributes.IndexOf('"');
            attributes = attributes.Substring(start + 1);
            end = attributes.IndexOf('"');

            Value = attributes.Substring(start, end);
            attributes = attributes.Substring(end + 1);

            return true;
        }

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
                        Console.Write("<" + reader.Name);

                        while (reader.MoveToNextAttribute())
                        {
                            Console.Write(" " + reader.Name + "=\"" + reader.Value + "\"");
                        }

                        Console.Write(">");
                        break;
                    // Si es un elemento de cierre, imprimir el nombre de la etiqueta de cierre
                    case XmlNodeType.EndElement:
                        Console.Write("</" + reader.Name + ">");
                        break;
                    // Si es texto, imprimir el valor del texto
                    case XmlNodeType.Text:
                        Console.Write(reader.Value);
                        break;
                    // Si es un salto de línea, imprimir una nueva línea
                    case XmlNodeType.Whitespace:
                        Console.Write(Environment.NewLine);
                        break;
                }
            }

            reader.content.Dispose();
            reader.attributes.Dispose();

            Console.Write(Environment.NewLine);

            return reader;
        }


    }
    
}
