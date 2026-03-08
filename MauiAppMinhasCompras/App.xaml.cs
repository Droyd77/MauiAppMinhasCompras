using System.Globalization;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras
{
    public partial class App : Application
    {

        static SQLiteDatabaseHelper _db;
       
        public static SQLiteDatabaseHelper Db
        {
            get
            {
                if (_db == null)
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                        "banco_sqlite_compras.db3");            
                    
                    _db = new SQLiteDatabaseHelper(path);
                }
                 return _db;

            }
        }




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

            window.Width = 500;
            window.Height = 800;

            return window;// retorno da mesma instancia configurada
        }
    }
}
