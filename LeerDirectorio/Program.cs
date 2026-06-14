string? ruta = "";

// 1. Usamos un bucle while para no dejarlo avanzar hasta que ponga una ruta real
while (true)
{
    Console.WriteLine("Ingrese la ruta destino: ");
    ruta = Console.ReadLine();

    if (Directory.Exists(ruta))
    {
        break; // La ruta existe, cortamos el bucle y seguimos con el programa
    }
    else
    {
        // Notificamos y el bucle vuelve a empezar
        Console.WriteLine("\nError: No existe esa ruta. Intente nuevamente.\n");
    }
}

// Si el programa llegó a esta línea, es porque la ruta es válida sí o sí
string[] directorios = Directory.GetDirectories(ruta);
string[] archivos = Directory.GetFiles(ruta);

Console.WriteLine("\n--- CARPETAS ---");
foreach (string directorio in directorios)
{
    Console.WriteLine($"Carpeta: {Path.GetFileName(directorio)}");
}

Console.WriteLine("\n--- ARCHIVOS ---");
foreach (string archivo in archivos)
{
    // 2. Instanciamos FileInfo para "escanear" las propiedades del archivo
    FileInfo infoArchivo = new FileInfo(archivo);
    
    // .Length te da el peso en Bytes. Lo dividimos en 1024.0 para pasarlo a KB.
    double tamanioKB = infoArchivo.Length / 1024.0;
    
    // El ":F2" es un truquito de C# para redondear el número a solo 2 decimales
    Console.WriteLine($"Archivo: {infoArchivo.Name} | Tamaño: {tamanioKB:F2} KB");
}

// 1. Armamos la ruta final donde se va a guardar el CSV (junta la ruta de la carpeta con el nombre del archivo)
string rutaArchivoCsv = Path.Combine(ruta, "reporte_archivos.csv");

// 2. Abrimos la "manguera" (el bloque using asegura que se cierre sola al terminar)
using (StreamWriter escritor = new StreamWriter(rutaArchivoCsv))
{
    // 3. Escribimos el primer renglón: El encabezado con los títulos de las columnas
    escritor.WriteLine("Nombre del Archivo,Tamaño (KB),Fecha de Última Modificación");

    // 4. Recorremos tus archivos e inyectamos los datos renglón por renglón
    foreach (string archivo in archivos)
    {
        FileInfo infoArchivo = new FileInfo(archivo);
        
        string nombre = infoArchivo.Name;
        double tamanioKB = infoArchivo.Length / 1024.0;
        
        // Obtenemos la fecha de última modificación (como pide el TP)
        string fechaModificacion = infoArchivo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Armamos el renglón separando los datos con COMAS (eso lo convierte en formato CSV)
        string renglonCsv = $"{nombre},{tamanioKB:F2},{fechaModificacion}";

        // ¡Bombazo al archivo! Escribe esta línea en el disco duro
        escritor.WriteLine(renglonCsv);
    }
}

Console.WriteLine($"\n¡Éxito! Se generó el reporte en: {rutaArchivoCsv}");