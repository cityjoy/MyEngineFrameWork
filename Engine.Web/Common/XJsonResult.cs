using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web;
using System.Text.RegularExpressions;

namespace Engine.Web.Common
{
    /// <summary>
    /// 自定义MVC JsonResult
    /// 使用自定义JSON序列化代替微软原生JSON序列化，修正DateTime数据的序列化
    /// </summary>
    public class XJsonResult : JsonResult
    {

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == System.Web.Mvc.JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JsonRequest_GetNotAllowed");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                string json = JsonConvert.SerializeObject(this.Data);
                json = Regex.Replace(json, @"""\\/Date\((?<Timestamp>\-?\d+)(\+\d+)?\)\\/""", "${Timestamp}");
                json = Regex.Replace(json, @"""(?<yyyy>\d+)-(?<MM>\d+)-(?<dd>\d+)T(?<HH>\d+):(?<mm>\d+):(?<ss>\d+)(\.(?<fff>\d+))?""", "\"${yyyy}-${MM}-${dd} ${HH}:${mm}:${ss}\"");

                response.Write(json);
            }
        }
    }
}
