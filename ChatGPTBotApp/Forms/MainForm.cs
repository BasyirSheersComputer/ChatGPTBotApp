using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ChatGPTBotApp.Services;

namespace ChatGPTBotApp
{
    public partial class MainForm : Form
    {
        private readonly IChatService _chatService;
        private readonly AudioService _audioService;

        public MainForm(IChatService chatService, AudioService audioService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            sendButton.Click += sendButton_Click;
            inputBox.KeyDown += InputBox_KeyDown; // Press Enter to send message
            uploadButton.Click += uploadButton_Click;
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            var userMessage = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            DisplayMessage("\nUser", userMessage);
            inputBox.Clear();

            try
            {
                var response = await _chatService.SendMessageAsync(userMessage);
                DisplayMessage("\nChatGPT", response);
            }
            catch (Exception ex)
            {
                DisplayMessage("\nError", ex.Message);
            }
        }

        // Event handler to send message on Enter key press
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Function to display messages in the chat box
        private void DisplayMessage(string sender, string message)
        {
            chatBox.AppendText($"{sender}: {message}{Environment.NewLine}");
        }

        private async void uploadButton_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "\nAudio files (*.wav;*.mp3;*.mp4;*.m4a;*.webm)|*.wav;*.mp3;*.mp4;*.m4a;*.webm",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            foreach (var filePath in openFileDialog.FileNames)
            {
                try
                {
                    await ProcessAudioFileAsync(filePath);
                }
                catch (Exception ex)
                {
                    DisplayMessage("\nError", $"Failed to process {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }
        }

        private async Task ProcessAudioFileAsync(string filePath)
        {
            using (var httpClient = new HttpClient())
            {
                DisplayMessage("\nSystem", $"Processing file: {Path.GetFileName(filePath)}...");

                var isLargeFile = new FileInfo(filePath).Length > 25 * 1024 * 1024;
                var audioFiles = isLargeFile ? _audioService.SplitFile(filePath, 25) : new List<string> { filePath };

                foreach (var file in audioFiles)
                {
                    var transcription = await _audioService.TranscribeAudioAsync(file);
                    DisplayMessage("\nYou (Transcription)", transcription);
                    if (isLargeFile) File.Delete(file); // Clean up temporary chunks
                }
            }
        }

    }
}
