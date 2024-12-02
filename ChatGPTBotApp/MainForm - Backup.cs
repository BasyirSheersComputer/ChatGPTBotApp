using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ChatGPTBotApp
{
    public partial class Form1 : Form
    {
        private static readonly string apiKey = "xxxx";
        private static readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

        public Form1()
        {
            InitializeComponent();
            sendButton.Click += sendButton_Click;
            inputBox.KeyDown += InputBox_KeyDown; // Press Enter to send message
            uploadButton.Click += uploadButton_Click;
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            var userMessage = inputBox.Text.Trim();
            if (!string.IsNullOrEmpty(userMessage))
            {
                DisplayMessage("You", userMessage);
                inputBox.Clear();
                await SendMessageToChatGPT(userMessage);
            }
        }

        // Event handler to send message on Enter key press
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendButton_Click(this, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Function to display messages in the chat box
        private void DisplayMessage(string sender, string message)
        {
            chatBox.AppendText($"{sender}: {message}\n");
        }

        private List<string> SplitAudioFile(string filePath, int chunkSizeMB)
        {
            const int BytesPerMB = 1024 * 1024;
            int chunkSizeBytes = chunkSizeMB * BytesPerMB;

            var outputFiles = new List<string>();
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var fileExtension = Path.GetExtension(filePath);
            var fileDirectory = Path.GetDirectoryName(filePath);

            using (var reader = new NAudio.Wave.AudioFileReader(filePath))
            {
                var totalBytes = reader.Length;
                var bytesPerSecond = reader.WaveFormat.AverageBytesPerSecond;

                int chunkDurationInSeconds = chunkSizeBytes / bytesPerSecond;
                int totalChunks = (int)Math.Ceiling((double)totalBytes / chunkSizeBytes);

                for (int i = 0; i < totalChunks; i++)
                {
                    var chunkFilePath = Path.Combine(fileDirectory, $"{fileNameWithoutExt}_chunk{i + 1}{fileExtension}");
                    outputFiles.Add(chunkFilePath);

                    using (var writer = new NAudio.Wave.WaveFileWriter(chunkFilePath, reader.WaveFormat))
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

        private async void uploadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio files (*.wav;*.mp3;*.mp4;*.m4a;*.webm)|*.wav;*.mp3;*.mp4;*.m4a;*.webm";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        string fileExtension = Path.GetExtension(filePath).ToLower();

                        if (fileExtension == ".wav" || fileExtension == ".mp3" || fileExtension == ".mp4" ||
                            fileExtension == ".m4a" || fileExtension == ".webm")
                        {
                            // Check if file size exceeds 25MB
                            FileInfo fileInfo = new FileInfo(filePath);
                            if (fileInfo.Length > 25 * 1024 * 1024) // 25MB in bytes
                            {
                                DisplayMessage("System", $"Splitting large audio file: {fileInfo.Name}...");

                                // Split the file into chunks
                                var audioChunks = SplitAudioFile(filePath, 25);

                                // Process each chunk asynchronously
                                foreach (var chunk in audioChunks)
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        string transcription = await TranscribeAudioFile(chunk);
                                        if (!string.IsNullOrEmpty(transcription))
                                        {
                                            DisplayMessage($"You (Chunk {Path.GetFileName(chunk)})", transcription);
                                        }

                                        // Delete chunk after processing to save disk space
                                        File.Delete(chunk);
                                    });
                                }
                            }
                            else
                            {
                                // Process smaller files directly
                                DisplayMessage("System", $"Processing audio file: {fileInfo.Name}...");
                                string transcription = await TranscribeAudioFile(filePath);
                                if (!string.IsNullOrEmpty(transcription))
                                {
                                    DisplayMessage($"You (Transcription for {fileInfo.Name})", transcription);
                                }
                            }
                        }
                        else
                        {
                            DisplayMessage("System", $"Unsupported file type: {filePath}.");
                        }
                    }
                }
            }
        }

        // Function to send user message to OpenAI's ChatGPT API
        private async Task SendMessageToChatGPT(string userMessage)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Define the request payload
                var requestData = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    }
                };

                // Serialize request payload to JSON
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                try
                {
                    // Send POST request
                    var response = await httpClient.PostAsync(apiUrl, jsonContent);
                    response.EnsureSuccessStatusCode();

                    // Parse response
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseContent);
                    var botMessage = responseJson.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    DisplayMessage("ChatGPT", botMessage);
                }
                catch (Exception ex)
                {
                    DisplayMessage("Error", $"Failed to connect: {ex.Message}");
                }
            }
        }

        private async Task<string> TranscribeAudioFile(string filePath)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Create the multipart form data
                using (var form = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(await System.IO.File.ReadAllBytesAsync(filePath));
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
                    form.Add(fileContent, "file", System.IO.Path.GetFileName(filePath));

                    // Add the model type
                    form.Add(new StringContent("whisper-1"), "model");

                    try
                    {
                        // Send the POST request to the Whisper API endpoint
                        var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
                        response.EnsureSuccessStatusCode();

                        // Parse the JSON response
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseJson = JsonDocument.Parse(responseContent);
                        var transcription = responseJson.RootElement.GetProperty("text").GetString();

                        return transcription;
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage("Error", $"Failed to transcribe audio: {ex.Message}");
                        return null;
                    }
                }
            }
        }

    }
}
