using Microsoft.Extensions.DependencyInjection;

namespace EcoTrack
{
    public partial class App : Application
    {
        private readonly IServiceProvider _servicios;

        public App(IServiceProvider servicios)
        {
            InitializeComponent();
            _servicios = servicios;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = _servicios.GetRequiredService<AppShell>();
            return new Window(shell) { Title = "EcoTrack" };
        }
    }
}