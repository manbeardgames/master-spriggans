using System;

namespace XIVApiLib.Models
{
    public class ClassJobModel
    {
        public int ClassID { get; set; }
        public int ExpLevel { get; set; }
        public int ExpMax { get; set; }
        public int ExpLevelToGo { get; set; }
        public bool IsSpecialised { get; set; }
        public int JobID { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public UnlockStateModel UnlockedState { get; set; }

    }
}