using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassFunction_Eform;
using System.Data;

namespace RoleEform
{
    public class RoleUser
    {
        #region Properties
        private string _roleId;
        public string RoleId
        {
            get { return _roleId; }
            set { _roleId = value; }
        }
        #endregion

        #region Constuctor
        public RoleUser()
        {

        }
        public RoleUser(string roleId)
        {
            _roleId = roleId;
        }
        #endregion

        #region Static
        static public RoleUser[] getRoleId(string oEmpCode)
        {
            string sql = "SELECT * FROM T_Role_User WHERE emp_code='" + oEmpCode + "'";
            ConnectDatabase fn = new ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
            DataTable dtbRole = fn.GetDataAdapter(sql);
            if (dtbRole.Rows.Count > 0)
            {
                List<RoleUser> oRoleUser = new List<RoleUser>();
                foreach (DataRow drv in dtbRole.Rows)
                {
                    RoleUser role = new RoleUser(drv["Role_id"].ToString());
                    oRoleUser.Add(role);
                }
                return oRoleUser.ToArray();
            }
            return null;
        }
        static public bool checkRoleId(string oEmpCode)
        {
            string sql = @"SELECT * FROM T_Role_User WHERE emp_code = '" + oEmpCode + "' ";
            ConnectDatabase fn = new ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
            DataTable dtbRole = fn.GetDataAdapter(sql);
            if (dtbRole.Rows.Count == 0)
                return false;
            return true;
        }
        static public bool findEmpInRole(string oEmpCode, string oRoleId)
        {
            //string sql = @"SELECT * FROM T_Role_User WHERE emp_code = '" + oEmpCode + "' AND Role_id='" + oRoleId + "'";
            //ConnectDatabase fn = new ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
            //DataTable dtbRole = fn.GetDataAdapter(sql);
            //if (dtbRole.Rows.Count == 0)
            //    return false;
            //return true;

            if (oRoleId.Trim().ToLower() == "acc_credit") return true;
            return false;
        }
        #endregion
    }
}

