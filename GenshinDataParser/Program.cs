﻿global using Dapper;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using Xunkong.GenshinData;
using GenshinDataParser;
using Microsoft.EntityFrameworkCore;
using Xunkong.GenshinData.Character;
using Xunkong.GenshinData.Weapon;

using var db = new XunkongDbContext();


RewardParser.Run();
{
    var reIds = db.Set<Reward>().Select(x => x.RewardId).ToList();
    var inserts = RewardParser.Rewards.ExceptBy(reIds, x => x.RewardId).ToList();
    var updates = RewardParser.Rewards.IntersectBy(reIds, x => x.RewardId).ToList();
    db.AddRange(inserts);
    db.UpdateRange(updates);
    db.SaveChanges();
}


MaterialParser.Run();
{
    var reIds = db.Set<MaterialItemModel>().Select(x => x.Id).ToList();
    var inserts = MaterialParser.MaterialItemModels.ExceptBy(reIds, x => x.Id).ToList();
    var updates = MaterialParser.MaterialItemModels.IntersectBy(reIds, x => x.Id).ToList();
    db.AddRange(inserts);
    db.UpdateRange(updates);
    db.SaveChanges();
}


AchievementParser.Run();
{
    var reIds = db.Set<AchievementGoalModel>().Select(x => x.Id).ToList();
    var inserts = AchievementParser.AchievementGoalModels.ExceptBy(reIds, x => x.Id).ToList();
    var updates = AchievementParser.AchievementGoalModels.IntersectBy(reIds, x => x.Id).ToList();
    db.AddRange(inserts);
    db.UpdateRange(updates);
    db.SaveChanges();
}
{
    // 84517 未实装， 81219 重复, goal id = 22 需要设置为达成后不显示进度
    foreach (var item in AchievementParser.AchievementItemModels.Where(x => x.GoalId == 37))
    {
        item.Description = item.Description.Replace("{param0}", item.Progress.ToString());
    }
    var reIds = db.Set<AchievementItemModel>().Select(x => x.Id).ToList();
    var inserts = AchievementParser.AchievementItemModels.ExceptBy(reIds, x => x.Id).ToList();
    var updates = AchievementParser.AchievementItemModels.IntersectBy(reIds, x => x.Id).ToList();
    foreach (var item in AchievementParser.AchievementItemModels.Where(x => x.GoalId == 22))
    {
        item.IsDeleteWatcherAfterFinish = true;
    }
    db.AddRange(inserts);
    db.UpdateRange(updates);
    db.SaveChanges();
}

{
    await CharacterParser.Run();
    var liulangzhe = CharacterParser.Result.Where(x => x.Id == 10000075).FirstOrDefault();
    if (liulangzhe != null)
    {
        foreach (var item in liulangzhe.Voices)
        {
            item.Content = item.Content.Replace("#", "").Replace("{REALNAME[ID(1)]}", "流浪者");
        }
    }
    var ids = db.Set<CharacterInfo>().AsNoTracking().Select(x => x.Id).ToList();
    db.AddRange(CharacterParser.Result.ExceptBy(ids, x => x.Id));
    db.UpdateRange(CharacterParser.Result.IntersectBy(ids, x => x.Id));
    db.SaveChanges();
}



{
    await WeaponParser.Run();
    var ids = db.Set<WeaponInfo>().AsNoTracking().Select(x => x.Id).ToList();
    db.AddRange(WeaponParser.weaponInfos.ExceptBy(ids, x => x.Id));
    db.UpdateRange(WeaponParser.weaponInfos.IntersectBy(ids, x => x.Id));
    db.SaveChanges();
}


await CharacterParser.GetVoice();


Console.WriteLine();