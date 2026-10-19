using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.Services
{
    public class AuthService : IAuthService
    {
        private const string CorreoValido = "admin@ecotrack.com";
        private const string ClaveValida = "Eco2026!";

        public bool EstaAutenticado { get; private set; }
        public string NombreUsuario { get; private set; } = string.Empty;

        public bool IniciarSesion(string correo, string clave)
        {
            var correcto = string.Equals(correo?.Trim(), CorreoValido, StringComparison.OrdinalIgnoreCase)
                           && clave == ClaveValida;

            EstaAutenticado = correcto;
            NombreUsuario = correcto ? "Admin" : string.Empty;
            return correcto;
        }

        public void CerrarSesion()
        {
            EstaAutenticado = false;
            NombreUsuario = string.Empty;
        }
    }
}
