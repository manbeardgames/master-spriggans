using System;
using System.Threading.Tasks;
using Discord.Commands;
using MasterSpriggans.Utils;

namespace MasterSpriggans.Modules
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {
        protected readonly string _renaThumbnail = @"https://vignette.wikia.nocookie.net/starocean/images/f/fb/Tales_Rena_default.png/revision/latest?cb=20190624002000";
    }
}