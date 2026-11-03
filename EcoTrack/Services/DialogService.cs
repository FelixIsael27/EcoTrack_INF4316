using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    public class DialogService : IDialogService
    {
        public async Task MostrarAlertaAsync(string titulo, string mensaje, string aceptar = "OK")
        {
            if (Shell.Current is not { } pagina)
                return;

            await pagina.DisplayAlertAsync(titulo, mensaje, aceptar);
        }
    }
}