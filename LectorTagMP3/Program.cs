using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using LectorMp3; // <-- ¡Acá llamamos a tu nuevo namespace!

class Program
{
    static void Main()
    {
        List<Id3v1Tag> listaCanciones = new List<Id3v1Tag>();

        Console.WriteLine("=== LECTOR DE TAGS MP3 (MÉTODO BUFFER) ===");
        Console.Write("Ingrese la ruta del directorio que contiene los MP3: ");
        string? rutaDirectorio = Console.ReadLine();

        // Chequeamos si el directorio existe usando el Objeto Directory de la teoría
        if (Directory.Exists(rutaDirectorio))
        {
            // Obtenemos todos los archivos MP3 del directorio
            string[] archivosMp3 = Directory.GetFiles(rutaDirectorio, "*.mp3");
            
            // Contabilizamos los archivos encontrados como pide la teoría
            Console.WriteLine($"\n>> Se encontraron {archivosMp3.Length} archivos MP3.\n");

            foreach (string rutaMp3 in archivosMp3)
            {
                // Chequeamos la existencia del archivo usando el Objeto File
                if (File.Exists(rutaMp3))
                {
                    // 1. INSTANCIAMOS FILESTREAM (Exactamente como en la teoría)
                    FileStream MiArchivo = new FileStream(rutaMp3, FileMode.Open);

                    if (MiArchivo.Length >= 128)
                    {
                        // 2. POSICIONAMIENTO
                        // Nos movemos 128 posiciones hacia atrás desde el final
                        MiArchivo.Seek(-128, SeekOrigin.End);

                        // 3. LECTURA CRUDA A UN BUFFER (Como en la diapositiva)
                        // Creamos un arreglo para almacenar los bytes transferidos
                        byte[] buffer = new byte[128];
                        
                        // Leemos 128 bytes y el puntero Position avanza
                        int bytesleidos = MiArchivo.Read(buffer, 0, 128);

                        // 4. TRANSFORMANDO BYTES A TEXTO
                        // Usamos GetString(fuente de bytes, Posición Original, cantidad)
                        string header = Encoding.Default.GetString(buffer, 0, 3);

                        if (header == "TAG")
                        {
                            Id3v1Tag miCancion = new Id3v1Tag();

                            // Extraemos los datos respetando la tabla de Offsets y Longitudes del TP
                            // Offset 3, Longitud 30
                            miCancion.Titulo = Encoding.Default.GetString(buffer, 3, 30).Trim('\0', ' ');
                            
                            // Offset 33, Longitud 30
                            miCancion.Artista = Encoding.Default.GetString(buffer, 33, 30).Trim('\0', ' ');
                            
                            // Offset 63, Longitud 30
                            miCancion.Album = Encoding.Default.GetString(buffer, 63, 30).Trim('\0', ' ');
                            
                            // Offset 93, Longitud 4
                            miCancion.Anio = Encoding.Default.GetString(buffer, 93, 4).Trim('\0', ' ');

                            listaCanciones.Add(miCancion);
                        }
                    }

                    // 5. CIERRE DE LA SECUENCIA (Libera los recursos tomados por MiArchivo)
                    MiArchivo.Close();
                }
            }

            // Mostramos los datos por consola
            if (listaCanciones.Count > 0)
            {
                Console.WriteLine("--- DATOS EXTRAÍDOS DE LOS ARCHIVOS ---");
                foreach (Id3v1Tag cancion in listaCanciones)
                {
                    Console.WriteLine($"Título: {cancion.Titulo,-25} | Artista: {cancion.Artista,-20} | Año: {cancion.Anio}");
                }
            }
            else
            {
                Console.WriteLine("Ninguno de los MP3 contiene un Tag ID3v1 válido.");
            }
        }
        else
        {
            Console.WriteLine("\nError: El directorio ingresado no existe.");
        }
    }
}