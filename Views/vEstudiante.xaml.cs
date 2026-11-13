
using jjitalaS6.Models;
using jjitalaS6.Services;
using System.Collections.ObjectModel;

namespace jjitalaS6.Views;

public partial class vEstudiante : ContentPage
{
    private readonly ObservableCollection<Estudiante> _estudiante = new();

    public vEstudiante()
    {
        InitializeComponent();

        listaEstudiante.ItemsSource = _estudiante;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await MostrarEstudiante();
    }

    public async Task MostrarEstudiante()
    {
        try
        {
            var mostrar = await EstudianteService.ListarAsync();

            // Actualizar colección
            _estudiante.Clear();

            foreach (var estudiante in mostrar)
            {
                _estudiante.Add(estudiante);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                "No se pudieron cargar los estudiantes: " + ex.Message,
                "Aceptar"
            );
        }
    }

    private async void btnAgregar_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new vAgregarEstudiante());
    }

    private async void listaEstudiante_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        Estudiante? estudianteSeleccionado =
            e.CurrentSelection[0] as Estudiante;

        if (estudianteSeleccionado == null)
            return;

        // Limpiar la selección
        listaEstudiante.SelectedItem = null;

        // Abrir formulario con los datos seleccionados
        await Navigation.PushAsync(
            new vActualizarEliminar(estudianteSeleccionado)
        );

    }
}
