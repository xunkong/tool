namespace GenshinDataParser;

internal class RewardParser
{

    public static List<Reward> Rewards;



    public static void Run()
    {
        var str = Config.GetFileText(@"ExcelBinOutput\RewardExcelConfigData.json");

        Rewards = JsonSerializer.Deserialize<List<Reward>>(str, Config.JsonOptions);
        foreach (var item in Rewards)
        {
            item.RewardItemList = item.RewardItemList.Where(x => x.ItemId > 0 && x.ItemCount > 0).ToList();
        }
    }



}
