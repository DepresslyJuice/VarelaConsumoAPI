using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;

class Program
{
    static HttpClient httpClient = new HttpClient();
    static string baseUrl = "https://apicloudutn.azurewebsites.net/api/";

    static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== Menú de la Consola ===");
            Console.WriteLine("1. Participantes");
            Console.WriteLine("2. Inscripciones");
            Console.WriteLine("3. Certificados");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await MenuParticipante();
                    break;
                case "2":
                    await MenuInscripcion();
                    break;
                case "3":
                    await MenuCertificados();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    static async Task MenuParticipante()
    {
        Console.WriteLine("\n--- Participantes ---");
        Console.WriteLine("1. Listar");
        Console.WriteLine("2. Crear");
        Console.Write("Seleccione una acción: ");
        var accion = Console.ReadLine();

        switch (accion)
        {
            case "1":
                await ListarEntidades("Participante");
                break;
            case "2":
                await CrearParticipante();
                break;
            default:
                Console.WriteLine("Acción no válida.");
                break;
        }
    }

    static async Task MenuInscripcion()
    {
        Console.WriteLine("\n--- Inscripciones ---");
        Console.WriteLine("1. Listar");
        Console.WriteLine("2. Cancelar");
        Console.Write("Seleccione una acción: ");
        var accion = Console.ReadLine();

        switch (accion)
        {
            case "1":
                await ListarEntidades("Inscripcion");
                break;
            case "2":
                Console.Write("Ingrese ID de inscripción a cancelar: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                var result = await httpClient.PatchAsync($"{baseUrl}Inscripcion/{id}", null);
                Console.WriteLine($"Resultado: {await result.Content.ReadAsStringAsync()} Inscripción cancelada");
                break;
            default:
                Console.WriteLine("Acción no válida.");
                break;
        }
    }

    static async Task MenuCertificados()
    {
        Console.WriteLine("\n--- Certificado ---");
        Console.WriteLine("1. Listar");
        Console.Write("Seleccione una acción: ");
        var accion = Console.ReadLine();

        switch (accion)
        {
            case "1":
                await ListarEntidades("Certificado");
                break;
            default:
                Console.WriteLine("Acción no válida.");
                break;
        }
    }

    static async Task ListarEntidades(string entidad)
    {
        try
        {
            var response = await httpClient.GetAsync($"{baseUrl}{entidad}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                int i = 1;
                foreach (var item in root.EnumerateArray())
                {
                    Console.WriteLine($"\n{i++}. Datos:");
                    foreach (var prop in item.EnumerateObject())
                    {
                        Console.WriteLine($"{prop.Name}: {prop.Value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task CrearParticipante()
    {
        Console.Write("Nombre: ");
        var nombre = Console.ReadLine();
        Console.Write("Apellido: ");
        var apellido = Console.ReadLine();
        Console.Write("Email: ");
        var email = Console.ReadLine();
        Console.Write("Teléfono: ");
        var telefono = Console.ReadLine();
        Console.Write("Institución: ");
        var institucion = Console.ReadLine();

        var data = new
        {
            nombre = nombre,
            apellido = apellido,
            email = email,
            telefono = telefono,
            institucion = institucion,
            fecha_nacimiento = DateTime.UtcNow
        };

        var jsonData = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{baseUrl}Participante", content);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}
