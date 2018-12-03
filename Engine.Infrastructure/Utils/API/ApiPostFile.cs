using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// Api上传的文件模型
    /// </summary>
    /// <remarks>

    public class ApiPostFile
    {
        /// <summary>
        /// 表单名
        /// </summary>
        public string FormName
        {
            get;
            set;
        }

        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream
        {
            get;
            set;
        }

        /// <summary>
        /// 源文件名
        /// </summary>
        public string SourceFileName
        {
            get;
            set;
        }

    }
}
