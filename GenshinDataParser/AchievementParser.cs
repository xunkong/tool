using Microsoft.EntityFrameworkCore;
using Xunkong.GenshinData.Achievement;
using Xunkong.GenshinData.Material;

namespace GenshinDataParser;

internal class AchievementParser
{


    public static List<AchievementGoalModel> AchievementGoalModels;

    public static List<AchievementItemModel> AchievementItemModels;



    public static void Run()
    {
        var file_goal = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AchievementGoalExcelConfigData.json");
        var file_item = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AchievementExcelConfigData.json");
        var str_goal = File.ReadAllText(file_goal);
        var str_item = File.ReadAllText(file_item);

        using var db = new XunkongDbContext();

        var rewards = db.Set<Reward>().AsNoTracking().ToDictionary(x => x.RewardId);

        using var dapper = Config.CreateConnection();
        var nameCards = dapper.Query<NameCard>($"SELECT Id, Name, Description, Icon, ItemType, MaterialType, TypeDescription, RankLevel, StackLimit, `Rank`, GalleryBackground, ProfileImage FROM info_material WHERE MaterialType='{MaterialType.NameCard}';").ToDictionary(x => x.Id);

        var list_goal = JsonSerializer.Deserialize<List<AchievementGoalModel>>(str_goal, Config.JsonOptions);
        foreach (var item in list_goal)
        {
            item.Id = item.Id == 0 ? 10001 : item.Id;
            item.Name = Config.TextMap.GetValueOrDefault(item.NameTextMapHash);
            var icon = item.IconPath;
            item.IconPath = $"https://file.xunkong.cc/genshin/achievement/{icon}.png";
            item.SmallIcon = $"https://file.xunkong.cc/genshin/achievement/Sprite{icon}.png";
            var reward = rewards.GetValueOrDefault(item.FinishRewardId);
            if (reward != null)
            {
                var r = reward.RewardItemList.FirstOrDefault();
                if (r != null)
                {
                    var namecard = nameCards.GetValueOrDefault(r.ItemId);
                    item.RewardNameCard = namecard;
                }
            }
        }

        Console.WriteLine("Finish Achievement goal");



        var list_item = JsonSerializer.Deserialize<List<AchievementItemModel>>(str_item, Config.JsonOptions);
        foreach (var item in list_item)
        {
            item.Title = Config.TextMap.GetValueOrDefault(item.TitleTextMapHash);
            item.Description = Config.TextMap.GetValueOrDefault(item.DescTextMapHash);
            item.IsHide = !string.IsNullOrWhiteSpace(item.IsShow);
            item.GoalId = item.GoalId == 0 ? 10001 : item.GoalId;
            var reward = rewards.GetValueOrDefault(item.FinishRewardId);
            if (reward != null)
            {
                var r = reward.RewardItemList.FirstOrDefault(x => x.ItemId == 201);
                if (r != null)
                {
                    item.RewardCount = r.ItemCount;
                }
            }
        }

        Console.WriteLine("Finish Achievement item");

        AchievementGoalModels = list_goal;

        AchievementItemModels = list_item;


        Console.WriteLine("Finish Achievement");

    }
}


internal class AchievementItemModel : AchievementItem
{
    public long TitleTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }

    public string IsShow { get; set; }

    public bool? IsDisuse { get; set; }
}


internal class AchievementGoalModel : AchievementGoal
{
    public long NameTextMapHash { get; set; }
}