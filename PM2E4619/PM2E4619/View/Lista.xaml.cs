using PM2E4619.Model;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E4619.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lista : ContentPage
    {
        public Lista()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            txtLista.ItemsSource = await App.Database.ObtenerDatos();
        }

        private async void txtLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String opcion = await DisplayActionSheet("Seleccioneuna opcion", "Cancelar", null, "Eliminar", "Editar", "Geolocalizar");

            switch (opcion)
            {
                case "Editar":
                    Datos datos1 = (Datos)e.CurrentSelection.FirstOrDefault();
                    var editar = new Editar();
                    editar.BindingContext = datos1;
                    await Navigation.PushAsync(editar);
                    break;

                case "Eliminar":
                    bool seleccion = await DisplayAlert("Pregunta", "Estas seguro que deseas eliminar", "Si", "No");
                    if (seleccion)
                    {
                        Datos datos2 = (Datos)e.CurrentSelection.FirstOrDefault();
                        int resultado = await App.Database.BorrarDatos(datos2);
                        if (resultado != 0) await DisplayAlert("Aviso", "El empleado se a eliminado exitosamente", "Ok");
                        else await DisplayAlert("Error", "A ocurrido un error al eliminar al empleado", "Ok");
                        txtLista.ItemsSource = await App.Database.ObtenerDatos();
                    }
                    break;
                case "Geolocalizar":
                    Datos datos3 = (Datos)e.CurrentSelection.FirstOrDefault();
                    var mapa = new Mapa();
                    mapa.BindingContext = datos3;
                    await Navigation.PushAsync(mapa);
                    break;

                default:
                    break;
            }
        }

        private async void btnAgregar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}