
using Core.Exceptions;
using FAST_API_V2.Extenstions;
using FAST_API_V2.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;

namespace FAST_API_V2.Services
{
    public class UserService : IUserService
    {
        private static readonly string DateTimeFormat = "yyyyMMddHHss";
     

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
            var contact = await SqlService.ReadDataSet("SELECT * FROM CONTACTSLIST c WHERE c.userName = @username and c.password = @password", connect, username, pass);
            var customer=await SqlService.ReadDataSet("SELECT * FROM PARTNERS p WHERE p.Username = @username AND p.Password = @password AND p.CONFIRMREGISTER = 1", connect, username, pass);
            if(!contact[0].IsNullOrEmpty())
            {
                var  Reposne = await SqlService.ReadDataSet(
                "\r\nSELECT \r\ncontactId,\r\nc.userName,\r\nc.DeptID,\r\ncase \r\n\twhen d.CmpID like '%HML/%' then 'HAMINH'\r\n\twhen d.CmpID like '%TNN/%' then 'TNN'\r\n\telse d.CmpID\r\n\tEND AS cmpId,\r\nd.cmpId topic,\r\nd.Department deptName,\r\nc.ContactName contactName,\r\n'' version,\r\n'success' result,\r\n'Welcome '+ c.ContactName content,\r\n'' role,\r\nc.LinkedTruckNo 'vehicleNo',\r\na.accessKpiReport,\r\na.importDoc,\r\na.exportDoc,\r\ncase \r\n\twhen d.Department='CBD' or c.accessRight=1 then  1\r\n\telse 0\r\n\tEND AS isShowHandling,\r\ncase \r\n\twhen d.Department='FFD' or a.assignOpsStaff=1 then  1\r\n\telse 0\r\n\tEND AS isShowLogistics,\r\ncase\r\n\twhen (SELECT COUNT(*) FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL)AND a.AppActive = 1 AND (ISNULL(a.SettleManagerApp,0) = 1 OR ISNULL(a.SettleAccsMngApp,0) = 1 OR ISNULL(a.SettlePBOBApp,0) = 1 OR ISNULL(a.SettleAccsFNApp,0) = 1))>0\r\n\t\tor (SELECT COUNT(*) FROM ACSSETLEMENTPAYMENT asp WHERE asp.StltDate IS NOT NULL AND asp.SltSCID = contactId )>0\r\n\t\tthen 1\r\n\t\telse 0\r\n\t\tend as isAuthorizedSetlement,\r\ncase \r\n\twhen (SELECT COUNT (adv.advId) FROM ADVANCEPAYMENTREQUEST adv WHERE adv.AdvSCID = contactId) > 0\r\n\t\tor (SELECT COUNT(*) FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL)AND a.AppActive = 1 AND (ISNULL(a.AdvManagerApp,0) = 1 OR ISNULL(a.AdvAccsMngApp,0) = 1 OR ISNULL(a.AdvPBOBApp,0) = 1 OR ISNULL(a.AdvAccsFNApp,0) = 1))>0\r\n\t\t\tthen 1\r\n\t\t\telse 0\r\n\t\t\tend as isAuthorizedAdvance,\r\n\r\ncase\r\n\twhen c.contactID=d.managerContact\r\n\tthen (SELECT COUNT(*) AS SIZE FROM ADVANCEPAYMENTREQUEST adv WHERE \r\n\t\t\t\t(adv.advBodId = contactId\r\n\t\t\t\tAND ISNULL(adv.ADVBODSTICKAPP,0)=0 \r\n\t\t\t\tAND ISNULL(adv.ADVBODSTICKDENY,0)=0 \r\n\t\t\t\tAND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=1\r\n\t\t\t\tAND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1)\r\n\t\t\t\t\r\n\t\t\t\tOR (adv.ADVACSDPMANAGERID = contactId \r\n\t\t\t\tAND ISNULL(adv.ADVACSDPMANAGERSTICKDENY,0)=0 \r\n\t\t\t\tAND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=0 \r\n\t\t\t\tAND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1)\r\n\t\t\r\n\t\t\t\tOR (adv.ADVDPMANAGERID = contactId \r\n\t\t\t\tAND ISNULL(adv.ADVDPMANAGERSTICKDENY,0)=0 \r\n\t\t\t\tAND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=0) \r\n\t\t\t\tAND ((ISNULL(adv.AdvSCID,'') <> '' AND ISNULL(adv.AdvCSStickApp,0) = 1) OR ISNULL(adv.AdvSCID,'') = ''))\r\n\telse 0\r\n\tend as countAdvance,\r\n\tCASE \r\n    WHEN c.contactID = d.managerContact \r\n    THEN\r\n        (CASE \r\n            -- Kiểm tra điều kiện y.CmpID và y.ACApproveAfterMNg có giá trị không\r\n            WHEN y.CmpID IS NOT NULL AND y.ACApproveAfterMNg IS NOT NULL \r\n            THEN\r\n                (SELECT COUNT(*) AS SIZE \r\n                 FROM ACSSETLEMENTPAYMENT asp \r\n                 WHERE asp.StltDate IS NOT NULL \r\n                 AND \r\n                 (\r\n                     (asp.sltBodId = contactId \r\n                     AND ISNULL(asp.SLTBODSTICKAPP, 0) = 0 \r\n                     AND ISNULL(asp.SLTBODSTICKDENY, 0) = 0 \r\n                     AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 \r\n                     AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 1)\r\n                     \r\n                     OR (ISNULL(asp.SltSCID, '') = contactId \r\n                         AND ISNULL(asp.SltCSStickDeny, 0) = 0 \r\n                         AND ISNULL(asp.SltCSStickApp, 0) = 0 \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1)\r\n                     \r\n                     OR (asp.SLTDPMANAGERID = contactId \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKDENY, 0) = 0 \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 0)\r\n                     \r\n                     OR (asp.SLTACSDPMANAGERID = contactId \r\n                         AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY, 0) = 0 \r\n                         AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 0 \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 \r\n                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))\r\n                 ))\r\n            ELSE\r\n                (SELECT COUNT(*) AS SIZE \r\n                 FROM ACSSETLEMENTPAYMENT asp \r\n                 WHERE asp.StltDate IS NOT NULL \r\n                 AND \r\n                 (\r\n                     (asp.sltBodId = contactId \r\n                     AND ISNULL(asp.SLTBODSTICKAPP, 0) = 0 \r\n                     AND ISNULL(asp.SLTBODSTICKDENY, 0) = 0 \r\n                     AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 \r\n                     AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 1)\r\n                     \r\n                     OR (asp.SLTDPMANAGERID = contactId \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKDENY, 0) = 0 \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 0 \r\n                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))\r\n                     \r\n                     OR (ISNULL(asp.SltSCID, '') = contactId \r\n                         AND ISNULL(asp.SltCSStickDeny, 0) = 0 \r\n                         AND ISNULL(asp.SltCSStickApp, 0) = 0)\r\n                     \r\n                     OR (asp.SLTACSDPMANAGERID = contactId \r\n                         AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY, 0) = 0 \r\n                         AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 0 \r\n                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 \r\n                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))\r\n                 ))\r\n        END)\r\n    ELSE 0\r\nEND AS countSetle,\r\ncase\r\n\twhen c.contactID=d.managerContact\r\n\tthen (SELECT COUNT (TasksList.ID) \r\n\t\t FROM            TasksList \r\n\t\t WHERE        (TasksList.Username = contactId) AND (ISNULL(TasksList.CheckRead, 0) = 0) AND (ISNULL(TasksList.Status, '') = 'JobFileUnlockRequest' OR ISNULL(TasksList.Status, '') = 'InvoiceUnlockRequest' OR ISNULL(TasksList.Status, '') = 'BILLUnlockRequest'))\r\n\telse 0\r\n\tend as countUnlockRequest,\r\nCASE\r\n\tWHEN (SELECT TOP 1 a.[ContactID] FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL) AND a.AppActive = 1) IS NOT NULL AND c.contactID=d.managerContact\r\n\tTHEN 1\r\n\tELSE 0\r\n\tEND AS isManager,\r\nCASE\r\n\tWHEN d.Department='%LOG%' and d.MngCode=2\r\n\tTHEN 1\r\n\tELSE 0\r\n\tEND AS isLog,\r\nCASE\r\n\tWHEN d.Department='%LOG%' and d.MngCode=2\r\n\tTHEN (SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK  \r\n\t\t\t\t\t FROM TASK AS task    LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  \r\n\t\t\t\t\t LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  \r\n\t\t\t\t\t LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  \r\n\t\t\t\t\t LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  \r\n\t\t\t\t\t LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK  \r\n\t\t\t\t\t WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 0 AND task.STAFFID = staffId AND task.ASSIGNMOBILE = 1 )\r\n\tELSE 0\r\n\tEND AS countTask,\r\nCASE\r\n\tWHEN d.Department='%LOG%' and d.MngCode=2\r\n\tTHEN (SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK  \r\n\t\t\t\t\t FROM TASK AS task    LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  \r\n\t\t\t\t\t LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  \r\n\t\t\t\t\t LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  \r\n\t\t\t\t\t LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  \r\n\t\t\t\t\t LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK  \r\n\t\t\t\t\t WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 1 AND task.STAFFID =  staffId AND task.ASSIGNMOBILE = 1)\r\n\tELSE 0\r\n\tEND AS countProcessingTask,\r\nCASE\r\n\tWHEN d.Department='%TRUCK%' and d.MngCode=3\r\n\tTHEN 1\r\n\tELSE 0\r\n\tEND AS isTruck,\r\nCASE\r\n\tWHEN d.Department='%TRUCK%' and d.MngCode=3\r\n\tTHEN (SELECT COUNT(*) AS countTrucking FROM TransactionDetails LEFT JOIN Transactions ON TransactionDetails.TransID = Transactions.TransID  \r\n\t\t\t\t WHERE TransactionDetails.ServiceMode = linkedTruckNo AND TransactionDetails.TKStatus IS NULL AND ISNULL(TransactionDetails.TKFinish,0) = 0 AND ISNULL(TransactionDetails.RemocStatus,0) = 0 )\r\n\tELSE 0\r\n\tEND AS countTruck,\r\nCASE\r\n\tWHEN d.Department='%TRUCK%' and d.MngCode=3\r\n\tTHEN ''\r\n\tELSE ''\r\n\tEND AS idKeyShipment,\r\n\r\n(SELECT COUNT (BookingLocal.BkgID) FROM BookingLocal WHERE BookingLocal.ContactID=contactId AND ISNULL(BookingLocal.CFConfirm,0) = 0) countBooking,\r\n\r\n(SELECT COUNT (adv.advId) FROM ADVANCEPAYMENTREQUEST adv WHERE (adv.AdvContactId = contactId OR adv.OldAdvContactId = contactId) AND adv.AdvDate >= '' AND adv.AdvDate <= '' AND ISNULL(adv.ADVBODSTICKAPP,0)=0) countMyAdvance,\r\n\r\n(SELECT COUNT(*) FROM ACSSETLEMENTPAYMENT asp WHERE asp.StltDate IS NOT NULL AND (asp.sltContactId = contactId OR asp.OldSltContactID = contactId)AND ISNULL(asp.SLTBODSTICKAPP,0)=0) countMySetle,\r\n\r\n c.DeptID deptId,\r\n d.Department deptName,\r\n d.ManagerContact mangerId,\r\n CASE \r\n\tWHEN y.CmpID like '%NPV%'THEN 'Budget'\r\n\tWHEN y.CmpID like '%HML/%' THEN 'Budget'\r\n\tELSE 'Trouble'\r\n\tEND AS label,\r\n\r\n CASE \r\n\tWHEN y.CmpID like '%NPV%' THEN 'Quality'\r\n\tWHEN y.CmpID like '%HML/%' THEN 'Quality'\r\n\tELSE 'Trouble2'\r\n\tEND AS label,\r\n CASE \r\n\tWHEN y.CmpID like '%NPV%'THEN 'C/S'\r\n\tWHEN y.CmpID like '%HML/%' THEN 'C/S'\r\n\tELSE 'Trouble3'\r\n\tEND AS label,\r\nCASE\r\n/*if1*/\r\n\tWHEN y.CmpID like '%NPV%'\r\n\tTHEN (CASE \r\n\t\t\t\tWHEN a.DELETEHANDLINGTASK is not null and a.DELETEHANDLINGTASK=1\r\n\t\t\t\tTHEN 1\r\n\t\t\t\tEND  )\r\n\tWHEN y.CmpID like '%HML/%'\r\n\tTHEN (CASE \r\n\t\t\t\tWHEN a.DELETEHANDLINGTASK is not null and a.DELETEHANDLINGTASK=1\r\n\t\t\t\tTHEN 1\r\n\t\t\t\tEND  )\r\n\tELSE 0\r\n\tEND AS getDeleteHandlingTask,\r\nCASE\r\n/*if1*/\r\n\tWHEN y.CmpID like '%NPV%'\r\n\tTHEN 'sgn-ceo;sgn-general;sgn-ianlinh;sgn-wh'\r\n\tWHEN y.CmpID like '%HML/%'\r\n\tTHEN 'sgn-ceo;sgn-general;sgn-ianlinh'\r\n\tELSE 'CT001'\r\n\tEND AS adminList,\r\n\r\nCASE\r\n\tWHEN y.CmpID like '%NPV%'\r\n\tTHEN 'http://103.77.167.101:8080/goodway-report/notification/push-notification'\r\n\tWHEN y.CmpID like '%HML/%'\r\n\tTHEN ''\r\n\tELSE 'http://www.myworkspace.vn/goodway-report/notification/push-notification'\r\n\tEND AS pushNotifyUrl,\r\n\r\nCASE\r\n/*if1*/\r\n\tWHEN y.CmpID like '%NPV%'\r\n\tTHEN 0\r\n\tWHEN y.CmpID like '%HML/%'\r\n\tTHEN 0\r\n\tELSE 1\r\n\tEND AS isAssignRight,\r\n720000 resetTime,\r\nCASE \r\n\tWHEN Y.ApiKeyGps IS NOT NULL THEN Y.ApiKeyGps\r\n\tELSE ''\r\n\tEND AS apiKeyGps,\r\nCASE \r\n\tWHEN C.LinkedTruckNo IS NOT NULL \r\n\tTHEN (SELECT TOP 1 vhVIN FROM VehicleList AS vh WHERE vh.vhUnitNo = vhUnitNo)\r\n\tELSE ''\r\n\tEND AS idGps,\r\n0 isCustomer,\r\n'' partnerName,\r\nA.SENDEMAIL sendEmail,\r\nA.ASSIGNOPSSTAFF assignOpsStaff,\r\n\r\nCASE\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' \r\n\tTHEN 0\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' \r\n\tTHEN 1\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' \r\n\tTHEN 0\r\n\tELSE 0\r\n\tEND AS isAgent,\r\nCASE\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' \r\n\tTHEN 0\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' \r\n\tTHEN 0\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' \r\n\tTHEN 1\r\n\tELSE 0\r\n\tEND AS isColoader,\r\nCASE\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' \r\n\tTHEN C.PartnerID\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' \r\n\tTHEN C.PartnerID\r\n\tWHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' \r\n\tTHEN C.PartnerID\r\n\tELSE ''\r\n\tEND AS partnerId,\r\n\r\na.ProfitRead saleProfitReport,\r\na.DebtRecordRead sheetDebtReport\r\n\r\n\t\t\t\t\t\r\nFROM CONTACTSLIST c \r\njoin Departments d \r\non c.deptId=d.DeptID \r\njoin AccessRight a\r\non c.userName= a.UserName\r\njoin YourCompany y\r\non y.CmpID= d.cmpId\r\n" +
                "WHERE c.userName = @username and c.password = @password", connect, username, pass);
                if (!Reposne[0].IsNullOrEmpty())
                {
                    
                    int accessRight = Convert.ToInt32(contact[0][0]["AccessRight"].ToString());
                    Reposne[0][0]["token"] = Utils.Encrypt(username.ToString() + DateTime.Now.ToString(DateTimeFormat));
                    if(username.Value.ToString().Contains( "SGN - IANLINH" )|| username.Value.ToString().Contains( "SGN-SALEADMIN01")
                       || username.Value.ToString().Contains("SGN-SALE02") || username.Value.ToString().Contains ("SGN-CBD01")
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
                        Reposne[0][0]["isAdmin"] = true;
                        Reposne[0][0]["isManager"] = true;
                    }
                    else
                    {
                        Reposne[0][0]["isAdmin"] = false;
                    }
                    if (contact[0][0]["DeptID"].ToString().Contains("CB"))
                    {
                        Reposne[0][0]["isManager"] = true;

                    }
                    if (accessRight == 7)
                    {
                        Reposne[0][0]["saleProfitReport"] = true;
                        Reposne[0][0]["sheetDebtReport"] = true;
                    }
                

                    return Reposne[0][0]; // Trả về kết quả nếu có dữ liệu

                }
                else
                {
                    throw new ApiException("Login fail");
                }
                
            }else if (!customer[0].IsNullOrEmpty())
            {
                var response = await SqlService.ReadDataSet("SELECT \r\nCASE\r\n\tWHEN p.[Group] like 'CUSTOMERS' THEN 1\r\n\tELSE 0\r\n\tEND AS isCustomer,\r\nCASE\r\n\tWHEN p.[Group] like 'AGENTS' THEN 1\r\n\tELSE 0\r\n\tEND AS isAgent,\r\nCASE\r\n\tWHEN p.[Group] like 'COLOADERS' THEN 1\r\n\tELSE 0\r\n\tEND AS isColoader,\r\np.PartnerID partnerId,\r\np.PartnerName partnerName,\r\np.Username userName,\r\n'success' result,\r\n'Welcome' content,\r\n'' deptId,\r\n'' deptName,\r\n0 accessRight,\r\n'' yourCompany,\r\n'' contactName,\r\n0 version,\r\n720000 resetTime,\r\n'' role ,\r\n'' contactID,\r\n'' vehicleNo,\r\n'' isManager,\r\n'' isAdmin,\r\n'' isLog,\r\n'' isTruck,\r\n'' idKeyShipment,\r\n0 countAdvance,\r\n0 countSetle,\r\n0 isDeleteHandlingTask,\r\n'' listTroubleLabel,\r\n'' adminList,\r\n'' pushNotifyUrl,\r\n0 isAssignRight,\r\n'' apiKeyGps,\r\n'' idGps,\r\n0 assignOpsStaff,\r\n0 sendEmail,\r\n0 saleProfitReport,\r\n0 sheetDebtReport,\r\n0 importDoc,\r\n0 exportDoc,\r\nCASE\r\n\tWHEN (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany \r\n\t\t\t\tLEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID \r\n\t\t\t\tLEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId\r\n\t\t\t\tWHERE contactsList.ContactId = contactId)\r\n\t\tlike '%HML/%'\r\n\t\tTHEN 'HAIMINH'\r\n\tWHEN (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany \r\n\t\t\t\tLEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID \r\n\t\t\t\tLEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId\r\n\t\t\t\tWHERE contactsList.ContactId = contactId)\r\n\t\tlike '%TNN/%'\r\n\t\tTHEN 'TNN'\r\n\tELSE (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany \r\n\t\t\t\tLEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID \r\n\t\t\t\tLEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId\r\n\t\t\t\tWHERE contactsList.ContactId = contactId)\r\n\tEND AS cmpId\r\nFROM PARTNERS p \r\n" +
                    "WHERE p.Username = @username AND p.Password = @password AND p.CONFIRMREGISTER = 1", connect, username, pass);
                if (response != null)
                {
                    return response[0][0];

                }
                else
                {
                    throw new ApiException("Login fail");
                }
            }
            else 
            {
                var response =new  LoginOutputVM{ 
                    isCustomer= false,
                    isAgent=false,
                    isColoader=false,
                    partnerId="",
                    userName="",
                    content= "The user name or password is incorrect.",
                    result="Fail",
                    deptId="",
                    deptName="",
                    accessRight="0",
                    yourCompany="",
                    contactName="",
                    version="0",
                    role= "",
                    contactId= "",
                    vehicleNo= "",
                    isAdmin= "",
                    isManager= "",
                    resetTime="720000",
                    isLog= "",
                    isTruck= "",
                    idKeyShipment= "",
                    countAdvance="0",
                    countSetle="0",
                    isDeleteHandlingTask=false,
                    listTroubleLabel=null,
                    adminList="",
                    pushNotifyUrl="",
                    isAssignRight=false,
                    apiKeyGps="",
                    idGps="",
                    assignOpsStaff=false,
                    sendEmail=false,
                    saleProfitReport=false,
                    sheetDebtReport=false,
                    importDoc=false,
                    exportDoc=false,
                    cmpId=""

                };

                
                return response.ToDictionary();
            }
            
        }
    }
}
