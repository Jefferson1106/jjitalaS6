using jjitalaS6.Services;

namespace jjitalaS6.Views;

public partial class vAgregarEstudiante : ContentPage
{
    public vAgregarEstudiante()
    {
        InitializeComponent();
    }

    private async void btnCancelar_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void btnAgregar_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
            string.IsNullOrWhiteSpace(txtApellido.Text) ||
            !int.TryParse(txtEdad.Text, out int edad) || edad <= 0 || edad > 120)
        {
            await DisplayAlertAsync("Alerta", "Ingrese nombre, apellido y una edad entre 1 y 120 años.", "OK");
            return;
        }

        btnAgregar.IsEnabled = btnCancelar.IsEnabled = false;
        try
        {
            await EstudianteService.GuardarAsync(HttpMethod.Post, new()
            {
                ["nombre"] = txtNombre.Text.Trim(),
                ["apellido"] = txtApellido.Text.Trim(),
                ["edad"] = edad.ToString()
            });
            await DisplayAlertAsync("Correcto", "Estudiante agregado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo agregar: " + ex.Message, "OK");
        }
        finally
        {
            btnAgregar.IsEnabled = btnCancelar.IsEnabled = true;
        }
    }
}
