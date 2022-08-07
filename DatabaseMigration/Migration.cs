using Xunkong.Desktop.Database.Old.Services;

namespace DatabaseMigration;

public static class Migration
{

    private static string oldDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Xunkong\Data\XunkongData.db");

    private static string newDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Xunkong\Database\XunkongData.db");



    public static bool NeedToMigrate()
    {
        return File.Exists(oldDbFile) && !File.Exists(newDbFile);
    }


    public static void Migrate()
    {
        Console.WriteLine("已找到需要迁移的数据库：");
        Console.WriteLine(oldDbFile);

        Console.WriteLine("即将开始迁移");
        using var dbContext = new XunkongDbContext($"Data Source={oldDbFile};");
        dbContext.Database.Migrate();

        using var dapper = DatabaseProvider.CreateConnection();

        using var t = dapper.BeginTransaction();

        Console.ForegroundColor = ConsoleColor.Gray;


        Console.WriteLine("正在迁移签到记录");
        var dailyCheckItems = dbContext.DailyCheckInItems.AsNoTracking().ToList();
        dapper.Execute("INSERT OR REPLACE INTO DailySignInHistory (Uid, Date, Time) VALUES (@Uid, @Date, @Time);", dailyCheckItems, t);

        Console.WriteLine("正在迁移用户信息");
        var genshinRoles = dbContext.UserGameRoleInfos.AsNoTracking().ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO GenshinRoleInfo (Uid, GameBiz, Region, Nickname, Level, IsChosen, RegionName, IsOfficial, Cookie)
        VALUES (@Uid, @GameBiz, @Region, @Nickname, @Level, @IsChosen, @RegionName, @IsOfficial, @Cookie);
        """, genshinRoles, t);

        var hoyolabUsers = dbContext.UserInfos.AsNoTracking().ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO HoyolabUserInfo (Uid, Nickname, Introduce, Avatar, Gender, AvatarUrl, Pendant, Cookie)
        VALUES (@Uid, @Nickname, @Introduce, @Avatar, @Gender, @AvatarUrl, @Pendant, @Cookie);
        """, hoyolabUsers, t);

        Console.WriteLine("正在迁移设置");
        var settings = dbContext.UserSettings.AsNoTracking().ToList();
        dapper.Execute("INSERT OR REPLACE INTO Setting (Key, Value) VALUES (@Key, @Value);", settings, t);


        Console.WriteLine("正在迁移深境螺旋记录");
        var abyss = dbContext.SpiralAbyssInfos.AsNoTracking()
                                            .Include(x => x.DamageRank)
                                            .Include(x => x.DefeatRank)
                                            .Include(x => x.EnergySkillRank)
                                            .Include(x => x.NormalSkillRank)
                                            .Include(x => x.RevealRank)
                                            .Include(x => x.TakeDamageRank)
                                            .Include(x => x.Floors)
                                            .ThenInclude(x => x.Levels)
                                            .ThenInclude(x => x.Battles)
                                            .ThenInclude(x => x.Avatars)
                                            .ToList();
        var abyssObjs = abyss.Select(info => new
        {
            Uid = info.Uid,
            ScheduleId = info.ScheduleId,
            StartTime = info.StartTime,
            EndTime = info.EndTime,
            TotalBattleCount = info.TotalBattleCount,
            TotalWinCount = info.TotalWinCount,
            MaxFloor = info.MaxFloor,
            TotalStar = info.TotalStar,
            Value = JsonSerializer.Serialize(info),
        }).ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO SpiralAbyssInfo (Uid, ScheduleId, StartTime, EndTime, TotalBattleCount, TotalWinCount, MaxFloor, TotalStar, Value)
        VALUES (@Uid, @ScheduleId, @StartTime, @EndTime, @TotalBattleCount, @TotalWinCount, @MaxFloor, @TotalStar, @Value);
        """, abyssObjs, t);


        Console.WriteLine("正在迁移旅行札记记录");
        var travelNotesAwardItems = dbContext.TravelRecordAwardItems.AsNoTracking().ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO TravelNotesAwardItem (Uid, Year, Month, Type, ActionId, ActionName, Time, Number)
        VALUES (@Uid, @Year, @Month, @Type, @ActionId, @ActionName, @Time, @Number);
        """, travelNotesAwardItems, t);

        var travelNotesMonthData = dbContext.TravelRecordMonthDatas.AsNoTracking().Include(x => x.PrimogemsGroupBy).ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO TravelNotesMonthData (Uid, Year, Month, CurrentPrimogems, CurrentMora, LastPrimogems, LastMora, CurrentPrimogemsLevel, PrimogemsChangeRate, MoraChangeRate, PrimogemsGroupBy)
        VALUES (@Uid, @Year, @Month, @CurrentPrimogems, @CurrentMora, @LastPrimogems, @LastMora, @CurrentPrimogemsLevel, @PrimogemsChangeRate, @MoraChangeRate, @PrimogemsGroupBy);
        """, travelNotesMonthData, t);

        var webtools = dbContext.WebToolItems.AsNoTracking().ToList();
        dapper.Execute("INSERT OR REPLACE INTO WebToolItem (Title, Icon, [Order], Url, JavaScript) VALUES (@Title, @Icon, @Order, @Url, @JavaScript);", webtools, t);

        Console.WriteLine("正在迁移祈愿记录");
        var wishlogs = dbContext.WishlogItems.AsNoTracking().ToList();
        dapper.Execute("""
        INSERT OR REPLACE INTO WishlogItem (Uid, Id, WishType, Time, Name, Language, ItemType, RankType, QueryType)
        VALUES (@Uid, @Id, @WishType, @Time, @Name, @Language, @ItemType, @RankType, @QueryType);
        """, wishlogs, t);

        var wishlogUrls = dbContext.WishlogAuthkeys.AsNoTracking().ToList();
        dapper.Execute("INSERT OR REPLACE INTO WishlogUrl (Uid, Url, DateTime) VALUES (@Uid, @Url, @DateTime);", wishlogUrls, t);

        Console.WriteLine("正在提交更改");
        t.Commit();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("数据库迁移完成，请检查是否有数据丢失");
        Console.WriteLine("迁移后的数据库位置：");
        Console.WriteLine(DatabaseProvider.SqlitePath);
    }





}
