using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Engine.Infrastructure.Utils;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 文件管理器
    /// </summary>
    public class FileManager
    {


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="saveRelativePath"></param>
        /// <param name="thumbConfig"></param>
        /// <param name="cutImage"></param>
        /// <returns></returns>
        public static UploadResult UploadFile(Stream fileStream, int store, string saveRelativePath, string thumbConfig, string cutImage, params string[] sourthFile)
        {
            UploadResult uploadResult = null;
            try
            {
                IFileStoreHandler fileStoreHandler = FileStoreFactory.CreateFileStoreHandler(store);

                uploadResult = fileStoreHandler.Upload(fileStream, store, saveRelativePath, thumbConfig, cutImage, sourthFile);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                uploadResult = null;
            }
            return uploadResult;
        }


        /// <summary>
        /// 删除指定路径下的物理文件
        /// </summary>
        /// <param name="saveRelativePath"></param>
        /// <returns></returns>
        public static DeleteFileResult DeleteFile(int store, string saveRelativePath, params string[] sourthFile)
        {
            DeleteFileResult result = null;

            try
            {
                IFileStoreHandler fileStoreHandler = FileStoreFactory.CreateFileStoreHandler(store);

                result = fileStoreHandler.Delete(store, saveRelativePath, sourthFile);
            }
            catch (Exception ex)
            {
                result = null;
                LogHelper.WriteLog(ex);
            }

            return result;
        }

    }
}
