using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunkong.Core;
using Xunkong.GenshinData.Character;
using Xunkong.GenshinData.Material;
using Xunkong.GenshinData.Weapon;

namespace GenshinDataParser;

internal static class WeaponParser
{

    private static string WeaponExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\WeaponExcelConfigData.json");
    private static string EquipAffixExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\EquipAffixExcelConfigData.json");
    private static string DocumentExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\DocumentExcelConfigData.json");
    private static string LocalizationExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\LocalizationExcelConfigData.json");
    private static string sortIdExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\WeaponCodexExcelConfigData.json");
    private static string promoteExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\WeaponPromoteExcelConfigData.json");



    public static List<WeaponInfo> weaponInfos;


    public static async Task Run()
    {
        weaponInfos = new List<WeaponInfo>();
        var str_weaponExcel = await File.ReadAllTextAsync(WeaponExcelConfigData);
        var node_weaponExcel = JsonNode.Parse(str_weaponExcel);
        var str_affix = await File.ReadAllTextAsync(EquipAffixExcelConfigData);
        var node_affix = JsonNode.Parse(str_affix);
        var str_docExcel = await File.ReadAllTextAsync(DocumentExcelConfigData);
        var node_docExcel = JsonNode.Parse(str_docExcel) as JsonArray;
        var str_locExcel = await File.ReadAllTextAsync(LocalizationExcelConfigData);
        var node_loc = JsonNode.Parse(str_locExcel) as JsonArray;
        var str_sort = await File.ReadAllTextAsync(sortIdExcel);
        var node_sort = JsonNode.Parse(str_sort) as JsonArray;


        using var dapper = Config.CreateConnection();
        var str_promote = await File.ReadAllTextAsync(promoteExcel);
        var weaponPromotions = JsonSerializer.Deserialize<List<WeaponPromotion>>(str_promote, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        foreach (var item in weaponPromotions)
        {
            foreach (var promoteItem in item.CostItems)
            {
                var material = await dapper.QueryFirstOrDefaultAsync<MaterialItem>("SELECT * FROM info_material WHERE Id=@Id;", new { Id = promoteItem.Id });
                promoteItem.Item = material;
            }
            item.CostItems = item.CostItems.Where(x => x.Id > 0).ToList();
            item.AddProps = item.AddProps.Where(x => x.Value > 0).ToList();
        }

        foreach (var node in node_weaponExcel as JsonArray)
        {
            var id = (int)node["id"];
            var nameTextMap = (long)node["nameTextMapHash"];
            var descTextMapHash = (long)node["descTextMapHash"];
            var rarity = (int)node["rankLevel"];
            var type = (string)node["weaponType"] switch
            {
                "WEAPON_SWORD_ONE_HAND" => WeaponType.Sword,
                "WEAPON_CLAYMORE" => WeaponType.Claymore,
                "WEAPON_POLE" => WeaponType.Polearm,
                "WEAPON_CATALYST" => WeaponType.Catalyst,
                "WEAPON_BOW" => WeaponType.Bow,
                _ => WeaponType.None,
            };
            var icon = $"https://file.xunkong.cc/genshin/weapon/{node["icon"]}.png";
            var awakenIcon = $"https://file.xunkong.cc/genshin/weapon/{node["awakenIcon"]}.png";
            var iconName = ((string)node["icon"]).Substring(((string)node["icon"]).IndexOf('_') + 1);
            var gachaIcon = $"https://file.xunkong.cc/genshin/weapon/UI_Gacha_{iconName}.png";
            var storyId = (int)(node["storyId"] ?? 0);
            var affixId = (int)((node["skillAffix"] as JsonArray)?.FirstOrDefault() ?? 0);
            var promoteId = ((int)node["weaponPromoteId"]);
            var sortId = (int)(node_sort.Where(x => ((int)x["weaponId"]) == id).FirstOrDefault()?["SortOrder"] ?? 0);

            var model = new WeaponInfo
            {
                Id = id,
                //NameTextMapHash = nameTextMap,
                //DescTextMapHash = descTextMapHash,
                Name = Config.TextMap.GetValueOrDefault(nameTextMap),
                Description = Config.TextMap.GetValueOrDefault(descTextMapHash)?.Trim(),
                Icon = icon,
                AwakenIcon = awakenIcon,
                GachaIcon = gachaIcon,
                Rarity = rarity,
                WeaponType = type,
                SortId = sortId,
                Properties = JsonSerializer.Deserialize<List<WeaponProperty>>(node["weaponProp"]).Where(x => x.InitValue > 0).ToList(),
                Promotions = weaponPromotions.Where(x => x.WeaponPromoteId == promoteId && x.PromoteLevel > 0).ToList(),
            };
            if (affixId != 0)
            {
                var node_skill = (node_affix as JsonArray).Where(x => (int)x["id"] == affixId).ToList();
                var list = new List<WeaponSkill>();
                foreach (var skill in node_skill)
                {
                    var id_skill = (int)skill["affixId"];
                    var level = (int)(skill["level"] ?? 0);
                    var name = (long)(skill["nameTextMapHash"] ?? 0);
                    var desc = (long)(skill["descTextMapHash"] ?? 0);
                    var skil = new WeaponSkill
                    {
                        Id = id_skill,
                        //NameTextMapHash = name,
                        //DescTextMapHash = desc,
                        Name = Config.TextMap.GetValueOrDefault(name),
                        Description = Config.TextMap.GetValueOrDefault(desc),
                        Level = level,
                    };
                    list.Add(skil);
                }
                list = list.OrderBy(x => x.Level).ToList();
                model.Skills = list;
            }
            if (storyId != 0)
            {
                var locId = (int)(node_docExcel.Where(x => ((int)x["id"]) == storyId).FirstOrDefault()?["contentLocalizedId"] ?? 0);
                if (locId != 0)
                {
                    var path = ((string?)node_loc.Where(x => ((int)x["id"]) == locId).FirstOrDefault()?["scPath"])?.Replace("ART/UI/", "");
                    var file = Path.Combine(Config.GenshinDataPath, path + ".txt");
                    var story = await File.ReadAllTextAsync(file);
                    model.Story = story.Trim();
                }
            }
            weaponInfos.Add(model);
        }


    }

}


public class WeaponInfoModel : WeaponInfo
{
    public long NameTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }

    public int StoryId { get; set; }

    public new List<WeaponSkillModel> Skills { get; set; }
}


public class WeaponSkillModel : WeaponSkill
{
    public long NameTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }

}