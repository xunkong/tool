﻿
namespace Xunkong.Desktop.Database.Old.Models
{

    [Table("WebToolItems")]
    public class WebToolItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
        public string? JavaScript { get; set; }

    }

}
