using System.Collections.Generic;

namespace MELI.Helpers
{
    public static class StringExtension
    {
        public static string Where(this string query, Dictionary<string, string> parameterValueFilter)
        {
            var index = 0;
            foreach (var pair in parameterValueFilter)
            {
                if (index == 0)
                    query += "WHERE ";
                else
                    query += " AND ";

                query += $"{pair.Key} = {pair.Value}";
                index++;
            }
            return query;
        }
    }
}
