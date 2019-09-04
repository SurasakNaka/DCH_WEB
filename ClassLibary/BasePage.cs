using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
 
using System.Net.Mail;

namespace CMGBase
{
    public class BasePage : Page
    {
        #region Variable
        public static string strConn = ConfigurationManager.ConnectionStrings["Myconnect"].ToString().Trim();
 
        #endregion

        #region Message box
        protected void MessageBox(string msg, string redirectPage)
        {
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert(\"{0}\");window.location='{1}';", replaceLinebreak(doubleQuoteToSingle(msg)), redirectPage), true);
        }
        protected void MessageBox(string msg)
        {
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("alert(\"{0}\");", replaceLinebreak(doubleQuoteToSingle(msg))), true);
        }
        protected void MessageCloseForm(string msg)
        {
            if (msg == null)
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", "closeFormNoMessage();", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("closeForm(\"{0}\");", replaceLinebreak(doubleQuoteToSingle(msg))), true);
            }
        }
        protected void MessageCloseConfirm(string msg)
        {
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "AlertMessage", String.Format("CloseFormConfirm(\"{0}\");", replaceLinebreak(doubleQuoteToSingle(msg))), true);
        }

        #endregion

        #region Encryption
        protected string urlEncrypt(string value)
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
        protected string urlDecrypt(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Byte[] b = Convert.FromBase64String(value);
                string strEncrypt = System.Text.Encoding.Default.GetString(b);
                return strEncrypt;
            }
            else
                return "";
        }
        protected string passwordEncrypt(string value)
        {
            return "";
        }
        protected string passwordDecrypt(string value)
        {
            return "";
        }
        #endregion

        #region Datetime
        protected string DateTimeFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["FormatDateTime"];
            }
        }

        protected string DateTimeFormat_Culture(string TxtDateTime, CultureInfo NameCulture,string MyFormat)
        {
            CultureInfo inputCultureInfo = new CultureInfo(NameCulture.IetfLanguageTag);
            inputCultureInfo.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
            DateTime d = DateTime.Parse(TxtDateTime, inputCultureInfo);
            CultureInfo displayCultureInfo = new CultureInfo("en-US");
            return d.ToString(MyFormat, displayCultureInfo);
        }

        protected bool ValidationDate(string strVal)
        {
            Regex reg = new Regex(@"([1-9]|0[1-9]|[12][0-9]|3[01])[- /.]([1-9]|0[1-9]|1[012])[- /.][0-9]{4}$");
            return (!reg.IsMatch(strVal));
        }//Check Format วันที่

        protected bool ValidationTime(string strVal)
        {
            Regex reg = new Regex(@"(([0-1][0-9])|([2][0-4])).([0-5][0-9])");
            return (!reg.IsMatch(strVal));
        }//Check Format ของเวลา

        public bool Validateime(string strVal)
        {
            Regex reg = new Regex(@"(([0-1][0-9])|([2][0-4])).([0-5][0-9])");
            return (!reg.IsMatch(strVal));
        }//Check Format ของเวลา

        protected bool ValidationTime_X(string strVal)
        {
            Regex reg = new Regex(@"(([0-1][0-9])|([2][0-3])):([0-5][0-9])");
            return (!reg.IsMatch(strVal));
        }//Check Format ของเวลา

        /*******************************************************************************
        * Function CheckFormatDate: ฺ
        *******************************************************************************/
        public bool CheckFormatDate(string pDate)
        {
            bool trResult = true;

            string[] arrDate = pDate.Split('/');
            short vDate = 0;
            short vMonth = 0;
            short vYear = 0;

            vDate = Convert.ToInt16(arrDate[2]);
            vMonth = Convert.ToInt16(arrDate[1]);
            vYear = Convert.ToInt16(arrDate[0]);

            if (vYear == 0)
            {
                trResult = false;
            }

            if (vYear > 2500)
            {
                trResult = false;
            }

            if (vMonth == 0)
            {
                trResult = false;
            }

            if (vMonth > 12)
            {
                trResult = false;
            }

            switch (vMonth)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    if (vDate > 31) { trResult = false; }
                    break;

                case 2:

                    if (System.DateTime.IsLeapYear(vYear))
                    {
                        if (vDate > 29) { trResult = false; }
                    }
                    else
                    {
                        if (vDate > 28) { trResult = false; }
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:

                    if (vDate > 30) { trResult = false; }
                    break;
            }

            if (vDate == 0)
            {
                trResult = false;
            }

            return trResult;
        }

        public bool isLeapYear(int year)
        {
            return (((year % 4) == 0) && ((year % 100) != 0) || ((year % 400) == 0));
        }

        /*******************************************************************************
        * Function SetYearList: ฺ BindDropDownList ตามปีปัจจุบัน (บวก/ลบ ตามจำนวน กี่ปีข้างหน้า/กี่ปี่ข้างหลัง)
        *******************************************************************************/
        public void SetYearList(DropDownList objYear, short nBackward, short nForward)
        {
            string strYear = "";
            string Today = "";
            int Index, iYear;

            Today = ConvertYear(DateTime.Now.Year.ToString(), "C");
            strYear = string.Format(Today, "yyyy");
            Index = 0;

            if (nBackward > 0)
            {
                iYear = nBackward;
                do
                {
                    objYear.Items.Add(new ListItem(Convert.ToString(Convert.ToInt32(strYear) - Convert.ToInt32(iYear))));
                    objYear.Items[Index].Value = Convert.ToString(Convert.ToInt32(strYear) - iYear);
                    Index = Index + 1;
                    iYear = iYear - 1;
                }
                while (!(iYear == 0));
            }

            objYear.Items.Add(strYear);
            objYear.Items[Index].Value = strYear;

            if (nForward > 0)
            {
                iYear = 1;
                for (iYear = 1; iYear <= nForward; iYear++)
                {
                    objYear.Items.Add(Convert.ToString(Convert.ToInt32(strYear) + Convert.ToInt32(iYear)));
                    objYear.Items[Index + 1].Value = Convert.ToString(Convert.ToInt32(strYear) + iYear);
                    Index = Index + 1;
                }
            }
            objYear.ClearSelection();
            objYear.Items.FindByValue(strYear).Selected = true;
        }

        /*******************************************************************************
        * Function ConvertYear: Return รูปแบบปีที่ต้องการ
        *    B = พุทธศักราช   /  C = คริสต์ศักราช 
        *******************************************************************************/
        public string ConvertYear(string strYear, string EraType)
        {
            int valYear = 0;
            switch (EraType)
            {
                case "B":
                    if (Convert.ToInt32(strYear) < 2400)
                    {
                        valYear = Convert.ToInt32(strYear) + 543;
                    }
                    else
                    {
                        valYear = Convert.ToInt32(strYear);
                    }
                    break;

                case "C":
                    if (Convert.ToInt32(strYear) < 2400)
                    {
                        valYear = Convert.ToInt32(strYear);
                    }
                    else
                    {
                        valYear = Convert.ToInt32(strYear) - 543;
                    }
                    break;
            }
            return Convert.ToString(valYear);
        }

        public string Convert_to_yyyymmdd(string pDate, string pType)
        {
            string[] arrDate = pDate.Split('/');
            string retDate = "";
            string vDate = "";
            string vMonth = "";
            short vYear = 0;

            vDate = arrDate[0].PadRight(2, '0');
            vMonth = arrDate[1].PadRight(2, '0');
            vYear = Convert.ToInt16(arrDate[2]);

            switch (pType)
            {
                case "B":
                    //--> พ.ศ.
                    if (vYear < 2500)
                    {
                        vYear += 543;
                    }
                    break;
                case "C":
                    //--> ค.ศ.
                    if (vYear > 2500)
                    {
                        vYear -= 543;
                    }
                    break;
            }
            retDate = Convert.ToString(vYear) + "/" + vMonth + "/" + vDate;
            return retDate;
        }

        /*******************************************************************************
        * Function Get_SysDate: Return วันที่ปัจจุบัน Format --> yyyy/mm/dd
        *******************************************************************************/
        public string Get_SysDate()
        {
            string sYear = null;
            string sRet = null;

            sYear = Convert.ToString(DateTime.Now.Year);
            if (Convert.ToInt32(sYear) > 2500) sYear = Convert.ToString(Convert.ToInt32(sYear) - 543);

            sRet = sYear + "/" + Convert.ToString(DateTime.Now.Month).PadRight(2) + "/" + Convert.ToString(DateTime.Now.Day).PadRight(2);
            return sRet;
        }

        public DataTable getMonthList()
        {
            DataTable dtbMonth = new DataTable();
            dtbMonth.Columns.Add("no", typeof(int));
            dtbMonth.Columns.Add("nameTH", typeof(string));
            dtbMonth.Columns.Add("nameEN", typeof(string));
            dtbMonth.Columns.Add("subNameTH", typeof(string));
            dtbMonth.Columns.Add("subNameEN", typeof(string));

            dtbMonth.Rows.Add("1", "มกราคม", "January", "ม.ค.", "Jan");
            dtbMonth.Rows.Add("2", "กุมภาพันธ์", "February", "ก.พ.", "Feb");
            dtbMonth.Rows.Add("3", "มีนาคม", "March", "มี.ค.", "Mar");
            dtbMonth.Rows.Add("4", "เมษายน", "April", "เม.ย.", "Apr");
            dtbMonth.Rows.Add("5", "พฤษภาคม", "May", "พ.ค.", "May");
            dtbMonth.Rows.Add("6", "มิถุนายน", "June", "มิ.ย.", "Jun");
            dtbMonth.Rows.Add("7", "กรกฎาคม", "July", "ก.ค.", "Jul");
            dtbMonth.Rows.Add("8", "สิงหาคม", "August", "ส.ค.", "Aug");
            dtbMonth.Rows.Add("9", "กันยายน", "September", "ก.ย.", "Sep");
            dtbMonth.Rows.Add("10", "ตุลาคม", "October", "ต.ค.", "Oct");
            dtbMonth.Rows.Add("11", "พฤศจิกายน", "November", "พ.ย.", "Nov");
            dtbMonth.Rows.Add("12", "ธันวาคม", "December", "ธ.ค.", "Dec");

            return dtbMonth;
        }
        #endregion

        #region Dropdownlist
        protected void BindDropDownList(ref DropDownList ddl, string DataTextField, string DataValueField, DataTable dtSource)
        {
            ddl.DataSource = dtSource;
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataBind();
        }
        protected void BindDropDownList(ref DropDownList ddl, string DataTextField, string DataValueField, string HeadTitle, DataTable dtSource)
        {
            DataRow dr = dtSource.NewRow();
            dr[DataValueField] = "0";
            dr[DataTextField] = HeadTitle;
            dtSource.Rows.InsertAt(dr, 0);
            ddl.DataSource = dtSource;
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataBind();
        }
        protected void BindDropDownList(ref DropDownList ddl, string DataTextField, string DataValueField, string HeadTitleValue, string HeadTitleName, DataTable dtSource)
        {
            ddl.DataSource = dtSource;
            ddl.DataTextField = DataTextField;
            ddl.DataValueField = DataValueField;
            ddl.DataBind();

            ListItem li = new ListItem(HeadTitleName, HeadTitleValue);
            ddl.Items.Insert(0, li);
        }
        #endregion

        #region Button
        /*******************************************************************************
        * Function ClickOnceButton: ให้ Button Control กดปุ่มได้แค่ครั้งเดียว
        *******************************************************************************/
        public void ClickOnceButton(ref Page page, ref Button b)
        {
            System.Text.StringBuilder oneClickScript = new System.Text.StringBuilder();
            oneClickScript.Append("if (typeof(Page_ClientValidate) == 'function') { ");
            oneClickScript.Append("if (Page_ClientValidate() == false) { return false; }} ");
            oneClickScript.Append("this.value = 'Please wait…';");
            oneClickScript.Append("this.disabled = true;");
            oneClickScript.Append(page.ClientScript.GetPostBackEventReference(b, ""));
            oneClickScript.Append(";");
            b.Attributes.Add("onclick", oneClickScript.ToString());
        }
        #endregion

        #region Datatable
        /*******************************************************************************
        * Function GetDataTable: Return DataTable
        *******************************************************************************/
        public DataTable GetDataTable(string strSQL)
        {
            DataTable Returndt = null;
            DataTable dt = new DataTable();
            ClassFunction_Eform.ConnectDatabase Conn = new ClassFunction_Eform.ConnectDatabase(strConn);

            try
            {
                Conn.OpenDatabase();
                dt = Conn.GetData(strSQL);

                if ((dt == null))
                {
                    dt = null;
                }
                else
                {
                    if (dt.Rows.Count == 0)
                    {
                        Returndt = null;
                    }
                    else
                    {
                        Returndt = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Conn.RollbackDatabase();
                throw ex;
            }
            finally
            {
                Conn.CloseDatabase();
            }
            return Returndt;
        }

        public static DataTable getSAPRangeTable()
        {
            DataTable dtb = new DataTable();
            dtb.Columns.Add("Sign", typeof(string));
            dtb.Columns.Add("Option", typeof(string));
            dtb.Columns.Add("Low", typeof(string));
            dtb.Columns.Add("High", typeof(string));
            return dtb;
        }
        #endregion

 

 

        #region
        /// <summary>
        /// Create By Boy 2014/07/17
        /// </summary>
        /// <param name="oUserAD"></param>
        /// <returns></returns>
        public bool SendEmail(string oFromAddress, string oTo, string oCc, string oBcc, string oSubject, string oBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(oFromAddress);
                smtpClient.Host = "smtp.cmg.co.th";
                smtpClient.Port = 25;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential();
                message.From = fromAddress;
                message.To.Add(oTo);
                if (oCc != null)
                    message.CC.Add(oCc);
                if (oBcc != null)
                    message.Bcc.Add(oBcc);
                message.Subject = oSubject;
                message.IsBodyHtml = true;
                message.Body = oBody;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region Text
        public string doubleQuoteToSingle(string strMessage)
        {
            return strMessage.Replace("\"", "'");
        }
        public string replaceLinebreak(string strMessage)
        {
            return strMessage.Replace(System.Environment.NewLine, " ");
        }
        public static string replaceSpecialChars(string strText)
        {
            strText = strText.Replace("&", "&amp;");
            strText = strText.Replace("<", "&lt;");
            strText = strText.Replace(">", "&gt;");
            strText = strText.Replace("'", "&apos;");
            strText = strText.Replace("\"", "&quot;");
            return strText;
        }
        public static string replaceHexaChar(string strText)
        {
            byte[] ba = Encoding.Default.GetBytes(strText);
            string hexString = BitConverter.ToString(ba);
            hexString = replaceHexaWithSpace(hexString);

            byte[] data = HexToByte(hexString);
            Encoding objEncoding = Encoding.GetEncoding("Windows-874");
            string result = objEncoding.GetString(data);
            return result;
        }
        private static string replaceHexaWithSpace(string hex)
        {
            //Hex00 to Hex1F คือ Special Charactor
            //Hex20 = Space
            string result = hex;
            result = result.Replace("00", "20");
            result = result.Replace("01", "20");
            result = result.Replace("02", "20");
            result = result.Replace("03", "20");
            result = result.Replace("04", "20");
            result = result.Replace("05", "20");
            result = result.Replace("06", "20");
            result = result.Replace("07", "20");
            result = result.Replace("08", "20");
            result = result.Replace("09", "20");
            result = result.Replace("0A", "20");
            result = result.Replace("0B", "20");
            result = result.Replace("0C", "20");
            result = result.Replace("0D", "20");
            result = result.Replace("0E", "20");
            result = result.Replace("0F", "20");
            result = result.Replace("10", "20");
            result = result.Replace("11", "20");
            result = result.Replace("12", "20");
            result = result.Replace("13", "20");
            result = result.Replace("14", "20");
            result = result.Replace("15", "20");
            result = result.Replace("16", "20");
            result = result.Replace("17", "20");
            result = result.Replace("18", "20");
            result = result.Replace("19", "20");
            result = result.Replace("1A", "20");
            result = result.Replace("1B", "20");
            result = result.Replace("1C", "20");
            result = result.Replace("1D", "20");
            result = result.Replace("1E", "20");
            result = result.Replace("1F", "20");
            return result;
        }
        private static byte[] HexToByte(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        #endregion

        #region U
        /*******************************************************************************
        * Function ConvertToDate
        *******************************************************************************/
        public DateTime ConvertToDateControl(string dateString)
        {
            int indexYear = 2;
            int indexMonth = 1;
            int indexDay = 0;
            string[] dateArray = dateString.Split('/');
            return new System.DateTime(Convert.ToInt32(dateArray[indexYear]), Convert.ToInt32(dateArray[indexMonth]), Convert.ToInt32(dateArray[indexDay]));
        }
        #endregion

    }
}
