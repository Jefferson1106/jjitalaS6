using jjitalaS6.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace jjitalaS6.Views;

public partial class vEstudiante : ContentView
{
    private const string Url = "http://192.168.1.37/ws_estudiantes/estudiante_ws.php";
    private readonly HttpClient _client = new HttpClient();
    private ObservableCollection<Estudiante> _estudiante;

    public vEstudiante()
    {
        InitializeComponent();
        MostraEstudiate();
    }
    public async void MostraEstudiate()
    {
        var content = await _client.GetStringAsync(Url);
        List<Models.Estudiante> estudiante = JsonConvert.DeserializeObject<List<Models.Estudiante>>(content);
        _estudiante = new ObservableCollection<Models.Estudiante>(estudiante);
        listaEstudiante.ItemsSource = _estudiante;
    }
}