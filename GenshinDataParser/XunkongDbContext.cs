using Microsoft.EntityFrameworkCore;
using Xunkong.GenshinData.Achievement;
using Xunkong.GenshinData.Character;
using Xunkong.GenshinData.Material;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunkong.GenshinData.Weapon;

namespace GenshinDataParser;

internal class XunkongDbContext : DbContext
{

    private static JsonSerializerOptions _options = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(Config.MySqlConnectionString, new MySqlServerVersion("8.0.29")).EnableSensitiveDataLogging(true);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AchievementGoalModel>().ToTable("Info_Achievement_Goal");
        modelBuilder.Entity<AchievementGoalModel>().Property(x => x.RewardNameCard).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<NameCard>(str, _options));

        modelBuilder.Entity<AchievementItemModel>().Ignore(x => x.IsShow);
        modelBuilder.Entity<AchievementItemModel>().ToTable("Info_Achievement_Item");
        modelBuilder.Entity<AchievementItemModel>().Property(x => x.TriggerConfig).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<TriggerConfig>(str, _options));

        modelBuilder.Entity<Reward>().ToTable("info_reward");
        modelBuilder.Entity<Reward>().HasKey(x => x.RewardId);
        modelBuilder.Entity<Reward>().Property(x => x.RewardItemList).HasConversion(list => JsonSerializer.Serialize(list, _options), str => JsonSerializer.Deserialize<List<RewardItem>>(str, _options));

        modelBuilder.Entity<MaterialItemModel>().ToTable("Info_Material");
        modelBuilder.Entity<MaterialItemModel>().Ignore(x => x.PicPath);

        modelBuilder.Entity<CharacterInfo>().ToTable("info_character_v1");
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Talents).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterTalent>>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Constellations).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterConstellation>>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Promotions).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterPromotion>>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.NameCard).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<NameCard>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Food).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<Food>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Stories).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterStory>>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Voices).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterVoice>>(str, _options));
        modelBuilder.Entity<CharacterInfo>().Property(x => x.Outfits).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<CharacterOutfit>>(str, _options));



        modelBuilder.Entity<WeaponInfo>().ToTable("info_weapon_v1");
        modelBuilder.Entity<WeaponInfo>().Property(x => x.Properties).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<WeaponProperty>>(str, _options));
        modelBuilder.Entity<WeaponInfo>().Property(x => x.Skills).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<WeaponSkill>>(str, _options));
        modelBuilder.Entity<WeaponInfo>().Property(x => x.Promotions).HasConversion(obj => JsonSerializer.Serialize(obj, _options), str => JsonSerializer.Deserialize<List<WeaponPromotion>>(str, _options));
    }
}




