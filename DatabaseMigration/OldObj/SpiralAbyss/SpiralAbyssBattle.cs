﻿

namespace Xunkong.Core.Old.SpiralAbyss
{

    /// <summary>
    /// 深境螺旋一场战斗
    /// </summary>
    [Table("SpiralAbyss_Battles")]
    [Index(nameof(Time))]
    public class SpiralAbyssBattle
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int SpiralAbyssLevelId { get; set; }


        [JsonPropertyName("index")]
        public int Index { get; set; }


        [JsonPropertyName("timestamp"), JsonConverter(typeof(SpiralAbyssTimeJsonConverter))]
        public DateTimeOffset Time { get; set; }


        [JsonPropertyName("avatars")]
        public List<SpiralAbyssAvatar> Avatars { get; set; }


    }
}
