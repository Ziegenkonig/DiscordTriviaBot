using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Net;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordTriviaBot
{
    public class TriviaBot
    {
        private DiscordClient client;
        private Timer timer;

        public TriviaBot()
        {
            client = new DiscordClient();
            timer = new Timer();
        }

        public void Commands()
        {

        }  

        public void Execute(string token)
        {
            client.JoinedServer += async (s, e) =>
            {
                await e.Server.DefaultChannel.SendMessage("Type !trivia for a list of commands.");
            };

            client.MessageReceived += async (s, e) =>
            {
                if (e.Message.RawText.Equals("!trivia"))
                    await e.Server.DefaultChannel.SendMessage("");
            };

            client.ExecuteAndWait(async () =>
            {
                await client.Connect(token, TokenType.Bot);
                client.SetGame(new Game("!trivia"));
            });
        }
    }
}
