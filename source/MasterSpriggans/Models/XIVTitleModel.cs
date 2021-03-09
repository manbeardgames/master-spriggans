using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterSpriggans.Data.Models
{
    [Table("xiv_title")]
    public class XIVTitleModel
    {
        [Column("id")]
        public int ID { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}