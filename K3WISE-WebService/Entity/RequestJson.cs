using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K3WISE_WebService.Entity
{
    public class RequestJson
    {
        
        public string DBAccount { get; set; }

        public string OperateType { get; set; }

        public string OperateTable { get; set; }

        public string Number { get; set; }

        public string Data { get; set; }

        public string Filter { get; set; }

        public string ErroString()
        {
            if (this.GetType().GetProperties()
                    .Where(p => p.Name.Equals("DBAccount") || p.Name.Equals("OperateType") || p.Name.Equals("OperateTable"))
                    .Select(p => (string)p.GetValue(this, null))
                    .Any(value => !string.IsNullOrEmpty(value))
                    )
                return "DBAccount/OperateType/OperateTable值不能为空";

            if (string.Equals(OperateType, "UPDATE", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(Number))
                return "OperateType为UPDATE时，Number不可为空";

            if ((new[] { "SAVE", "UPDATE", "DELETE" ,"GET" }).Contains(OperateType, StringComparer.OrdinalIgnoreCase))
                return "OperateType值不符合规范";
            
            return "";

        }

    }
}