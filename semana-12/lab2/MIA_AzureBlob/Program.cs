using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

string? connectionString =
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

const string containerName = "mia-archivos";

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("ERROR: No se encontró la Connection String.");
    Console.WriteLine("Configure la variable AZURE_STORAGE_CONNECTION_STRING.");
    return;
}

BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobContainerClient containerClient =
    blobServiceClient.GetBlobContainerClient(containerName);

bool salir = false;

while (!salir)
{
    Console.Clear();

    Console.WriteLine("=================================");
    Console.WriteLine("     MIA - AZURE BLOB STORAGE");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Subir archivo");
    Console.WriteLine("2. Listar archivos");
    Console.WriteLine("3. Descargar archivo");
    Console.WriteLine("4. Eliminar archivo");
    Console.WriteLine("5. Salir");
    Console.WriteLine("=================================");
    Console.Write("Seleccione una opción: ");

    string? opcion = Console.ReadLine();

    Console.WriteLine();

    try
    {
        switch (opcion)
        {
            case "1":
                await SubirArchivo(containerClient);
                break;

            case "2":
                await ListarArchivos(containerClient);
                break;

            case "3":
                await DescargarArchivo(containerClient);
                break;

            case "4":
                await EliminarArchivo(containerClient);
                break;

            case "5":
                salir = true;
                Console.WriteLine("Programa finalizado.");
                break;

            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine("Ocurrió un error:");
        Console.WriteLine(ex.Message);
    }

    if (!salir)
    {
        Console.WriteLine();
        Console.WriteLine("Presione una tecla para regresar al menú...");
        Console.ReadKey();
    }
}

static async Task SubirArchivo(BlobContainerClient containerClient)
{
    Console.Write("Ingrese la ruta completa del archivo: ");
    string? ruta = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(ruta))
    {
        Console.WriteLine("Debe ingresar una ruta.");
        return;
    }

    ruta = ruta.Trim('"');

    if (!File.Exists(ruta))
    {
        Console.WriteLine("El archivo no existe.");
        return;
    }

    string nombreArchivo = Path.GetFileName(ruta);

    BlobClient blobClient =
        containerClient.GetBlobClient(nombreArchivo);

    await blobClient.UploadAsync(ruta, overwrite: true);

    Console.WriteLine();
    Console.WriteLine($"Archivo '{nombreArchivo}' subido correctamente.");
}

static async Task ListarArchivos(BlobContainerClient containerClient)
{
    Console.WriteLine("ARCHIVOS EN AZURE");
    Console.WriteLine();
    Console.WriteLine("{0,-35} {1,15}", "Nombre", "Tamaño");
    Console.WriteLine(new string('-', 52));

    bool hayArchivos = false;

    await foreach (BlobItem blob in containerClient.GetBlobsAsync())
    {
        hayArchivos = true;

        long tamano = blob.Properties.ContentLength ?? 0;

        Console.WriteLine(
            "{0,-35} {1,12} bytes",
            blob.Name,
            tamano
        );
    }

    if (!hayArchivos)
    {
        Console.WriteLine("No hay archivos en el container.");
    }
}

static async Task DescargarArchivo(BlobContainerClient containerClient)
{
    Console.Write("Ingrese el nombre del blob: ");
    string? nombreBlob = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Debe ingresar un nombre.");
        return;
    }

    BlobClient blobClient =
        containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine("El archivo no existe en Azure.");
        return;
    }

    Console.Write("Ingrese la carpeta donde desea guardarlo: ");
    string? carpetaDestino = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(carpetaDestino))
    {
        Console.WriteLine("Debe ingresar una carpeta.");
        return;
    }

    carpetaDestino = carpetaDestino.Trim('"');

    if (!Directory.Exists(carpetaDestino))
    {
        Console.WriteLine("La carpeta de destino no existe.");
        return;
    }

    string rutaDestino =
        Path.Combine(carpetaDestino, nombreBlob);

    await blobClient.DownloadToAsync(rutaDestino);

    Console.WriteLine();
    Console.WriteLine("Archivo descargado correctamente.");
    Console.WriteLine($"Ubicación: {rutaDestino}");
}

static async Task EliminarArchivo(BlobContainerClient containerClient)
{
    Console.Write("Ingrese el nombre del blob a eliminar: ");
    string? nombreBlob = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Debe ingresar un nombre.");
        return;
    }

    BlobClient blobClient =
        containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine("El archivo no existe en Azure.");
        return;
    }

    Console.Write($"¿Está seguro de eliminar '{nombreBlob}'? (S/N): ");
    string? confirmacion = Console.ReadLine();

    if (!string.Equals(confirmacion, "S",
        StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Eliminación cancelada.");
        return;
    }

    await blobClient.DeleteIfExistsAsync(
        DeleteSnapshotsOption.IncludeSnapshots
    );

    Console.WriteLine("Archivo eliminado correctamente.");
}