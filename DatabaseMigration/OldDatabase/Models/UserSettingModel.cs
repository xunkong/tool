namespace Xunkong.Desktop.Database.Old.Models
{

    [Table("UserSettings")]
    public class UserSettingModel
    {

        [Key]
        public string Key { get; set; }


        public string? Value { get; set; }


    }
}
