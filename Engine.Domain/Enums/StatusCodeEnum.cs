
using Engine.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Engine.Domain.Enums
{
    /// <summary>
    /// 状态码枚举
    /// </summary>
    public enum StatusCodeEnum
    {
        [Text("请求(或处理)成功")]
        Success = 200, //请求(或处理)成功

        [Text("内部请求出错")]
        InternalServerError = 500, //内部请求出错

        [Text("不支持的请求")]
        NotImplemented = 501,//不支持的请求

        [Text("未授权标识")]
        Unauthorized = 401,//未授权标识

        [Text("请求参数不完整或不正确")]
        ParameterError = 400,//请求参数不完整或不正确

        [Text("请求的TOKEN失效")]
        TokenInvalid = 403,//请求TOKEN失效

        [Text("请求的资源不在服务器上")]
        NotFound = 404,//请求的资源不在服务器上

        [Text("HTTP请求类型不合法")]
        HttpMehtodError = 405,//HTTP请求类型不合法

        [Text("HTTP请求不合法,请求参数可能被篡改")]
        HttpRequestError = 406,//HTTP请求不合法

        [Text("该URL已经失效")]
        URLExpireError = 407,//HTTP请求不合法

        [Text("请求超时")]
        RequestTimeout = 408,//客户端没有在服务器期望请求的时间内发送请求
    }
}