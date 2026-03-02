using System.Globalization;

namespace MauiAppMinhasCompras
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Define cultura padrão do app como pt-BR
            CultureInfo culture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            MainPage = new AppShell();

            // MainPage = new AppShell();
            MainPage = new NavigationPage(new Views.ListaProduto());
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.Width = 400;
            window.Height = 800;

            return window;// retorno da mesma instancia configurada
        }
    }
}
