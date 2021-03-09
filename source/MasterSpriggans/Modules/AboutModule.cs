using System.Threading.Tasks;
using Discord.Commands;

namespace MasterSpriggans.Modules
{
    public class AboutModule : BaseModule
    {
        [Command("about")]
        public async Task AboutAsync()
        {
            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = "Master Spriggans";
            embed.Description = "The provider of callouts to all those in need.";
            embed.AddField("About Me", "I'm a discord bot. I'm not ready for use yet, but I will be soon.");
            embed.ThumbnailUrl = _renaThumbnail;
            await ReplyAsync(null, false, embed.Build());

        }
    }
}