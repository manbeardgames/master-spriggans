using Discord.Commands;
using Discord.WebSocket;
using MasterSpriggans.Data;
using MasterSpriggans.Data.Models;
using MasterSpriggans.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XIVApiLib;
using XIVApiLib.Models;
using XIVApiLib.Models.Responses;
using XIVApiLib.Utilities;

namespace MasterSpriggans.Modules
{
    [Group("whois")]
    public class WhoIsModule : BaseModule
    {
        private readonly XIVApi _xivApi;
        private readonly MasterSpriggansDatabaseContext _db;
        private readonly DiscordSocketClient _client;

        public WhoIsModule(DiscordSocketClient client, XIVApi xivApi, MasterSpriggansDatabaseContext db)
        {
            _xivApi = xivApi;
            _db = db;
            _client = client;
        }


        [Command]
        [Priority(1)]
        public async Task WhoIsAsync(string lodestoneID)
        {
            if (string.IsNullOrWhiteSpace(lodestoneID))
            {
                await ErrorAsync("Invalid parameters");
                return;
            }

            CharacterResponseModel response = await _xivApi.CharacterByLodestoneID(lodestoneID);

            if (response.Error.HasValue)
            {
                await ErrorAsync(response.Message);
                return;
            }

            CharacterModel character = response.Character;

            XIVTitleModel title = await _db.XIVTitles.FirstOrDefaultAsync(t => t.ID == character.Title);

            using (var stream = await CreateCharacterCard(response.Character, response.FreeCompany))
            {
                //await Context.Channel.SendFileAsync(@"C:\Users\Dart\Desktop\saved_card.png", $"{character.Name}_Character_Card.png");
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, $"{character.Name}_Character_Card.png");
            }


            //EmbedBuilder embed = new Discord.EmbedBuilder();
            //embed.Title = $"{character.Name}, {title.Name}";
            //embed.Description = $@"[View on lodestone](https://na.finalfnatasyxiv.com/lodestone/character/{character.ID})";
            //embed.ThumbnailUrl = character.Avatar;
            //embed.ImageUrl = character.Portrait;
            //embed.AddField("Character Info", $"**Job**: {character.ActiveClassJob.UnlockedState.Name} [lvl {character.ActiveClassJob.Level}]");
            //await ReplyAsync(null, false, embed.Build());

        }

        [Command]
        [Priority(2)]
        public async Task WhoIsAsync(string server, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                await ErrorAsync("`server` cannot be blank");
                return;
            }


            if (string.IsNullOrWhiteSpace(firstName))
            {
                await ErrorAsync("`First Name` cannot be blank");
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                await ErrorAsync("`Last Name` cannot be blank");
                return;
            }


            CharacterResponseModel response = await _xivApi.CharacterByName(server, firstName, lastName);

            if(response.Results != null && response.Results.Count > 0)
            {
                await WhoIsAsync(response.Results[0].ID.ToString());
                return;
            }
            else
            {
                await ErrorAsync("Unable to locate that character by server and name given");
            }

        }

        private async Task EmbedCharacterAsync(CharacterResponseModel xivApiResponse)
        {
            if (xivApiResponse.Error.HasValue)
            {
                await ErrorAsync(xivApiResponse.Message);
                return;
            }

            CharacterModel character = xivApiResponse.Character;


            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = character.Name;
            embed.ImageUrl = character.Portrait;
            embed.ThumbnailUrl = character.Avatar;
            await ReplyAsync(null, false, embed.Build());
        }

        private async Task ErrorAsync(string message)
        {
            Discord.EmbedBuilder embed = new Discord.EmbedBuilder();
            embed.Title = "whois Command Error";
            embed.Description = "An error has occurred trying to execute your command.";
            embed.AddField("Error Message", message);
            await ReplyAsync(null, false, embed.Build());
        }

        private async Task<MemoryStream> CreateCharacterCard(CharacterModel character, FreeCompanyModel freeCompany)
        {
            string cardFilePath = Path.Combine(Environment.CurrentDirectory, "Images/character_card.png");
            if (!File.Exists(cardFilePath))
            {
                Logger.Error($"Unabled to located card file. Looked for it at {cardFilePath}");
                return null;
            }

            XIVTitleModel title = await _db.XIVTitles.FirstOrDefaultAsync(t => t.ID == character.Title);
            ClassJobIndex activeJob = (ClassJobIndex)character.ActiveClassJob.JobID;

            using (var card = Bitmap.FromFile(cardFilePath))
            {
                using (var g = Graphics.FromImage(card))
                {
                    Font nameFont = new Font("Russo One", 40, GraphicsUnit.Pixel);
                    Font jobFont = new Font("Russo One", 30, GraphicsUnit.Pixel);
                    Font baseFont = new Font("Russo One", 20, GraphicsUnit.Pixel);


                    SolidBrush whiteBrush = new SolidBrush(System.Drawing.Color.White);
                    SolidBrush goldBrush = new SolidBrush(System.Drawing.Color.FromArgb(148, 134, 90));

                    StringFormat centerCenterFormat = new StringFormat();
                    centerCenterFormat.Alignment = StringAlignment.Center;
                    centerCenterFormat.LineAlignment = StringAlignment.Center;

                    StringFormat leftCenterFormat = new StringFormat();
                    leftCenterFormat.Alignment = StringAlignment.Near;
                    leftCenterFormat.LineAlignment = StringAlignment.Center;

                    StringFormat leftTopFormat = new StringFormat();
                    leftTopFormat.Alignment = StringAlignment.Near;
                    leftTopFormat.LineAlignment = StringAlignment.Near;

                    StringFormat leftBottomFormat = new StringFormat();
                    leftBottomFormat.Alignment = StringAlignment.Near;
                    leftBottomFormat.LineAlignment = StringAlignment.Far;

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(character.Portrait);

                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                MemoryStream tempStream = new MemoryStream();
                                await stream.CopyToAsync(tempStream);
                                tempStream.Position = 0;

                                using (var portrait = Bitmap.FromStream(tempStream))
                                {
                                    g.DrawImage(portrait, new Rectangle(14, 14, 640, 873));
                                }
                            }
                        }
                    }

                    string portraitBorderFilePath = Path.Combine(Environment.CurrentDirectory, "Images/portrait_border.png");

                    if (File.Exists(portraitBorderFilePath))
                    {
                        using (var jobIcon = Bitmap.FromFile(portraitBorderFilePath))
                        {
                            g.DrawImage(jobIcon, 14, 14, 640, 640);
                        }
                    }

                    //  Title
                    if (title != null)
                    {
                        g.DrawString(title.Name, baseFont, whiteBrush, new Rectangle(714, 32, 542, 63), leftTopFormat);
                    }

                    //  Name
                    g.DrawString(character.Name, nameFont, goldBrush, new Rectangle(709, 50, 542, 45), leftTopFormat);

                    //  Server
                    g.DrawString(character.Server, baseFont, whiteBrush, new Rectangle(714, 132, 542, 34), leftCenterFormat);

                    //  Free Company
                    if (freeCompany != null)
                    {
                        g.DrawString($"{freeCompany.Name} <{freeCompany.Tag}>", baseFont, whiteBrush, new Rectangle(714, 201, 542, 34), leftCenterFormat);
                    }
                    else
                    {
                        g.DrawString("-", baseFont, whiteBrush, new Rectangle(714, 201, 542, 34), leftCenterFormat);
                    }

                    string jobIconFilePath = ClassJobUtilities.ClassJobToAbbr(activeJob);

                    if (!string.IsNullOrEmpty(jobIconFilePath))
                    {
                        jobIconFilePath = Path.Combine(Environment.CurrentDirectory, $"Images/Icons/{jobIconFilePath}.png");

                        if (File.Exists(jobIconFilePath))
                        {
                            using (var jobIcon = Bitmap.FromFile(jobIconFilePath))
                            {
                                g.DrawImage(jobIcon, 713, 275, 24, 24);
                            }
                        }
                    }

                    //  Active Job Level
                    g.DrawString($"Level {character.ActiveClassJob.Level}", baseFont, whiteBrush, new Rectangle(739, 270, 120, 34), leftCenterFormat);

                    //  Active Job
                    g.DrawString(ClassJobUtilities.ClassJobToName(activeJob), baseFont, goldBrush, new Rectangle(835, 270, 169, 34), leftCenterFormat);

                    //  Paladin
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Paladin).Level}", jobFont, whiteBrush, new Rectangle(710, 388, 55, 55), centerCenterFormat);

                    //  Warrior
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Warrior).Level}", jobFont, whiteBrush, new Rectangle(780, 388, 55, 55), centerCenterFormat);

                    //  Dark Knight
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.DarkKnight).Level}", jobFont, whiteBrush, new Rectangle(850, 388, 55, 55), centerCenterFormat);

                    //  Gunbreaker
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Gunbreaker).Level}", jobFont, whiteBrush, new Rectangle(920, 388, 55, 55), centerCenterFormat);

                    //  Whitemage
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.WhiteMage).Level}", jobFont, whiteBrush, new Rectangle(1060, 388, 55, 55), centerCenterFormat);

                    //  Scholar
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Scholar).Level}", jobFont, whiteBrush, new Rectangle(1130, 388, 55, 55), centerCenterFormat);

                    //  Astro
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Astrologian).Level}", jobFont, whiteBrush, new Rectangle(1200, 388, 55, 55), centerCenterFormat);

                    //  Monk
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Monk).Level}", jobFont, whiteBrush, new Rectangle(710, 498, 55, 55), centerCenterFormat);

                    //  Dragoon
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Dragoon).Level}", jobFont, whiteBrush, new Rectangle(780, 498, 55, 55), centerCenterFormat);

                    //  Ninja
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Ninja).Level}", jobFont, whiteBrush, new Rectangle(850, 498, 55, 55), centerCenterFormat);
                    //  Samurai
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Samurai).Level}", jobFont, whiteBrush, new Rectangle(920, 498, 55, 55), centerCenterFormat);

                    //  Bard
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Bard).Level}", jobFont, whiteBrush, new Rectangle(1060, 498, 55, 55), centerCenterFormat);

                    //  Machinist
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Machinist).Level}", jobFont, whiteBrush, new Rectangle(1130, 498, 55, 55), centerCenterFormat);

                    //  Dancer
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Dancer).Level}", jobFont, whiteBrush, new Rectangle(1200, 498, 55, 55), centerCenterFormat);

                    //  Black Mage
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.BlackMage).Level}", jobFont, whiteBrush, new Rectangle(710, 608, 55, 55), centerCenterFormat);

                    //  Summoner
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Summoner).Level}", jobFont, whiteBrush, new Rectangle(780, 608, 55, 55), centerCenterFormat);

                    //  Red Mage
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.RedMage).Level}", jobFont, whiteBrush, new Rectangle(850, 608, 55, 55), centerCenterFormat);

                    //  Blue Mage
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.BlueMage).Level}", jobFont, whiteBrush, new Rectangle(1060, 608, 55, 55), centerCenterFormat);

                    //  Carpenter
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Carpenter).Level}", jobFont, whiteBrush, new Rectangle(710, 718, 55, 55), centerCenterFormat);

                    //  Blacksmith
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Blacksmith).Level}", jobFont, whiteBrush, new Rectangle(780, 718, 55, 55), centerCenterFormat);

                    //  Armorer
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Armorer).Level}", jobFont, whiteBrush, new Rectangle(850, 718, 55, 55), centerCenterFormat);

                    //  Goldsmith
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Goldsmith).Level}", jobFont, whiteBrush, new Rectangle(920, 718, 55, 55), centerCenterFormat);

                    //  Leatherworker
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Leatherworker).Level}", jobFont, whiteBrush, new Rectangle(990, 718, 55, 55), centerCenterFormat);

                    //  Weaver
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Weaver).Level}", jobFont, whiteBrush, new Rectangle(1060, 718, 55, 55), centerCenterFormat);

                    //  Alchemist
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Alchemist).Level}", jobFont, whiteBrush, new Rectangle(1130, 718, 55, 55), centerCenterFormat);

                    //  Culinarian
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Culinarian).Level}", jobFont, whiteBrush, new Rectangle(1200, 718, 55, 55), centerCenterFormat);

                    //  Miner
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Miner).Level}", jobFont, whiteBrush, new Rectangle(710, 828, 55, 55), centerCenterFormat);

                    //  Botanist
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Botanist).Level}", jobFont, whiteBrush, new Rectangle(780, 828, 55, 55), centerCenterFormat);

                    //  Fisher
                    g.DrawString($"{character.ClassJobs.First(x => x.JobID == (int)ClassJobIndex.Fisher).Level}", jobFont, whiteBrush, new Rectangle(850, 828, 55, 55), centerCenterFormat);
                }

                MemoryStream memoryStream = new MemoryStream();

                card.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                //card.Save(@"C:\Users\Dart\Desktop\saved_card.png");
                return memoryStream;


            }
        }
    }
}