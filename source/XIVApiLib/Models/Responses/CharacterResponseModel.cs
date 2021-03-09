using System;
using System.Collections.Generic;

namespace XIVApiLib.Models.Responses
{
    public class CharacterResponseModel : BaseResponseModel
    {
        public CharacterModel Character { get; set; }
        public FreeCompanyModel FreeCompany { get; set; }
        public List<ResultsModel> Results { get; set; }
    }
}