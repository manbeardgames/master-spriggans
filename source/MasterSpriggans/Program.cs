using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MasterSpriggans.Data;
using MasterSpriggans.Handlers;
using MasterSpriggans.Utils;
using XIVApiLib;
using System.Threading;

namespace MasterSpriggans
{
    public class Program
    {
        //  The Discord client used to communicate with the discord services.
        private static DiscordSocketClient _client;

        //  Service provider used for dependency injection.
        public static IServiceProvider _serviceProvider;

        /// <summary>
        ///     Application entry point, provides immediate async context for 
        ///     entire application from the start.
        /// </summary>
        /// <param name="args">
        ///     Arguments provided from the command line. Currently non supported.
        /// </param>
        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     The main async entry point for the application.
        /// </summary>
        /// <returns>
        ///     Task.Completed.
        /// </returns>
        public async Task MainAsync(string[] args)
        {
            // -------------------------------------------
            //  Get Environment Variables
            // -------------------------------------------
            string xivapiKey = Environment.GetEnvironmentVariable("XIVAPI_KEY", EnvironmentVariableTarget.Machine);
#if DEBUG
            string discordKey = Environment.GetEnvironmentVariable("BABYSPRIGGANS_API_KEY", EnvironmentVariableTarget.Machine);
#else
            string discordKey = Environment.GetEnvironmentVariable("MASTERSPRIGGANS_API_KEY", EnvironmentVariableTarget.Machine);
#endif

            // -------------------------------------------
            //   Initialize services for dependency injection
            // -------------------------------------------

            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<MasterSpriggansDatabaseContext>((o) =>
            {
                o.UseSqlite("Data Source=spriggans.db");
            });

            serviceCollection.AddSingleton<DiscordEventHandler>();
            serviceCollection.AddSingleton<DiscordCommandHandler>();
            serviceCollection.AddSingleton<DiscordSocketClient>();
            serviceCollection.AddSingleton<XIVApi>(new XIVApi(xivapiKey));

            _serviceProvider = serviceCollection.BuildServiceProvider();


            // -------------------------------------------
            //   Ensure the database
            // -------------------------------------------
            Logger.Message("Ensuring database is created");
            MasterSpriggansDatabaseContext context = _serviceProvider.GetRequiredService<MasterSpriggansDatabaseContext>();
#if DEBUG
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("DEBUG: Delete Database? [y/n]");
                string deletedb = Console.ReadLine();
                if (string.Equals("y", deletedb, StringComparison.InvariantCultureIgnoreCase))
                {
                    await context.Database.EnsureDeletedAsync();
                    break;
                }
                else if (string.Equals("n", deletedb, StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
#endif
            if (await context.Database.EnsureCreatedAsync())
            {
                await MasterSpriggansDatabaseContext.SeedDatabase(context, _serviceProvider.GetRequiredService<XIVApi>());
            }

            // -------------------------------------------
            //   Login the client
            // -------------------------------------------
            using (_client = _serviceProvider.GetRequiredService<DiscordSocketClient>())
            {
                _serviceProvider.GetRequiredService<DiscordEventHandler>().InitializeEvents();
                await _serviceProvider.GetRequiredService<DiscordCommandHandler>().InitializeAsync();

                bool loggedIn = false;
                try
                {
                    await _client.LoginAsync(TokenType.Bot, discordKey);
                    loggedIn = true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"The following exception occurred while attempting to log in the discord client. \"{ex.Message}\"");
                }

                if (loggedIn)
                {
                    _client.Ready += OnClientReady;
                    await _client.StartAsync();
                    await Task.Delay(-1);
                }
            }
        }


        private static Task OnClientReady()
        {
#if !DEBUG

            new Thread(UpdateClockChannelsThreadTask).Start();
#endif
            return Task.CompletedTask;
        }

        private static async void UpdateClockChannelsThreadTask()
        {
            const int minutesToWait = 30;

            do
            {
                //  Update the clock channels
                await UpdateClockChannels();

                //  Cache the current UTC time
                DateTime utcNow = DateTime.UtcNow;

                //  Create a temp DateTime object that we can work with
                DateTime temp = utcNow;

                //  Calculate the next time to do the update
                int offest = utcNow.Minute % minutesToWait;
                offest = offest == 0 ? minutesToWait : minutesToWait - offest;
                temp = utcNow.AddMinutes(offest).AddSeconds(-utcNow.Second);

                //  Calculate how long it will take to reach the next update
                TimeSpan timeout = temp.Subtract(utcNow);

                //  Sleep this thread until the next update
                Logger.Message($"Next clock update in {timeout}");
                System.Threading.Thread.Sleep(timeout);
            } while (true);
        }

        private static async Task UpdateClockChannels()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime estNow = TimeConverter.UtcToEst(utcNow);
            DateTime cstNow = TimeConverter.UtcToCst(utcNow);
            DateTime mstNow = TimeConverter.UtcToMst(utcNow);
            DateTime pstNow = TimeConverter.UtcToPst(utcNow);
            DateTime nzstNow = TimeConverter.UtcToNzst(utcNow);

            IVoiceChannel estChannel = _client.GetChannel(815729186673131561) as IVoiceChannel;
            IVoiceChannel cstChannel = _client.GetChannel(815729295556739073) as IVoiceChannel;
            IVoiceChannel mstChannel = _client.GetChannel(815729373659136061) as IVoiceChannel;
            IVoiceChannel pstChannel = _client.GetChannel(815729434715357236) as IVoiceChannel;
            IVoiceChannel nzstChannel = _client.GetChannel(818611936220282970) as IVoiceChannel;

            if (estChannel != null)
            {
                await estChannel.ModifyAsync(x =>
                {
                    x.Name = $"{GetClockEmoji(estNow)} {estNow.ToString("hh:mm tt")} EST";
                });

                await Task.Delay(1000);
            }
            else
            {
                Logger.Error("Unable to locate the EST Clock Chanel");
            }

            if (cstChannel != null)
            {
                await cstChannel.ModifyAsync(x =>
                {
                    x.Name = $"{GetClockEmoji(cstNow)} {cstNow.ToString("hh:mm tt")} CST";
                });

                await Task.Delay(1000);
            }
            else
            {
                Logger.Error("Unable to locate the CST Clock Channel");
            }

            if (mstChannel != null)
            {
                await mstChannel.ModifyAsync(x =>
                {
                    x.Name = $"{GetClockEmoji(mstNow)} {mstNow.ToString("hh:mm tt")} MST";
                });

                await Task.Delay(1000);
            }
            else
            {
                Logger.Error("Unable to locate the MST Clock Channel");
            }

            if (pstChannel != null)
            {
                await pstChannel.ModifyAsync(x =>
                {
                    x.Name = $"{GetClockEmoji(pstNow)} {pstNow.ToString("hh:mm tt")} PST";
                });

                await Task.Delay(1000);
            }
            else
            {
                Logger.Error("Unable to locate the PST Clock Channel");
            }

            if (nzstChannel != null)
            {
                await nzstChannel.ModifyAsync(x =>
                {
                    x.Name = $"{GetClockEmoji(nzstNow)} {nzstNow.ToString("hh:mm tt")} NZST";
                });

                await Task.Delay(1000);
            }
            else
            {
                Logger.Error("Unable to locate the NZST Clock Channel");
            }
        }

        private static string GetClockEmoji(DateTime time)
        {
            int hour = time.Hour > 12 ? time.Hour - 12 : time.Hour;
            int min = time.Minute < 30 ? 0 : 30;

            return (hour, min) switch
            {
                (0, 0) => StandardEmojis.Symbols.TwelveOClock,
                (0, 30) => StandardEmojis.Symbols.TwelveThirty,
                (1, 0) => StandardEmojis.Symbols.OneOClock,
                (1, 30) => StandardEmojis.Symbols.OneThirty,
                (2, 0) => StandardEmojis.Symbols.TwoOClock,
                (2, 30) => StandardEmojis.Symbols.TwoThirty,
                (3, 0) => StandardEmojis.Symbols.ThreeOClock,
                (3, 30) => StandardEmojis.Symbols.ThreeThirty,
                (4, 0) => StandardEmojis.Symbols.FourOClock,
                (4, 30) => StandardEmojis.Symbols.FourThirty,
                (5, 0) => StandardEmojis.Symbols.FiveOClock,
                (5, 30) => StandardEmojis.Symbols.FiveThirty,
                (6, 0) => StandardEmojis.Symbols.SixOClock,
                (6, 30) => StandardEmojis.Symbols.SixThirty,
                (7, 0) => StandardEmojis.Symbols.SevenOClock,
                (7, 30) => StandardEmojis.Symbols.SevenThirty,
                (8, 0) => StandardEmojis.Symbols.EightOClock,
                (8, 30) => StandardEmojis.Symbols.EightThirty,
                (9, 0) => StandardEmojis.Symbols.NineOClock,
                (9, 30) => StandardEmojis.Symbols.NineThirty,
                (10, 0) => StandardEmojis.Symbols.TenOClock,
                (10, 30) => StandardEmojis.Symbols.TenThirty,
                (11, 0) => StandardEmojis.Symbols.ElevenOClock,
                (11, 30) => StandardEmojis.Symbols.ElevenThirty,
                (12, 0) => StandardEmojis.Symbols.TwelveOClock,
                (12, 30) => StandardEmojis.Symbols.TwelveThirty,
                (_, _) => StandardEmojis.Symbols.ThreeOClock,
            };
        }
    }
}
