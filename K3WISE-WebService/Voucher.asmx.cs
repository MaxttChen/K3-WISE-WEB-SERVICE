using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using K3WISE_WebService.Bill;
using Newtonsoft.Json;

namespace K3WISE_WebService
{
    /// <summary>
    /// Voucher 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class Voucher : System.Web.Services.WebService
    {

        /// <summary>
        /// 自定义接口，新增/覆盖凭证
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public void Update()
        {
            var erroMsg = "";
            var bodyText = Entity.Tools.GetRequestBody();

            //校验json格式
            erroMsg = Entity.Tools.ValiJson(bodyText);
            if (!string.IsNullOrEmpty(erroMsg))
            {
                ResponError(erroMsg);
                return;
            }

            SqlConnection connection = null;
            SqlTransaction transaction = null;
            //处理
            try
            {
                //解析json，获取数据库账套
                var vj = JsonConvert.DeserializeObject<VoucherJson>(bodyText);
                
                //根据db获取数据源并打开对应数据连接
                connection = Entity.DBHelper.GetConnectionByDBName(vj.DB);
                connection.Open();
                
                //检测数据 
                //检查制单人
                var sql_Preparer = String.Format(@"SELECT COUNT(1) FROM dbo.t_User WHERE FName = '{0}'",vj.VoucherData.FPreparer);
                if (Entity.DBHelper.Exists(sql_Preparer, connection) <= 0)
                {
                    erroMsg = vj.VoucherData.FPreparer + "制单人不存在！";
                }

                //检查是否会计期间
                var sql_CheckDate = String.Format(@"
SELECT t1.FKey key1 ,t1.FValue CurrentYearVal , t2.FKey key2, t2.FValue CurrentPeriodVal  FROM dbo.t_SystemProfile t1
JOIN t_SystemProfile t2 ON t2.FCategory = t1.FCategory AND t2.FKey = 'CurrentPeriod'
WHERE 1=1AND t1.FCategory = 'GL' AND  t1.FKey = 'CurrentYear'
AND CONVERT(INT,t1.FValue) <= CONVERT(INT,'{0}') AND CONVERT(INT,t2.FValue) <= CONVERT(INT,'{1}')
", vj.VoucherData.FYear, vj.VoucherData.FPeriod);
                if (Entity.DBHelper.Fill(sql_CheckDate, connection).Rows.Count <= 0)
                {
                    erroMsg = vj.VoucherData.FYear + "-" + vj.VoucherData.FPeriod + ":凭证会计期间小于结账日期！";
                }


                if (string.IsNullOrEmpty(erroMsg))
                {
                    foreach (VoucherEntry ve in vj.VoucherData.Entries)
                    {

                        //检查币别
                        var sql_currency = string.Format(@"SELECT COUNT(1) FROM dbo.t_Currency WHERE FCurrencyID <> 0 
AND FNumber = '{0}'", ve.FCurrencyNumber);
                        if (Entity.DBHelper.Exists(sql_currency, connection) <= 0)
                        {
                            erroMsg = ve.FCurrencyNumber + "币别不存在！";
                            break;
                        }


                        //检查科目
                        var sql_account = string.Format(@"SELECT A.FNumber+'/'+A.FFullName act,A.FDetailID,CONVERT(VARCHAR(100),IC.FItemClassID) itemClassNumber FROM dbo.t_Account A
 LEFT JOIN dbo.t_ItemDetailV V ON V.FDetailID = A.FDetailID
 LEFT JOIN dbo.t_ItemClass IC ON IC.FItemClassID = V.FItemClassID
 WHERE FDelete=0 AND FDetail = 1 AND A.FNumber = '{0}' ", ve.FAccountNumber);
                        DataTable dt_account = Entity.DBHelper.FillAlone(sql_account, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                        if (dt_account.Rows.Count <= 0)
                        {
                            erroMsg = ve.FAccountNumber + "科目不存或禁用或不是明细科目";
                            break;
                        }

                        //明细数目
                        if ( !dt_account.Rows[0]["FDetailID"].ToString().Equals("0") && null!= ve.DetailEntries && dt_account.Rows.Count != ve.DetailEntries.Length)
                        {
                            erroMsg = dt_account.Rows[0]["act"].ToString() + "  - 科目核算项目数目和凭证核算项目数目对应不上";
                            break;
                        }
                        if (!dt_account.Rows[0]["FDetailID"].ToString().Equals("0") && null == ve.DetailEntries )
                        {
                            erroMsg = dt_account.Rows[0]["act"].ToString() + "  - 科目核算项目数目和凭证核算项目数目对应不上";
                            break;
                        }
                        if (dt_account.Rows[0]["FDetailID"].ToString().Equals("0") && null != ve.DetailEntries && ve.DetailEntries.Length != 0)
                        {
                            erroMsg = dt_account.Rows[0]["act"].ToString() + "  - 科目核算项目数目和凭证核算项目数目对应不上";
                            break;
                        }

                        //检查核算项目数据
                        //foreach (VoucherEntryItem vei in ve.DetailEntries)
                        for(int j=0,len_j = null==ve.DetailEntries ? 0 : ve.DetailEntries.Length; j < len_j; j++) 
                        {
                            var sql_item = string.Format(@"SELECT I.FItemID , IC.FItemClassID FROM dbo.t_Item I(NOLOCK) 
JOIN dbo.t_ItemClass IC(NOLOCK) ON i.FItemClassID = ic.FItemClassID
WHERE i.FNumber = '{0}' AND ic.FItemClassID = {1}", ve.DetailEntries[j].FDetailNumber, ve.DetailEntries[j].FTypeNumber);
                            DataTable dt_item = Entity.DBHelper.FillAlone(sql_item, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                            if (dt_item.Rows.Count <= 0 )
                            {
                                erroMsg = "FDetailNumber：" + ve.DetailEntries[j].FDetailNumber + '/' + "FTypeNumber:" + ve.DetailEntries[j].FTypeNumber + "，核算类型或核算项目不存在。";
                                break;
                            }
                            ve.DetailEntries[j].FItemID = dt_item.Rows[0]["FItemID"].ToString();
                            ve.DetailEntries[j].FItemClassID = dt_item.Rows[0]["FItemClassID"].ToString();

                            //检查核算项目和科目的核算项目是否对应
                            for (int i = 0, len = dt_account.Rows.Count; i < len; i++)
                            {
                                if (!dt_account.Rows[i]["itemClassNumber"].ToString().Equals("") && Convert.ToInt16(dt_account.Rows[i]["itemClassNumber"].ToString()) == 
                                    (string.IsNullOrEmpty(ve.DetailEntries[j].FTypeNumber) ? 0 : Convert.ToInt16(ve.DetailEntries[j].FTypeNumber) ))
                                {
                                    dt_account.Rows[i]["itemClassNumber"] = "";
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(erroMsg))
                        {
                            ResponError(erroMsg);
                            return;
                        }

                        for (int i = 0, len = dt_account.Rows.Count; i < len && !dt_account.Rows[0]["FDetailID"].ToString().Equals("0"); i++)
                        {
                            if (!dt_account.Rows[i]["itemClassNumber"].ToString().Equals(""))
                            {
                                erroMsg = dt_account.Rows[i]["act"].ToString() + "  - 缺少科目对应核算项目：" + dt_account.Rows[i]["itemClassNumber"].ToString();
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(erroMsg))
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(erroMsg))
                {
                    ResponError(erroMsg);
                    return;
                }

                var ves = vj.VoucherData.Entries;
                //写入数据

                //确认voucherid最大值
                string FVoucherID = "";
                string sql_max_FVoucherID = "SELECT ISNULL(MAX(FVoucherID),0) + 1 AS MAXID  FROM dbo.t_Voucher where 1=1 ";
                DataTable dt_max_FVoucherID = Entity.DBHelper.FillAlone(sql_max_FVoucherID, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                FVoucherID = dt_max_FVoucherID.Rows[0]["MAXID"].ToString();

                //获取凭证号
                string VoucherFNumber = "0";
                

                //是否覆盖--获取凭证ID
                FVoucherID = vj.Replace ? vj.VoucherData.FVoucherID.ToString() : FVoucherID;
                if (vj.Replace)
                {
                    FVoucherID = vj.VoucherData.FVoucherID.ToString();

                    string sql_voucher_number = string.Format("SELECT FNumber  FROM dbo.t_Voucher where FVoucherID =  {0}", FVoucherID);
                    DataTable dt_voucher_number = Entity.DBHelper.FillAlone(sql_voucher_number, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                    VoucherFNumber = dt_voucher_number.Rows.Count > 0 ? dt_voucher_number.Rows[0][0].ToString() : "0";
                    //删除凭证
                    string sql_delete = String.Format(@"
DELETE FROM dbo.t_Voucher WHERE FVoucherID='{0}'
DELETE FROM dbo.t_VoucherEntry WHERE FVoucherID='{0}'
", FVoucherID);
                    Entity.DBHelper.Query(sql_delete, connection, transaction);
                }

                //确认voucherentry的detailid
                for (int i =0 , len = ves.Length; i < len; i++)
                {
                    if (null != ves[i].DetailEntries && ves[i].DetailEntries.Length > 0 && ves[i].FDetailID.Equals("0") )
                    {
                        var sql_detail_where = string.Format(@" where 1=1 AND FDetailCount = {0}  ", ves[i].DetailEntries.Length.ToString()) ;

                        //找出FDetailID，如果没有则新增
                        foreach(var vei in ves[i].DetailEntries)
                        {
                            sql_detail_where += "  AND F" + vei.FItemClassID + "=" + vei.FItemID;
                        }
                        var sql_detail = "SELECT FDetailID FROM dbo.t_ItemDetail " + sql_detail_where;
                        DataTable dt_detail = Entity.DBHelper.FillAlone(sql_detail, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                        if (dt_detail.Rows.Count > 0)
                            ves[i].FDetailID = dt_detail.Rows[0]["FDetailID"].ToString();
                        else
                        {
                            string FDetailID = "";
                            string FDetailCount = ves[i].DetailEntries.Length.ToString();
                            DataTable dt_t_ItemDetail = Entity.DBHelper.FillAlone("select * from t_ItemDetail(nolock) where 1=2", Entity.DBHelper.GetConnectionByDBName(vj.DB));

                            DataRow dr = dt_t_ItemDetail.NewRow();

                            for(int j = 0 , len_j = dt_t_ItemDetail.Columns.Count; j < len_j; j++)
                            {
                                foreach (var vei in ves[i].DetailEntries)
                                {
                                    if(dt_t_ItemDetail.Columns[j].ColumnName.Equals("F" + vei.FItemClassID.ToString()))
                                    {
                                        dr[j] = vei.FItemID;
                                        break;
                                    }
                                    else
                                    {
                                        dr[j] = "0";
                                    }
                                }
                            }

                            transaction = connection.BeginTransaction();
                            //获取最大FDetailID
                            var sql_detailid_max = @"SELECT ISNULL(MAX(FDetailID),0) + 1 AS FDetailID FROM dbo.t_ItemDetail where 1=1 ";
                            DataTable dt_detailid_max = Entity.DBHelper.FillAlone(sql_detailid_max, Entity.DBHelper.GetConnectionByDBName(vj.DB));
                            FDetailID = dt_detailid_max.Rows[0]["FDetailID"].ToString();
                            dr["FDetailID"] = FDetailID;
                            dr["FDetailCount"] = FDetailCount;
                            dt_t_ItemDetail.Rows.Add(dr);

                            //插入SQL生成FDetailID
                            string sql_insert_detail = "";
                            string sql_insert_detail_col = "";
                            string sql_insert_detail_value = "";
                            for (int k=0,len_k= dt_t_ItemDetail.Columns.Count; k < len_k; k++)
                            {
                                sql_insert_detail_col = sql_insert_detail_col + (k == 0 ? "" : ",") + dt_t_ItemDetail.Columns[k].ColumnName;
                                sql_insert_detail_value = sql_insert_detail_value + (k == 0 ? "" : ",") + dt_t_ItemDetail.Rows[0][k].ToString();
                                
                            }
                            sql_insert_detail = "   insert into t_ItemDetail ( " + sql_insert_detail_col + " ) values (" + sql_insert_detail_value + ")  ";

                            //插入SQL生成FDetailIDV
                            string sql_insert_detailv = "";
                            foreach (var vei in ves[i].DetailEntries)
                            {
                                sql_insert_detailv += string.Format(@" 
INSERT INTO dbo.t_ItemDetailV
(
    FDetailID,
    FItemClassID,
    FItemID
)
VALUES
(   {0}, -- FDetailID - int
    {1}, -- FItemClassID - int
    {2}  -- FItemID - int
    )  
",FDetailID,vei.FItemClassID,vei.FItemID);
                            }

                            //数据库执行插入ItemDetail
                            Entity.DBHelper.Query(sql_insert_detailv+ sql_insert_detail,connection,transaction);
                            transaction.Commit();

                            ves[i].FDetailID = FDetailID;
                        }

                    }

                }

                



                transaction = connection.BeginTransaction();
                string sql_insert_t_voucherEntry = "";
                //插入t_vocuherentry表
                for(int i = 0, len = ves.Length; i < len; i++)
                {
                    sql_insert_t_voucherEntry += string.Format(@"
  INSERT INTO 
t_VoucherEntry
(FBrNo, FVoucherID, FEntryID, FExplanation, FAccountID, FDetailID, FCurrencyID, FExchangeRate, FDC, FAmountFor, FAmount, FQuantity, FMeasureUnitID, FUnitPrice, FInternalInd, FAccountID2, FSettleTypeID, FSettleNo, FTransNo, FCashFlowItem, FTaskID, FResourceID, FExchangeRateType, FSideEntryID)
SELECT   TOP 1
'0', " 
+ FVoucherID + ", " 
+ i.ToString() + " , '"
+ ves[i].FExplanation  + "' , A.FAccountID, " 
+ ves[i].FDetailID + ", c.FCurrencyID ,  " 
+ ves[i].FExchangeRate.ToString() + " , '"
+ ves[i].FDC + "' , " 
+ ves[i].FAmountFor.ToString() + ", " 
+ ves[i].FAmount.ToString()
+ @", 0, 0, 0, NULL, A.FAccountID, 0, NULL, NULL, 0, 0, 0, 1, 0
FROM dbo.t_Account A
JOIN dbo.t_Currency C ON C.FNumber = '{1}'
WHERE A.FNumber = '{0}'    
", ves[i].FAccountNumber.ToString(), ves[i].FCurrencyNumber.ToString());
                }

                //插入t_voucher表
                string sql_insert_t_voucher = string.Format(@"
INSERT INTO dbo.t_Voucher
	(
	    FBrNo, FVoucherID, FDate, FYear, FPeriod, FGroupID, FNumber, FReference, FExplanation, FAttachments, FEntryCount, FDebitTotal, FCreditTotal, FInternalInd, FChecked, FPosted, FPreparerID
		, FCheckerID, FPosterID, FCashierID, FHandler, FOwnerGroupID, FObjectName, FParameter, FSerialNum, FTranType, FTransDate, FFrameWorkID, FApproveID, FFootNote, UUID
	)SELECT
	'0', {6}, '{2}', YEAR('{2}'), MONTH('{2}'), VG.FGroupID, 
CASE WHEN '{7}'='0' THEN (
Select ISNULL(MAX(FNumber),0) + 1
 From (select FNumber,FYear,FPeriod,FGroupID from t_Voucher  
 Union all select FNumber,FYear,FPeriod,FGroupID from t_VoucherBlankout) a  
 Where FYear= YEAR('{2}') AND FPeriod=  MONTH('{2}') and FGroupID = VG.FGroupID
) ELSE '{7}' END
, '{3}', '{4}', 0, {5}, ve.FAmountFor, ve.FAmountFor, NULL, 0, 0, U.FUserID, -1, -1, -1, NULL, 0, NULL, NULL, (SELECT ISNULL(MAX(FSerialNum),0)+1 FROM dbo.t_Voucher), 0
	, '{2}', -1, -1, '', NEWID()
	FROM dbo.t_User U 
	JOIN dbo.t_VoucherGroup VG ON VG.FName='{1}'
JOIN (SELECT FVoucherID,SUM(FAmountFor) FAmountFor FROM t_VoucherEntry ve WHERE FDC=1 AND ve.FVoucherID = '{6}' GROUP BY FVoucherID) ve ON 1=1
	WHERE U.FName = '{0}'
"
, vj.VoucherData.FPreparer
, vj.VoucherData.FGroup
, vj.VoucherData.FDate
, vj.VoucherData.FReference
, vj.VoucherData.FExplanation
, vj.VoucherData.Entries.Length
, FVoucherID
, VoucherFNumber
);
                //更新对方科目accountid2
                string sql_update_accountid2 = string.Format(@"  EXEC UPDATE_VOUCHERENTRY_ACCOUNTID2 {0}  ", FVoucherID);


                Entity.DBHelper.Query(sql_insert_t_voucherEntry + "  " + sql_insert_t_voucher, connection,transaction);
                //Entity.DBHelper.Query(sql_insert_t_voucher, connection, transaction);

                transaction.Commit();
                //返回值
                Respon(FVoucherID);

            }
            catch (Exception ex)
            {
                erroMsg = ex.ToString();
                if (null != transaction)
                    transaction.Rollback();
            }
            finally
            {
                if(null != connection)
                    connection.Close();
            }

            if (!string.IsNullOrEmpty(erroMsg))
            {
                ResponError(erroMsg);
                return;
            }
           


            
        }

        private void Respon(string json)
        {
            String res = @"{
  ""flag"": true,
  ""StatusCode"": 200,
  ""Message"": ""Successful"",
  ""Data"": ""保存凭证成功"",
  ""FVoucherID"": " + json + @"
}";
            Context.Response.Clear();
            Context.Response.StatusCode = 200;
            Context.Response.Status = "200 OK";
            Context.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            Context.Response.AddHeader("content-length", System.Text.Encoding.UTF8.GetBytes(res).Length.ToString());
            Context.Response.ContentType = "application/text";
            Context.Response.Charset = "utf-8";
            Context.Response.Write(res);
            Context.Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

        private void ResponError(string Msg)
        {
            String res = @"{
  ""flag"": false,
  ""StatusCode"": 201,
  ""Message"": ""Faild: " + Msg + @""",
  ""Data"": ""凭证数据格式错误 " + Msg + @" ""
}";
            Context.Response.Clear();
            Context.Response.StatusCode = 200;
            Context.Response.Status = "200 OK";
            Context.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            Context.Response.AddHeader("content-length", System.Text.Encoding.UTF8.GetBytes(res).Length.ToString());
            Context.Response.ContentType = "application/text";
            Context.Response.Charset = "utf-8";
            Context.Response.Write(res);
            Context.Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

    }
}
