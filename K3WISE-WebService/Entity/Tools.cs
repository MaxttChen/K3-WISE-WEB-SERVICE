using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Services;

namespace K3WISE_WebService.Entity
{
    public static class Tools
    {

        /// <summary>
        /// 检验JSON格式
        /// </summary>
        /// <param name="JsonText">json文本</param>
        /// <returns>无错误提示返回空字符串""，有错误返回 错误信息</returns>
        public static String ValiJson(string JsonText)
        {
            var Msg = "";
            //判断是否JSON格式       
            try
            {
                var obj = Newtonsoft.Json.Linq.JToken.Parse(JsonText);
            }
            catch (Exception jex)
            {
                Msg = jex.ToString();               
            }
            return Msg;
        }

        //检验Json格式
        public static ResponseJson ValidateJsonFormat(string JsonText)
        {
            var ResponseJson = new Entity.ResponseJson();
            //判断是否JSON格式
            if ((JsonText.StartsWith("{") && JsonText.EndsWith("}")) || //For object
                (JsonText.StartsWith("[") && JsonText.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = Newtonsoft.Json.Linq.JToken.Parse(JsonText);
                    //解析成功
                    ResponseJson.StatusCode = Entity.StatusCode.Success;
                    return ResponseJson;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    ResponseJson.StatusCode = Entity.StatusCode.Fail;
                    ResponseJson.Message = jex.ToString();
                    return ResponseJson;
                }
                catch (Exception ex) //some other exception
                {
                    ResponseJson.StatusCode = Entity.StatusCode.Fail;
                    ResponseJson.Message = ex.ToString();
                    return ResponseJson;
                }
            }
            else
            {
                ResponseJson.StatusCode = Entity.StatusCode.Fail;
                ResponseJson.Message = "JSON格式不正确";
                return ResponseJson;
            }
        }


        //从数据流获取请求内容
        public static String GetRequestBody()
        {
            using (System.IO.Stream stream = HttpContext.Current.Request.InputStream)
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                string bodyText = reader.ReadToEnd();
                return bodyText;
            }
            
        }

    }
}