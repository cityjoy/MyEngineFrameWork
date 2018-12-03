using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 二进制，序列化相关的助手类
    /// </summary>
 
    public sealed class BinaryHelper
    {

        /// <summary>
        /// 序列化一个类
        /// </summary>
        /// <param name="instance">要序列化的类的实例</param>
        public static byte[] SerializeClass<T>(T instance) where T : class, new()
        {
            try
            {
                if (instance == null)
                {
                    return null;
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, instance);
                    byte[] binarys = stream.GetBuffer();
                    return binarys;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }

        /// <summary>
        /// 序列化一个类,并将二进制数据保存到文件
        /// </summary>
        /// <param name="instance">要序列化的类的实例</param>
        /// <param name="binaryFilePath">要保存的二进制文件的物理地址</param>
        public static bool SerializeClass<T>(T instance, string binaryFilePath) where T : class, new()
        {
            try
            {
                if (instance == null)
                {
                    return false;
                }
                using (FileStream fileStream = new FileStream(binaryFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, instance);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 反序列化二进制数据为一个类
        /// </summary>
        public static T DeSerializeClass<T>(byte[] binarys) where T : class, new()
        {
            try
            {
                if (binarys == null)
                {
                    return null;
                }
                T instance = new T();
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(binarys, 0, binarys.Length);
                    stream.Position = 0;
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    instance = binaryFormatter.Deserialize(stream) as T;
                    return instance;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }

        /// <summary>
        /// 反序列化文件中的二进制数据为一个类
        /// </summary>
        /// <param name="instance">传递类的实例</param>
        /// <param name="binaryFilePath">保存的二进制文件的物理地址</param>
        public static bool DeSerializeClass<T>(ref T instance, string binaryFilePath) where T : class, new()
        {
            try
            {
                if (instance == null)
                {
                    instance = new T();
                }
                if (!File.Exists(binaryFilePath))
                {
                    return false;
                }
                using (FileStream fileStream = new FileStream(binaryFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (fileStream.Length <= 0)
                    {
                        return false;
                    }
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    instance = binaryFormatter.Deserialize(fileStream) as T;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static byte[] ConvertStreamToBytes(Stream fileStream)
        {
            byte[] fileData = new byte[fileStream.Length];
            using (fileStream)
            {
                fileStream.Read(fileData, 0, Convert.ToInt32(fileStream.Length));
            }
            return fileData;
        }

        /// <summary>
        /// 转换10进制为36进制
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string ConvertBase10To36(int i)
        {
            string s = "";
            int j = 0;
            while (i > 36)
            {
                j = i % 36;
                if (j <= 9)
                    s += j.ToString();
                else
                    s += Convert.ToChar(j - 10 + 'a');
                i = i / 36;
            }
            if (i <= 9)
                s += i.ToString();
            else
                s += Convert.ToChar(i - 10 + 'a');
            Char[] c = s.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }

    }
}
