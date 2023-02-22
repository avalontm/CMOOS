using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace System.Xml
{
    
    public partial class XmlReader
    {
        string content { set; get; }
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
                        case '<':
                            NodeType = XmlNodeType.Element;
                            onElement("<");
                            break;
                        case '\n':
                            NodeType = XmlNodeType.Whitespace;
                            onWhitespace();
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        void onElement(string value)
        {
            int start = content.IndexOf("<");
            int end = content.IndexOf(">");

            Name = content.Substring(start + 1, end - 1);
            content = content.Substring(end + 1);

            end = content.IndexOf($"</{Name}>");

            Value = content.Substring(0, end);
            content = content.Substring(end + Name.Length + 3);
        }

        void onWhitespace()
        {
            content = content.Substring(1);
        }

        public bool MoveToNextAttribute()
        {
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
                        Debug.Write("<" + reader.Name);
                        while (reader.MoveToNextAttribute())
                        {
                            Debug.Write(" " + reader.Name + " ");
                        }
                        Debug.Write(">");
                        break;
                    // Si es un elemento de cierre, imprimir el nombre de la etiqueta de cierre
                    case XmlNodeType.EndElement:
                        Debug.Write("</" + reader.Name + ">");
                        break;
                    // Si es texto, imprimir el valor del texto
                    case XmlNodeType.Text:
                        Debug.Write(reader.Value);
                        break;
                    // Si es un salto de línea, imprimir una nueva línea
                    case XmlNodeType.Whitespace:
                        Debug.Write(Environment.NewLine);
                        break;
                }
            }

            reader.content.Dispose();

            return reader;
        }


    }
    
}
