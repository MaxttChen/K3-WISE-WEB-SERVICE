using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DemoForm
{
    public class ResponseJson
    {
        public StatusCode StatusCode;

        public string Message { get; set; }

        public  string Data { get; set; }

        public override string ToString()
        {           
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return str;
        }

        public ResponseJson(StatusCode StatusCode, string Message )
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
        }

        public ResponseJson(StatusCode StatusCode, string Message , string Data)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Data = Data;
        }
    }

    public enum StatusCode
    {
        Success = 200, Fail = 400
    }


}