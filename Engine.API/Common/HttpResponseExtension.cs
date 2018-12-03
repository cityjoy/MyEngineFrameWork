using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Engine.API.Model;
using System.Net;

namespace Engine.API.Common
{
    /// <summary>
    ///HttpResponse扩展
    /// </summary>
    public class HttpResponseExtension
    {
        /// <summary>
        /// 返回ResponseMessage
        /// </summary>
        /// <param name="resultMsg"></param>
        /// <returns></returns>
        public static HttpResponseMessage ToResponseMessage(ResultMsg resultMsg)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            if (resultMsg != null)
            {
                result = new HttpResponseMessage { 
                     //封装处理异常信息，返回指定JSON对象
                        Content = new StringContent(JsonConvert.SerializeObject(resultMsg)),
                        ReasonPhrase = "Exception"
                };

            }
            return result;
        }


    }
}