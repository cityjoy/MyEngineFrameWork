using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 上传结果
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// 存储点
        /// </summary>
        public int Store
        {
            get;
            set;
        }

        /// <summary>
        /// 上传返回的相对路径
        /// </summary>
        public string RelativePath
        {
            get;
            set;
        }

        /// <summary>
        /// 上传返回的完整路径，包含完整域名
        /// </summary>
        public string FullPath
        {
            get;
            set;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 删除结果
    /// </summary>
    public class DeleteFileResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }



    /// <summary>
    /// 文件存储处理器接口
    /// </summary>

    public interface IFileStoreHandler
    {

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="store"></param>
        /// <param name="saveRelativePath"></param>
        /// <param name="thumbConfig">如果是图片，且要生成缩略图，此项必须有值，格式为 100x100,150x150,200x200</param>
        /// <param name="cutImage">如果是上传图片，可设置此参数来指定裁剪图片，如：200x150|0表示强制裁剪为200x150尺寸(不约束宽高比例)，200x150|1表示裁剪为200x150尺寸(约束宽高比例)</param>
        /// <returns></returns>
        UploadResult Upload(Stream fileStream, int store, string saveRelativePath, string thumbConfig, string cutImage, params string[] sourthFile);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        DeleteFileResult Delete(int store, string saveRelativePath, params string[] sourthFile);


    }
}
