using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace K3WISE_WebService.Entity
{
    public class ResponseJson
    {
        public StatusCode StatusCode;

        public string Message { get; set; }

        public string Data { get; set; }

        public override string ToString()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json;
        }

        public ResponseJson(StatusCode StatusCode, string Message)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
        }

        public ResponseJson()
        {

        }

        public ResponseJson(StatusCode StatusCode, string Message, string Data)
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