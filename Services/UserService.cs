
using Core.Exceptions;
using FAST_API_V2.Extenstions;
using FAST_API_V2.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Text;

namespace FAST_API_V2.Services
{
    public class UserService : IUserService
    {
        private static readonly string DateTimeFormat = "yyyyMMddHHss";
        private IJwtUtils _jwtUtils;

        public UserService(IJwtUtils jwtUtils)
        {
            _jwtUtils = jwtUtils;
        }

        public async Task<Dictionary<string, object>> ChangePassword(ChangePasswordVM vm, string connect)
        {
            var username = new ParametersVM()
            {
                Field = "username",
                Value = vm.username//Encoding.UTF8.GetString(Convert.FromBase64String(vm.username))
            };
            var newpassword = new ParametersVM()
            {
                Field = "newpassword",
                Value = vm.newPassword//Encoding.UTF8.GetString(Convert.FromBase64String(vm.newPassword))
            };
            var oldpassword = new ParametersVM()
            {
                Field = "oldpassword",
                Value =vm.oldPassword //Encoding.UTF8.GetString(Convert.FromBase64String(vm.oldPassword))
            };
            var isCustomer = new ParametersVM()
            {
                Field = "isCustomer",
                Value = vm.isCustomer
            };
            Dictionary<string, object> data = new Dictionary<string, object>();
            /*string oldPassword = Encoding.UTF8.GetString(Convert.FromBase64String(oldpassword.ToString()));
            string newPassword = Encoding.UTF8.GetString(Convert.FromBase64String(newpassword.ToString()));
            string Username = Encoding.UTF8.GetString(Convert.FromBase64String(username.ToString()));*/
            try
            {
                if (vm.isCustomer)
                {

                    var partner = await SqlService.ReadDataSet(SqlQuery.loginPartners(), connect, username, oldpassword);
                    if (partner[0].IsNullOrEmpty())
                    {

                        data.Add("status", "fail");
                        data.Add("message", "Wrong Password");
                        return data;
                    }
                    else
                    {
                        var update = await SqlService.ReadDataSet("UPDATE PARTNERS\r\nSET Password=@newpassword\r\nWHERE Username=@username;", connect, newpassword, username);

                        data.Add("status", "success");

                        return data;
                    };
                }
                else
                {
                    var currentUser = await SqlService.ReadDataSet("SELECT * FROM CONTACTSLIST c WHERE c.userName = @username and c.password = @oldpassword", connect, username, oldpassword);
                    if (!currentUser[0].IsNullOrEmpty())
                    {
                        var update = await SqlService.ReadDataSet("UPDATE CONTACTSLIST\r\nSET  Password=@newpassword\r\nWHERE Username=@username;", connect, newpassword, username);
                        data.Add("status", "success");
                        return data;

                    }
                    else
                    {
                        data.Add("status", "Fail");
                        data.Add("message", "Wrong Password");
                        return data;

                    }
                }
            }
            catch (Exception ex)
            {
                data.Add("Resutl", "Error");
                data.Add("message", ex.Message);
                Console.WriteLine("change-password.html");
                Console.WriteLine("InputData" + ex.Message.ToString());
                return data;
            }

        }

        public async Task<Dictionary<string, object>> CountRequest(CountRequestVM vm, string connect)
        {
            var contactId = new ParametersVM()
            {
                Field = "contactId",
                Value = vm.contactId
            };
            var isManager = new ParametersVM()
            {
                Field = "isManager",
                Value = vm.isManager ?? false
            };
            var isLog = new ParametersVM()
            {
                Field = "isLog",
                Value = vm.isLog ?? false
            };
            var isTruck = new ParametersVM()
            {
                Field = "isTruck",
                Value = vm.isTruck ?? false
            };
            var isMobile = new ParametersVM()
            {
                Field = "isMobile",
                Value = vm.isMobile
            };
            try
            {
               
                var loginedUser = await SqlService.ReadDataSet("SELECT * FROM ContactsList WHERE ContactID=@contactId", connect, contactId);
                int countTotal = 0;
                var loginedCompany = await SqlService.ReadDataSet(SqlQuery.getYourCompanyByContactId(), connect, contactId);
                bool accountDepartmentAfter = false;
                if (!loginedCompany[0].IsNullOrEmpty() && loginedCompany[0][0]["ACApproveAfterMNg"] != null)
                {
                    accountDepartmentAfter = bool.Parse(loginedCompany[0][0]["ACApproveAfterMNg"].ToString());
                }
                if (loginedCompany[0][0]["CmpID"] == "DDLOG")
                {
                    SqlQuery.createTaskManual(connect);

                }
                var rps = new CountRequestOutputVM();
                
                if (isManager.Value is bool boolValue && boolValue == true)
                {
                    var countAdvance = await SqlService.ExeScalar(SqlQuery.countAllAdvancePaymentRequestByContactId(vm.contactId), connect);
                   
                    var countSetle = await SqlService.ExeScalar(SqlQuery.countAllAcsSetlementPaymentByContactId(vm.contactId,accountDepartmentAfter), connect);
                    var countUnlockRequest = await SqlService.ExeScalar(SqlQuery.contRequestUnlockFile(vm.contactId), connect);
                    //
                    rps.countSetle = countSetle;
                    rps.countAdvance = countAdvance;
                    rps.countUnlockRequest = countUnlockRequest;
                    countTotal=countAdvance+countSetle+countUnlockRequest;
                }
                else
                {
                    rps.countSetle = 0;
                    rps.countAdvance = 0;
                    rps.countUnlockRequest = 0;
                }
                //
                if(isLog.Value is bool boolvalue && boolvalue == true)
                {
                    int countTask = await SqlService.ExeScalar(SqlQuery.countUnfinisedTaskInPeriod(vm.contactId, (bool)isLog.Value),connect);
                    int countProcessingTask= await SqlService.ExeScalar(SqlQuery.countProcessingTaskInPeriod((string)contactId.Value, (bool)isLog.Value),connect);
                    rps.countTask = countTask;
                    rps.countProcessingTask = countProcessingTask;
                    countTotal += countTask + countProcessingTask;
                }
                else
                {
                    rps.countTask = 0;
                    rps.countProcessingTask = 0;

                }
                //
                if ((bool)isTruck.Value)
                {
                    string linkedTruckNo = loginedUser[0][0]["LinkedTruckNo"] != null
                       ? loginedUser[0][0]["LinkedTruckNo"].ToString()
                       : "";
                    int countTruck = await SqlService.ExeScalar(SqlQuery.countTrucking(linkedTruckNo), connect);
                    rps.countTruck = countTruck;
                    countTotal += countTruck;
                }
                else
                {
                    rps.countTruck = 0;
                }
                Console.WriteLine("Returning response: " + rps.ToDictionary());

                return rps.ToDictionary();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }

        }

        public async Task<Dictionary<string, object>> Login(LoginInputVM vm, string connect)
        {
            var username = new ParametersVM()
            {
                Field = "username",
                Value = vm.username
            };
            var pass = new ParametersVM()
            {
                Field = "password",
                Value = vm.password
            };
            var contact = await SqlService.ReadDataSet(SqlQuery.getContactsListByUserNameAndPassWord(), connect, username, pass);
            var customer = await SqlService.ReadDataSet(SqlQuery.loginPartners(), connect,username,pass);
            if (!contact[0].IsNullOrEmpty())
            {
              
                var reposne = await SqlService.ReadDataSet(SqlQuery.reponseLogin(), connect, username, pass);
                if (!reposne[0].IsNullOrEmpty())
                {
                    int accessRight = Convert.ToInt32(contact[0][0]["AccessRight"].ToString());
                    //reposne[0][0]["token"] = Utils.Encrypt(username.ToString() + DateTime.Now.ToString(DateTimeFormat));
                    if (username.Value.ToString().Contains("SGN - IANLINH") || username.Value.ToString().Contains("SGN-SALEADMIN01")
                       || username.Value.ToString().Contains("SGN-SALE02") || username.Value.ToString().Contains("SGN-CBD01")
                       || username.Value.ToString().Contains("SGN-CUR01") || username.Value.ToString().Contains("SGN-CUR03")
                       || username.Value.ToString().Contains("SGN-FFD03") || username.Value.ToString().Contains("SGN-FFD02")
                       || username.Value.ToString().Contains("SGN-FFD01") || username.Value.ToString().Contains("SGN-SALEADMIN")
                       || username.Value.ToString().Contains("SGN-WH") || username.Value.ToString().Contains("SGN-FFD04")
                       || username.Value.ToString().Contains("GN1") || username.Value.ToString().Contains("GN4")
                       )
                    {
                        accessRight = 6;
                    }
                    contact[0][0]["accessRight"] = accessRight;
                    if (accessRight >= 5)
                    {
                        reposne[0][0]["isAdmin"] = true;
                        reposne[0][0]["isManager"] = true;
                    }
                    else
                    {
                        reposne[0][0]["isAdmin"] = false;
                    }
                    if (contact[0][0]["DeptID"].ToString().Contains("CB"))
                    {
                        reposne[0][0]["isManager"] = true;

                    }
                    if (accessRight == 7)
                    {
                        reposne[0][0]["saleProfitReport"] = true;
                        reposne[0][0]["sheetDebtReport"] = true;
                    }
                    
                    var rps = reposne[0][0].MapTo<LoginOutputVM>();
                    rps.token = _jwtUtils.GenerateJwtToken(rps);
                    var listdept = await SqlService.ReadDataSet(SqlQuery.getAllDepartmentsList(), connect);
                    rps.listDepts = new List<ListDept>();
                    if (!listdept.IsNullOrEmpty()) {
                        foreach (Dictionary<string,object>[] dept in listdept)
                        {
                            foreach (Dictionary<string, object> depts in dept)
                            {
                                ListDept listDeptss = new ListDept
                                {
                                    deptId = depts["DeptID"].ToString(),
                                    deptName = depts["Department"].ToString(),
                                    mangerId = depts["ManagerContact"].ToString(),
                                };
                                
                                rps.listDepts.Add(listDeptss);
                            }
                            
                        }
                    }
                    rps.listTroubleLabel = new List<ListTroubleLabel>();
                    if (rps.cmpId == "NPV"|| rps.cmpId == "HML/")
                    {
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "Budget" });
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "Quality" });
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "C/S" });


                    }
                   
                    else
                    {
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "Trouble" });
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "Trouble2" });
                        rps.listTroubleLabel.Add(new ListTroubleLabel { label = "Trouble3" });

                    }



                    return rps.ToDictionary(); // Trả về kết quả nếu có dữ liệu

                }
                else
                {
                    throw new ApiException("Login fail");
                }

            }
            else if (!customer[0].IsNullOrEmpty())
            {
                var response = await SqlService.ReadDataSet(SqlQuery.reponseLogin2(), connect, username, pass);
                if (response != null)
                {
                    
                    var rps = response[0][0].MapTo<LoginOutputVM>();
                    rps.token = _jwtUtils.GenerateJwtToken(rps);

                    return rps.ToDictionary(); // Trả về kết quả nếu có dữ liệu

                }
                else
                {
                    throw new ApiException("Login fail");
                }
            }
            else
            {
                var response = new LoginOutputVM
                {
                    isCustomer = false,
                    isAgent = false,
                    isColoader = false,
                    partnerId = "",
                    userName = "",
                    content = "The user name or password is incorrect.",
                    result = "Fail",
                    deptId = "",
                    deptName = "",
                    accessRight = 0,
                    yourCompany = "",
                    contactName = "",
                    version = "0",
                    role = "",
                    contactId = "",
                    vehicleNo = "",
                    
                    resetTime = 720000,
                   
                    idKeyShipment = "",
                    countAdvance = 0,
                    countSetle = 0,
                    isDeleteHandlingTask = false,
                    listTroubleLabel = null,
                    adminList = "",
                    pushNotifyUrl = "",
                    isAssignRight = false,
                    apiKeyGps = "",
                   
                    assignOpsStaff = false,
                    sendEmail = false,
                    saleProfitReport = false,
                    sheetDebtReport = false,
                    importDoc = false,
                    exportDoc = false,
                    cmpId = ""

                };


                return response.ToDictionary();
            }

        }
    }
}
