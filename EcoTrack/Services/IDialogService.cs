using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    /// <summary>Permite que un ViewModel muestre una alerta sin conocer la pantalla.</summary>
    public interface IDialogService
    {
        Task MostrarAlertaAsync(string titulo, string mensaje, string aceptar = "OK");
    }
}
