# MIA - Azure Blob Storage

## Objetivo

Desarrollar una aplicación de consola en C# capaz de conectarse a Azure Blob Storage mediante una Connection String y realizar operaciones básicas de manejo de archivos en la nube.

## Tecnologías utilizadas

- C#
- .NET 10
- Azure Blob Storage
- Azure.Storage.Blobs
- Visual Studio Code
- PowerShell

## Configuración de Azure

Se utilizó una cuenta de almacenamiento de Azure llamada:

`miamichael2026`

Dentro de la cuenta se creó un contenedor privado llamado:

`mia-archivos`

El contenedor fue configurado con acceso privado, sin acceso anónimo.

## Arquitectura de la solución

La aplicación utiliza los clientes proporcionados por el SDK de Azure:

- `BlobServiceClient`: permite conectarse al servicio de Azure Blob Storage.
- `BlobContainerClient`: permite trabajar con el contenedor.
- `BlobClient`: permite trabajar con un archivo o blob específico.

La aplicación presenta un menú de consola desde el cual el usuario puede realizar las diferentes operaciones.

## Operaciones

### 1. Subir archivo

Solicita al usuario la ruta de un archivo local.

El programa valida que el archivo exista y luego lo sube al contenedor de Azure Blob Storage.

### 2. Listar archivos

Obtiene todos los blobs almacenados dentro del contenedor y muestra:

- Nombre del archivo
- Tamaño en bytes

### 3. Descargar archivo

Solicita el nombre del blob que se desea descargar.

El programa verifica que exista, solicita una carpeta de destino y descarga el archivo indicando posteriormente la ubicación donde fue guardado.

### 4. Eliminar archivo

Solicita el nombre del blob.

Antes de eliminarlo, valida que exista y solicita confirmación al usuario.

Si el usuario confirma la operación, el archivo es eliminado de Azure Blob Storage.

## Manejo de errores

La aplicación valida diferentes situaciones, entre ellas:

- Archivos locales inexistentes.
- Blobs inexistentes.
- Carpetas de destino inexistentes.
- Opciones inválidas del menú.
- Errores durante la comunicación con Azure.

También se utiliza manejo de excepciones para evitar que la aplicación finalice inesperadamente.

## Seguridad de la Connection String

La Connection String no se almacena directamente dentro del código fuente.

Para proteger la credencial se utiliza una variable de entorno llamada:

`AZURE_STORAGE_CONNECTION_STRING`

De esta manera, la Connection String no queda almacenada dentro del proyecto ni es publicada en GitHub.

## Ejecución del proyecto

Primero se debe configurar la variable de entorno desde PowerShell:

```powershell
$env:AZURE_STORAGE_CONNECTION_STRING="CONNECTION_STRING"