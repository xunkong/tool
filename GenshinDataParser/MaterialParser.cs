using Xunkong.GenshinData.Material;

namespace GenshinDataParser;

internal class MaterialParser
{



    public static List<MaterialItemModel> MaterialItemModels;


    public static void Run()
    {
        var file = Path.Combine(Config.GenshinDataPath, @"ExcelBinOutput\MaterialExcelConfigData.json");
        var str = File.ReadAllText(file);

        var list = JsonSerializer.Deserialize<List<MaterialItemModel>>(str, Config.JsonOptions);
        foreach (var item in list)
        {
            item.Name = Config.TextMap.GetValueOrDefault(item.NameTextMapHash);
            item.Description = Config.TextMap.GetValueOrDefault(item.DescTextMapHash);
            item.EffectDescription = Config.TextMap.GetValueOrDefault(item.EffectDescTextMapHash);
            item.SpecialDescription = Config.TextMap.GetValueOrDefault(item.SpecialDescTextMapHash);
            item.TypeDescription = Config.TextMap.GetValueOrDefault(item.TypeDescTextMapHash);
            item.Icon = $"https://file.xunkong.cc/genshin/item/{item.Icon}.png";
        }

        Console.WriteLine("Finish MaterialItemModel");


        // namecard

        foreach (var item in list.Where(x => x.MaterialType == MaterialType.NameCard))
        {
            item.GalleryBackground = item.PicPath.FirstOrDefault();
            item.ProfileImage = item.PicPath.LastOrDefault();
            item.Icon = $"https://file.xunkong.cc/genshin/namecard/{Path.GetFileName(item.Icon)}";
            if (!string.IsNullOrWhiteSpace(item.GalleryBackground))
            {
                item.GalleryBackground = $"https://file.xunkong.cc/genshin/namecard/{item.GalleryBackground}.png";
            }
            if (!string.IsNullOrWhiteSpace(item.ProfileImage))
            {
                item.ProfileImage = $"https://file.xunkong.cc/genshin/namecard/{item.ProfileImage}.png";
            }
        }



        foreach (var item in list.Where(x => x.MaterialType == MaterialType.Avatar))
        {
            item.Icon = $"https://file.xunkong.cc/genshin/character/{Path.GetFileName(item.Icon)}";
        }


        foreach (var item in list.Where(x => x.MaterialType == MaterialType.Costume))
        {
            item.Icon = $"https://file.xunkong.cc/genshin/character/{Path.GetFileName(item.Icon)}";
        }



        MaterialItemModels = list;




    }


}



internal class MaterialItemModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public int RankLevel { get; set; }

    public string? ItemType { get; set; }

    public string? MaterialType { get; set; }

    /// <summary>
    /// 一叠限制多少个
    /// </summary>
    public int StackLimit { get; set; }

    public string? EffectDescription { get; set; }

    public string? SpecialDescription { get; set; }

    public string? TypeDescription { get; set; }

    public List<string> PicPath { get; set; }


    /// <summary>
    /// 名片好友列表背景图
    /// </summary>
    public string? GalleryBackground { get; set; }

    /// <summary>
    /// 名片个人展示图
    /// </summary>
    public string? ProfileImage { get; set; }

    /// <summary>
    /// buff 图标
    /// </summary>
    public string? EffectIcon { get; set; }


    /// <summary>
    /// 在背包中的顺序
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 书籍集合
    /// </summary>
    public int SetID { get; set; }

    /// <summary>
    /// 食物品质
    /// </summary>
    public string? FoodQuality { get; set; }

    /// <summary>
    /// 食物冷却
    /// </summary>
    public int CdTime { get; set; }
    public long NameTextMapHash { get; set; }
    public long DescTextMapHash { get; set; }
    public long EffectDescTextMapHash { get; set; }
    public long SpecialDescTextMapHash { get; set; }
    public long TypeDescTextMapHash { get; set; }




}

