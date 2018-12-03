using System;
using System.Collections.Generic;
using System.Text;
using Engine.Infrastructure.Utils;

namespace Engine.Infrastructure.Utils
{
    public abstract class DaoBase<Dao> where Dao : DaoBase<Dao>, new()
    {
        public DaoBase()
        {

        }

        /// <summary>
        /// 防sql注入过滤
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string FilterUnsafeSql(string text)
        {
            return StringHelper.FilterUnsafeSql(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string FormatSqlDateTime(DateTime time)
        {
            return StringHelper.FormatSqlDateTime(time);
        }



    }
}
