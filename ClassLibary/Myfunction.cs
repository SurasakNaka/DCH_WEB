using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.OleDb;

/// <summary>
/// Summary description for Myfunction
/// </summary>
public class Myfunction
{
    public Myfunction()
    {

    }

    #region By MEE
    public static bool CheckSeqApproved(string Empcode, string Costcenter, string formId)//Check User ว่ามี Approved list ใน Form หรือไม่
    {
        ClassFunction_Eform.ConnectDatabase fn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
        string[] Paraname = new string[] { "@Current_user", "@Cost_Center", "@Flow_ID" };
        string[] Paravalue = new string[] { Empcode, Costcenter, formId };
        fn.OpenDatabase();
        DataTable dt = fn.GetDataStored_Para("sps_GetMaxApprover", Paraname, Paravalue);
        fn.CloseDatabase();
        if (dt.Rows.Count > 0)
        { return true; }
        else { return false; }
    }
    public static decimal Sumdata(DataTable dt, string ColumnSum)
    {
        object objSum;
        objSum = dt.Compute("Sum(" + ColumnSum + ")", "");
        return decimal.Parse(objSum.ToString());
    }
    public static string CalculateHour(string StartTime, string EndTime)
    {
        string[] StrTmp; string Totaltime = string.Empty;
        StrTmp = Splittime(StartTime); TimeSpan tstmp = new TimeSpan(1, 0, 0);
        TimeSpan tsstarttime = new TimeSpan(int.Parse(StrTmp[0]), int.Parse(StrTmp[1]), 0);
        StrTmp = Splittime(EndTime);
        TimeSpan tsendttime = new TimeSpan(int.Parse(StrTmp[0]), int.Parse(StrTmp[1]), 0);
        if (tsendttime > tsstarttime)
        {
            TimeSpan tstotal = TimeSpan.FromTicks((tsendttime.Ticks - tsstarttime.Ticks) - tstmp.Ticks);//คำนวณเวลาโดยหักเวลาพักเที่ยงออก 1 ชม
            int Mytime = CalTotalHour(tstotal.Days, tstotal.Hours);
            Totaltime = Mytime + "." + tstotal.Minutes;
        }
        return Totaltime;
    }
    public static int CalTotalHour(int Day, int Hour)
    {
        int total = 0;
        if (Day > 0)
        {
            Day = Day * 24;
            return total = Day + Hour;
        }
        else { return Hour; }
    }
    public static string SubStringUser(string UserLogin)
    {
        string MyUserlogin = UserLogin.Substring(4, UserLogin.Length - 4);
        return MyUserlogin;
    }
    public string MyDatediff(string startdate, string enddate)
    {
        string returndate = "";
        try
        {
            if (Convert.ToDateTime(enddate) >= Convert.ToDateTime(startdate))
            {
                TimeSpan ts = Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate);
                returndate = (ts.Days + 1).ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return returndate;
    }
    public static string Mytimediff(string starttime, string endtime, int breaktime)
    {
        string totaltime = "";
        string[] wordstart = Splittime(starttime);
        string[] wordend = Splittime(endtime);
        TimeSpan st = new TimeSpan(Convert.ToInt16(wordstart[0]), Convert.ToInt16(wordstart[1]), 0);
        TimeSpan et = new TimeSpan(Convert.ToInt16(wordend[0]), Convert.ToInt16(wordend[1]), 0);
        if (et > st)
        {
            TimeSpan ts = et - st;
            TimeSpan tsbreak = new TimeSpan(breaktime, 0, 0);
            ts = TimeSpan.FromTicks(ts.Ticks - tsbreak.Ticks);
            string hour, min;
            hour = ts.Hours.ToString();
            min = ts.Minutes.ToString();
            totaltime = hour + "." + min;
            //Mytimetotal = Convert.ToDecimal(hour + "." + min);
        }
        return totaltime;
    }
    public static string[] SplitFormatTime(string time)
    {
        string[] word = time.Split(':');
        int i = 0;
        foreach (string str in word)
        {
            word[i] = str;
            i++;
        }
        return word;
    }
    public static string[] Splittime(string time)
    {
        string[] word = time.Split('.');
        int i = 0;
        foreach (string str in word)
        {
            word[i] = str;
            i++;
        }
        return word;
    }
    public static string[] SplitComma(string Myword)
    {
        string[] word = Myword.Split(',');
        int i = 0;
        foreach (string str in word)
        {
            word[i] = str;
            i++;
        }
        return word;
    }
    public string[] Wrap(string text, int maxLength)
    {
        text = text.Replace("\n", " ");
        text = text.Replace("\r", " ");
        text = text.Replace(".", ". ");
        text = text.Replace(">", "> ");
        text = text.Replace("\t", " ");
        text = text.Replace(",", ", ");
        text = text.Replace(";", "; ");
        text = text.Replace("<br>", " ");
        text = text.Replace(" ", " ");

        string[] Words = text.Split(' ');
        int currentLineLength = 0;
        ArrayList Lines = new ArrayList(text.Length / maxLength);
        string currentLine = "";
        bool InTag = false;

        foreach (string currentWord in Words)
        {
            //ignore html
            if (currentWord.Length > 0)
            {

                if (currentWord.Substring(0, 1) == "<")
                    InTag = true;

                if (InTag)
                {
                    //handle filenames inside html tags
                    if (currentLine.EndsWith("."))
                    {
                        currentLine += currentWord;
                    }
                    else
                        currentLine += " " + currentWord;

                    if (currentWord.IndexOf(">") > -1)
                        InTag = false;
                }
                else
                {
                    if (currentLineLength + currentWord.Length + 1 < maxLength)
                    {
                        currentLine += " " + currentWord;
                        currentLineLength += (currentWord.Length + 1);
                    }
                    else
                    {
                        Lines.Add(currentLine);
                        currentLine = currentWord;
                        currentLineLength = currentWord.Length;
                    }
                }
            }

        }
        if (currentLine != "")
            Lines.Add(currentLine);

        string[] textLinesStr = new string[Lines.Count];
        Lines.CopyTo(textLinesStr, 0);
        return textLinesStr;
    }
    public static DataTable SumAccum(DataTable dtLeave)
    {
        var query = from row in dtLeave.AsEnumerable()
                    group row by row.Field<int>("Id") into grp
                    orderby grp.Key
                    select new
                    {
                        Id = grp.Key,
                        Sum = grp.Sum(r => r.Field<decimal>("Value"))
                    };
        DataTable dt = dtLeave.Clone();
        foreach (var grp in query)
        {
            DataRow dr = dt.NewRow();
            dr["Id"] = grp.Id;
            dr["Value"] = grp.Sum;
            dt.Rows.Add(dr);
        }
        return dt;
    }
    public static bool CheckDateHoliday(string Mydate, string Working_Location)//Check ว่าวันที่ลาตรงกับวันหยุดหรือไม่
    {
        string name = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;
        System.Globalization.CultureInfo inputCultureInfo = new System.Globalization.CultureInfo(name);
        inputCultureInfo.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
        System.Globalization.CultureInfo displayCultureInfo = new System.Globalization.CultureInfo("en-US");

        DateTime Dttime = DateTime.Parse(Mydate, inputCultureInfo);
        string sql = @"select Holiday from M_Holiday 
                       WHERE Holiday='" + Dttime.ToString("yyyyMMdd", inputCultureInfo) + "' AND Working_Location = '" + Working_Location + "'";
        ClassFunction_Eform.ConnectDatabase fn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
        fn.OpenDatabase();
        DataTable dt = fn.GetData(sql);
        fn.CloseDatabase();
        if (dt.Rows.Count > 0)//Retrun true= ตรงกลับวันหยุดประจำปี หรือเสาร์-อาทิตย์
        {
            return true;
        }
        switch (Dttime.DayOfWeek)
        {
            case DayOfWeek.Saturday: return true;
                break;
            case DayOfWeek.Sunday: return true;
                break;
            default: return false;
                break;
        }
    }
    public DataTable GetDataExcel(string oStrcon, string sql)
    {
        DataTable dtdata = new DataTable();

        OleDbCommand cm = new OleDbCommand();
        OleDbConnection cn = new OleDbConnection(oStrcon);
        OleDbDataReader rd = null;

        try
        {
            cm = new OleDbCommand(sql, cn);
            cn.Open();
            rd = cm.ExecuteReader();
            dtdata.Load(rd);
        }
        catch (Exception ex)
        {
            
        }
        finally
        {
            cn.Close();
            rd.Close();
            rd.Dispose();
        }
        return dtdata;
    }
    #endregion

    #region By X
    //Get month name in datatable form
    public static DataTable getMonthName()
    {
        DataTable dtb = new DataTable();
        dtb.Columns.Add("text", typeof(string));
        dtb.Columns.Add("value", typeof(string));

        dtb.Rows.Add("มกราคม", "1");
        dtb.Rows.Add("กุมภาพันธ์", "2");
        dtb.Rows.Add("มีนาคม", "3");
        dtb.Rows.Add("เมษายน", "4");
        dtb.Rows.Add("พฤษภาคม", "5");
        dtb.Rows.Add("มิถุนายน", "6");
        dtb.Rows.Add("กรกฎาคม", "7");
        dtb.Rows.Add("สิงหาคม", "8");
        dtb.Rows.Add("กันยายน", "9");
        dtb.Rows.Add("ตุลาคม", "10");
        dtb.Rows.Add("พฤศจิกายน", "11");
        dtb.Rows.Add("ธันวาคม", "12");

        return dtb;
    }

    //Check validate date
    public static bool checkValidateDate(string strDate, IFormatProvider format)
    {
        try
        {
            DateTime dt = DateTime.Parse(strDate, format);
            return true;
        }
        catch
        {
            return false;
        }
    }

    //If number, Return number without quote.
    public static string convertNumber(string str)
    {
        if (str != null)
        {
            if (!str.Equals(""))
            {
                return str;
            }
            else
            {
                return ("NULL");
            }
        }
        else
        {
            return ("NULL");
        }
    }

    //If string, Return string with quote.
    public static string convertVarchar(string str)
    {
        if (str != null)
        {
            if (!str.Equals(""))
            {
                return "'" + ReplaceQuote(str).ToString() + "'";
            }
            else
            {
                return ("NULL");
            }
        }
        else
        {
            return ("NULL");
        }
    }

    //If string, Return string with 'N' and quote.
    public static string convertNVarchar(string str)
    {
        if (str != null)
        {
            if (!str.Equals(""))
            {
                return "N'" + ReplaceQuote(str).ToString() + "'";
            }
            else
            {
                return ("NULL");
            }
        }
        else
        {
            return ("NULL");
        }
    }

    //Convert Christ Era to Buddha Era
    public static string convertToBuddhaEra(string strChristEra)
    {
        try
        {
            int iYear;
            iYear = int.Parse(strChristEra) + 543;
            return iYear.ToString();
        }
        catch (Exception err)
        {
            throw err;
        }
    }

    //Convert Buddha Era to Christ Era
    public static string convertToChristEra(string strBuddhaEra)
    {
        try
        {
            int iYear;
            iYear = int.Parse(strBuddhaEra) - 543;
            return iYear.ToString();
        }
        catch (Exception err)
        {
            throw err;
        }
    }
    #endregion

    #region Ou

    /*-----------------------------------------------------------------------------
    | Function ClickOnceButton: ให้ Button Control กดปุ่มได้แค่ครั้งเดียว
    -----------------------------------------------------------------------------*/
    public static void ClickOnceButton(ref Page page, ref Button b)
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

    public static void ClickOnceButton(ref Page page, ref Button b, string Mes)
    {
        System.Text.StringBuilder oneClickScript = new System.Text.StringBuilder();
        oneClickScript.Append("if(!confirm('คุณต้องการจะบันทึกข้อมูลหรือไม่.. ?')){{ ");
        oneClickScript.Append("if (typeof(Page_ClientValidate) == 'function') { ");
        oneClickScript.Append("if (Page_ClientValidate() == false) { return false; }} ");
        oneClickScript.Append("this.value = 'Please wait…';");
        oneClickScript.Append("this.disabled = true;");
        oneClickScript.Append(page.ClientScript.GetPostBackEventReference(b, ""));
        oneClickScript.Append("; }} { return false; }");
        b.Attributes.Add("onclick", oneClickScript.ToString());
    }
    /*-----------------------------------------------------------------------------
    | Function CheckFormatDate: ฺ
    -----------------------------------------------------------------------------*/
    public static bool CheckFormatDate(string pDate)
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

    public static bool isLeapYear(int year)
    {
        return (((year % 4) == 0) && ((year % 100) != 0) || ((year % 400) == 0));
    }

    /*-----------------------------------------------------------------------------
    | Function SetYearList: ฺ BindDropDownList ตามปีปัจจุบัน (บวก/ลบ ตามจำนวน กี่ปีข้างหน้า/กี่ปี่ข้างหลัง)
    -----------------------------------------------------------------------------*/
    public static void SetYearList(DropDownList objYear, short nBackward, short nForward)
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

    /*-----------------------------------------------------------------------------
    | Function SetYearList: ฺ BindDropDownList ตามปีปัจจุบัน (บวก/ลบ ตามจำนวน กี่ปีข้างหน้า/กี่ปี่ข้างหลัง)
    -----------------------------------------------------------------------------*/
    public static void SetYearList(DropDownList objYear, short nBackward, short nForward, bool Alltxt)
    {
        string strYear = "";
        string Today = "";
        int Index, iYear;

        Today = ConvertYear(DateTime.Now.Year.ToString(), "C");
        strYear = string.Format(Today, "yyyy");

        if (Alltxt == true)
        {
            Index = 0;
            objYear.Items.Add("--- All ---");
            objYear.Items[Index].Value = "0";
            Index = Index + 1;
        }
        else
        {
            Index = 0;
        }

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

    /*-----------------------------------------------------------------------------
    | Function ConvertYear: Return รูปแบบปีที่ต้องการ
    |    B = พุทธศักราช   /  C = คริสต์ศักราช 
    -----------------------------------------------------------------------------*/
    public static string ConvertYear(string strYear, string EraType)
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

    /*-----------------------------------------------------------------------------
    | Function Convert_to_yyyymmdd:
    -----------------------------------------------------------------------------*/
    public static string Convert_to_yyyymmdd(string pDate, string pType)
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

    /*-----------------------------------------------------------------------------
    | Function GetDataTable: Return DataTable
    -----------------------------------------------------------------------------*/
    public static DataTable GetDataTableSchema(string strSQL)
    {
        DataTable dt = new DataTable();
        ClassFunction_Eform.ConnectDatabase Conn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);

        try
        {
            Conn.OpenDatabase();
            dt = Conn.GetData(strSQL);
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
        return dt;
    }

    /*-----------------------------------------------------------------------------
    | Function GetDataTable: Return DataTable
    -----------------------------------------------------------------------------*/
    public static DataTable GetDataTable(string strSQL)
    {
        DataTable Returndt = null;
        DataTable dt = new DataTable();
        ClassFunction_Eform.ConnectDatabase Conn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);

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

    /*-----------------------------------------------------------------------------
    | Function GetDataTable: Return DataTable
    -----------------------------------------------------------------------------*/
    public static DataTable GetDataTable(string strSQL, string strConn)
    {
        DataTable Returndt = null;
        DataTable dt = new DataTable();
        ClassFunction_Eform.ConnectDatabase Conn;

        if (strConn == "")
        {
            Conn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings["Myconnect"].ConnectionString);
        }
        else
        {
            Conn = new ClassFunction_Eform.ConnectDatabase(System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString);
        }

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

    /*-----------------------------------------------------------------------------
    | Function Get_SysDate: Return วันที่ปัจจุบัน Format --> yyyy/mm/dd
    -----------------------------------------------------------------------------*/
    public static string Get_SysDate()
    {
        string sYear = null;
        string sRet = null;

        sYear = Convert.ToString(DateTime.Now.Year);
        if (Convert.ToInt32(sYear) > 2500) sYear = Convert.ToString(Convert.ToInt32(sYear) - 543);

        sRet = sYear + "/" + Convert.ToString(DateTime.Now.Month).PadRight(2) + "/" + Convert.ToString(DateTime.Now.Day).PadRight(2);
        sRet = sRet + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        return sRet;
    }

    /*------------------------------------------------------------------------------
    | Function ReplaceQuote: แทนค่า Query String ที่มี Single Quote (' --> '')
    --------------------------------------------------------------------------------*/
    public static Object ReplaceQuote(string strString)
    {
        object retValue = null;

        if (Convert.IsDBNull(strString))
        {
            retValue = strString;
        }
        else
        {
            //retValue = String.Replace(strString, "'", "''");
            retValue = strString.Replace("'", "''");
        }

        return retValue;
    }

    /*------------------------------------------------------------------------------
    | Function ValidateFloatFormat
    --------------------------------------------------------------------------------*/
    public static bool ValidateFloatFormat(string vStr)
    {
        //Regex regX = new Regex(@"\b[0-9]+\.([0-9]+\b)?|\.[0-9]+\b");
        Regex regX = new Regex("^[0-9]*[.]?[0-9]+$");
        return (regX.IsMatch(vStr));
    }

    /*------------------------------------------------------------------------------
    | Function Convert_DateFormat
    --------------------------------------------------------------------------------*/
    public static string Convert_DateFormat(string pDate, string pType, string pFormat)
    {
        string[] arrDate = pDate.Split('/');
        string retDate = "";
        string vDate = "";
        string vMonth = "";
        short vYear = 0;

        vYear = Convert.ToInt16(arrDate[0]);
        vMonth = arrDate[1].PadRight(2, '0');
        vDate = arrDate[2].PadRight(2, '0');

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

        if (pFormat == "1")
        {
            // format yyyy/mm/dd
            retDate = Convert.ToString(vYear) + "/" + Convert.ToDecimal(vMonth).ToString("00") + "/" + vDate;
        }
        else
        {
            // format dd/mm/yyyy
            retDate = Convert.ToDecimal(vDate).ToString("00") + "/" + Convert.ToDecimal(vMonth).ToString("00") + "/" + Convert.ToString(vYear);
        }

        return retDate;
    }

    #endregion
}
