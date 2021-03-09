using System;
using System.Collections.Generic;

namespace XIVApiLib.Models.Responses
{
    public class TitleListResponseModel : BaseResponseModel
    {
        public Pagination Pagination { get; set; }
        public List<TitleModel> Results { get; set; } = new List<TitleModel>();
    }
}