using ChatGPTBotApp.Services;
using ChatGPTBotApp.Utilities;

namespace ChatGPTBotApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var apiKey = "xxxx";
            var httpClient = ApiHelper.CreateHttpClient(apiKey);

            var chatService = new ChatService(httpClient, apiKey);
            var audioService = new AudioService(httpClient, apiKey);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(chatService, audioService));
        }
    }
}