using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using ClassFunction_Eform;
using System.Data;

namespace RoleEform
{
    public class MenuEform
    {
        #region Properties
        private string _menuCode;
        private string _menuName;
        private int _menuParent;
        private string _menuPageUrl;
        private string _menuType;
        private int _menuOrder;
        private bool _menuStatus;
        private int _menuSeq;

        public string MenuCode
        {
            get { return _menuCode; }
            set { _menuCode = value; }
        }
        public string MenuName
        {
            get { return _menuName; }
            set { _menuName = value; }
        }
        public string MenuPageUrl
        {
            get { return _menuPageUrl; }
            set { _menuPageUrl = value; }
        }
        public string MenuType
        {
            get { return _menuType; }
            set { _menuType = value; }
        }
        public bool MenuStatus
        {
            get { return _menuStatus; }
            set { _menuStatus = value; }
        }
        public int MenuParent
        {
            get { return _menuParent; }
            set { _menuParent = value; }
        }
        public int MenuOrder
        {
            get { return _menuOrder; }
            set { _menuOrder = value; }
        }
        public int MenuSeq
        {
            get { return _menuSeq; }
            set { _menuSeq = value; }
        }
        #endregion
        public MenuEform()
        {

        }
        public MenuEform(string menucode, string menuname, string menupageUrl, 
                    string menutype, bool menustatus, int menuparent, 
                    int menuorder, int menuseq)
        {
            _menuCode = menucode;
            _menuName = menuname;
            _menuPageUrl = menupageUrl;
            _menuType = menutype;
            _menuStatus = menustatus;
            _menuParent = menuparent;
            _menuOrder = menuorder;
            _menuSeq = menuseq;
        }
        #region Static
        static public MenuEform[] getMenu(string oEmpcode, string oRoleId)
        {
            string sql = @"SELECT m.Menu_Code, m.Menu_Name, m.Menu_Parent, m.Menu_PageURL, m.Menu_Type, 
                           m.Menu_Order, m.Menu_Status, m.Menu_Seq, ro.Role_All, ru.Role_id
                           FROM T_Role_User as ru INNER JOIN
                           T_Role_Object as ro ON ru.Role_id = ro.Role_id INNER JOIN
                           T_Menu_Mee as m ON ro.Search_header_id = m.Menu_Code where ru.Role_id in ("+ oRoleId +")" +
					       " and ru.emp_code = '" + oEmpcode + "' and m.Menu_Status = 1 ";
            ConnectDatabase fn = new ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
            DataTable dtbMenu = fn.GetDataAdapter(sql);
            if (dtbMenu.Rows.Count > 0)
            {
                List<MenuEform> oMenu = new List<MenuEform>();
                foreach (DataRow drv in dtbMenu.Rows)
                {
                    MenuEform menu = new MenuEform(drv["Menu_Code"].ToString(), drv["Menu_Name"].ToString(), drv["Menu_PageURL"].ToString(), drv["Menu_Type"].ToString(),
                                        bool.Parse(drv["Menu_Status"].ToString()), int.Parse(drv["Menu_Parent"].ToString()), int   .Parse(drv["Menu_Order"].ToString()),
                                        int.Parse(drv["Menu_Seq"].ToString()));
                    oMenu.Add(menu);
                }
                return oMenu.ToArray();
            }
            return null;
        }
        static public DataTable getMenuDT(string oEmpcode, string oRoleId, string oTypeEform)
        {
            string sql = @"SELECT m.Menu_Code, m.Menu_Name, m.Menu_Parent, m.Menu_PageURL, m.Menu_Type, 
                           m.Menu_Order, m.Menu_Status, m.Menu_Seq, sum(Convert(int,ro.Role_All))  as  Role_All
                           FROM T_Role_User as ru INNER JOIN
                           T_Role_Object as ro ON ru.Role_id = ro.Role_id INNER JOIN
                           T_Menu as m ON ro.Search_header_id = m.Menu_Code where ru.Role_id in (" + oRoleId + ") and ro.type_eform='" + oTypeEform + "'" +
                           " and ru.emp_code = '" + oEmpcode + "' and m.Menu_Status = 1 " +
                           " group by m.Menu_Code, m.Menu_Name, m.Menu_Parent, m.Menu_PageURL, m.Menu_Type, " +
                           " m.Menu_Order, m.Menu_Status, m.Menu_Seq " +
                           " order by m.menu_seq, m.Menu_Order";
            ConnectDatabase fn = new ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
            return  fn.GetDataAdapter(sql);
        }
        
        #endregion
    }
    
}

