using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K3WISE_WebService
{
    public class AccountGroup
    {

        string FGroup;

        string FName;

        public void GET()
        {
            string sql = string.Format(@"select * from T_AcctGroup where FGroup = {0}", FGroup);
        }

        public void Update()
        {
            string sql = string.Format(@"update T_AcctGroup set FName=N'{1}' where FGroup = {0} ",FGroup,FName);
        }

        public void Delete()
        {
            string sql = string.Format(@"delete from T_AcctGroup where FGroup={0}", FGroup);
        }

        public void Save()
        {
            string sql = string.Format(@"INSERT dbo.T_AcctGroup
(
    FClassID,
    FGroupID,
    FName,
    FName_Cht,
    FName_EN,
    UUID
)
VALUES
(   {0},   -- FClassID - int
    {1},   -- FGroupID - int
    N'{2}', -- FName - nvarchar(26)
    N'{2}', -- FName_Cht - nvarchar(255)
    N'{2}', -- FName_EN - nvarchar(255)
    newid() -- UUID - uniqueidentifier
    )"
, FGroup.ToString().Substring(0,1), FGroup, FName);


        }

    }
}