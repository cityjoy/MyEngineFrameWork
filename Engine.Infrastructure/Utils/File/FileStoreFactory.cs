using System;
using System.Collections.Generic;
using System.Text;
using Engine.Infrastructure.Utils;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 文件存储工厂
    /// </summary>

    public class FileStoreFactory
    {

        /// <summary>
        /// 创建文件存储处理器
        /// </summary>
        /// <returns></returns>
        public static IFileStoreHandler CreateFileStoreHandler(int store)
        {
            switch (store)
            {
                case 0:
                default:
                    return new DefaultFileStoreHandler();
            }
        }


    }
}
