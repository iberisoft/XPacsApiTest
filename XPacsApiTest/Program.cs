using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace XPacsApiTest
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new($"http://159.69.14.215:5201/api/v1/")
            };
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", args[0]);

            var startDate = DateTime.Today.AddYears(-5);
            var endDate = DateTime.Today;
            var studies = await httpClient.GetFromJsonAsync<Study[]>($"study?StartStudyDateTime={startDate:yyyy-MM-dd}&EndStudyDateTime={endDate:yyyy-MM-dd}");
            foreach (var study in studies)
            {
                Console.WriteLine($"=== Study #{study.Id} ===");

                var files = await httpClient.GetFromJsonAsync<File[]>($"study/{study.Id}/file");
                foreach (var file in files)
                {
                    Console.WriteLine($"=== File #{file.Id} ===");

                    var tags = await httpClient.GetStringAsync($"file/{file.Id}/tag");
                    Console.WriteLine(tags);

                    var data = await httpClient.GetByteArrayAsync($"file/{file.Id}/data?jpeg=true");
                    var fileName = $"{file.Id}.jpg";
                    System.IO.File.WriteAllBytes(fileName, data);
                    Console.WriteLine($"{fileName} saved");
                }
            }
        }
    }
}
