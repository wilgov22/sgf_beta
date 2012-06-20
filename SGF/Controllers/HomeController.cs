using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGF.Models.EF;
using SGF.Models;
using System.Data.Objects;

namespace SGF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            if (Session["userOnline"] != null)
            {
                AccountModel user = (AccountModel)Session["userOnline"];
                GetAccount(user.UserId);
            }
            else
                return RedirectToAction("LogOn", "Account");
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult GetAccount(int UserId = 0)
        {
            if (UserId == 0)
                return View();

            UserAccountModel model = null;

            using (SGFEntitiess uAccount = new SGFEntitiess())
            {
                /*select a.Account_Number from SGF_T_ACCOUNT a, SGF_T_ACCOUNT_USER au
                where a.Account_Id = au.Account_Id and au.User_id = 1*/

                var result = from n in uAccount.SGF_T_ACCOUNT
                             join n2 in uAccount.SGF_T_ACCOUNT_USER on n.ACCOUNT_ID equals n2.ACCOUNT_ID
                             where n2.USER_ID == UserId
                             select n;
                result.ToList();

                model = new UserAccountModel
                {
                    UserId = UserId,
                    Accounts = new List<SGF_T_ACCOUNT>()
                };

                if (result.Count() > 0)
                    //model.Accounts = result.AsEnumerable().Cast<AccountData>().ToList();
                    model.Accounts = result.ToList();
                else
                    model.Accounts = null;

                Session["userOnlineAccounts"] = model;
            }

            return View();

        }//END GetAccount()

        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAccountDashBoard(int Id = 0)
        {
            if (Id == 0)
            {
                Session["accountDashboard"] = null;
                return View();
            }

            AccountDashBoardModel dashboardmodel = new AccountDashBoardModel();
            SGF_PROC_DASHBOARD_Result dashboard = new SGF_PROC_DASHBOARD_Result();

            using (SGFEntitiess aDashBoard = new SGFEntitiess())
            {
                dashboard = aDashBoard.SGF_PROC_DASHBOARD(Id).SingleOrDefault();
            }

            dashboardmodel.dashboard = dashboard;
            Session["accountDashboard"] = dashboardmodel;

            GetAccountUser(Id);
            GetTop10Movements(Id);

            return View();
            //string returnUrl = HttpContext.Request.UrlReferrer.AbsolutePath;
            //return Redirect(returnUrl);
            //return RedirectToAction("Index", "Home");

        }//GetAccountDashBoard

        //public ActionResult RegisterAccountBank()
        //{
        //    AccountDashBoardModel dashboardmodel = null;
        //    AccountDataModel model = new AccountDataModel();

        //    if (Session["accountDashboard"] != null)
        //    {
        //        int AccountId = 0;
        //        dashboardmodel = (AccountDashBoardModel)Session["accountDashboard"];
        //        AccountId = dashboardmodel.dashboard.Account_Id;

        //        using (SGFEntitiess accountData = new SGFEntitiess())
        //        {
        //            var result = from n in accountData.SGF_T_ACCOUNT
        //                         where n.ACCOUNT_ID == AccountId
        //                         select n;

        //            var acc = result.ToList().SingleOrDefault();
        //            model.Account_Id = acc.ACCOUNT_ID;
        //            model.Account_Name = acc.ACCOUNT_NAME;
        //            model.Account_Number = acc.ACCOUNT_NUMBER;
        //            model.Balance = (double)acc.BALANCE;
        //            model.Bank_Name = acc.BANK_NAME;
        //            model.Other_Info = acc.OTHER_INFO;

        //            Session["accountOnline"] = model;

        //        }
        //    }

        //    return View(model);
        //}//END RegisterAccountBank

        //// POST: /Home/RegisterAccountBank
        //[HttpPost]
        //public ActionResult RegisterAccountBank(AccountDataModel model)
        //{
        //    bool createAccount = false;
        //    CreateAccount(model.Account_Id, model.Account_Number, model.Account_Name, model.Bank_Name, model.Other_Info, out createAccount);

        //    if (createAccount)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return View(model);
        //        //ModelState.AddModelError("", ErrorCodeToString(createStatus));
        //        //return RedirectToAction("Register", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        public ActionResult RegisterAccountBank(int Id = 0)
        {
            /*
            AccountDashBoardModel dashboardmodel = null;
            AccountDataModel model = new AccountDataModel();

            if (Session["accountDashboard"] != null)
            {
                int AccountId = 0;
                dashboardmodel = (AccountDashBoardModel)Session["accountDashboard"];
                AccountId = dashboardmodel.dashboard.Account_Id;

                using (SGFEntitiess accountData = new SGFEntitiess())
                {
                    var result = from n in accountData.SGF_T_ACCOUNT
                                 where n.ACCOUNT_ID == AccountId
                                 select n;

                    var acc = result.ToList().SingleOrDefault();
                    model.Account_Id = acc.ACCOUNT_ID;
                    model.Account_Name = acc.ACCOUNT_NAME;
                    model.Account_Number = acc.ACCOUNT_NUMBER;
                    model.Balance = (double)acc.BALANCE;
                    model.Bank_Name = acc.BANK_NAME;
                    model.Other_Info = acc.OTHER_INFO;

                    Session["accountOnline"] = model;

                }
            }
            */

            AccountDataModel model = new AccountDataModel();

            if (Id == 0)
            {
                return View(model);
            }
            int AccountId = Id;

            using (SGFEntitiess accountData = new SGFEntitiess())
            {
                var result = from n in accountData.SGF_T_ACCOUNT
                             where n.ACCOUNT_ID == AccountId
                             select n;

                var acc = result.ToList().SingleOrDefault();
                model.Account_Id = acc.ACCOUNT_ID;
                model.Account_Name = acc.ACCOUNT_NAME;
                model.Account_Number = acc.ACCOUNT_NUMBER;
                model.Balance = (double)acc.BALANCE;
                model.Bank_Name = acc.BANK_NAME;
                model.Other_Info = acc.OTHER_INFO;

                Session["accountOnline"] = model;
            }

            return View(model);
        }//END RegisterAccountBank

        // POST: /Home/RegisterAccountBank
        [HttpPost]
        public ActionResult RegisterAccountBank(AccountDataModel model)
        {
            bool createAccount = false;
            CreateAccount(model.Account_Id, model.Account_Number, model.Account_Name, model.Bank_Name, model.Other_Info, out createAccount);

            if (createAccount)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
                //ModelState.AddModelError("", ErrorCodeToString(createStatus));
                //return RedirectToAction("Register", "Account");
            }

        }

        public ActionResult Accounts()
        {
            return View();
        }
        private void CreateAccount(int AccountId, string AccountNumber, string AccountName,
                                    string BankName, string OtherInfo, out bool status)
        {
            using (SGFEntitiess sessionAccount = new SGFEntitiess())
            {
                AccountModel user = (AccountModel)Session["userOnline"];

                //Valida se existe já conta registada para este User
                var result = from n in sessionAccount.SGF_T_ACCOUNT
                                join n2 in sessionAccount.SGF_T_ACCOUNT_USER on n.ACCOUNT_ID equals n2.ACCOUNT_ID
                                where n2.USER_ID == user.UserId && n.ACCOUNT_NUMBER == AccountNumber
                                select n;
                result.ToList();

                //Já existe a conta em questão
                if (result.SingleOrDefault() != null)
                {
                    SGF_T_ACCOUNT acc = result.SingleOrDefault();
                    acc.ACCOUNT_NAME = AccountName;
                    acc.BANK_NAME = BankName;
                    acc.OTHER_INFO = OtherInfo;
                }
                else
                {
                    //Add new Account
                    SGF_T_ACCOUNT acc =  SGF_T_ACCOUNT.CreateSGF_T_ACCOUNT(-1, AccountNumber, 0, AccountName, BankName, OtherInfo);
                    //SGF_T_ACCOUNT acc = SGF_T_ACCOUNT.CreateSGF_T_ACCOUNT(-1, AccountNumber,0);//so os obrigatorios
                    sessionAccount.AddToSGF_T_ACCOUNT(acc);
                    //Add new Account to User
                    SGF_T_ACCOUNT_USER accUser = SGF_T_ACCOUNT_USER.CreateSGF_T_ACCOUNT_USER(acc.ACCOUNT_ID, user.UserId, 1);
                    sessionAccount.AddToSGF_T_ACCOUNT_USER(accUser);
                }
                //commit changes
                sessionAccount.SaveChanges();
                
            }

            status = true;
            return;
        }//END CreateAccount


        //Get Current Account Online
        public void GetAccountUser(int AccountId)
        { 
            if (Session["userOnline"] == null) return;
            
            AccountModel user = (AccountModel)Session["userOnline"];
            AccountDataModel accUser = null;
            using (SGFEntitiess acc = new SGFEntitiess())
            {
                var result = from a in acc.SGF_T_ACCOUNT
                                join au in acc.SGF_T_ACCOUNT_USER on a.ACCOUNT_ID equals au.ACCOUNT_ID
                                where a.ACCOUNT_ID == AccountId && au.USER_ID == user.UserId
                                select a; 
                    
                var accResult = result.SingleOrDefault();
                
                //Asign account
                accUser = new AccountDataModel();
                accUser.Account_Id = accResult.ACCOUNT_ID;
                accUser.Account_Name = accResult.ACCOUNT_NAME;
                accUser.Account_Number = accResult.ACCOUNT_NUMBER;
                accUser.Balance = (double)accResult.BALANCE;
                accUser.Bank_Name = accResult.BANK_NAME;
                accUser.Other_Info = accResult.OTHER_INFO;
            }
            Session["accountOnline"] = accUser;
        }
        
        //Return only the top 10 movements order by Date descending
        public void GetTop10Movements(int AccountId)
        {
               using (SGFEntitiess accMovements = new SGFEntitiess())
                {
                   /*SELECT mt.DESCRIPTION MOV_TYPE_DES, c.DESCRIPTION CAT_DES, sc.DESCRIPTION SUB_CAT_DES,
                    ac.AMOUNT, ac.DESCRIPTION, CONVERT(VARCHAR(19),ac.CREATED_DATE,120) CREATED_DATE
	                FROM	SGF_T_ACCOUNT_MOVEMENT ac,
			                SGF_R_MOVEMENT_TYPE mt,
			                SGF_R_CATEGORY c,
			                SGF_R_SUB_CATEGORY sc
	                        WHERE ac.MOV_TYPE_ID = mt.MOV_TYPE_ID
	                            AND c.CAT_ID = ac.CAT_ID
	                            AND sc.SUB_CAT_ID = ac.SUB_CAT_ID
	                            AND ac.ACCOUNT_ID = 2*/
                    var result = (from ac in accMovements.SGF_T_ACCOUNT_MOVEMENT
                                 join mt in accMovements.SGF_R_MOVEMENT_TYPE on ac.MOV_TYPE_ID equals mt.MOV_TYPE_ID
                                 join c in accMovements.SGF_R_CATEGORY on ac.CAT_ID equals c.CAT_ID
                                 join sc in accMovements.SGF_R_SUB_CATEGORY on ac.SUB_CAT_ID equals sc.SUB_CAT_ID
                                 where ac.ACCOUNT_ID == AccountId
                                 orderby ac.CREATED_DATE descending
                                 select new { ac.MOV_ID, ac.ACCOUNT_ID, MOV_TYPE_DES = mt.DESCRIPTION, CAT_DES = c.DESCRIPTION, 
                                              SUB_CAT_DES = sc.DESCRIPTION, ac.AMOUNT, ac.DESCRIPTION, ac.CREATED_DATE}).Take(10); 
                    List<AccountMovementModel> lstResult = new List<AccountMovementModel>();
                    //foreach (var obj in result.ToList().Take(10)) 
                    foreach (var obj in result.ToList())
                    {
                        AccountMovementModel amm = new AccountMovementModel();
                        amm.MovId = obj.MOV_ID;
                        amm.AccountId = obj.ACCOUNT_ID;
                        amm.MovTypeDes = obj.MOV_TYPE_DES;
                        amm.CatDes = obj.CAT_DES;
                        amm.SubCatDes = obj.SUB_CAT_DES;
                        amm.Amount = (double)obj.AMOUNT;
                        amm.Description = obj.DESCRIPTION;
                        amm.CreatedDate = obj.CREATED_DATE;
                        lstResult.Add(amm);
                    }
                    Session["accountOnlineMovements"] = lstResult;
                }
        }

        //View for Register Movements
        public ActionResult RegisterMovement()
        {
            if (Session["accountOnline"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Only load movtypes if nor yet in memory
                if(Session["movementTypes"] == null) GetMovementTypes();

                AccountDataModel acc = (AccountDataModel)Session["accountOnline"];
                //Get all movements of Account Online
                using (SGFEntitiess accMovements = new SGFEntitiess())
                {
                    var result = from ac in accMovements.SGF_T_ACCOUNT_MOVEMENT
                                 join mt in accMovements.SGF_R_MOVEMENT_TYPE on ac.MOV_TYPE_ID equals mt.MOV_TYPE_ID
                                 join c in accMovements.SGF_R_CATEGORY on ac.CAT_ID equals c.CAT_ID
                                 join sc in accMovements.SGF_R_SUB_CATEGORY on ac.SUB_CAT_ID equals sc.SUB_CAT_ID
                                 where ac.ACCOUNT_ID == acc.Account_Id
                                 orderby ac.CREATED_DATE descending
                                 select new
                                 {
                                     ac.MOV_ID,
                                     ac.ACCOUNT_ID,
                                     MOV_TYPE_DES = mt.DESCRIPTION,
                                     CAT_DES = c.DESCRIPTION,
                                     SUB_CAT_DES = sc.DESCRIPTION,
                                     ac.AMOUNT,
                                     ac.DESCRIPTION,
                                     ac.CREATED_DATE
                                 };

                    List<AccountMovementModel> lstResult = new List<AccountMovementModel>();
                    foreach (var obj in result.ToList())
                    {
                        AccountMovementModel amm = new AccountMovementModel();
                        amm.MovId = obj.MOV_ID;
                        amm.AccountId = obj.ACCOUNT_ID;
                        amm.MovTypeDes = obj.MOV_TYPE_DES;
                        amm.CatDes = obj.CAT_DES;
                        amm.SubCatDes = obj.SUB_CAT_DES;
                        amm.Amount = (double)obj.AMOUNT;
                        amm.Description = obj.DESCRIPTION;
                        amm.CreatedDate = obj.CREATED_DATE;
                        lstResult.Add(amm);
                    }
                    Session["accountOnlineAllMovements"] = lstResult;
                    return View();
                }
            }
        }//END RegisterMovement

        //View for Reports
        public ActionResult Reports()
        {
            if (Session["accountOnline"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                AccountDataModel acc = (AccountDataModel)Session["accountOnline"];
                ReportsModel rpt = new ReportsModel();
                List<SGF_PROC_REPORTS_BY_CATEGORY_Result> data = new List<SGF_PROC_REPORTS_BY_CATEGORY_Result>();
                rpt.ReportType = 1;

                using (SGFEntitiess accRpt = new SGFEntitiess())
                {
                    data = accRpt.SGF_PROC_GET_REPORT_BY_CATEGORIES(acc.Account_Id,null,null,null).ToList();
                }

                rpt.ReportData = data;
                Session["accountReport"] = rpt;
                return View();
            }
        }//END Reports

        public void GetMovementTypes()
        {
            MovementTypesModel mtm = new MovementTypesModel();
            using (SGFEntitiess mov = new SGFEntitiess())
            {
                mtm.MovTypes = mov.SGF_PROC_GET_MOVEMENT_TYPES().ToList();
            }

            Session["movementTypes"] = mtm;
        }

        //Method to be used to switch later for method of list of several movements
        [HttpPost]
        public ActionResult SaveMovement()
        { 
            //For several movements at same time should use json
            //string[] movs = operations.Split('|');
            
            string operations = null;
            operations = HttpContext.Request.Form[0].ToString();

            if (operations == null)
                return View();

            string[] vars = operations.ToString().Split(';');
            //    var sendstr = "-1;true;" + ";" + mtype + ";" + cattype + ";" + subcattype + ";" + movamount + ";" + movdescription + ";" + dipckerdate;
            using (SGFEntitiess accM = new SGFEntitiess())
            {
                AccountDataModel acc = (AccountDataModel)Session["accountOnline"];
                AccountModel user = (AccountModel)Session["userOnline"];

                Int32? subcat = null;
                try{
                    subcat = Int32.Parse(vars[4]);
                }
                catch(Exception e){}

                //Add new Movement
                SGF_T_ACCOUNT_MOVEMENT accMov = SGF_T_ACCOUNT_MOVEMENT.CreateSGF_T_ACCOUNT_MOVEMENT(Int32.Parse(vars[0]), acc.Account_Id, Int32.Parse(vars[2]), Int32.Parse(vars[3]), subcat, (decimal)Double.Parse(vars[5]), vars[6].ToString(), user.Name.ToString(), DateTime.Parse(vars[7]));
                       
                accM.AddToSGF_T_ACCOUNT_MOVEMENT(accMov);
                
                //commit changes
                accM.SaveChanges();
            }

            return View();
        }

    }//END Class
}//END Namespace
