using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Services
{
    public class AudioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AudioService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public List<string> SplitFile(string filePath, int chunkSizeMB)
        {
            const int BytesPerMB = 1024 * 1024;
            int chunkSizeBytes = chunkSizeMB * BytesPerMB;

            var outputFiles = new List<string>();
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var fileExtension = Path.GetExtension(filePath);
            var fileDirectory = Path.GetDirectoryName(filePath);

            using (var reader = new AudioFileReader(filePath))
            {
                var totalBytes = reader.Length;
                var bytesPerSecond = reader.WaveFormat.AverageBytesPerSecond;

                int chunkDurationInSeconds = chunkSizeBytes / bytesPerSecond;
                int totalChunks = (int)Math.Ceiling((double)totalBytes / chunkSizeBytes);

                for (int i = 0; i < totalChunks; i++)
                {
                    var chunkFilePath = Path.Combine(fileDirectory, $"{fileNameWithoutExt}_chunk{i + 1}{fileExtension}");
                    outputFiles.Add(chunkFilePath);

                    using (var writer = new WaveFileWriter(chunkFilePath, reader.WaveFormat))
                    {
                        int bytesToWrite = chunkSizeBytes;
                        byte[] buffer = new byte[1024];
                        while (bytesToWrite > 0 && reader.Position < reader.Length)
                        {
                            int bytesRead = reader.Read(buffer, 0, Math.Min(buffer.Length, bytesToWrite));
                            if (bytesRead == 0) break;
                            writer.Write(buffer, 0, bytesRead);
                            bytesToWrite -= bytesRead;
                        }
                    }
                }
            }

            return outputFiles;
        }

        public async Task<string> TranscribeAudioAsync(string filePath)
        {
            using var form = new MultipartFormDataContent
            {
                { new ByteArrayContent(await File.ReadAllBytesAsync(filePath)), "file", Path.GetFileName(filePath) },
                { new StringContent("whisper-1"), "model" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _apiKey) },
                Content = form
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return responseJson.RootElement.GetProperty("text").GetString();
        }
    }
}
