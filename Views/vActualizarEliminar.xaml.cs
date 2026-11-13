using jjitalaS6.Models;
using jjitalaS6.Services;

namespace jjitalaS6.Views;

public partial class vActualizarEliminar : ContentPage
{
    public vActualizarEliminar(Estudiante datos)
    {
        InitializeComponent();
        txtCodigo.Text = datos.codigo.ToString();
        txtNombre.Text = datos.nombre;
        txtApellido.Text = datos.apellido;
        txtEdad.Text = datos.edad.ToString();
    }

    private async void btnActualizar_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
            string.IsNullOrWhiteSpace(txtApellido.Text) ||
            !int.TryParse(txtEdad.Text, out int edad) || edad <= 0 || edad > 120)
        {
            await DisplayAlertAsync("Alerta", "Ingrese nombre, apellido y una edad entre 1 y 120 años.", "OK");
            return;
        }
        if (!await DisplayAlertAsync("Confirmar", "¿Desea actualizar los datos del estudiante?", "Sí", "No"))
            return;

        await EnviarAsync(HttpMethod.Put, new()
        {
            ["codigo"] = txtCodigo.Text,
            ["nombre"] = txtNombre.Text.Trim(),
            ["apellido"] = txtApellido.Text.Trim(),
            ["edad"] = edad.ToString()
        }, "actualizado");
    }

    private async void btnEliminar_Clicked(object? sender, EventArgs e)
    {
        if (!await DisplayAlertAsync("Eliminar estudiante",
            $"¿Está seguro de eliminar a {txtNombre.Text} {txtApellido.Text}?", "Sí, eliminar", "Cancelar"))
            return;

        await EnviarAsync(HttpMethod.Delete, new() { ["codigo"] = txtCodigo.Text }, "eliminado");
    }

    private async Task EnviarAsync(HttpMethod metodo, Dictionary<string, string> parametros, string accion)
    {
        btnActualizar.IsEnabled = btnEliminar.IsEnabled = false;
        try
        {
            await EstudianteService.GuardarAsync(metodo, parametros);
            await DisplayAlertAsync("Correcto", $"Estudiante {accion} correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo completar la operación: " + ex.Message, "OK");
        }
        finally
        {
            btnActualizar.IsEnabled = btnEliminar.IsEnabled = true;
        }
    }
}
