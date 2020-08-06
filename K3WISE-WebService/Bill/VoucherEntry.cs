using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K3WISE_WebService.Bill
{
    public class VoucherEntry
    {

        /// <summary>
        /// 科目代码
        /// </summary>
        public string FAccountNumber;

        /// <summary>
        /// 科目代码
        /// </summary>
        public double FAmount;

        /// <summary>
        /// 科目代码
        /// </summary>
        public double FAmountFor;

        /// <summary>
        /// 币别代码
        /// </summary>
        public string FCurrencyNumber;

        /// <summary>
        /// 借贷；只能为0和1
        /// </summary>
        public string FDC;

        /// <summary>
        /// 序号
        /// </summary>
        public int FEntryID;

        /// <summary>
        /// 转换率
        /// </summary>
        public double FExchangeRate;

        /// <summary>
        /// 摘要
        /// </summary>
        public string FExplanation;

        /// <summary>
        /// 核算项目
        /// </summary>
        public VoucherEntryItem[] DetailEntries;
        
        public string FDetailID="0";

        public string FMeasureUnit = "";
        public string FMeasureUnitUUID = "";
        public double FQuantity = 0;
        public string FSettleNo = "";
        public string FSettleTypeName = "";
        public string FTransNo = "";
        public double FUnitPrice = 0;




    }
}