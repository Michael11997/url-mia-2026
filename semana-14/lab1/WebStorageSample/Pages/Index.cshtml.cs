using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebStorageSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<string> Archivos { get; set; } = new List<string>();

        public string Mensaje { get; set; }

        [BindProperty]
        public IFormFile Archivo { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await CargarArchivos();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Archivo == null || Archivo.Length == 0)
            {
                Mensaje = "Seleccione un archivo para subir.";
                await CargarArchivos();
                return Page();
            }

            string endpoint =
                Environment.GetEnvironmentVariable(
                    Const.ENDPOINT_ENV_KEY);

            string containerName =
                Environment.GetEnvironmentVariable(
                    Const.CONTAINER_ENV_KEY);

            string nombreSeguro =
                Path.GetFileName(Archivo.FileName);

            using Stream stream =
                Archivo.OpenReadStream();

            await StorageHelper.UploadFile(
                endpoint,
                containerName,
                nombreSeguro,
                stream);

            Mensaje =
                $"Archivo '{nombreSeguro}' subido correctamente.";

            await CargarArchivos();

            return Page();
        }

        private async Task CargarArchivos()
        {
            string endpoint =
                Environment.GetEnvironmentVariable(
                    Const.ENDPOINT_ENV_KEY);

            string containerName =
                Environment.GetEnvironmentVariable(
                    Const.CONTAINER_ENV_KEY);

            Archivos =
                await StorageHelper.ListBlobs(
                    endpoint,
                    containerName);
        }
    }
}