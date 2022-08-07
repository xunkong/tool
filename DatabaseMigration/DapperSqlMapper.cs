using System.Data;

namespace DatabaseMigration
{
    internal class DapperSqlMapper
    {

        public class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
        {
            public override DateTimeOffset Parse(object value)
            {
                if (value is string str)
                {
                    return DateTimeOffset.Parse(str);
                }
                else
                {
                    return new DateTimeOffset();
                }
            }

            public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
            {
                parameter.Value = value.ToString();
            }

        }



        public class TravelNotesPrimogemsMonthGroupStatsListHandler : SqlMapper.TypeHandler<List<Xunkong.Core.Old.TravelRecord.TravelRecordPrimogemsMonthGroupStats>>
        {
            public override List<Xunkong.Core.Old.TravelRecord.TravelRecordPrimogemsMonthGroupStats> Parse(object value)
            {
                if (value is string str)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        return JsonSerializer.Deserialize<List<Xunkong.Core.Old.TravelRecord.TravelRecordPrimogemsMonthGroupStats>>(str)!;
                    }
                }
                return new();
            }

            public override void SetValue(IDbDataParameter parameter, List<Xunkong.Core.Old.TravelRecord.TravelRecordPrimogemsMonthGroupStats> value)
            {
                parameter.Value = JsonSerializer.Serialize(value);
            }
        }


    }
}
