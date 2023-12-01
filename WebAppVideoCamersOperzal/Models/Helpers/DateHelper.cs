using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace WebAppVideoCamersOperzal.Models.Helpers
{
    public static class DateHelper
    {

        public static string ToDate(long? time)
        {
            if (time == null)
            {
                return "";
            }
            DateTime d = new DateTime(1970, 1, 1)                
                .AddSeconds(double.Parse(time.ToString()) + double.Parse(TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds.ToString()));
            return d.ToString("dd.MM.yyyy");
        }


        public static string ToDateTime(long? time)
        {
            if (time == null)
            {
                return "";
            }
            DateTime d = new DateTime(1970, 1, 1)
                .AddSeconds(double.Parse(time.ToString()) + double.Parse(TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds.ToString()));
            return d.ToString("dd.MM.yyyy HH:mm:ss");
        }

    }
}
