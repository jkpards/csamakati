﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        checkUsertype.filter("STAFF", Session["UserType"].ToString());

        SqlCommand cmd = new SqlCommand("SELECT StaffId, LName + ', ' + FName + ' (' + MName + ')' as FullName, Status, DateRegistered FROM dbo.Staff WHERE STATUS = 'ACTIVE'");

        ListViewStaff.DataSource = Class2.getDataSet(cmd);
        ListViewStaff.DataBind();
    }



    protected void btnAddStaff_Click(object sender, EventArgs e)
    {
        try
        {
            Literal1.Text = " <script> alert('FAILED TO ADD A STAFF'); </script>";
            SqlCommand cmdUser = new SqlCommand("[sp_t_User_ups]");
            cmdUser.CommandType = CommandType.StoredProcedure;
            cmdUser.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = "0";
            cmdUser.Parameters.Add("@UserType", SqlDbType.NVarChar).Value = "STAFF";
            cmdUser.Parameters.Add("@Username", SqlDbType.NVarChar).Value = tboxUsername.Text;
            cmdUser.Parameters.Add("@Password", SqlDbType.NVarChar).Value = "";
            Class2.exe(cmdUser);

            SqlCommand cmdStaff = new SqlCommand("[sp_t_Staff_ups]");
            cmdStaff.CommandType = CommandType.StoredProcedure;
            cmdStaff.Parameters.Add("@StaffId", SqlDbType.NVarChar).Value = "0";
            cmdStaff.Parameters.Add("@LName", SqlDbType.NVarChar).Value = tboxLName.Text;
            cmdStaff.Parameters.Add("@MName", SqlDbType.NVarChar).Value = tboxMName.Text;
            cmdStaff.Parameters.Add("@FName", SqlDbType.NVarChar).Value = tboxFName.Text;
            cmdStaff.Parameters.Add("@Status", SqlDbType.NVarChar).Value = ddlStatus.Text;
            cmdStaff.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = DBNull.Value;
            cmdStaff.Parameters.Add("@DateRegistered", SqlDbType.NVarChar).Value = DBNull.Value;
            Class2.exe(cmdStaff);

            Literal1.Text = " <script> alert('USER CREATION SUCCESSFUL.'); </script>";
        }
        catch
        {
            Literal1.Text = " <script> alert('FAILED TO ADD A STAFF.'); </script>";
        }
        Response.Redirect("ManageStaff.aspx");
    }

    protected void ListViewStaff_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteStaff")
        {
            SqlCommand cmdUser = new SqlCommand("UPDATE[dbo].[Staff] SET [Status] = 'INACTIVE' WHERE StaffId =" + e.CommandArgument);
            Class2.exe(cmdUser);
            Response.Redirect("ManageStaff.aspx");
        }
    }
}