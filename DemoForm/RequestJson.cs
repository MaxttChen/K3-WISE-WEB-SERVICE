using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoForm
{
    public class RequestJson
    {
        
        public string DBAccount { get; set; }

        public string OperateType { get; set; }

        public string OperateTable { get; set; }

        public string Number { get; set; }

        public string Data { get; set; }

        public string Filter { get; set; }

        public bool ValidateJson()
        {
            return this.GetType().GetProperties()
                .Where(p => p.Name.Equals("DBAccount") || p.Name.Equals("OperateType") || p.Name.Equals("OperateTable"))
                .Select(p => (string)p.GetValue(this, null))
                .Any(value => !string.IsNullOrEmpty(value));

        }
    }
}