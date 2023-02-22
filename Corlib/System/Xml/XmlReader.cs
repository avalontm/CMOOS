using System;
using System.IO;
using System.Text.RegularExpressions;

namespace System.Xml
{
    
    public abstract partial class XmlReader
    {
        public static XmlReader Create(string file)
        {
            // Abrir el archivo XML en modo lectura
            byte[] archivo = File.ReadAllBytes(file);
          
            // Crear un lector de texto y leer todo el contenido del archivo
            StreamReader lector = new StreamReader(archivo);
            string contenido = lector.ReadToEnd();

            // Cerrar el archivo y el lector
            archivo.Dispose();
            lector.Close();

            // Eliminar los espacios en blanco innecesarios del contenido XML
            contenido = Regex.Replace(contenido, @">\s+<", "><");

            // Crear un objeto StringBuilder para almacenar el contenido procesado
            string xml = new string(contenido);

            // Recorrer los elementos del archivo XML
            int indice = 0;
            while (indice < xml.Length)
            {
                // Buscar el inicio de un elemento XML
                int inicio = xml.IndexOf("<", indice);

                if (inicio >= 0 && inicio < xml.Length - 1)
                {
                    // Buscar el final del elemento XML
                    int final = xml.IndexOf(">", inicio);

                    if (final >= 0)
                    {
                        // Obtener el nombre del elemento XML
                        string nombre = xml.Substring(inicio + 1, final - inicio - 1);

                        // Buscar el cierre del elemento XML
                        int cierre = xml.IndexOf("</" + nombre + ">", final);

                        if (cierre >= 0)
                        {
                            // Obtener el contenido del elemento XML
                            string contenidoElemento = xml.Substring(final + 1, cierre - final - 1);

                            // Procesar el contenido del elemento XML

                            // ...

                            // Actualizar el índice de búsqueda
                            indice = cierre + nombre.Length + 3;
                        }
                        else
                        {
                            // El elemento XML no está cerrado correctamente
                            // throw new Exception("Elemento XML no cerrado correctamente: " + nombre);
                            return null;
                        }
                    }
                    else
                    {
                        // El elemento XML no está formateado correctamente
                        //throw new Exception("Elemento XML no formateado correctamente");
                        return null;
                    }
                }
                else
                {
                    // No se encontró ningún elemento XML
                    break;
                }
            }

            return null;
        }
    }
    
}
