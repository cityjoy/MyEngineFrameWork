using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
 

namespace  Engine.Infrastructure.Utils
{
    /// <summary>
    /// 文件存储助手
    /// </summary>
    public class FileStoreUtil
    {

        /// <summary>
        /// 缩略图保存的目录名前缀，位于图片保存的目录
        /// </summary>
        public const string THUMB_DIRECTORY_NAME_PREFIX = "thumbs";

        /// <summary>
        /// 获取文件服务器站点url，不以/结束，如http://filestore.51xuanxiao.com
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static string GetFileStoreSite(int store)
        {
            string url;
            switch (store)
            {
                case 0:
                default:
                    url = Constants.FILESTORE_SITE;
                    break;
                case 1:
                    url = Constants.MAIN_SITE;
                    break;
            }
            return url;
        }

        /// <summary>
        /// 文件地址
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="relSavePath"></param>
        /// <returns></returns>
        public static string GetUploadFileUrl(int storeType, string relSavePath)
        {
            string site = GetFileStoreSite(storeType);
            string url = string.Concat(site, "/", relSavePath);

            return url;
        }

        /// <summary>
        /// 生成缩略图保存的物理路径
        /// </summary>
        /// <returns></returns>
        public static string GenerateThumbnailSavePath(string savePath, int width, int height)
        {
            string saveDirPath = Path.GetDirectoryName(savePath);
            string ext = Path.GetExtension(savePath);
            string fileName = Path.GetFileNameWithoutExtension(savePath);
            string thumbSaveDirPath = Path.Combine(saveDirPath, FileStoreUtil.THUMB_DIRECTORY_NAME_PREFIX + "_" + fileName + "\\");
            string thumbSavePath = Path.Combine(thumbSaveDirPath, string.Concat(width, "_", height, ext));
            return thumbSavePath;
        }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        /// <param name="store"></param>
        /// <param name="relSavePath">相对路径或绝对地址</param>
        /// <returns></returns>
        public static string GetImageThumbUrl(int store, string relSavePath, int width, int height)
        {
            if (string.IsNullOrEmpty(relSavePath))
            {
                return string.Empty;
            }

            string dirPath = relSavePath.Substring(0, relSavePath.LastIndexOf("/"));
            int nStartPos = relSavePath.LastIndexOf("/") + 1;
            int nEndPos = relSavePath.LastIndexOf(".");
            string onlyFileName = relSavePath.Substring(nStartPos, nEndPos - nStartPos);
            string ext = relSavePath.Substring(nEndPos);

            string url;
            if (Uri.IsWellFormedUriString(relSavePath, UriKind.Absolute))
            {
                url = string.Concat(dirPath, "/", FileStoreUtil.THUMB_DIRECTORY_NAME_PREFIX, "_", onlyFileName, "/", width, "_", height, ext);
            }
            else
            {
                string site = GetFileStoreSite(store);
                url = string.Concat(site, "/", dirPath, "/", FileStoreUtil.THUMB_DIRECTORY_NAME_PREFIX, "_", onlyFileName, "/", width, "_", height, ext);
            }
            return url;
        }

        /// <summary>  
        /// 截取图片中的一部分  
        /// </summary>  
        /// <param name="img">待截取的图片</param>  
        /// <param name="cropperWidth">截取图片的宽</param>  
        /// <param name="cropperHeight">截取图片的高</param>  
        /// <param name="offsetX">水平偏移量</param>  
        /// <param name="offsetY">垂直偏移量</param>  
        /// <param name="imgFormat">截取后的图片保存格式</param>  
        public static Stream ImageCropper(System.Drawing.Image img, int cropperWidth, int cropperHeight, int offsetX, int offsetY)
        {
            using (var bmp = new System.Drawing.Bitmap(cropperWidth, cropperHeight))
            {
                //从Bitmap创建一个System.Drawing.Graphics对象，用来绘制高质量的缩小图。  
                using (var gr = System.Drawing.Graphics.FromImage(bmp))
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                    //把原始图像绘制成上面所设置宽高的截取图  
                    var rectDestination = new System.Drawing.Rectangle(offsetX, offsetY, cropperWidth, cropperHeight);
                    gr.DrawImage(img, 0, 0, rectDestination,
                                 System.Drawing.GraphicsUnit.Pixel);
                    MemoryStream msAvatar = new MemoryStream();
                    //保存截取的图片位图片流  
                    bmp.Save(msAvatar, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return msAvatar;
                }
            }
        }

        /// <summary>  
        /// 生成指定大小的图片  
        /// </summary>  
        /// <param name="maxWidth">生成图片的最大宽度</param>  
        /// <param name="maxHeight">生成图片的最大高度</param>  
        /// <param name="imgFileStream">图片文件流对象</param>  
        public static System.Drawing.Bitmap GenerateThumbnail(int maxWidth, int maxHeight, System.IO.Stream imgFileStream)
        {
            using (var img = System.Drawing.Image.FromStream(imgFileStream))
            {
                var sourceWidth = img.Width;
                var sourceHeight = img.Height;

                var thumbWidth = sourceWidth; //要生成的图片的宽度  
                var thumbHeight = sourceHeight; //要生成图片的的高度  

                //计算生成图片的高度和宽度  
                if (sourceWidth > maxWidth || sourceHeight > maxHeight)
                {
                    var rateWidth = (double)sourceWidth / maxWidth;
                    var rateHeight = (double)sourceHeight / maxHeight;

                    if (rateWidth > rateHeight)
                    {
                        thumbWidth = maxWidth;
                        thumbHeight = (int)Math.Round(sourceHeight / rateWidth);
                    }
                    else
                    {
                        thumbWidth = (int)Math.Round(sourceWidth / rateHeight);
                        thumbHeight = maxHeight;
                    }
                }

                var bmp = new System.Drawing.Bitmap(thumbWidth, thumbHeight);
                using (var gr = System.Drawing.Graphics.FromImage(bmp))
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                    //把原始图像绘制成上面所设置宽高的缩小图  
                    var rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                    gr.DrawImage(img, rectDestination, 0, 0, sourceWidth, sourceHeight,
                                 System.Drawing.GraphicsUnit.Pixel);
                    return bmp;
                }
            }
        }
    }
}
