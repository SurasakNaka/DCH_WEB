using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using RoleEform;

namespace DCH_WEB.masterpage
{
    public partial class msp_main : System.Web.UI.MasterPage
    {
        #region ประกาศตัวแปร Session
        public string sMain
        {
            get
            {
                object sMain = Session["sMain"];
                if (sMain == null)
                {
                    return null;
                }
                return (string)sMain;
            }
            set { Session["sMain"] = value; }
        }

        public string eform
        {
            get
            {
                object eform = Session["eform"];
                if (eform == null)
                {
                    return null;
                }
                return (string)eform;
            }
            set { Session["eform"] = value; }
        }

        public DataTable dtMenu
        {
            get
            {
                object dt = Session["dtMenu"];
                if (dt == null)
                {
                    return null;
                }
                return (DataTable)dt;
            }
            set { Session["dtMenu"] = value; }
        }

        #endregion
        #region Properties
        private DataTable dtbMenu
        {
            get
            {
                if (ViewState["dtbMenu"] == null)
                {
                    ViewState["dtbMenu"] = new DataTable();
                }
                return (DataTable)ViewState["dtbMenu"];
            }
            set
            {
                ViewState["dtbMenu"] = value;
            }
        }
        #endregion
        string ImgIconLevel0 = "<img  src='../image/menuItem.gif' style='vertical-align: bottom' />&nbsp;";
        string ImgIconlevel1 = "<img  src='../image/menuSubItem.gif'/>&nbsp;";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ComponentArt.Web.UI.Menu sys = (ComponentArt.Web.UI.Menu)Page.Master.FindControl("SysMenu");
                Initial(ref sys);//Bind Menu
            }
        }
        private void AddMenuHeader(ref ComponentArt.Web.UI.Menu SysMenu, string MenuName, string MenuValue, string MenuURL)
        {
            ComponentArt.Web.UI.MenuItem newItem = default(ComponentArt.Web.UI.MenuItem);

            newItem = new ComponentArt.Web.UI.MenuItem();
            newItem.Text = ImgIconLevel0 + MenuName;
            newItem.Value = MenuValue;
            newItem.LookId = "TopItemLook";
            if (MenuURL != string.Empty)
                newItem.NavigateUrl = MenuURL;

            SysMenu.Items.Add(newItem);
        }

        protected string Encrypt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Byte[] b = System.Text.Encoding.Default.GetBytes(value);
                string strEncrypt = Convert.ToBase64String(b);
                return strEncrypt;
            }
            else
                return "";
        }
        private bool AddMenuChild(ComponentArt.Web.UI.MenuItem oItem, DataTable dtbMenu, string MenuParent)
        {
            int oLoop = 0;
            DataRow[] rowsChild = dtbMenu.Select("Menu_Parent='" + MenuParent + "'");
            if (rowsChild.Length > 0)//Check ว่ายังมี Menu Child อีกหรือไม่
            {
                foreach (DataRow drvChild in rowsChild)
                {
                    ComponentArt.Web.UI.MenuItem newItem = default(ComponentArt.Web.UI.MenuItem);
                    newItem = new ComponentArt.Web.UI.MenuItem();
                    newItem.LookId = "DefaultItemLook";
                    newItem.Text = ImgIconlevel1 + drvChild["Menu_Name"].ToString(); // Add Name Menu
                    newItem.Value = drvChild["Menu_Code"].ToString(); // Add Value Menu
                    if (!string.IsNullOrEmpty(drvChild["Role_All"].ToString()))//Role_All เป็นการบอกว่าถึงสิทธิ์ในการใช้งาน Master And Report เท่านั้น ไม่เกี่ยวกับ Transaction
                    {
                        string[] oSeparator = drvChild["Menu_PageURL"].ToString().Split('?');
                        if (oSeparator.Length > 1)// URL มี ?
                            newItem.NavigateUrl = drvChild["Menu_PageURL"].ToString() + "&Permission=" + Encrypt(drvChild["Role_All"].ToString());
                        else
                            newItem.NavigateUrl = drvChild["Menu_PageURL"].ToString() + "?Permission=" + Encrypt(drvChild["Role_All"].ToString());
                    }
                    else //Role_All = Null คือเป็นฟอร์ม Transaction ต่าง ๆ
                        newItem.NavigateUrl = drvChild["Menu_PageURL"].ToString();//Add URL Menu

                    oItem.Items.Add(newItem);
                    dtbMenu.Rows.Remove(drvChild);//เมื่อ Add Menu แล้วจะลบออกจาก Dt ที่ได้ Select มา
                    ComponentArt.Web.UI.MenuItem SubItem = oItem.Items[oLoop];
                    oLoop++;
                    SubItem.Look.RightIconUrl = "../image/arrow.gif";
                    SubItem.Look.RightIconWidth = System.Web.UI.WebControls.Unit.Parse("20");
                    AddMenuChild(SubItem, dtbMenu, newItem.Value);// Recrusive Menu
                }
            }
            else
            {
                oItem.Look.RightIconUrl = ""; // ลบ Img กรณีที่ไม่มี Menu Child
            }
            return false;
        }
        private void CreateMenu(string oEmpCode, string oRoleId)
        {
            try
            {
                eform = "exp";
                if (!string.IsNullOrEmpty(eform))
                {
                    dtbMenu = MenuEform.getMenuDT(oEmpCode, oRoleId, eform);//Get menu ทั้งหมดตาม Role ของ User
                    string MenuParent = string.Empty;
                    int oLoop = 0;

                    foreach (DataRow drv in dtbMenu.Select("Menu_Parent IS NULL"))
                    {
                        if (drv["Menu_Parent"].ToString() == string.Empty)//Menu Header
                        {
                            AddMenuHeader(ref SysMenu, drv["Menu_Name"].ToString(), drv["Menu_Code"].ToString(), drv["Menu_PageURL"].ToString());//Bind Header Menu
                            ComponentArt.Web.UI.MenuItem oItemMenu = SysMenu.Items[oLoop];
                            AddMenuChild(oItemMenu, dtbMenu, drv["Menu_Code"].ToString());
                        }
                        oLoop++;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert('{0}');", ex.Message), true);
            }
        }
        private string getRoleUser(string oEmpCode)
        {
            string msgError = string.Empty;
            RoleUser[] oRole = RoleUser.getRoleId(oEmpCode);
            if (oRole != null)
            {
                string oRoleCondition = string.Empty;
                for (int i = 0; i < oRole.Length; i++)
                {
                    oRoleCondition = (i == 0) ? oRole[i].RoleId : oRoleCondition + "," + oRole[i].RoleId;
                }
                return oRoleCondition;
            }
            else
            {
                msgError = "ไม่พบสิทธิ์การใช้งานของรหัสพนักงาน " + oEmpCode + " กรุณาติดต่อผู้ดูแลระบบ";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert('{0}');", msgError), true);
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "javascript", "closeWeb();", true);
            }
            return null;
        }
        public void Initial(ref ComponentArt.Web.UI.Menu SysMenu)
        {
            string strUserId = Page.User.Identity.Name;//Myfunction.SubStringUser(Page.User.Identity.Name);

            string EmpCode = "SurasakNaka";
            string oRoleId = getRoleUser(EmpCode);//get role ของพนักงาน
            CreateMenu(EmpCode, oRoleId);//Bind menu ตาม Role ที่ได้

            //string EmpCode;
            //if (Employee.getEmployeeAD(strUserId) != null)
            //{
            //    BasePage bp = new BasePage();
            //    if (bp.GetEmail(strUserId) != "")
            //    {
            //        EmpCode = Employee.getEmployeeAD(strUserId).empCode;
            //        if (RoleUser.checkRoleId(EmpCode))
            //        {
            //            lblAlias_Name.Text = "ผู้ใช้ระบบ: " + Employee.getEmployeeAD(strUserId).name + " [ " + Employee.getEmployeeAD(strUserId).userAD + " ]";
            //            CostCenter oCost = CostCenter.getCostCenter(Employee.getEmployeeAD(strUserId).costCenter);
            //            lblProfile.Text = oCost.costId + ": " + oCost.costName;

            //            string oRoleId = getRoleUser(EmpCode);//get role ของพนักงาน
            //            CreateMenu(EmpCode, oRoleId);//Bind menu ตาม Role ที่ได้
            //        }
            //        else
            //        {
            //            string msgError;
            //            msgError = "ไม่พบสิทธิ์การใช้งานของรหัสพนักงาน " + EmpCode + " กรุณาติดต่อผู้ดูแลระบบ";

            //            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert('{0}');", msgError), true);
            //            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "javascript", "closeWeb();", true);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        string msgError;
            //        msgError = "ไม่มี e-mail ในระบบของผู้ใช้ระบบ UserAD: " + strUserId;

            //        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert('{0}');", msgError), true);
            //        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "javascript", "closeWeb();", true);
            //        return;
            //    }
            //}
            //else
            //{
            //    string msgError;
            //    msgError = "ไม่มีข้อมูลผู้ใช้ระบบ UserAD: " + strUserId;

            //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert('{0}');", msgError), true);
            //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "javascript", "closeWeb();", true);
            //    return;
            //}
        }
    }
}