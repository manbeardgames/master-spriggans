using Discord;
using Discord.Commands;
using MasterSpriggans.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MasterSpriggans.Modules
{
    [Group("poll")]
    public class PollModule : BaseModule
    {
        [Command("create")]
        public async Task CreatePollAsync(string question, params string[] answers)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                await InvalidAsync("You must add a question for the poll");
                return;
            }

            if (answers.Length < 2)
            {
                await InvalidAsync("You must supply at minimum 2 answers");
            }

            if (answers.Length > 10)
            {
                await InvalidAsync("No more than 10 answers are allowed");
            }

            StringBuilder responseBuilder = new StringBuilder();

            for (int i = 0; i < answers.Length; i++)
            {
                responseBuilder.Append(i switch
                {
                    0 => Reactions.Standard.Symbols.one,
                    1 => Reactions.Standard.Symbols.two,
                    2 => Reactions.Standard.Symbols.three,
                    3 => Reactions.Standard.Symbols.four,
                    4 => Reactions.Standard.Symbols.five,
                    5 => Reactions.Standard.Symbols.six,
                    6 => Reactions.Standard.Symbols.seven,
                    7 => Reactions.Standard.Symbols.eight,
                    8 => Reactions.Standard.Symbols.nine,
                    9 => Reactions.Standard.Symbols.keycap_ten,
                    _ => string.Empty
                });

                responseBuilder.AppendLine($" {answers[i]}");
            }

            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = "New Poll";
            embed.Description = $"{Context.User.Username} created the following poll. To Vote in the poll, click the appropriate discord reaction at the bottom of the post";
            embed.AddField("Poll Question", question);
            embed.AddField("Available Responses", responseBuilder.ToString());
            embed.ThumbnailUrl = _renaThumbnail;

            IUserMessage message = await ReplyAsync(null, false, embed.Build());
            IEmote[] reactions = new IEmote[answers.Length];
            for (int i = 0; i < reactions.Length; i++)
            {
                reactions[i] = i switch
                {
                    0 => new Emoji(StandardEmojis.Symbols.KeycapDigitOne),
                    1 => new Emoji(StandardEmojis.Symbols.KeycapDigitTwo),
                    2 => new Emoji(StandardEmojis.Symbols.KeycapDigitThree),
                    3 => new Emoji(StandardEmojis.Symbols.KeycapDigitFour),
                    4 => new Emoji(StandardEmojis.Symbols.KeycapDigitFive),
                    5 => new Emoji(StandardEmojis.Symbols.KeycapDigitSix),
                    6 => new Emoji(StandardEmojis.Symbols.KeycapDigitSeven),
                    7 => new Emoji(StandardEmojis.Symbols.KeycapDigitEight),
                    8 => new Emoji(StandardEmojis.Symbols.KeycapDigitNine),
                    9 => new Emoji(StandardEmojis.Symbols.KeycapDigitTen),
                    _ => throw new Exception("Unknown reaction to add to poll message")
                };
                Logger.Message(reactions[i].ToString());
            }

            await Context.Message.DeleteAsync();

            await message.AddReactionsAsync(reactions);
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = "Poll Help";
            embed.Description = $"To create a poll, use the following command\n`!rena poll <\"Question\"> <\"Response 1\"> <\"Response 2\"> ....[\"Response 10\"]`{Environment.NewLine}{Environment.NewLine}";
            embed.Description += "A minimum of 2 answers must be supplied and no more than 10 answers can be given.";
            embed.ThumbnailUrl = _renaThumbnail;
            await ReplyAsync(null, false, embed.Build());
        }

        private async Task InvalidAsync(string reason)
        {
            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = "Poll error";
            embed.Description = $"{reason}{Environment.NewLine}";
            embed.Description += "For more information use the command `!rena poll help`";
            embed.ThumbnailUrl = _renaThumbnail;
            await ReplyAsync(null, false, embed.Build());
        }
    }
}
