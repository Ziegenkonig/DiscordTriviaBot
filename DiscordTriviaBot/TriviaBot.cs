using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordTriviaBot
{
    public class TriviaBot
    {
        private DiscordClient client;
        private CommandService commands;

        public TriviaBot()
        {
            client = new DiscordClient();
            client.JoinedServer += JoinedServerCallback;
            client.MessageReceived += MessageRecievedCallback;
            client.UsingCommands(command =>
            {
                command.PrefixChar = '!';
                command.HelpMode = HelpMode.Public;
                command.ErrorHandler = CommandErroredCallback;
                command.ExecuteHandler = CommandExecutedCallback;
            });

            commands = client.GetService<CommandService>();
            commands.CreateCommand("trivia")
                .Description("Lists all commands provided by Trivia Bot.")
                .Do(async (e) => { await CommandCreationCallback(e); });
        }

        public void Execute(string token)
        {
            client.ExecuteAndWait(async () =>
            {
                await client.Connect(token, TokenType.Bot);
                client.SetGame(new Game("Commands: !trivia"));
            });
        }

        /* ================================ */
        /* Callbacks                        */
        /* ================================ */

        private async Task CommandCreationCallback(CommandEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("Description: " + e.Command.Description);   
        }

        private async void CommandErroredCallback(object s, CommandErrorEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("!" + e.Command.Text + "failed execution with error.");
        }

        private async void CommandExecutedCallback(object s, CommandEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("!" + e.Command.Text + " was executed.");
        }

        private async void MessageRecievedCallback(object s, MessageEventArgs e)
        {
            // Bot can recieve its own messages, 
            // therefore we only want to check non-bot messages.
            if(!e.Message.IsAuthor)
                await e.Server.DefaultChannel.SendMessage("Testing");
        }

        private async void JoinedServerCallback(object s, ServerEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("Type !trivia for a list of commands.");
        }
    }
}
