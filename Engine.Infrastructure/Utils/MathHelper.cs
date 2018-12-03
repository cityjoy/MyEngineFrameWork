using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 数学相关助手
    /// </summary>
   
    public sealed class MathHelper
    {

        /// <summary>
        /// 产生随机整数
        /// 以GUID的哈希值为种子值
        /// </summary>
        /// <returns></returns>
        public static int GetRandomInt()
        {
            return GetRandomInt(null, null);
        }

        /// <summary>
        /// 产生随机整数
        /// 以GUID的哈希值为种子值
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int GetRandomInt(int? minValue, int? maxValue)
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random rand = new Random(seed);
            int result;
            if (minValue != null && maxValue != null)
            {
                result = rand.Next(minValue.Value, maxValue.Value);
            }
            else
            {
                if (maxValue != null)
                {
                    result = rand.Next(maxValue.Value);
                }
                else
                {
                    result = rand.Next();
                }
            }

            return result;
        }

        /// <summary>
        /// 产生随机数，介于0.0与1.0之间的随机数字
        /// 以GUID的哈希值为种子值
        /// </summary>
        /// <returns></returns>
        public static double GetRandomDouble()
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random rand = new Random(seed);
            double result = rand.NextDouble();

            return result;
        }

        
        /// <summary>
        /// 合并列表至列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetList">合并至该列表</param>
        /// <param name="sourceList">进行合并的列表</param>
        /// <returns></returns>
        public static void MergeToList<T>(List<T> targetList, List<T> sourceList)
        {
            MergeToList<T>(targetList, sourceList, false);
        }
        /// <summary>
        /// 合并列表至列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetList">合并至该列表</param>
        /// <param name="sourceList">进行合并的列表</param>
        /// <param name="distinct">是否去除掉重复项</param>
        /// <returns></returns>
        public static void MergeToList<T>(List<T> targetList, List<T> sourceList, bool distinct)
        {
            if (targetList == null || sourceList == null)
            {
                return;
            }

            foreach (T item in sourceList)
            {
                if (distinct && targetList.Contains(item))
                {
                    continue;
                }
                else
                {
                    targetList.Add(item);
                }
            }
        }

        /// <summary>
        /// 列表转换为数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ConvertListToArray<T>(IList<T> list)
        {
            if (list == null) { return null; }

            T[] arr = new T[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                arr[i] = item;
            }

            return arr;
        }

    }
}
