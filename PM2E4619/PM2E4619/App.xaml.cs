using PM2E4619.Controller;
using System;
using System.IO;
using Xamarin.Forms;

namespace PM2E4619
{
    public partial class App : Application
    {

        static DatosController database;

        public static DatosController Database
        {
            get
            {
                if (database == null)
                {
                    database = new DatosController(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dbDatos.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
