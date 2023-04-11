using Scighost.PixivApi;
using System.Text.RegularExpressions;
using Windows.Graphics.Imaging;

// 壁纸功能 sql 生成器

Environment.CurrentDirectory=AppContext.BaseDirectory;

var files = Directory.GetFiles("images");
Directory.CreateDirectory("todo/height");
Directory.CreateDirectory("todo/width");
bool needSuperResolution = false;


foreach (var file in files)
{
    using var fs = File.OpenRead(file);
    var image = await BitmapDecoder.CreateAsync(fs.AsRandomAccessStream());
    var width = image.PixelWidth;
    var height = image.PixelHeight;
    if (width < 3840 || height < 2160)
    {
        needSuperResolution = true;
        if (width * 9 > height * 16)
        {
            File.Copy(file, "todo/height/" + Path.GetFileName(file), true);
        }
        else
        {
            File.Copy(file, "todo/width/" + Path.GetFileName(file), true);
        }
    }
}

if (needSuperResolution)
{
    Console.WriteLine("需要超分部分图片");
    return;
}

var sqls = new List<string>();
int count = 0;
var error = new List<string>();

var client = new PixivClient("127.0.0.1:10809");
client.HttpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN");

await Parallel.ForEachAsync(files, async (file, _) =>
{
    try
    {
        if (TryGetPidFromImageFile(file, out int pid, out int p))
        {
            var info = await client.GetIllustInfoAsync(pid);
            var filename = $"[{FileNameReplace(info.UserName)}] {FileNameReplace(info.Title)} [{pid}_p{p}].webp";
            var title = info.Title;
            var auther = info.UserName;
            var description = info.Description.Replace("<br />", "\\n").Replace("'", "\\'");
            var tags = info.Tags.Select(t => t.Name).Concat(info.Tags.Select(x => x.Translation)).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var url = $"https://file.xunkong.cc/wallpaper/{pid}_p{p}.webp";
            var source = $"https://www.pixiv.net/artworks/{pid}";
            var sql = $"""
                       INSERT INTO wallpapers (FileName, Title, Author, Description, Tags, Url, Source) VALUES ('{filename}', '{title}', '{auther}', '{description}', '{string.Join(';', tags)}', '{url}', '{source}') ON DUPLICATE KEY UPDATE Id=Id;
                       """;
            lock (sqls)
            {
                sqls.Add(sql);
            }
        }
    }
    catch
    {
        error.Add(file);
    }
    finally
    {
        lock (sqls)
        {
            Console.WriteLine(++count);
        }
    }

});


for (int i = 0; i < 5; i++)
{
    sqls = sqls.OrderBy(x => Random.Shared.Next()).ToList();
}

File.WriteAllLines("sql.sql", sqls);

foreach (var item in error)
{
    Console.WriteLine(item);
}


static bool TryGetPidFromImageFile(string path, out int pid, out int p)
{
    pid = 0;
    p = 0;
    var fileName = Path.GetFileName(path);
    var match = Regex.Match(fileName, @"(\d+)_p(\d+)");
    if (!int.TryParse(match.Groups[1].Value, out pid))
    {
        return false;
    }
    if (!int.TryParse(match.Groups[2].Value, out p))
    {
        return false;
    }
    return true;
}



static string FileNameReplace(string name)
{
    name = name.Replace("\"", "\u2033");
    name = name.Replace("<", "\u02C2");
    name = name.Replace(">", "\u02C3");
    name = name.Replace("|", "\uFFE8");
    name = name.Replace(":", "\u0589");
    name = name.Replace("*", "\u2217");
    name = name.Replace("?", "\uFFEF");
    name = name.Replace("/", "\u2215");
    name = name.Replace("\\", "\u29F5");
    foreach (var item in Path.GetInvalidFileNameChars())
    {
        name = name.Replace(item, ' ');
    }
    while (name.Contains("  "))
    {
        name = name.Replace("  ", " ");
    }
    return name.Trim();
}
