using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Threading;
using Xamarin.Forms;
using PM2E4619.Model;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.IO;

namespace PM2E4619.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Editar : ContentPage
    {
        MediaFile foto = null;
        CancellationTokenSource cts;

        public Editar()
        {
            InitializeComponent();
        }

        private async void btnTomarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Message("Error", "No se han otorgado los permisos para suar la camara o la camara no esta disponible.");
                    return;
                }
                foto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
                if (foto != null)
                {
                    txtImagen.Source = ImageSource.FromStream(() =>
                    {
                        return foto.GetStream();
                    });
                }
                else return;
            }
            catch (Exception ex)
            {
                Message("Error", "Error: " + ex.Message.ToString());
            }
        }

        private async void btnUbicacion_Clicked(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    txtLatitud.Text = location.Latitude.ToString();
                    txtLongitud.Text = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Message("Error", "Error: " + fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Message("Error", "Error: " + fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                Message("Error", "Error: " + pEx.Message);
            }
            catch (Exception ex)
            {
                Message("Error", "Error: " + ex.Message);
            }
        }

        private async void btnEditar_Clicked(object sender, EventArgs e)
        {
            if (!EmptyField(txtLatitud) && !EmptyField(txtLongitud) && !EmptyField(txtDescripcion))
            {
                try
                {
                    int resultado = 0;
                    if (foto == null)
                    {
                        var dataEdit = await App.Database.ObtenerDatosId(Convert.ToInt32(txtID.Text));

                        var datos = new Datos
                        {
                            Id = Convert.ToInt16(txtID.Text),
                            latitud = txtLatitud.Text,
                            longitud = txtLongitud.Text,
                            descripcion = txtDescripcion.Text,
                            image = dataEdit.image
                        };
                        resultado = await App.Database.GuardarDatos(datos);
                    } else
                    {
                        var datos = new Datos
                        {
                            Id = Convert.ToInt16(txtID.Text),
                            latitud = txtLatitud.Text,
                            longitud = txtLongitud.Text,
                            descripcion = txtDescripcion.Text,
                            image = foto.Path
                        };
                        resultado = await App.Database.GuardarDatos(datos);
                    }

                    if (resultado > 0)
                    {
                        Message("Informacion", "Los datos se actualizaron exitosamente");
                        await Navigation.PushAsync(new Lista());
                    }
                    else Message("Error", "A ocurrido un error al actualizar los datos");
                }
                catch (Exception ex)
                {
                    Message("Error", "Error: " + ex.Message.ToString());
                }
            }
            else Message("Error", "Debe llenar todos los campos solicitados.");
        }

        private async void btnLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Lista());
        }

        public async void Message(String title, String msg)
        {
            await DisplayAlert(title, msg, "Ok");
        }

        public bool EmptyField(Entry campo)
        {
            return String.IsNullOrEmpty(campo.Text);
        }
    }
}