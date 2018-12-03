using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    public class StringUtils
    {
        /// <summary>
        /// 年级中文
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static string GetGradeString(int grade)
        {
            string str;

            int mod = grade % 10;
            int cf = grade / 10;
            switch (cf)
            {
                case 1:
                default: // 11 ~ 13: 高一 ~ 高三
                    str = string.Concat("高", StringHelper.ConvertIntToChinese(mod), "年");
                    break;
                case 0: // 1 ~ 3: 初一 ~ 初三
                    str = string.Concat("初", StringHelper.ConvertIntToChinese(mod), "年");
                    break;
            }

            return str;
        }
        /// <summary>
        /// 班级中文
        /// </summary>
        /// <param name="banji"></param>
        /// <returns></returns>
        public static string GetClassString(int banji)
        {
            string str = string.Concat(StringHelper.ConvertIntToChinese(banji), "班");

            return str;
        }
        /// <summary>
        /// 学期中文
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static string GetTermString(int term)
        {
            string str = string.Concat("第", StringHelper.ConvertIntToChinese(term), "学期");

            return str;
        }
        /// <summary>
        /// Request值转转成INT
        /// </summary>
        /// <param name="requestStr"></param>
        /// <returns></returns>
        public static int GetRequestToInt(string requestStr, int defaultVal = 0)
        {
            return string.IsNullOrWhiteSpace(requestStr) ? defaultVal : Convert.ToInt32(requestStr);
        }
    }
}
