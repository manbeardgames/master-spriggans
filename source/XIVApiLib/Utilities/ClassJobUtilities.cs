using System;
using System.Collections.Generic;
using System.Text;
using XIVApiLib.Models;

namespace XIVApiLib.Utilities
{
    public static class ClassJobUtilities
    {
        public static string ClassJobToAbbr(ClassJobIndex classJob) => classJob switch
        {
            ClassJobIndex.Alchemist => "alc",
            ClassJobIndex.Armorer => "arm",
            ClassJobIndex.Astrologian => "ast",
            ClassJobIndex.Bard => "brd",
            ClassJobIndex.BlackMage => "blm",
            ClassJobIndex.Blacksmith => "bsm",
            ClassJobIndex.BlueMage => "blu",
            ClassJobIndex.Botanist => "bot",
            ClassJobIndex.Carpenter => "crp",
            ClassJobIndex.Culinarian => "cul",
            ClassJobIndex.Dancer => "dnc",
            ClassJobIndex.DarkKnight => "drk",
            ClassJobIndex.Dragoon => "drg",
            ClassJobIndex.Fisher => "fsh",
            ClassJobIndex.Goldsmith => "gsm",
            ClassJobIndex.Gunbreaker => "gnb",
            ClassJobIndex.Leatherworker => "ltw",
            ClassJobIndex.Machinist => "mch",
            ClassJobIndex.Miner => "min",
            ClassJobIndex.Monk => "mnk",
            ClassJobIndex.Ninja => "nin",
            ClassJobIndex.Paladin => "pld",
            ClassJobIndex.RedMage => "rdm",
            ClassJobIndex.Samurai => "sam",
            ClassJobIndex.Scholar => "sch",
            ClassJobIndex.Summoner => "smn",
            ClassJobIndex.Warrior => "war",
            ClassJobIndex.Weaver => "wvr",
            ClassJobIndex.WhiteMage => "whm",
            _ => string.Empty
        };

        public static string ClassJobToName(ClassJobIndex classJob) => classJob switch
        {
            ClassJobIndex.Alchemist => "Alchemist",
            ClassJobIndex.Armorer => "Armorer",
            ClassJobIndex.Astrologian => "Astrologian",
            ClassJobIndex.Bard => "Bard",
            ClassJobIndex.BlackMage => "Black Mage",
            ClassJobIndex.Blacksmith => "Blacksmith",
            ClassJobIndex.BlueMage => "Blue Mage",
            ClassJobIndex.Botanist => "Botanist",
            ClassJobIndex.Carpenter => "Carpenter",
            ClassJobIndex.Culinarian => "Culinarian",
            ClassJobIndex.Dancer => "Dancer",
            ClassJobIndex.DarkKnight => "Dark Knight",
            ClassJobIndex.Dragoon => "Dragoon",
            ClassJobIndex.Fisher => "Fisher",
            ClassJobIndex.Goldsmith => "Goldsmith",
            ClassJobIndex.Gunbreaker => "Gunbreaker",
            ClassJobIndex.Leatherworker => "Leatherworker",
            ClassJobIndex.Machinist => "Machinist",
            ClassJobIndex.Miner => "Miner",
            ClassJobIndex.Monk => "Monk",
            ClassJobIndex.Ninja => "Ninja",
            ClassJobIndex.Paladin => "Paladin",
            ClassJobIndex.RedMage => "RedMage",
            ClassJobIndex.Samurai => "Samurai",
            ClassJobIndex.Scholar => "Scholar",
            ClassJobIndex.Summoner => "Summoner",
            ClassJobIndex.Warrior => "Warrior",
            ClassJobIndex.Weaver => "Weaver",
            ClassJobIndex.WhiteMage => "White Mage",
            _ => string.Empty
        };
    }
}
