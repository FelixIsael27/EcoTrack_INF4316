using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    public interface IAuthService
    {
        bool EstaAutenticado { get; }
        string NombreUsuario { get; }
        bool IniciarSesion(string correo, string clave);
        void CerrarSesion();
    }
}
