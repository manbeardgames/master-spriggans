using System;
using System.Collections.Generic;

namespace XIVApiLib.Models
{
    public class CharacterModel
    {
        public ClassJobModel ActiveClassJob { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
        public List<ClassJobModel> ClassJobs { get; set; }
        //public ClassJobsBozjanModel ClassJobsBozjan { get; set; }
        //public ClassJobsElementalModel ClassJobsElemental { get; set; }
        public string DC { get; set; }
        public string FreeCompanyId { get; set; }

        //  TODO: Implement GearSetModel

        public int Gender { get; set; }
        //public GrandCompanyModel GrandCompany { get; set; }
        public int GuardianDeity { get; set; }
        public int ID { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public int ParseDate { get; set; }
        public string Portrait { get; set; }
        public int? PvPTeamId { get; set; }
        public int Race { get; set; }
        public string Server { get; set; }
        public int Title { get; set; }
        public bool TitleTop { get; set; }
        public int Town { get; set; }
        public int Tribe { get; set; }


    }
}