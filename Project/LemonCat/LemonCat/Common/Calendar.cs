using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LemonCat.Common
{
    public static class Calendar
    {
        public static string GetNow()
        {
            return DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-");
        }
        public static int SubDate(string d1, string d2)
        {
            d1 = d1.Replace("-", "/");
            d2 = d2.Replace("-", "/");
            DateTime dt1 = DateTime.ParseExact(d1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dt2 = DateTime.ParseExact(d2, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return (int)(dt2 - dt1).TotalDays;
        }
        public static int GetHourStart()
        {
            return int.Parse(DateTime.Now.ToString("HH"));
        }
        public static List<string> GetDatesBetween(string eDate)
        {
            eDate = eDate.Replace("-", "/");
            DateTime endDate = DateTime.ParseExact(eDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = DateTime.Now; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            List<string> result = new List<string>();
            foreach (var item in allDates)
            {
                result.Add(item.ToString("dd/MM/yyyy").Replace("/", "-"));
            }
            return result;
        }
    }
}