using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using Xamarin.Forms;
using PM2E4619.Model;
using PM2E4619.View;
using Xamarin.Essentials;
using System.Threading;

namespace PM2E4619
{
    public partial class MainPage : ContentPage
    {
        MediaFile foto = null;
        CancellationTokenSource cts;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnTomarFoto_Clicked(object sender, EventArgs e)
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

        private async void BtnGuardar_Clicked(object sender, EventArgs e)
        {
            if (foto != null)
            {
                if (!EmptyField(txtLatitud) && !EmptyField(txtLongitud) && !EmptyField(txtDescripcion))
                {
                    try
                    {
                        var datos = new Datos
                        {
                            latitud = txtLatitud.Text,
                            longitud = txtLongitud.Text,
                            descripcion = txtDescripcion.Text,
                            image = foto.Path
                        };

                        int resultado = await App.Database.GuardarDatos(datos);

                        if (resultado > 0)
                        {
                            Message("Informacion", "Los datos se guardaron exitosamente");
                            CleanFields();
                        }
                        else Message("Error", "A ocurrido un error al guardar los datos");
                    } catch (Exception ex)
                    {
                        Message("Error", "Error: " + ex.Message.ToString());
                    }
                } else Message("Error", "Debe llenar todos los campos solicitados.");
            } else Message("Error", "Debe tomar una foto");
        }

        private async void BtnLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Lista());
        }

        private async void BtnUbicacion_Clicked(object sender, EventArgs e)
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

        public async void Message(String title, String msg)
        {
            await DisplayAlert(title, msg, "Ok");
        }

        public bool EmptyField(Entry campo)
        {
            return String.IsNullOrEmpty(campo.Text);
        }

        public void CleanFields()
        {
            txtLatitud.Text = "";
            txtLongitud.Text = "";
            txtDescripcion.Text = "";
            txtImagen.Source = "https://cdn.pixabay.com/photo/2014/05/21/19/14/the-question-mark-350168_960_720.png";
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }
    }
}
