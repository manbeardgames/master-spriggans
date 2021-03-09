using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MasterSpriggans.Utils;

namespace MasterSpriggans.Handlers
{
    public class DiscordCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public DiscordCommandHandler(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = client;

            CommandServiceConfig config = new CommandServiceConfig();
            config.CaseSensitiveCommands = false;
            config.DefaultRunMode = RunMode.Async;
            config.IgnoreExtraArgs = true;
            config.SeparatorChar = ' ';

            _commandService = new CommandService(config);
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _commandService.CommandExecuted += OnCommandExecutedAsync;
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            //  We have access ot the information of the command executed,
            //  the context of the command, and the result returned from the
            //  execution in this event.

            //  We can tell the user what went wrong
            if(!string.IsNullOrEmpty(result?.ErrorReason))
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }

            //  ... or even log the result (themethod used should fit into 
            //  your existing log handler)
            var commandName = command.IsSpecified ? command.Value.Name : "A command";
            Logger.Message($"CommandExecution {commandName} was executed");

            if(!string.IsNullOrEmpty(result?.ErrorReason))
            {
                Logger.Error(result.ErrorReason);
            }
        }

        public async Task HandleCommandsAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) { return; }
            if (message.Author.IsBot) { return; }

            //  Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            if (!message.HasStringPrefix("!spriggan ", ref argPos)) { return; }

            //  Create a WebSocket-based command context based on the message
            SocketCommandContext context = new SocketCommandContext(_client, message);

            //  Execute the command with the command context we just created, along with
            //  the service provider for precondition checks.
            //
            //  Keep in mind that result does not indicate a return value
            //  rather an object stating if the command executed successfully
            using (context.Channel.EnterTypingState())
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
            }
        }
    }
}