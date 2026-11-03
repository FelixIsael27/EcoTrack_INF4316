using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoTrack.Models;
using EcoTrack.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcoTrack.ViewsModels
{
    /// <summary>
    /// ViewModel de la pantalla del formulario: guarda los cuatro formularios
    /// (uno por entidad) y muestra el de la categoría elegida.
    /// </summary>
    public partial class ActivityFormViewModel : ObservableObject
    {
        private readonly Dictionary<CategoriaEco, FormularioViewModelBase> _formularios;
        private readonly INavegacionService _navegacion;

        public ActivityFormViewModel(
            TransporteViewModel transporte,
            EnergiaViewModel energia,
            ReciclajeViewModel reciclaje,
            AguaViewModel agua,
            INavegacionService navegacion)
        {
            _navegacion = navegacion;
            _formularios = new Dictionary<CategoriaEco, FormularioViewModelBase>
            {
                [CategoriaEco.Transporte] = transporte,
                [CategoriaEco.Energia] = energia,
                [CategoriaEco.Residuos] = reciclaje,
                [CategoriaEco.Agua] = agua
            };

            Actual = transporte;
        }

        /// <summary>Formulario que se está mostrando (el de la categoría elegida).</summary>
        [ObservableProperty] private FormularioViewModelBase _actual = null!;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EsTransporte))]
        [NotifyPropertyChangedFor(nameof(EsEnergia))]
        [NotifyPropertyChangedFor(nameof(EsResiduos))]
        [NotifyPropertyChangedFor(nameof(EsAgua))]
        private CategoriaEco _categoria = CategoriaEco.Transporte;

        public bool EsTransporte => Categoria == CategoriaEco.Transporte;
        public bool EsEnergia => Categoria == CategoriaEco.Energia;
        public bool EsResiduos => Categoria == CategoriaEco.Residuos;
        public bool EsAgua => Categoria == CategoriaEco.Agua;

        [RelayCommand]
        private void SeleccionarCategoria(string clave)
        {
            if (!Enum.TryParse<CategoriaEco>(clave, out var nueva) || nueva == Categoria)
                return;

            var anterior = Actual;
            var siguiente = _formularios[nueva];

            // El formulario de destino empieza limpio, pero conserva lo común:
            // descripción, fecha, nota y foto.
            siguiente.Limpiar();
            siguiente.ConservarDatosComunesDe(anterior);

            Categoria = nueva;
            Actual = siguiente;
        }

        [RelayCommand]
        private Task CerrarAsync() => _navegacion.VolverAsync();
    }
}
