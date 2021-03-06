using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            checkUsertype.filter("STAFF", Session["UserType"].ToString());
        }
        catch(Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('You have been inactive for too long. Please relogin.');window.location ='Out.aspx';", true);
        }

       populateLView();

        if(!IsPostBack)
        {
            ddlDepartment.DataSource = Class2.getDataSet("SELECT DeptName, DeptId FROM dbo.Department");
            ddlDepartment.DataValueField = "DeptId";
            ddlDepartment.DataTextField = "DeptName";
            ddlDepartment.DataBind();
        }
       
    }

   public void populateLView()
   {
         SqlCommand cmd = new SqlCommand("SELECT dbo.Department.DeptName, dbo.AcademicAdviser.AAdviserId, dbo.AcademicAdviser.LName + ', ' + dbo.AcademicAdviser.FName + ' (' + dbo.AcademicAdviser.MName + ')' as [FullName], dbo.AcademicAdviser.Status, dbo.AcademicAdviser.DateRegistered FROM dbo.AcademicAdviser INNER JOIN dbo.Department ON dbo.AcademicAdviser.DeptId = dbo.Department.DeptId");
        ListViewAAdvisers.DataSource = Class2.getDataSet(cmd);
        ListViewAAdvisers.DataBind();
    }

    protected void btnAddAcademicAdvisers_Click(object sender, EventArgs e)
    {
        if(btnAddAcademicAdviser.Text == "UPDATE ADVISER")
        {
            SqlCommand cmdEdAdv = new SqlCommand("UPDATE [dbo].[AcademicAdviser] SET [LName] = '" + tboxLName.Text + "', [MName] = '" + tboxMName.Text + "', [FName] = '" + tboxFName.Text + "', [DeptId] = " + ddlDepartment.SelectedValue + ", [Status] = 'ACTIVE' WHERE AAdviserId = " + Session["AAId"]);
            Class2.exe(cmdEdAdv);
        }
        else
        {
        try
        {
            SqlCommand cmdUser = new SqlCommand("[sp_t_User_ups]");
            cmdUser.CommandType = CommandType.StoredProcedure;
            cmdUser.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = "0";
            cmdUser.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = "FACULTY";
            cmdUser.Parameters.Add("@Username", SqlDbType.NVarChar).Value = tboxUsername.Text;
            cmdUser.Parameters.Add("@Password", SqlDbType.NVarChar).Value = "";
            Class2.exe(cmdUser);

            SqlCommand cmdFaculty = new SqlCommand("[sp_t_AcademicAdviser_ups]");
            cmdFaculty.CommandType = CommandType.StoredProcedure;
            cmdFaculty.Parameters.Add("@AAdviserId", SqlDbType.NVarChar).Value = "0";
            cmdFaculty.Parameters.Add("@DeptId", SqlDbType.NVarChar).Value = ddlDepartment.SelectedValue;
            cmdFaculty.Parameters.Add("@LName", SqlDbType.NVarChar).Value = tboxLName.Text;
            cmdFaculty.Parameters.Add("@MName", SqlDbType.NVarChar).Value = tboxMName.Text;
            cmdFaculty.Parameters.Add("@FName", SqlDbType.NVarChar).Value = tboxFName.Text;
            cmdFaculty.Parameters.Add("@Status", SqlDbType.NVarChar).Value = ddlStatus.Text.ToUpper();
            cmdFaculty.Parameters.Add("@DateRegistered", SqlDbType.NVarChar).Value = DBNull.Value;
            cmdFaculty.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = DBNull.Value;
            cmdFaculty.Parameters.Add("@AdviserSchedule", SqlDbType.NVarChar).Value = ";";
            Class2.exe(cmdFaculty);

            Literal1.Text = " <script> alert('USER CREATION SUCCESSFUL.'); </script>";
        }
        catch
        {
            Literal1.Text = " <script> alert('FAILED TO ADD AN ACADEMIC ADVISER.'); </script>";
        }
        }
        Response.Redirect("ManageAcademicAdvisers.aspx");
    }

    public void closePopup(object sender, EventArgs e)
    {
        btnAddAcademicAdviser.Text = "ADD ADVISER";
        ScriptManager.RegisterStartupScript(this, typeof(string), "uniqueKey", "div_show()", true);
        tboxLName.Text = "";
        tboxFName.Text = "";
        tboxMName.Text = "";
        ddlStatus.Enabled = true;
        trUname.Visible = true;
    }

    public void searchKey(object sender, EventArgs e)
    {
         try
         {
         if(ddlDept.SelectedIndex != 0)
             sortByDept(Session["SearchBy"].ToString(), tboxSKey.Text);
         else
         {
              int i = 0;
              Session["SearchBy"] = "dbo.Department.DeptName, (dbo.AcademicAdviser.LName + ', ' + dbo.AcademicAdviser.FName + ' (' + dbo.AcademicAdviser.MName + ')')";
              do
              {
                 if(i == 1)
                      sortByDept(Session["SearchBy"].ToString().Split(',')[i] + Session["SearchBy"].ToString().Split(',')[i+1], tboxSKey.Text);
                  else
                      sortByDept(Session["SearchBy"].ToString().Split(',')[i], tboxSKey.Text);
                  i++;
              }while(ListViewAAdvisers.Items.Count == 0 && i <= 2);
         }
         }
         catch(Exception ex)
         {
              ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No data found for the search key.');window.location ='ManageAcademicAdvisers.aspx';", true);
         }
    }

   public void sortByDepartment(object sender, EventArgs e)
    {
        if(ddlDept.SelectedIndex == 1)
           Session["SearchBy"] = "dbo.Department.DeptName";
        else if(ddlDept.SelectedIndex == 2)
            Session["SearchBy"] = "(dbo.AcademicAdviser.LName + ', ' + dbo.AcademicAdviser.FName + ' (' + dbo.AcademicAdviser.MName + ')')";
    }


    public void sortByDept(string column, string key)
    {
        SqlCommand cmd = new SqlCommand("SELECT dbo.Department.DeptName, dbo.AcademicAdviser.AAdviserId, dbo.AcademicAdviser.LName + ', ' + dbo.AcademicAdviser.FName + ' (' + dbo.AcademicAdviser.MName + ')' as [FullName], dbo.AcademicAdviser.Status, dbo.AcademicAdviser.DateRegistered FROM dbo.AcademicAdviser INNER JOIN dbo.Department ON dbo.AcademicAdviser.DeptId = dbo.Department.DeptId WHERE Status = 'ACTIVE' and " + column + " LIKE '%" + key + "%'");

        ListViewAAdvisers.DataSource = Class2.getDataSet(cmd);
        ListViewAAdvisers.DataBind();
    }

    protected void ListViewAAdvisers_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteAcademic")
        {
            SqlCommand cmdUser = new SqlCommand("UPDATE[dbo].[AcademicAdviser] SET [Status] = 'INACTIVE' WHERE AAdviserId =" + e.CommandArgument);
            Class2.exe(cmdUser);
            Response.Redirect("ManageAcademicAdvisers.aspx");
        }
        else if(e.CommandName == "UpdateAcademic")
        {
            Session["AAId"] = e.CommandArgument;
            trUname.Visible = false;
            ddlStatus.Enabled = false;
            String adv = Class2.getSingleData("SELECT LName + ';' + FName + ';' + MName + ';' + CONVERT(varchar(10),DeptId) + ';' + Status FROM [AcademicAdviser] WHERE [AAdviserId] = " + e.CommandArgument);
            tboxLName.Text = adv.Split(';')[0];
            tboxFName.Text = adv.Split(';')[1];
            tboxMName.Text = adv.Split(';')[2];
            ddlDepartment.SelectedIndex = Convert.ToInt32(adv.Split(';')[3]) - 1;
            ScriptManager.RegisterStartupScript(this, typeof(string), "uniqueKey", "div_show()", true);
            btnAddAcademicAdviser.Text = "UPDATE ADVISER";
        }
    }
}



