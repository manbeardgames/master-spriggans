using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MasterSpriggans.Utils;

namespace MasterSpriggans.Handlers
{
    public class DiscordEventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordCommandHandler _commandHandler;

        public DiscordEventHandler(DiscordSocketClient client, DiscordCommandHandler commandHandler)
        {
            _client = client;
            _commandHandler = commandHandler;
        }

        public void InitializeEvents()
        {
            _client.Log += OnClientLog;
            _client.MessageReceived += OnClientMessageReceived;
        }

        private Task OnClientLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Logger.Error(log.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.Warning(log.Message);
                    break;
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                default:
                    Logger.Message(log.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task OnClientMessageReceived(SocketMessage message) => await _commandHandler.HandleCommandsAsync(message);
    }
}