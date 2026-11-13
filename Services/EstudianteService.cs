using jjitalaS6.Models;
using Newtonsoft.Json;

namespace jjitalaS6.Services;

public static class EstudianteService
{
    // Windows accede al XAMPP de esta PC sin depender de la IP del Wi-Fi.
#if WINDOWS
    public const string Url = "http://127.0.0.1/ws_estudiantes/estudiante_ws.php";
#else
    // En dispositivos físicos, usar la IP Wi-Fi de la PC que ejecuta XAMPP.
    public const string Url = "http://192.168.68.111/ws_estudiantes/estudiante_ws.php";
#endif
    private static readonly HttpClient Cliente = new() { Timeout = TimeSpan.FromSeconds(30) };

    public static async Task<List<Estudiante>> ListarAsync()
    {
        var contenido = await Cliente.GetStringAsync(Url);
        return JsonConvert.DeserializeObject<List<Estudiante>>(contenido) ?? new();
    }

    public static async Task GuardarAsync(HttpMethod metodo, Dictionary<string, string> parametros)
    {
        using var solicitud = new HttpRequestMessage(metodo, Url)
        {
            Content = new FormUrlEncodedContent(parametros)
        };
        using var respuesta = await Cliente.SendAsync(solicitud);
        respuesta.EnsureSuccessStatusCode();
    }
}
