using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SGF.App_GlobalResources;
//using System.Globalization;
using System.Web.Mvc;
//using System.Web.Security;
using SGF.Models.EF;


namespace SGF.Models
{
    public class AccountDataModel
    { 
        [Required]
        [Display(Name = "AccountId")]
        public int Account_Id { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "The AccountNumber must be less than 50 characters long.")]
        [Display(Name = "AccountNumber")]
        public string Account_Number { get; set; }

        [StringLength(50, ErrorMessage = "The AccountName must be less than 50 characters long.")]
        [Display(Name = "AccountName")]
        public string Account_Name { get; set; } 

        [StringLength(50, ErrorMessage = "The BankName must be less than 50 characters long.")]
        [Display(Name = "BankName")]
        public string Bank_Name	{ get; set; }

        [StringLength(100, ErrorMessage = "The OtherInfo must be less than 100 characters long.")]
        [Display(Name = "OtherInfo")]
        public string Other_Info { get; set; }
        
        [Required]
        [Display(Name = "Balance")]
        public double Balance	{ get; set; }

        //public AccountDataModel(int _Account_Id, string _Account_Number, string _Account_Name, string _Bank_Name,
        //                   string _Other_Info, double _Balance)
        //{
        //    Account_Id = _Account_Id;
        //    Account_Number = _Account_Number;
        //    Account_Name = _Account_Name;
        //    Bank_Name = _Bank_Name;
        //    Other_Info = _Other_Info;
        //    Balance = _Balance;
        //}
    }

    public class UserAccountModel
    {
        public int UserId { get; set; }

        public List<SGF_T_ACCOUNT> Accounts { get; set; }
    }

    public class AccountDashBoardModel
    {
        public SGF_PROC_DASHBOARD_Result dashboard  { get; set; }
    }

    public class AccountMovementModel
    {
        public int MovId { get; set; }
        public int AccountId { get; set; }
        public string MovTypeDes { get; set; }
        public int CatId { get; set; }  //new
        public string CatDes { get; set; }
        public int SubCatId { get; set; }   //new
        public string SubCatDes { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ReportsModel
    {
        //for now 0 - PieChart; 1 - Bars
        public int ReportType { get; set; }
        public List<SGF_PROC_REPORTS_BY_CATEGORY_Result> ReportData { get; set; }
    }

    public class MovementTypesModel
    {
        public List<SGF_PROC_GET_MOVEMENT_TYPES_Result> MovTypes { get; set; }
    }
}