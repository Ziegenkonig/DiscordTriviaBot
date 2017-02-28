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

        //Constructor - also includes all command instantiation
        public TriviaBot()
        {
            //Creating DiscordClient object
            client = new DiscordClient();

            //Assigning Callbacks
            client.JoinedServer += JoinedServerCallback;
            client.MessageReceived += MessageRecievedCallback;

            //Telling the program that client is using a CommandService
            client.UsingCommands(command =>
            {
                command.PrefixChar = '!';
                command.HelpMode = HelpMode.Public;
                command.ErrorHandler = CommandErroredCallback;
                command.ExecuteHandler = CommandExecutedCallback;
            });

            //Hooking up our commands object to our client's CommandService 
            commands = client.GetService<CommandService>();

            //!trivia - Display all commands and their descriptions
            commands.CreateCommand("trivia")
                .Description("Lists all commands provided by Trivia Bot.")
                .Do(async (e) => { await CommandCreationCallback(e); });
            //!greet - Greets the user, because this bot is passive aggressive not outright rude
            commands.CreateCommand("greet")
                .Alias("gr", "hello", "hi")
                .Description("Greets the plebian who calls for it")
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

        //Called when each command is created in the constructor
        private async Task CommandCreationCallback(CommandEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("");
            await e.Server.DefaultChannel.SendMessage("Description: " + e.Command.Description);   
        }
        //Called when a command fails to execute
        private async void CommandErroredCallback(object s, CommandErrorEventArgs e)
        {
            if (!e.Message.IsAuthor)
                await e.Server.DefaultChannel.SendMessage("!" + e.Command.Text + " failed execution with error.");
        }
        //Called whenever a command is executed by a user
        private async void CommandExecutedCallback(object s, CommandEventArgs e)
        {
            //Switch statement that handles all of the logic for our commands
            switch (e.Command.Text) {
                case "greet": //!greet
                    await e.Server.DefaultChannel.SendMessage($"{e.Command.Text}");
                    await e.Server.DefaultChannel.SendMessage($"Hello {e.User.Name}! Riddle me this!");
                    break;
                case "trivia": //!trivia
                    String helptext = "```";
                    foreach (Command command in commands.AllCommands)
                        helptext = helptext + $"!{command.Text} - {command.Description}\n";
                    helptext = helptext + "```";
                    await e.Server.DefaultChannel.SendMessage(helptext);
                    break;
                case "help": //!help
                    break;
                default: //this should probably never be run
                    await e.Server.DefaultChannel.SendMessage("!" + e.Command.Text + " was executed.");
                    break;
            }
        }

        private async void MessageRecievedCallback(object s, MessageEventArgs e)
        {
            // Bot can recieve its own messages, 
            // therefore we only want to check non-bot messages.
            //if(!e.Message.IsAuthor)
            //    await e.Server.DefaultChannel.SendMessage("Testing");
        }

        private async void JoinedServerCallback(object s, ServerEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage("Type !trivia for a list of commands.");
        }
    }
}
