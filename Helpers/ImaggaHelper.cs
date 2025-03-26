using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using imagein_api.Models;

namespace imagein_api.Helpers
{
    public class ImaggaHelper
    {
        private readonly string apiKey = Environment.GetEnvironmentVariable("IMAGGA_API_KEY");
        private readonly string apiSecret = Environment.GetEnvironmentVariable("IMAGGA_API_SECRET");

        public async Task<string> UploadImageAsync(string filePath)
        {
            var apiKey = Environment.GetEnvironmentVariable("IMAGGA_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("IMAGGA_API_SECRET");

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                Console.WriteLine("API credentials not found.");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist: {filePath}");
                return null;
            }

            var fileExtension = Path.GetExtension(filePath).ToLower();
            string mimeType = fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream" // fallback
            };

            Console.WriteLine($"📸 Uploading file: {filePath}");
            Console.WriteLine($"✅ File exists: {File.Exists(filePath)}");
            Console.WriteLine($"📦 File size: {new FileInfo(filePath).Length} bytes");
            Console.WriteLine($"🧾 Detected MIME type: {mimeType}");

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            // 👇 OPTIONAL: Force Content-Disposition if needed
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"image\"",
                FileName = $"\"{Path.GetFileName(filePath)}\""
            };

            form.Add(fileContent, "image", Path.GetFileName(filePath));

            var response = await client.PostAsync("https://api.imagga.com/v2/uploads", form);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📨 Imagga response status: {response.StatusCode}");
            Console.WriteLine("🧾 Imagga raw response: " + result);

            var json = JObject.Parse(result);

            var status = json["status"]?["type"]?.ToString();
            if (status == "error")
            {
                var errorMsg = json["status"]?["text"]?.ToString();
                Console.WriteLine("❌ Imagga upload error: " + errorMsg);
                return null;
            }

            var uploadId = json["result"]?["upload_id"]?.ToString();
            Console.WriteLine($"✅ Imagga upload_id: {uploadId}");

            return uploadId;
        }
       public async Task<List<TagResult>> GetImageTagsAsync(string uploadId)
        {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response = await client.GetAsync($"https://api.imagga.com/v2/tags?image_upload_id={uploadId}");
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Imagga response: " + result);

            var json = JObject.Parse(result);

            var tags = json["result"]?["tags"]
                ?.Select(tag => new TagResult
                {
                    Tag = tag["tag"]?["en"]?.ToString(),
                    Confidence = tag["confidence"]?.Value<float>() ?? 0
                })
                .ToList();

            return tags ?? new List<TagResult>();
        }
    }
}