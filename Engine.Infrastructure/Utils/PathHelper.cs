using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 对URL路径或其它路径的操作助手
    /// </summary>

    public sealed class PathHelper
    {
        /// <summary>
        /// 合并路径串
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombile(string path1, string path2)
        {
            path2 = path2.TrimStart(new char[] { '/', '\\' });
            string path = Path.Combine(path1, path2);

            return path;
        }

        /// <summary>
        /// 获取文件扩展名，不包含"."
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileExtensionWithouDot(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (extension.StartsWith("."))
            {
                return extension.TrimStart('.');
            }
            return extension;
        }

        /// <summary>
        /// 新网址合并上旧网址的GET参数
        /// </summary>
        /// <param name="newUrlInput">新网址</param>
        /// <param name="orgUrlInput">现网址</param>
        /// <param name="newParamKey">新的参数名</param>
        /// <param name="newParamValue">新的参数值</param>
        public static string ResolveParamUrl(string newUrlInput, string orgUrlInput, string newParamKey, string newParamValue)
        {
            string getParams = string.Empty;

            //判断指定的网址本身是否带参数
            if (!orgUrlInput.Contains("?"))
            {
                getParams = string.Concat(newParamKey, "=", newParamValue);
            }
            else
            {
                //判断指定的网址是否包含指定的参数
                if (!orgUrlInput.Contains(string.Concat(newParamKey, "=")))
                {
                    getParams = (orgUrlInput.Substring(orgUrlInput.IndexOf("?") + 1) + string.Concat("&", newParamKey, "=", newParamValue));
                }
                else
                {
                    string paramString = string.Empty;

                    //第一部分参数字串
                    string paramString1 = orgUrlInput.Substring(orgUrlInput.IndexOf("?") + 1, orgUrlInput.IndexOf(newParamKey + "=") - orgUrlInput.IndexOf("?") - 1);
                    //第二部分参数字串
                    string paramString2 = orgUrlInput.Substring(orgUrlInput.IndexOf(newParamKey + "="));

                    //如果含有"&",第二部分参数字串就是不止包含指定的参数
                    if (paramString2.Contains("&"))
                    {
                        paramString = string.Concat(paramString1, newParamKey, "=", newParamValue, paramString2.Substring(paramString2.IndexOf("&")));
                    }
                    else
                    {
                        paramString = string.Concat(paramString1, newParamKey, "=", newParamValue);
                    }

                    getParams = paramString;
                }
            }

            if (newUrlInput.Contains("?"))
            {
                return string.Concat(newUrlInput, "&", getParams);
            }
            else
            {
                return string.Concat(newUrlInput, "?", getParams);
            }
        }

        /// <summary>
        /// 给网址添加新参数,并保持原本的参数(单个新参数)
        /// </summary>
        /// <param name="urlInput">可能带参数的网址</param>
        /// <param name="newParamKey">新的参数名</param>
        /// <param name="newParamValue">新的参数值</param>
        public static string ResolveParamUrl(string urlInput, string newParamKey, string newParamValue)
        {
            if (string.IsNullOrEmpty(newParamValue))
            {
                if (!urlInput.Contains(newParamKey + "="))
                {
                    return urlInput;
                }
            }
            //判断指定的网址本身是否带参数
            if (!urlInput.Contains("?"))
            {
                return string.Concat(urlInput, "?", newParamKey, "=", newParamValue);
            }
            else
            {
                //判断指定的网址是否包含指定的参数
                if (!urlInput.Contains(string.Concat(newParamKey, "=")))
                {
                    return string.Concat(urlInput, "&", newParamKey, "=", newParamValue);
                }
                else
                {
                    string paramString = string.Empty;

                    //不带参数的纯网址
                    string pathString = urlInput.Substring(0, urlInput.IndexOf("?") + 1);
                    //第一部分参数字串
                    string paramString1 = urlInput.Substring(urlInput.IndexOf("?") + 1, urlInput.IndexOf(newParamKey + "=") - urlInput.IndexOf("?") - 1);
                    //第二部分参数字串
                    string paramString2 = urlInput.Substring(urlInput.IndexOf(newParamKey + "="));

                    //如果含有"&",第二部分参数字串就是不止包含指定的参数
                    if (paramString2.Contains("&"))
                    {
                        paramString = string.Concat(pathString, paramString1, newParamKey, "=", newParamValue, paramString2.Substring(paramString2.IndexOf("&")));
                    }
                    else
                    {
                        paramString = string.Concat(pathString, paramString1, newParamKey, "=", newParamValue);
                    }

                    return paramString;
                }
            }
        }

        /// <summary>
        /// 给网址添加新参数,并保持原本的参数(多个新参数)
        /// </summary>
        /// <param name="urlInput">可能带参数的网址</param>
        /// <param name="dicNewParams">新的参数名值对集合</param>
        public static string ResolveParamsUrl(string urlInput, IDictionary<string, string> dicNewParams)
        {
            if (dicNewParams.Count < 1)
            {
                return urlInput;
            }

            string paramString = urlInput;

            foreach (KeyValuePair<string, string> newParam in dicNewParams)
            {
                paramString = ResolveParamUrl(paramString, newParam.Key, newParam.Value);
            }

            return paramString;
        }

        /// <summary>
        /// 从指定网址中去除掉某个参数
        /// </summary>
        /// <param name="urlInput"></param>
        /// <param name="paramName"></param>
        public static string RemoveUrlParam(string urlInput, string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
                return urlInput;
            int hasParamPos = urlInput.IndexOf("?");
            if (hasParamPos < 0)
                return urlInput;

            string paramKey = string.Concat(paramName, "=");
            int paramKeyPos = urlInput.IndexOf(paramKey);

            if (paramKeyPos >= 0)
            {
                string paramString = urlInput.Substring(hasParamPos + 1);
                string prevChar = urlInput.Substring(paramKeyPos - 1, 1);
                if (prevChar == "?")
                {
                    if (!paramString.Contains("&"))
                        return urlInput.Substring(0, hasParamPos);
                    else
                    {
                        int andPos = paramString.IndexOf("&");
                        string paramString2 = paramString.Substring(andPos + 1);
                        return string.Concat(urlInput.Substring(0, hasParamPos + 1), paramString2);
                    }
                }
                else
                {
                    string paramString3 = urlInput.Substring(paramKeyPos);
                    int andPos = paramString3.IndexOf("&");

                    if (andPos > -1)
                    {
                        string paramString4 = paramString3.Substring(andPos + 1);
                        return string.Concat(urlInput.Substring(0, paramKeyPos), paramString4);
                    }
                    else
                    {
                        return urlInput.Substring(0, paramKeyPos - 1);
                    }
                }
            }
            return urlInput;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlInput"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static string RemoveUrlParam(string urlInput, params string[] paramNames)
        {
            if (paramNames != null)
            {
                foreach (string sParamKey in paramNames)
                {
                    urlInput = RemoveUrlParam(urlInput, sParamKey);
                }
            }
            return urlInput;
        }
    }
}
