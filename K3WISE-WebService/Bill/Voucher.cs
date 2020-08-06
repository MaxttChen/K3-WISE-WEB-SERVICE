using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K3WISE_WebService.Bill
{
    public class Voucher
    {

        public string FAttachments;

        public string FDate;

        public string FExplanation;

        public string FGroup;

        public string FNumber;

        public string FPeriod;

        public string FPoster;

        public string FCashier;

        public string FPreparer;

        public string FSerialNum;

        public string FReference;

        public string FTransDate;

        public int FVoucherID;

        public int FYear;

        public VoucherEntry[] Entries;
    }
}