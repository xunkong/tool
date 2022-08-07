namespace Xunkong.Desktop.Database.Old.Models
{

    [Table("GenshinUserAccounts")]
    public class GenshinUserAccount
    {

        [Key]
        public string UserName { get; set; }

        public bool IsOversea { get; set; }

        public byte[] ADLPROD { get; set; }

        public DateTimeOffset CreateTime { get; set; }

        public DateTimeOffset LastAccessTime { get; set; }

    }
}
