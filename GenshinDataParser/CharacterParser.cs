using HtmlAgilityPack;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunkong.Core;
using Xunkong.GenshinData.Character;
using Xunkong.GenshinData.Material;
using static Dapper.SqlMapper;

namespace GenshinDataParser;

internal static class CharacterParser
{

    private static string AvatarExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarExcelConfigData.json");
    private static string FetterInfoExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\FetterInfoExcelConfigData.json");
    private static string AvatarSkillDepotExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarSkillDepotExcelConfigData.json");
    private static string AvatarSkillExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarSkillExcelConfigData.json");
    private static string ProudSkillExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\ProudSkillExcelConfigData.json");
    private static string AvatarTalentExcelConfigData = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarTalentExcelConfigData.json");
    private static string sortExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarCodexExcelConfigData.json");
    private static string namecardExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\FetterCharacterCardExcelConfigData.json");
    private static string promoteExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarPromoteExcelConfigData.json");
    private static string voiceExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\FettersExcelConfigData.json");
    private static string storyExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\FetterStoryExcelConfigData.json");
    private static string foodExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\CookBonusExcelConfigData.json");
    private static string outfitExcel = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\AvatarCostumeExcelConfigData.json");


    public static List<CharacterInfoModel> characterInfos = new();

    public static List<CharacterTalentInfoModel> characterTalentInfoModels = new();

    public static List<CharacterConstellationInfoModel> characterConstellationInfoModels = new();

    public static List<CharacterPromotion> characterPromotions = new();


    public static List<CharacterInfo> Result;


    public static async Task Run()
    {
        using var dapper = Config.CreateConnection();
        var str_avatarExcel = await File.ReadAllTextAsync(AvatarExcelConfigData);
        var node_avatarExcel = JsonNode.Parse(str_avatarExcel) as JsonArray;
        var str_fetter = await File.ReadAllTextAsync(FetterInfoExcelConfigData);
        var node_fetter = JsonNode.Parse(str_fetter);


        var str_skillDepot = await File.ReadAllTextAsync(AvatarSkillDepotExcelConfigData);
        var node_skillDepot = JsonNode.Parse(str_skillDepot) as JsonArray;
        var str_skillExcel = await File.ReadAllTextAsync(AvatarSkillExcelConfigData);
        var node_skillExcel = JsonNode.Parse(str_skillExcel) as JsonArray;
        var str_proud = await File.ReadAllTextAsync(ProudSkillExcelConfigData);
        var node_prouds = JsonNode.Parse(str_proud) as JsonArray;
        var str_talent = await File.ReadAllTextAsync(AvatarTalentExcelConfigData);
        var node_talents = JsonNode.Parse(str_talent) as JsonArray;


        var json_5 = node_avatarExcel.FirstOrDefault(x => ((int)x["id"]) == 10000005).ToJsonString();
        var json_7 = node_avatarExcel.FirstOrDefault(x => ((int)x["id"]) == 10000007).ToJsonString();
        foreach (var item in new (int Id, int Dept)[] { (30000001, 502), (30000003, 503), (30000005, 504), (30000007, 507), (30000009, 508), (30000011, 505), (30000013, 506) })
        {
            var node_1 = JsonNode.Parse(json_5);
            node_1["id"] = item.Id;
            node_1["skillDepotId"] = item.Dept;
            node_avatarExcel.Add(node_1);
        }
        foreach (var item in new (int Id, int Dept)[] { (30000002, 502), (30000004, 503), (30000006, 504), (30000008, 507), (30000010, 508), (30000012, 505), (30000014, 506) })
        {
            var node_1 = JsonNode.Parse(json_7);
            node_1["id"] = item.Id;
            node_1["skillDepotId"] = item.Dept;
            node_avatarExcel.Add(node_1);
        }


        // 基础信息
        foreach (var node in node_avatarExcel as JsonArray)
        {
            var id = (int)node["id"];
            var nameTextMap = (long)node["nameTextMapHash"];
            var descTextMapHash = (long)node["descTextMapHash"];
            var bodyType = (string)node["bodyType"] ?? "";
            var gender = bodyType.Contains("BOY") || bodyType.Contains("MALE") ? 0 : 1;
            var rarity = node["qualityType"]?.ToString() switch
            {
                "QUALITY_ORANGE" => 5,
                "QUALITY_PURPLE" => 4,
                _ => 0,
            };
            var weaponType = node["weaponType"]?.ToString() switch
            {
                "WEAPON_CATALYST" => WeaponType.Catalyst,
                "WEAPON_SWORD_ONE_HAND" => WeaponType.Sword,
                "WEAPON_CLAYMORE" => WeaponType.Claymore,
                "WEAPON_BOW" => WeaponType.Bow,
                "WEAPON_POLE" => WeaponType.Polearm,
                _ => WeaponType.None,
            };
            var hpBase = ((double)node["hpBase"]);
            var attackBase = ((double)node["attackBase"]);
            var defenseBase = ((double)node["defenseBase"]);
            var avatarPromoteId = ((int)node["avatarPromoteId"]);
            var card = $"https://file.xunkong.cc/genshin/character/{node["iconName"]}_Card.png";
            var faceIcon = $"https://file.xunkong.cc/genshin/character/{node["iconName"]}.png";
            var sideIcon = $"https://file.xunkong.cc/genshin/character/{node["sideIconName"]}.png";
            var iconName = ((string)node["iconName"]).Substring(((string)node["iconName"]).LastIndexOf('_') + 1);
            var gachaCard = $"https://file.xunkong.cc/genshin/character/UI_Gacha_AvatarIcon_{iconName}.png";
            var gachaSplash = $"https://file.xunkong.cc/genshin/character/UI_Gacha_AvatarImg_{iconName}.png";
            if (id == 10000062)
            {
                // 埃洛伊
                rarity = 5;
            }


            characterInfos.Add(new CharacterInfoModel
            {
                Id = id,
                Gender = gender,
                Rarity = rarity,
                WeaponType = weaponType,
                Card = card,
                FaceIcon = faceIcon,
                SideIcon = sideIcon,
                GachaCard = gachaCard,
                GachaSplash = gachaSplash,
                NameTextMapHash = nameTextMap,
                DescTextMapHash = descTextMapHash,
                HpBase = hpBase,
                AttackBase = attackBase,
                DefenseBase = defenseBase,
                PromoteId = avatarPromoteId,
            });

        }


        // 拓展信息
        foreach (var node in node_fetter as JsonArray)
        {
            var id = (int)node["avatarId"];
            var element = (long)(node["avatarVisionBeforTextMapHash"] ?? 0);
            var month = (int)(node["infoBirthMonth"] ?? 0);
            var day = (int)(node["infoBirthDay"] ?? 0);
            var affiliation = (long)node["avatarNativeTextMapHash"];
            var constllation = (long)node["avatarConstellationBeforTextMapHash"];
            var title = (long)node["avatarTitleTextMapHash"];
            var description = (long)node["avatarDetailTextMapHash"];
            var cv_cn = (long)node["cvChineseTextMapHash"];
            var cv_jp = (long)node["cvJapaneseTextMapHash"];
            var cv_en = (long)node["cvEnglishTextMapHash"];
            var cv_ko = (long)node["cvKoreanTextMapHash"];
            var info = characterInfos.Where(x => x.Id == id).FirstOrDefault();
            if (info != null)
            {
                if (string.IsNullOrWhiteSpace(info.Birthday))
                {
                    info.Birthday = month == 0 ? "" : $"{month}/{day}";
                }
                if (info.Element == 0)
                {
                    info.Element = (string)Config.TextMap[element] switch
                    {
                        "火" => ElementType.Pyro,
                        "水" => ElementType.Hydro,
                        "风" => ElementType.Anemo,
                        "雷" => ElementType.Electro,
                        "草" => ElementType.Dendro,
                        "冰" => ElementType.Cryo,
                        "岩" => ElementType.Geo,
                        _ => ElementType.None,
                    };
                }
                info.AffiliationTextMapHash = affiliation;
                info.ConstllationTextMapHash = constllation;
                info.TitleTextMapHash = title;
                info.DescTextMapHash = description;
                info.CvChineseTextMapHash = cv_cn;
                info.CvJapaneseTextMapHash = cv_jp;
                info.CvEnglishTextMapHash = cv_en;
                info.CvKoreanTextMapHash = cv_ko;
            }
        }



        // 天赋 命座
        foreach (var node in node_avatarExcel)
        {
            var id = (int)node["id"];
            //var info = characterInfos.Where(x => x.Id == id).FirstOrDefault();
            //if (info is null)
            //{
            //    continue;
            //}
            //if ((info.Talents?.Any() ?? false) && (info.Constellations?.Any() ?? false))
            //{
            //    continue;
            //}
            var depotId = (int)node["skillDepotId"];
            var node_depot = node_skillDepot.Where(x => (int)x["id"] == depotId).FirstOrDefault();
            if (node_depot is null)
            {
                continue;
            }
            // 技能天赋
            var skillIds = (node_depot["skills"] as JsonArray).Select(x => (int)x).Where(x => x != 0).ToList();
            var ene = node_depot["energySkill"];
            if (ene != null)
            {
                skillIds.Insert(2, (int)ene);
            }
            var skills = new List<CharacterTalentInfoModel>();
            foreach (var skillId in skillIds)
            {
                var node_skill = node_skillExcel.Where(x => (int)x["id"] == skillId).FirstOrDefault();
                if (node_skill is not null)
                {
                    var name = (long)node_skill["nameTextMapHash"];
                    var desc = (long)node_skill["descTextMapHash"];
                    var icon = (string)node_skill["skillIcon"];
                    var cdtime = node_skill["cdTime"];
                    float cd = 0;
                    if (cdtime != null)
                    {
                        cd = (float)cdtime;
                    }
                    var charge = (int)node_skill["maxChargeNum"];
                    float cost = 0;
                    var costval = node_skill["costElemVal"];
                    if (costval != null)
                    {
                        cost = (float)costval;
                    }
                    int proudSkillGroupId = 0;
                    var proudSkillGroupIdval = node_skill["proudSkillGroupId"];
                    if (proudSkillGroupIdval != null)
                    {
                        proudSkillGroupId = (int)proudSkillGroupIdval;
                    }
                    var skillModel = new CharacterTalentInfoModel
                    {
                        TalentId = skillId,
                        CharacterInfoId = id,
                        NameTextMapHash = name,
                        DescTextMapHash = desc,
                        Icon = $"https://file.xunkong.cc/genshin/skill/{icon}.png",
                        CdTime = cd,
                        MaxChargeNumber = charge,
                        CostElementValue = cost,
                    };
                    var levels = new List<CharacterTalentLevel>();
                    foreach (var node_proudSkill in node_prouds.Where(x => (int)x["proudSkillGroupId"] == proudSkillGroupId))
                    {
                        var level = JsonSerializer.Deserialize<CharacterTalentLevel>(node_proudSkill, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        foreach (var promoteItem in level.CostItems)
                        {
                            var material = await dapper.QueryFirstOrDefaultAsync<MaterialItem>("SELECT * FROM info_material WHERE Id=@Id;", new { Id = promoteItem.Id });
                            promoteItem.Item = material;
                        }
                        level.CostItems = level.CostItems.Where(x => x.Id > 0).ToList();
                        var paramList = (node_proudSkill["paramList"] as JsonArray).Select(x => ((object)((double)x))).Prepend(0).ToArray();
                        var paramDescList = (node_proudSkill["paramDescList"] as JsonArray).Select(x => ((long)x)).ToList();
                        level.Params = new List<CharacterTalentLevelParam>();
                        foreach (var paramdesc in paramDescList)
                        {
                            if (Config.TextMap.TryGetValue(paramdesc, out var val))
                            {
                                var splits = val.Split("|");
                                level.Params.Add(new CharacterTalentLevelParam
                                {
                                    Title = splits.FirstOrDefault(),
                                    Desc = val,
                                    Value = string.Format(splits.LastOrDefault()?.Replace("param", "")?.Replace("F1P", "P1")?.Replace("F2P", "P2")?.Replace("I", "N0"), paramList),
                                });
                            }
                        }
                        if (level.Params.Any())
                        {
                            levels.Add(level);
                        }
                    }
                    if (levels.Any())
                    {
                        skillModel.Levels = levels;
                    }
                    skills.Add(skillModel);
                }
            }

            // 固有天赋
            var proudIds = (node_depot["inherentProudSkillOpens"] as JsonArray).Select(x => (int)(x["proudSkillGroupId"] ?? 0)).Where(x => x != 0).ToList();
            foreach (var proudId in proudIds)
            {
                var node_proud = node_prouds.Where(x => (int)x["proudSkillGroupId"] == proudId).FirstOrDefault();
                if (node_proud is not null)
                {
                    var name = (long)node_proud["nameTextMapHash"];
                    var desc = (long)node_proud["descTextMapHash"];
                    var icon = (string)node_proud["icon"];
                    var skillModel = new CharacterTalentInfoModel
                    {
                        TalentId = proudId,
                        CharacterInfoId = id,
                        NameTextMapHash = name,
                        DescTextMapHash = desc,
                        Icon = $"https://file.xunkong.cc/genshin/talent/{icon}.png",
                    };
                    if (skillModel.TalentId == 5223)
                    {
                        skillModel.Name = "无法参与料理";
                        skillModel.Icon = "https://file.xunkong.cc/genshin/icon/UI_Icon_Intee_Cooking.png";
                    }
                    skills.Add(skillModel);
                }
            }
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].Order = i;
            }
            characterTalentInfoModels.AddRange(skills);


            // 命座
            var talentIds = (node_depot["talents"] as JsonArray).Select(x => (int)x).ToList();
            var talents = new List<CharacterConstellationInfoModel>();
            foreach (var talentId in talentIds)
            {
                var node_talent = node_talents.Where(x => (int)x["talentId"] == talentId).FirstOrDefault();
                if (node_talent is not null)
                {
                    var name = (long)node_talent["nameTextMapHash"];
                    var desc = (long)node_talent["descTextMapHash"];
                    var icon = (string)node_talent["icon"];
                    var preview = (int)(node_talent["prevTalent"] ?? 0);
                    var talent = new CharacterConstellationInfoModel
                    {
                        ConstellationId = talentId,
                        CharacterInfoId = id,
                        NameTextMapHash = name,
                        DescTextMapHash = desc,
                        Icon = $"https://file.xunkong.cc/genshin/talent/{icon}.png",
                        PreviewConstellationId = preview,
                    };
                    talents.Add(talent);
                }
            }
            characterConstellationInfoModels.AddRange(talents);

        }


        var node_sortId = JsonNode.Parse(await File.ReadAllTextAsync(sortExcel));
        // 排序
        foreach (var node in node_sortId as JsonArray)
        {
            var id = ((int)node["sortId"]);
            var avatarId = ((int)node["avatarId"]);
            var time = DateTime.Parse(((string?)node["beginTime"]));
            if (characterInfos.FirstOrDefault(x => x.Id == avatarId) is CharacterInfoModel info)
            {
                info.SortId = id;
                info.BeginTime = time;
            }
        }

        // 名片
        using var db = new XunkongDbContext();

        var rewards = db.Set<Reward>().AsNoTracking().ToDictionary(x => x.RewardId);

        var node_Namecard = JsonNode.Parse(await File.ReadAllTextAsync(namecardExcel));
        foreach (var node in node_Namecard as JsonArray)
        {
            var avatarId = ((int)node["avatarId"]);
            var rewardId = ((int)node["rewardId"]);
            var reward = rewards.GetValueOrDefault(rewardId);
            if (reward is not null)
            {
                var nameCardId = reward.RewardItemList.FirstOrDefault()?.ItemId ?? 0;
                if (nameCardId > 0)
                {
                    if (characterInfos.FirstOrDefault(x => x.Id == avatarId) is CharacterInfoModel info)
                    {
                        info.NameCardId = nameCardId;
                    }
                }
            }
        }

        // 突破
        var str_promote = await File.ReadAllTextAsync(promoteExcel);
        characterPromotions = JsonSerializer.Deserialize<List<CharacterPromotion>>(str_promote, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        foreach (var item in characterPromotions)
        {
            foreach (var promoteItem in item.CostItems)
            {
                var material = await dapper.QueryFirstOrDefaultAsync<MaterialItem>("SELECT * FROM info_material WHERE Id=@Id;", new { Id = promoteItem.Id });
                promoteItem.Item = material;
            }
            item.CostItems = item.CostItems.Where(x => x.Id > 0).ToList();
        }

        // 特色料理
        var node_food = JsonNode.Parse(await File.ReadAllTextAsync(foodExcel));
        foreach (var node in node_food as JsonArray)
        {
            var avatarId = ((int)node["avatarId"]);
            var paramVec = ((int)(node["paramVec"] as JsonArray).First());
            if (characterInfos.FirstOrDefault(x => x.Id == avatarId) is CharacterInfoModel info)
            {
                var material = await dapper.QueryFirstOrDefaultAsync<Food>("SELECT * FROM info_material WHERE Id=@Id;", new { Id = paramVec });
                info.Food = material;
            }
        }

        // 语音
        var node_voice = JsonNode.Parse(await File.ReadAllTextAsync(voiceExcel));
        foreach (var node in node_voice as JsonArray)
        {
            var fetterId = ((int)node["fetterId"]);
            var avatarId = ((int)node["avatarId"]);
            var titleHash = ((long)node["voiceTitleTextMapHash"]);
            var contentHash = ((long)node["voiceFileTextTextMapHash"]);
            var content = Config.TextMap.GetValueOrDefault(contentHash);
            if (content?.StartsWith("#") ?? false)
            {
                content = content.Substring(1).Replace("{NICKNAME}", "旅行者");
            }
            if (characterInfos.FirstOrDefault(x => x.Id == avatarId) is CharacterInfoModel info)
            {
                if (info.Voices == null)
                {
                    info.Voices = new List<CharacterVoice>();
                }
                info.Voices.Add(new CharacterVoice
                {
                    CharacterId = avatarId,
                    Content = content,
                    FetterId = fetterId,
                    Title = Config.TextMap.GetValueOrDefault(titleHash),
                });
            }
        }

        // 故事
        var node_story = JsonNode.Parse(await File.ReadAllTextAsync(storyExcel));
        foreach (var node in node_story as JsonArray)
        {
            var fetterId = ((int)node["fetterId"]);
            var avatarId = ((int)node["avatarId"]);
            var titleHash = ((long)node["storyTitleTextMapHash"]);
            var contentHash = ((long)node["storyContextTextMapHash"]);
            if (characterInfos.FirstOrDefault(x => x.Id == avatarId) is CharacterInfoModel info)
            {
                if (info.Stories == null)
                {
                    info.Stories = new List<CharacterStory>();
                }
                var content = Config.TextMap.GetValueOrDefault(contentHash);
                if (content?.StartsWith("#") ?? false)
                {
                    content = content.Substring(1).Replace("{NICKNAME}", "旅行者");
                }
                info.Stories.Add(new CharacterStory
                {
                    CharacterId = avatarId,
                    Content = content,
                    FetterId = fetterId,
                    Title = Config.TextMap.GetValueOrDefault(titleHash),
                });
            }
        }

        // 衣装
        var node_outfit = JsonNode.Parse(await File.ReadAllTextAsync(outfitExcel));
        var outfits = new List<CharacterOutfit>();
        foreach (var node in node_outfit as JsonArray)
        {
            var id = ((int)node["KKGNHHIFAMD"]);
            var avatarId = ((int)node["PIJICPMEBIP"]);
            var nameTextMapHash = ((long)node["nameTextMapHash"]);
            var descTextMapHash = ((long)node["descTextMapHash"]);
            var isDefault = ((bool)(node["isDefault"] ?? false));
            var outfit = new CharacterOutfit
            {
                Id = id,
                CharacterId = avatarId,
                Name = Config.TextMap.GetValueOrDefault(nameTextMapHash),
                Description = Config.TextMap.GetValueOrDefault(descTextMapHash),
                IsDefault = isDefault,
            };
            if (!isDefault)
            {
                outfit.FaceIcon = $"https://file.xunkong.cc/genshin/character/{node["NGEMPNOFHLJ"]}.png";
                outfit.Card = $"https://file.xunkong.cc/genshin/character/{node["NGEMPNOFHLJ"]}_Card.png";
                outfit.SideIcon = $"https://file.xunkong.cc/genshin/character/{node["sideIconName"]}.png";
                outfit.GachaSplash = $"https://file.xunkong.cc/genshin/character/{node["NGEMPNOFHLJ"].ToString().Replace("AvatarIcon", "Costume")}.png";
            }
            outfits.Add(outfit);
        }


        // 文本
        foreach (var item in characterInfos)
        {
            item.Affiliation = Config.TextMap.GetValueOrDefault(item.AffiliationTextMapHash);
            item.ConstllationName = Config.TextMap.GetValueOrDefault(item.ConstllationTextMapHash);
            item.CvChinese = Config.TextMap.GetValueOrDefault(item.CvChineseTextMapHash);
            item.CvEnglish = Config.TextMap.GetValueOrDefault(item.CvEnglishTextMapHash);
            item.CvJapanese = Config.TextMap.GetValueOrDefault(item.CvJapaneseTextMapHash);
            item.CvKorean = Config.TextMap.GetValueOrDefault(item.CvKoreanTextMapHash);
            item.Description = Config.TextMap.GetValueOrDefault(item.DescTextMapHash);
            item.Name = Config.TextMap.GetValueOrDefault(item.NameTextMapHash);
            item.Title = Config.TextMap.GetValueOrDefault(item.TitleTextMapHash);
            if (item.Id == 10000005)
            {
                // 空
                item.Name = "空";
                item.Title = "旅行者";
            }
            if (item.Id == 10000007)
            {
                // 荧
                item.Name = "荧";
                item.Title = "旅行者";
            }
            if (item.Id == 10000030)
            {
                // 钟离
                item.ConstllationName = "岩王帝君座";
            }
        }
        foreach (var item in characterConstellationInfoModels)
        {
            item.Description = Config.TextMap.GetValueOrDefault(item.DescTextMapHash);
            item.Name = Config.TextMap.GetValueOrDefault(item.NameTextMapHash);
        }
        foreach (var item in characterTalentInfoModels)
        {
            item.Description = Config.TextMap.GetValueOrDefault(item.DescTextMapHash);
            item.Name = Config.TextMap.GetValueOrDefault(item.NameTextMapHash);
            if (item.TalentId == 5223)
            {
                item.Name = "无法参与料理";
            }
        }

        var op = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        var s1 = JsonSerializer.Serialize(characterTalentInfoModels, op);
        var s2 = JsonSerializer.Serialize(characterConstellationInfoModels, op);
        var s3 = JsonSerializer.Serialize(characterPromotions, op);

        var nameCards = dapper.Query<NameCard>($"SELECT Id, Name, Description, Icon, ItemType, MaterialType, TypeDescription, RankLevel, StackLimit, `Rank`, GalleryBackground, ProfileImage FROM info_material WHERE MaterialType='{MaterialType.NameCard}';").ToDictionary(x => x.Id);
        foreach (var item in characterInfos)
        {
            item.Talents = characterTalentInfoModels.Where(x => x.CharacterInfoId == item.Id).ToList().Adapt<List<CharacterTalent>>();
            item.Constellations = characterConstellationInfoModels.Where(x => x.CharacterInfoId == item.Id).ToList().Adapt<List<CharacterConstellation>>();
            if (!item.Constellations.Any())
            {
                item.Constellations = null;
            }
            item.Promotions = characterPromotions.Where(x => x.AvatarPromoteId == item.PromoteId && x.PromoteLevel > 0).ToList();
            item.NameCard = nameCards.GetValueOrDefault(item.NameCardId);
            item.Outfits = outfits.Where(x => x.CharacterId == item.Id).ToList();
        }

        Result = characterInfos.Adapt<List<CharacterInfo>>();

    }



    public static async Task GetVoice()
    {
        Directory.CreateDirectory("E:/a");
        var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All });

        var api = "https://api-static.mihoyo.com/common/blackboard/ys_obc/v1/home/content/list?app_sn=ys_obc&channel_id=189";

        var a = await client.GetStringAsync(api);
        var apiNode = JsonNode.Parse(a);
        var ids = (apiNode["data"]["list"][0]["children"][0]["list"] as JsonArray).Select(x => ((int)x["content_id"])).ToList();

        foreach (var id in ids)
        {


            var str = await client.GetStringAsync($"https://api-static.mihoyo.com/common/blackboard/ys_obc/v1/content/info?app_sn=ys_obc&content_id={id}");
            var jsonnode = JsonNode.Parse(str);
            var name = jsonnode["data"]["content"]["title"].ToString();
            var content = jsonnode["data"]["content"]["contents"][2]["text"].ToString();

            var html = new HtmlDocument();
            html.LoadHtml(content);

            var cn = new Dictionary<string, string>();
            var en = new Dictionary<string, string>();
            var jp = new Dictionary<string, string>();
            var kr = new Dictionary<string, string>();

            var ordertext = html.DocumentNode.SelectNodes("//ul[@class='obc-tmpl__switch-btn-list']").Last().InnerText.Replace("文", "").Replace("（荧）", "").Replace("（空）", "").Replace("语", "").Trim();


            var nodes = html.DocumentNode.SelectNodes("//table[@class='obc-tmpl-character__voice-pc']/tbody");

            int i = 0;
            foreach (var voiceNode in nodes)
            {
                var dic = ordertext[i] switch
                {
                    '汉' or '中' => cn,
                    '英' => en,
                    '日' => jp,
                    '韩' => kr,
                };
                foreach (var tr in voiceNode.ChildNodes)
                {
                    var title = tr.ChildNodes[0].InnerText;
                    var url = tr.ChildNodes[2].SelectSingleNode("./div/div/audio/source")?.Attributes["src"]?.Value;
                    dic.Add(title, url);
                }
                i++;
            }

            var result = new Dictionary<string, object>();
            if (name.Contains("旅行者") && name.Contains("空"))
            {
                name = "空";
            }
            if (name.Contains("旅行者") && name.Contains("荧"))
            {
                name = "荧";
            }
            result.Add("name", name);
            result.Add("cn", cn);
            result.Add("en", en);
            result.Add("jp", jp);
            result.Add("kr", kr);

            var jsonresult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            File.WriteAllText($"E:/a/{name}.json", jsonresult);
            Console.WriteLine(name);
        }
        using var db = new XunkongDbContext();
        foreach (var file in Directory.EnumerateFiles("E:/a"))
        {
            var node = JsonNode.Parse(await File.ReadAllTextAsync(file));
            var name = node["name"].ToString();
            var cha = await db.Set<CharacterInfo>().Where(x => x.Name == name).FirstOrDefaultAsync();
            if (cha is null)
            {
                Console.WriteLine($"{name} not found.");
                return;
            }
            foreach (var item in cha.Voices)
            {
                item.Chinese = node["cn"][item.Title]?.ToString();
                item.English = node["en"][item.Title]?.ToString();
                item.Japanese = node["jp"][item.Title]?.ToString();
                item.Korean = node["kr"][item.Title]?.ToString();
            }
            db.Update(cha);
            await db.SaveChangesAsync();
        }
    }




}







public class CharacterInfoModel : CharacterInfo
{
    public long NameTextMapHash { get; set; }

    public long TitleTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }

    public long AffiliationTextMapHash { get; set; }

    public long ConstllationTextMapHash { get; set; }

    public long CvChineseTextMapHash { get; set; }

    public long CvJapaneseTextMapHash { get; set; }

    public long CvEnglishTextMapHash { get; set; }

    public long CvKoreanTextMapHash { get; set; }

    public int NameCardId { get; set; }

    public int PromoteId { get; set; }
}



public class CharacterTalentInfoModel : CharacterTalent
{
    public int CharacterInfoId { get; set; }

    public long NameTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }
}



public class CharacterConstellationInfoModel : CharacterConstellation
{

    public int CharacterInfoId { get; set; }
    public long NameTextMapHash { get; set; }

    public long DescTextMapHash { get; set; }
}




