using System.Text.RegularExpressions;

var path = @"C:\test\b\";
var oPath = @"C:\test\c\";
var files = new DirectoryInfo(path).GetFiles("*Texture2D*");

CopyFiles(files, "Achievement", oPath + @"achievement\");

CopyFiles(files, "AnimalIcon", oPath + @"animal\");

CopyFiles(files, "Chapter", oPath + @"chapter\");

CopyFiles(files, "AvatarIcon", oPath + @"character\");
CopyFiles(files, "AvatarImg", oPath + @"character\");

CopyFiles(files, "CoopCutScene", oPath + @"coop\");

CopyFiles(files, "DungeonPic", oPath + @"dungeon\");

CopyFiles(files, "Emotion", oPath + @"emotion\");

CopyFiles(files, "Fish", oPath + @"fish\");

CopyFiles(files, "GachaShow", oPath + @"gacha\");
CopyFiles(files, "Gacha_A0", oPath + @"gacha\");
CopyFiles(files, "Gacha_A1", oPath + @"gacha\");
CopyFiles(files, "Gacha_A2", oPath + @"gacha\");

CopyFiles(files, "UI_Homeworld", oPath + @"home\");

CopyFiles(files, "UI_Icon", oPath + @"icon\");

CopyFiles(files, "UI_BtnIcon", oPath + @"icon\");

CopyFiles(files, "UI_ItemIcon", oPath + @"item\");

CopyFiles(files, "UI_FlycloakIcon", oPath + @"flycloak\");

CopyFiles(files, "UI_Buff", oPath + @"item\");

CopyFiles(files, "Legend", oPath + @"legend\");

CopyFiles(files, "MonsterIcon", oPath + @"monster\");

CopyFiles(files, "NameCard", oPath + @"namecard\");

CopyFiles(files, "UI_RelicIcon", oPath + @"relic\");

CopyFiles(files, "Scenery", oPath + @"scenery\");

CopyFiles(files, "Skill_", oPath + @"skill\");

CopyFiles(files, "System", oPath + @"system\");

CopyFiles(files, "Talent", oPath + @"talent\");

CopyFiles(files, "EquipIcon", oPath + @"weapon\");



Console.WriteLine("======");
Console.WriteLine("Finish");


Console.ReadLine();


static void CopyFiles(IEnumerable<FileInfo> files, string keyWord, string folder)
{
    Directory.CreateDirectory(folder);
    Console.WriteLine(keyWord);
    var infos = files.Where(f => f.Name.Contains(keyWord)).OrderByDescending(f => f.Length).ToList();
    Console.WriteLine(infos.Count);
    foreach (var info in infos)
    {
        var name = Regex.Replace(info.Name, @"^.+Texture2D", "");
        var file = folder + name;
        int index = 1;
        while (File.Exists(file))
        {
            if (index == 1)
            {
                file = folder + Path.GetFileNameWithoutExtension(file) + " (2)" + Path.GetExtension(file);
            }
            else
            {
                file = file.Replace($"({index})", $"({index + 1})");
            }
            index++;
        }
        File.Copy(info.FullName, file, true);
    }

}