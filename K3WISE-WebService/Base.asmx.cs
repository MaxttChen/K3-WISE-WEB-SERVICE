using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;


namespace K3WISE_WebService
{
    /// <summary>
    /// Base 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/",Name ="Voucher")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1, Name = "Voucher")]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class Base : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            Context.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Charset = "utf-8";
            Context.Response.Charset = System.Text.Encoding.UTF8.ToString();
            return "Hello World，你好";
        }



        public void returnJson()
        {
            string contentType = HttpContext.Current.Request.ContentType;
            if (HttpContext.Current.Request.ContentType == "text/xml")
            {
                HttpContext.Current.Request.ContentType = "text/xml; charset=UTF-8";
            }
            using (System.IO.Stream stream = HttpContext.Current.Request.InputStream)           
            //使用curl的编码转换
            //using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("gb2312")))         
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                string bodyText = reader.ReadToEnd(); bodyText = bodyText == "" ? "{}" : bodyText;
            }

            HttpContext.Current.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/x-javascript";
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.Write("{property:\"中文\"}");
            //HttpContext.Current.Response.Write("{property:\"中文\"}");

            //curl使用
            //编码转换utf8转GB2312
            //HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("gb2312"), System.Text.Encoding.UTF8.GetBytes("{property:\"中文\"}")));
            
            
            
        }


        public void GetBase()
        {
            //获取请求文本
            var bodyText = Entity.Tools.GetRequestBody();

            //响应JSON实例
            var ResponseJson = Entity.Tools.ValidateJsonFormat(bodyText);

            //如果Json格式解析失败
            if (ResponseJson.StatusCode != Entity.StatusCode.Success)
                return;

            //反序列化到对象Deserialize
            var reqJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.RequestJson>(bodyText);
            string parseErro = reqJson.ErroString();
            if (!string.IsNullOrEmpty(parseErro))
            {
                ResponseJson.StatusCode = Entity.StatusCode.Fail;
                ResponseJson.Message = parseErro;
                return;
            }

            //科目新增
            //区分对象
            if (reqJson.OperateType.Equals("t_Account"))
            {
                //区分操作方法
                if (reqJson.OperateType.Equals("SAVE", StringComparison.OrdinalIgnoreCase))
                {
                    //读取DATA，转换成对象
                    
                    //除ITEMDETIAL外，全部直接INSERT t_item 表 和 t_account 表
                    
                }
            }

            //反射创建类
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            dynamic obj = assembly.CreateInstance(assembly.GetName().Name + reqJson.OperateType); 

            

            //返回JSON
            HttpContext.Current.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/x-javascript";
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.Write(ResponseJson.ToString()); 

        }
    }
}
