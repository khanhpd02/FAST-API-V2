using System.Text;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace FAST_API_V2.Extenstions
{
    public class SqlQuery
    {
        public static string loginPartners()
        {
            string query = "SELECT * FROM PARTNERS p WHERE p.Username = @username AND p.Password = @password AND p.CONFIRMREGISTER = 1";
            return query;
        }
        public static string getContactsListByUserNameAndPassWord()
        {
            string query = "SELECT * FROM CONTACTSLIST c WHERE c.userName = @username and c.password = @password";
            return query;
        }
        public static string getAllDepartmentsList()
        {
            string query = "SELECT * FROM DEPARTMENTS";
            return query;
        }
        public static string reponseLogin()
        {
            string query = @$"SELECT 
contactId,
c.userName,
c.DeptID,
case 
	when d.CmpID like '%HML/%' then 'HAMINH'
	when d.CmpID like '%TNN/%' then 'TNN'
	else d.CmpID
	END AS cmpId,
d.cmpId topic,
d.Department deptName,
c.ContactName contactName,
'' version,
'success' result,
'Welcome '+ c.ContactName content,
'' role,
c.LinkedTruckNo 'vehicleNo',
a.accessKpiReport,
a.importDoc,
a.exportDoc,
CAST(case 
	when d.Department='CBD' or c.accessRight=1 then  1
	else 0
	END AS BIT) AS isShowHandling,
CAST(case 
	when d.Department='FFD' or a.assignOpsStaff=1 then  1
	else 0
	END AS BIT) AS isShowLogistics,
CAST(case
	when (SELECT COUNT(*) FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL)AND a.AppActive = 1 AND (ISNULL(a.SettleManagerApp,0) = 1 OR ISNULL(a.SettleAccsMngApp,0) = 1 OR ISNULL(a.SettlePBOBApp,0) = 1 OR ISNULL(a.SettleAccsFNApp,0) = 1))>0
		or (SELECT COUNT(*) FROM ACSSETLEMENTPAYMENT asp WHERE asp.StltDate IS NOT NULL AND asp.SltSCID = contactId )>0
		then 1
		else 0
		end as BIT) AS isAuthorizedSetlement,
CAST(case 
	when (SELECT COUNT (adv.advId) FROM ADVANCEPAYMENTREQUEST adv WHERE adv.AdvSCID = contactId) > 0
		or (SELECT COUNT(*) FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL)AND a.AppActive = 1 AND (ISNULL(a.AdvManagerApp,0) = 1 OR ISNULL(a.AdvAccsMngApp,0) = 1 OR ISNULL(a.AdvPBOBApp,0) = 1 OR ISNULL(a.AdvAccsFNApp,0) = 1))>0
			then 1
			else 0
			end as BIT) AS isAuthorizedAdvance,

case
	when c.contactID=d.managerContact
	then (SELECT COUNT(*) AS SIZE FROM ADVANCEPAYMENTREQUEST adv WHERE 
				(adv.advBodId = contactId
				AND ISNULL(adv.ADVBODSTICKAPP,0)=0 
				AND ISNULL(adv.ADVBODSTICKDENY,0)=0 
				AND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=1
				AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1)
				
				OR (adv.ADVACSDPMANAGERID = contactId 
				AND ISNULL(adv.ADVACSDPMANAGERSTICKDENY,0)=0 
				AND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=0 
				AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1)
		
				OR (adv.ADVDPMANAGERID = contactId 
				AND ISNULL(adv.ADVDPMANAGERSTICKDENY,0)=0 
				AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=0) 
				AND ((ISNULL(adv.AdvSCID,'') <> '' AND ISNULL(adv.AdvCSStickApp,0) = 1) OR ISNULL(adv.AdvSCID,'') = ''))
	else 0
	end as countAdvance,
	CASE 
    WHEN c.contactID = d.managerContact 
    THEN
        (CASE 
            -- Kiểm tra điều kiện y.CmpID và y.ACApproveAfterMNg có giá trị không
            WHEN y.CmpID IS NOT NULL AND y.ACApproveAfterMNg IS NOT NULL 
            THEN
                (SELECT COUNT(*) AS SIZE 
                 FROM ACSSETLEMENTPAYMENT asp 
                 WHERE asp.StltDate IS NOT NULL 
                 AND 
                 (
                     (asp.sltBodId = contactId 
                     AND ISNULL(asp.SLTBODSTICKAPP, 0) = 0 
                     AND ISNULL(asp.SLTBODSTICKDENY, 0) = 0 
                     AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 
                     AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 1)
                     
                     OR (ISNULL(asp.SltSCID, '') = contactId 
                         AND ISNULL(asp.SltCSStickDeny, 0) = 0 
                         AND ISNULL(asp.SltCSStickApp, 0) = 0 
                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1)
                     
                     OR (asp.SLTDPMANAGERID = contactId 
                         AND ISNULL(asp.SLTDPMANAGERSTICKDENY, 0) = 0 
                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 0)
                     
                     OR (asp.SLTACSDPMANAGERID = contactId 
                         AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY, 0) = 0 
                         AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 0 
                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 
                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))
                 ))
            ELSE
                (SELECT COUNT(*) AS SIZE 
                 FROM ACSSETLEMENTPAYMENT asp 
                 WHERE asp.StltDate IS NOT NULL 
                 AND 
                 (
                     (asp.sltBodId = contactId 
                     AND ISNULL(asp.SLTBODSTICKAPP, 0) = 0 
                     AND ISNULL(asp.SLTBODSTICKDENY, 0) = 0 
                     AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 
                     AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 1)
                     
                     OR (asp.SLTDPMANAGERID = contactId 
                         AND ISNULL(asp.SLTDPMANAGERSTICKDENY, 0) = 0 
                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 0 
                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))
                     
                     OR (ISNULL(asp.SltSCID, '') = contactId 
                         AND ISNULL(asp.SltCSStickDeny, 0) = 0 
                         AND ISNULL(asp.SltCSStickApp, 0) = 0)
                     
                     OR (asp.SLTACSDPMANAGERID = contactId 
                         AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY, 0) = 0 
                         AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP, 0) = 0 
                         AND ISNULL(asp.SLTDPMANAGERSTICKAPP, 0) = 1 
                         AND (ISNULL(asp.SltCSStickApp, 0) = 1 OR ISNULL(asp.SltSCID, '') = ''))
                 ))
        END)
    ELSE 0
END AS countSetle,
case
	when c.contactID=d.managerContact
	then (SELECT COUNT (TasksList.ID) 
		 FROM            TasksList 
		 WHERE        (TasksList.Username = contactId) AND (ISNULL(TasksList.CheckRead, 0) = 0) AND (ISNULL(TasksList.Status, '') = 'JobFileUnlockRequest' OR ISNULL(TasksList.Status, '') = 'InvoiceUnlockRequest' OR ISNULL(TasksList.Status, '') = 'BILLUnlockRequest'))
	else 0
	end as countUnlockRequest,
CAST(CASE
	WHEN (SELECT TOP 1 a.[ContactID] FROM AuthorizedApproval a where a.ContactID = contactId AND (a.DateTo >= GETDATE() OR a.DateTo IS NULL) AND (a.DateFrom <= GETDATE() OR a.DateFrom IS NULL) AND a.AppActive = 1) IS NOT NULL AND c.contactID=d.managerContact
	THEN 1
	ELSE 0
	END AS BIT) AS isManager,
CAST(CASE
	WHEN d.Department='%LOG%' and d.MngCode=2
	THEN 1
	ELSE 0
	END AS BIT) AS isLog,
CASE
	WHEN d.Department='%LOG%' and d.MngCode=2
	THEN (SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK  
					 FROM TASK AS task    LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  
					 LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  
					 LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  
					 LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  
					 LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK  
					 WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 0 AND task.STAFFID = staffId AND task.ASSIGNMOBILE = 1 )
	ELSE 0
	END AS countTask,
CASE
	WHEN d.Department='%LOG%' and d.MngCode=2
	THEN (SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK  
					 FROM TASK AS task    LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  
					 LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  
					 LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  
					 LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  
					 LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK  
					 WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 1 AND task.STAFFID =  staffId AND task.ASSIGNMOBILE = 1)
	ELSE 0
	END AS countProcessingTask,
CAST(CASE
	WHEN d.Department='%TRUCK%' and d.MngCode=3
	THEN 1
	ELSE 0
	END AS BIT)AS isTruck,
CASE
	WHEN d.Department='%TRUCK%' and d.MngCode=3
	THEN (SELECT COUNT(*) AS countTrucking FROM TransactionDetails LEFT JOIN Transactions ON TransactionDetails.TransID = Transactions.TransID  
				 WHERE TransactionDetails.ServiceMode = linkedTruckNo AND TransactionDetails.TKStatus IS NULL AND ISNULL(TransactionDetails.TKFinish,0) = 0 AND ISNULL(TransactionDetails.RemocStatus,0) = 0 )
	ELSE 0
	END AS countTruck,
CASE
	WHEN d.Department='%TRUCK%' and d.MngCode=3
	THEN ''
	ELSE ''
	END AS idKeyShipment,

(SELECT COUNT (BookingLocal.BkgID) FROM BookingLocal WHERE BookingLocal.ContactID=contactId AND ISNULL(BookingLocal.CFConfirm,0) = 0) countBooking,

(SELECT COUNT (adv.advId) FROM ADVANCEPAYMENTREQUEST adv WHERE (adv.AdvContactId = contactId OR adv.OldAdvContactId = contactId) AND adv.AdvDate >= '' AND adv.AdvDate <= '' AND ISNULL(adv.ADVBODSTICKAPP,0)=0) countMyAdvance,

(SELECT COUNT(*) FROM ACSSETLEMENTPAYMENT asp WHERE asp.StltDate IS NOT NULL AND (asp.sltContactId = contactId OR asp.OldSltContactID = contactId)AND ISNULL(asp.SLTBODSTICKAPP,0)=0) countMySetle,

 c.DeptID deptId,
 d.Department deptName,
 d.ManagerContact mangerId,
/* CASE 
	WHEN y.CmpID like '%NPV%'THEN 'Budget'
	WHEN y.CmpID like '%HML/%' THEN 'Budget'
	ELSE 'Trouble'
	END AS label,

 CASE 
	WHEN y.CmpID like '%NPV%' THEN 'Quality'
	WHEN y.CmpID like '%HML/%' THEN 'Quality'
	ELSE 'Trouble2'
	END AS label,
 CASE 
	WHEN y.CmpID like '%NPV%'THEN 'C/S'
	WHEN y.CmpID like '%HML/%' THEN 'C/S'
	ELSE 'Trouble3'
	END AS label,*/
CAST(CASE
/*if1*/
	WHEN y.CmpID like '%NPV%'
	THEN (CASE 
				WHEN a.DELETEHANDLINGTASK is not null and a.DELETEHANDLINGTASK=1
				THEN 1
				END  )
	WHEN y.CmpID like '%HML/%'
	THEN (CASE 
				WHEN a.DELETEHANDLINGTASK is not null and a.DELETEHANDLINGTASK=1
				THEN 1
				END  )
	ELSE 0
	END AS BIT) AS getDeleteHandlingTask,
CASE
/*if1*/
	WHEN y.CmpID like '%NPV%'
	THEN 'sgn-ceo;sgn-general;sgn-ianlinh;sgn-wh'
	WHEN y.CmpID like '%HML/%'
	THEN 'sgn-ceo;sgn-general;sgn-ianlinh'
	ELSE 'CT001'
	END AS adminList,

CASE
	WHEN y.CmpID like '%NPV%'
	THEN 'http://103.77.167.101:8080/goodway-report/notification/push-notification'
	WHEN y.CmpID like '%HML/%'
	THEN ''
	ELSE 'http://www.myworkspace.vn/goodway-report/notification/push-notification'
	END AS pushNotifyUrl,

CAST(CASE
/*if1*/
	WHEN y.CmpID like '%NPV%'
	THEN 0
	WHEN y.CmpID like '%HML/%'
	THEN 0
	ELSE 1
	END AS BIT) AS isAssignRight,
720000 resetTime,
CASE 
	WHEN Y.ApiKeyGps IS NOT NULL THEN Y.ApiKeyGps
	ELSE ''
	END AS apiKeyGps,
CASE 
	WHEN C.LinkedTruckNo IS NOT NULL 
	THEN (SELECT TOP 1 vhVIN FROM VehicleList AS vh WHERE vh.vhUnitNo = vhUnitNo)
	ELSE ''
	END AS idGps,

'' partnerName,
A.SENDEMAIL sendEmail,
A.ASSIGNOPSSTAFF assignOpsStaff,

CAST(CASE
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' 
	THEN 0
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' 
	THEN 1
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' 
	THEN 0
	ELSE 0
	END AS BIT) AS isAgent,
CAST(CASE
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' 
	THEN 0
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' 
	THEN 0
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' 
	THEN 1
	ELSE 0
	END AS BIT) AS isColoader,
CASE
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'CUSTOMERS' 
	THEN C.PartnerID
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 P.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'AGENTS' 
	THEN C.PartnerID
	WHEN C.PartnerID IS NOT NULL AND (SELECT TOP 1 p.[Group] FROM PARTNERS p WHERE p.partnerId = partnerId) LIKE 'COLOADERS' 
	THEN C.PartnerID
	ELSE ''
	END AS partnerId,

a.ProfitRead saleProfitReport,
a.DebtRecordRead sheetDebtReport

					
FROM CONTACTSLIST c 
join Departments d 
on c.deptId=d.DeptID 
join AccessRight a
on c.userName= a.UserName
join YourCompany y
on y.CmpID= d.cmpId
WHERE c.userName = @username and c.password = @password";
            return query;
        }
        public static string reponseLogin2()
        {
            string query = @$"SELECT 
CAST(CASE
	WHEN p.[Group] like 'CUSTOMERS' THEN 1
	ELSE 0
	END AS BIT) AS isCustomer,
CAST(CASE
	WHEN p.[Group] like 'AGENTS' THEN 1
	ELSE 0
	END AS BIT)AS isAgent,
CAST(CASE
	WHEN p.[Group] like 'COLOADERS' THEN 1
	ELSE 0
	END AS BIT)AS isColoader,
p.PartnerID partnerId,
p.PartnerName partnerName,
p.Username userName,
'success' result,
'Welcome' content,
'' deptId,
'' deptName,
0 accessRight,
'' yourCompany,
'' contactName,
'' version,
720000 resetTime,
'' role ,
'' contactID,
'' vehicleNo,
CAST (0 as BIT) as isManager,
CAST (0 as BIT) as isAdmin,
CAST (0 as BIT) as isLog,
CAST (0 as BIT) as isTruck,
'' idKeyShipment,
0 countAdvance,
0 countSetle,
CAST (0 as BIT) as isDeleteHandlingTask,
'' pushNotifyUrl,
CAST (0 as BIT) as isAssignRight,
'' apiKeyGps,
'' idGps,
CAST (0 as BIT) as assignOpsStaff,
CAST (0 as BIT) as sendEmail,
CAST (0 as BIT) as saleProfitReport,
CAST (0 as BIT) as sheetDebtReport,
CAST (0 as BIT) as importDoc,
CAST (0 as BIT) as exportDoc,
CASE
	WHEN (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany 
				LEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID 
				LEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId
				WHERE contactsList.ContactId = contactId)
		like '%HML/%'
		THEN 'HAIMINH'
	WHEN (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany 
				LEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID 
				LEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId
				WHERE contactsList.ContactId = contactId)
		like '%TNN/%'
		THEN 'TNN'
	ELSE (SELECT TOP 1 yourCompany.CmpID FROM YOURCOMPANY AS yourCompany 
				LEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID 
				LEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId
				WHERE contactsList.ContactId = contactId)
	END AS cmpId
FROM PARTNERS p 
WHERE p.Username = @username AND p.Password = @password AND p.CONFIRMREGISTER = 1
";
            return query;
        }
        public static void createTaskManual(string connect)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO TASK (TRANSACTIONID,STAFFID,IDKEYSHIPMENT,TASKNAME,CREATEDON,ASSIGNMOBILE,[STATUS],HBLNO,CREATETRUCK,ISOPERATED,ASSIGNOPIC,GETDEFAULTFEE,ISCLEARED,HM_ISGETBILLTASK,HM_ISGETOTHERTASK,ATTITUDE,ISSUPPORT,ISCORRECTION,HM_ISGETDO,ChangeDeadline,ISPROBLEM,ISREPLACETASK,STATUSPROBLEM) ");

            // Câu lệnh SELECT đầu tiên
            builder.Append("SELECT TransactionDetails.TransID, ISNULL(Transactions.ContactID,''), TransactionDetails.IDKeyShipment, N'Thông quan', GETDATE(), 1, 0, TransactionDetails.HWBNO, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ");
            builder.Append("FROM TransactionDetails ");
            builder.Append("LEFT JOIN Transactions ON TransactionDetails.TransID = Transactions.TransID ");
            builder.Append("WHERE ISNULL(Transactions.ContactID,'') <> '' AND Transactions.LoadingDate >= DATEADD(day, -10, GETDATE()) ");
            builder.Append("AND (SELECT UPPER(PLuong) FROM CustomsDeclaration JOIN TransactionDetails T ON CustomsDeclaration.MasoTK = TransactionDetails.CustomsID WHERE T.TransID = TransactionDetails.TransID AND T.IDKeyShipment = TransactionDetails.IDKeyShipment) = 'DO' ");
            builder.Append("AND NOT EXISTS (SELECT TOP 1 TaskName FROM Task WHERE TransactionDetails.IDKeyShipment = TASK.IDKEYSHIPMENT AND TASKNAME = N'Thông quan') ");
            builder.Append("AND Transactions.TpyeofService = 'CustomsLogistics' ");

            // UNION ALL cho các câu lệnh SELECT tiếp theo
            builder.Append("UNION ALL SELECT TransactionDetails.TransID, ISNULL(Transactions.ContactID,''), TransactionDetails.IDKeyShipment, N'Kiểm hóa', GETDATE(), 1, 0, TransactionDetails.HWBNO, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ");
            builder.Append("FROM TransactionDetails ");
            builder.Append("LEFT JOIN Transactions ON TransactionDetails.TransID = Transactions.TransID ");
            builder.Append("WHERE ISNULL(Transactions.ContactID,'') <> '' AND Transactions.LoadingDate >= DATEADD(day, -10, GETDATE()) ");
            builder.Append("AND (SELECT UPPER(PLuong) FROM CustomsDeclaration JOIN TransactionDetails T ON CustomsDeclaration.MasoTK = TransactionDetails.CustomsID WHERE T.TransID = TransactionDetails.TransID AND T.IDKeyShipment = TransactionDetails.IDKeyShipment) = 'DO' ");
            builder.Append("AND NOT EXISTS (SELECT TOP 1 TaskName FROM Task WHERE TransactionDetails.IDKeyShipment = TASK.IDKEYSHIPMENT AND TASKNAME = N'Kiểm hóa') ");
            builder.Append("AND Transactions.TpyeofService = 'CustomsLogistics' ");
            string queryString = builder.ToString();

            using (SqlConnection connection = new SqlConnection(connect))
            {

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlService.ExeNonQuery(queryString, connect);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }



        public static string countAllAdvancePaymentRequestByContactId(string contactId)
        {
            string query = string.Format($"SELECT COUNT(*) AS SIZE FROM ADVANCEPAYMENTREQUEST adv WHERE " +
                "(adv.advBodId = '{0}'" +
                //				"AND ISNULL(adv.ADVBODSTICKWAIT,0)=0 " +
                "AND ISNULL(adv.ADVBODSTICKAPP,0)=0 " +
                "AND ISNULL(adv.ADVBODSTICKDENY,0)=0 " +
                "AND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=1 " +
                "AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1) " +

                "OR (adv.ADVACSDPMANAGERID = '{0}' " +
                "AND ISNULL(adv.ADVACSDPMANAGERSTICKDENY,0)=0 " +
                "AND ISNULL(adv.ADVACSDPMANAGERSTICKAPP,0)=0 " +
                //				"AND ISNULL(adv.ADVACSDPMANAGERSTICKWAIT,0)=0 " +
                "AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=1) " +

                "OR (adv.ADVDPMANAGERID = '{0}' " +
                "AND ISNULL(adv.ADVDPMANAGERSTICKDENY,0)=0 " +
                //				"AND ISNULL(adv.ADVDPMANAGERSTICKWAIT,0)=0 " +
                "AND ISNULL(adv.ADVDPMANAGERSTICKAPP,0)=0) " +
                "AND ((ISNULL(adv.AdvSCID,'') <> '' AND ISNULL(adv.AdvCSStickApp,0) = 1) OR ISNULL(adv.AdvSCID,'') = '')", contactId);
            return query;
        }
        public static string getYourCompanyByContactId()
        {
            string query = "SELECT yourCompany.* FROM YOURCOMPANY AS yourCompany "
                    + "LEFT JOIN DEPARTMENTS AS department ON yourCompany.CmpID = department.CmpID "
                    + "LEFT JOIN CONTACTSLIST AS contactsList ON department.DeptId = contactsList.DeptId "
                    + "WHERE contactsList.ContactId = @contactId";
            return query;
        }
        
        public static string countAllAcsSetlementPaymentByContactId(string contactID, Boolean accountDepartmentAfter)
        {
            String queryString = string.Format("SELECT COUNT(*) AS SIZE FROM ACSSETLEMENTPAYMENT asp WHERE asp.StltDate IS NOT NULL AND " +
                "(asp.sltBodId = '{0}' " +
                "AND ISNULL(asp.SLTBODSTICKAPP,0)=0 " +
                "AND ISNULL(asp.SLTBODSTICKDENY,0)=0 " +
                "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=1 " +
                "AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP,0)=1) ", contactID);

            if (accountDepartmentAfter)
            {
                queryString += string.Format(
                        "OR (ISNULL(asp.SltSCID,'') = '{0}' " +
                            "AND ISNULL(asp.SltCSStickDeny,0)=0 " +
                            "AND ISNULL(asp.SltCSStickApp,0)=0" +
                            "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=1" +
                        ") " +
                        "OR (asp.SLTDPMANAGERID = '{0}' " +
                            "AND ISNULL(asp.SLTDPMANAGERSTICKDENY,0)=0 " +
                            "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=0 " +
                            ") " +
                        "OR (asp.SLTACSDPMANAGERID = '{0}' " +
                            "AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY,0)=0 " +
                            "AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP,0)=0 " +
                            "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=1) " +
                            "AND (ISNULL(asp.SltCSStickApp,0)=1 OR ISNULL(asp.SltSCID,'') = '') ", contactID);

            }
            else
            {
                queryString += string.Format(
                    "OR (asp.SLTDPMANAGERID = '{0}' " +
                        "AND ISNULL(asp.SLTDPMANAGERSTICKDENY,0)=0 " +
                        "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=0 " +
                        "AND (ISNULL(asp.SltCSStickApp,0)=1 OR ISNULL(asp.SltSCID,'') = '') " +
                    ") " +

                    "OR (ISNULL(asp.SltSCID,'') = '{0}' " +
                        "AND ISNULL(asp.SltCSStickDeny,0)=0 " +
                        "AND ISNULL(asp.SltCSStickApp,0)=0 " +
                    ") " +
                    "OR (asp.SLTACSDPMANAGERID = '{0}' " +
                        "AND ISNULL(asp.SLTACSDPMANAGERSTICKDENY,0)=0 " +
                        "AND ISNULL(asp.SLTACSDPMANAGERSTICKAPP,0)=0 " +
                        "AND ISNULL(asp.SLTDPMANAGERSTICKAPP,0)=1 " +
                        "AND (ISNULL(asp.SltCSStickApp,0)=1 OR ISNULL(asp.SltSCID,'') = '')) ", contactID);
            }
            return queryString;
        }
        public static string contRequestUnlockFile(string contactId)
        {
            string query = string.Format("SELECT COUNT (TasksList.ID) " +
                                "FROM TasksList " +
                                "WHERE (TasksList.Username = '{0}') AND (ISNULL(TasksList.CheckRead, 0) = 0) AND (ISNULL(TasksList.Status, '') = 'JobFileUnlockRequest' OR ISNULL(TasksList.Status, '') = 'InvoiceUnlockRequest' OR ISNULL(TasksList.Status, '') = 'BILLUnlockRequest')",contactId);
            return query;
        }
        public static string countUnfinisedTaskInPeriod(String staffId, Boolean isMobile)
        {
            string query = string.Format("SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK "
                    + "FROM TASK AS task " + "LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID "
                    + "LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID "
                    + "LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT "
                    + "LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID "
                    + "LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK "
                    + "WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 0 AND task.STAFFID = '{0}' ",staffId);
            if (null != isMobile)
            {
                if (isMobile)
                {
                    query += "AND task.ASSIGNMOBILE = 1";
                }
                else
                {
                    query += "AND task.ASSIGNMOBILE = 0 ";
                }
            }
            return query;
        }
        public static string countProcessingTaskInPeriod(String staffId, Boolean isMobile)
        {
            string query = string.Format("SELECT ISNULL(COUNT(task.TASKID),0) AS countTASK "
                    + "FROM TASK AS task " + "LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID "
                    + "LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID "
                    + "LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT "
                    + "LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID "
                    + "LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK "
                    + "WHERE task.IDKEYSHIPMENT IS NOT NULL AND ISNULL(task.[STATUS],0) = 1 AND task.STAFFID = '{0}' ",staffId);
            if (null != isMobile)
            {
                if (isMobile)
                {
                    query += "AND task.ASSIGNMOBILE = 1";
                }
                else
                {
                    query += "AND task.ASSIGNMOBILE = 0 ";
                }
            }
            return query;
        }
        public static string countTrucking(String truckNumber)
        {
            string query = string.Format("SELECT COUNT(*) AS countTrucking FROM TransactionDetails LEFT JOIN Transactions ON TransactionDetails.TransID = Transactions.TransID " +
                "WHERE TransactionDetails.ServiceMode = '{0}' AND TransactionDetails.TKStatus IS NULL AND ISNULL(TransactionDetails.TKFinish,0) = 0 AND ISNULL(TransactionDetails.RemocStatus,0) = 0",truckNumber);
            return query;
        }
        public static string getAllContactsList()
        {
            string query = "SELECT * FROM CONTACTSLIST c ORDER BY c.CONTACTID ASC";
            return query;
        }
        public static string GetContactByDeptID(string deptId)
        {
            string query =string.Format( "SELECT * FROM CONTACTSLIST AS c INNER JOIN DEPARTMENTS AS d ON c.deptId = d.deptId INNER JOIN InfoNote AS i ON d.CmpID = i.CmpID where c.deptId = '{0}' AND CHARINDEX(c.Username,i.InfoNotes) <> 0",deptId);
            return query;
        } 
        
        public static string getAllDriver()
        {
            string query = "SELECT * FROM CONTACTSLIST c WHERE ISNULL(c.LinkedTruckNo,'') <> ''";
            return query;
        }
        public static string getContacsListByCode(int code, String cmpId)
        {
            string queryString = "SELECT * FROM CONTACTSLIST AS c INNER JOIN DEPARTMENTS AS d ON c.deptId = d.deptId INNER JOIN InfoNote AS i ON d.CmpID = i.CmpID ";
            queryString += string.Format( "WHERE d.mngCode = '{0}' AND CHARINDEX(c.Username,i.InfoNotes) <> 0 ",code);
            if (!cmpId.IsNullOrEmpty())
            {
                queryString +=string.Format( "AND d.CmpID = '{0}' ",cmpId);
            }

            
            return queryString;
        }
        public static string getUnAssignedTask(string managerContactId, bool isMobile)
        {
            string query =string.Format(@"SELECT task.TASKID as taskId,
task.TASKREGISTERID as taskRegisterId,
task.TASKNAME as taskName,
task.[STATUS] as status,
task.TASKORDER as 'order',
task.STAFFID as staffId,
staff.ENGLISHNAME as staffName,
task.IDKEYSHIPMENT as idKeyShipment,
ISNULL(task.HBLNo,transDetail.HWBNO) as hwbno, 
task.FINISHDATE as finishDate,
task.[NOTES] as discription, 
task.FILECONTENTBASE64 as fileContentBase64, 
cd.TKSo as tkSo, 
ISNULL(task.ASSIGNEDON,'') as ngayGui,
ISNULL(cd.CucHQ,trans.Consolidatater) as cucHQ, 
cd.NguoiGui as nguoiGui , 
cd.NguoiNhan as nguoiNhan,  
CASE WHEN IsDate(trans.[LoadingDate])<>0 THEN trans.[LoadingDate] ELSE trans.[TransDate] END AS shipmentDate, 
task.TRANSACTIONID as transactionId,  
 cd.Loaihinh as loaiHinh, 
 cd.Maloaihinh as maLoaiHinh, 
 ISNULL(cd.PLUONG,trans.MarksRegistration) as phanLuong, 
 ISNULL(transDetail.ShipperID,'') as shipperId, 
 ISNULL(task.createTruck,0) as createTruck, 
 ISNULL(task.deadlineOn,'')as deadlineOn, 
 ISNULL(task.NOTES,'')as notes  , 
 ISNULL(task.HM_ISGETBILLTASK,0)as isGetBillTask  , 
 ISNULL(trans.RouteDelivery,'') as warehouse   ,
 ISNULL(task.HM_ISGETDO,0) as isGetDOTask   ,
 ISNULL(task.HM_NUMBERATTACHEDPAGES, 0)as numberattachedPages    ,
 ISNULL(task.HM_PAYMENTAMOUNT, 0.0) as paymentAmount    ,
 ISNULL(task.HM_PAYMENTAMOUNTUSD, 0.0) as paymentAmountUSD   ,
 ISNULL(task.HM_CARRIERNAME, '') as carrierName    ,
 ISNULL(task.HM_GETBILLADDRESS, '') as getBillAddress   ,
 ISNULL(task.HM_GETBILLDISTRICT, '') as getBilldistrict   ,
 ISNULL(task.HM_GETBILLCONTACT, '') as getBillcontact  
 FROM TASK AS task  
 LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  
 LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  
 LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  
 LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  
 LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK	  
 WHERE task.STAFFID IS NULL AND dept.ManagerContact = '{0}'",managerContactId);
            if (null != isMobile)
            {
                if (isMobile)
                {
                    query += "AND task.ASSIGNMOBILE = 1";
                }
                else
                {
                    query += "AND task.ASSIGNMOBILE = 0 ";
                }
            }
            query += "ORDER BY task.IDKEYSHIPMENT, task.TASKORDER ASC";

            return query;
        }
        public static string findTaskById(string taskid)
        {
            string query =string.Format( "SELECT * FROM TASK WHERE TASKID={0}",taskid);
            return query;
        }
        public static string UpdateUnassignTask(string taskid)
        {
            string query = string.Format(@"UPDATE TASK
SET 
ASSIGNEDBY=null,
STAFFID=null,
STATUS=0,
ASSIGNEDON=null,
ISRETURN=0
WHERE TASKID={0};",taskid);
            return query;
        }
        public static string updateAssignIsReturnedMobileTask(string idKeyShipment)
        {
            string query = string.Format(@"Update 
TransactionDetails
SET 
ISASSIGNED=1,
ISRETURNED=0
WHERE 
IDKeyShipment ='{0}'", idKeyShipment);
            return query;
        }        
        public static string updateAssignedMobileTask(string idKeyShipment)
        {
            string query = string.Format(@"Update 
TransactionDetails
SET 
ISASSIGNED=1
WHERE 
IDKeyShipment ='{0}'", idKeyShipment);
            return query;
        }
        public static string UpdateTask(string staffid, string deadlineon,string Idkeyshpment)
        {
            string query = string.Format(@"Update 
TASK
SET 
STAFFID='{0}',
STATUS=0,
ASSIGNEDON=DATE.Now,
ISRETURN=0,
DEADLINEON='{1}'
WHERE 
IDKeyShipment ='{2}'",staffid,deadlineon,Idkeyshpment);
            return query;
        }
        public static string UpdateTask2(string staffid, string deadlineon,string Idkeyshpment,string AssignedBy)
        {
            string query = string.Format(@"Update 
TASK
SET 
STAFFID='{0}',
STATUS=0,
ASSIGNEDON=DATE.Now,
ISRETURN=0,
AssignedBy={1},
DEADLINEON='{2}'
WHERE 
IDKeyShipment ='{3}'", staffid,deadlineon,Idkeyshpment);
            return query;
        }     
        public static string getRemainAssignTask(string idKeyShipment)
        {
            string query = string.Format(@"SELECT * " +
                    "  FROM TASK AS t " +
                    "  WHERE t.IDKEYSHIPMENT = '{0}' AND ISNULl(t.STAFFID,'') = '' AND ISNULL(t.ASSIGNMOBILE,0) = 1 ORDER BY t.TASKORDER ASC",idKeyShipment);
            return query;
        }
        public static string getAssignedTaskInPeriod(string firstDay, string lastDay, string staffId, bool isFinished, bool isMobile, string jobId, string cdsNo)
        {
            string query = string.Format(@" SELECT task.TASKID, task.TASKREGISTERID, task.TASKNAME, task.[STATUS], task.TASKORDER, task.STAFFID, staff.ENGLISHNAME, task.IDKEYSHIPMENT, ISNULL(task.HBLNo,transDetail.HWBNO) as hwbno, task.FINISHDATE, task.[DESCRIPTION], task.FILECONTENTBASE64, cd.TKSo, task.ASSIGNEDON as ngayGui, ISNULL(cd.CucHQ,trans.Consolidatater) as cucHQ, customer.PartnerName as nguoiGui, customer.PartnerName as nguoiNhan,  
					 CASE WHEN IsDate(trans.[LoadingDate])<>0 THEN trans.[LoadingDate] ELSE trans.[TransDate] END AS   shipmentDate, task.TRANSACTIONID as transactionId ,  
					 cd.Loaihinh as loaiHinh, cd.Maloaihinh as maLoaiHinh , ISNULL(cd.PLUONG,trans.MarksRegistration) as phanLuong, ISNULL(transDetail.ShipperID,'') as shipperId, ISNULL(task.createTruck,0) as createTruck, task.deadlineOn as deadlineOn, ISNULL(task.NOTES,'') as notes 
					 , ISNULL(task.HM_ISGETBILLTASK,0)  as isGetBillTask 
					  , ISNULL(trans.RouteDelivery,'') as warehouse   
					 , ISNULL(task.ProblemName,'')  as problemName
					  , ISNULL(task.resolution,'')   as resolution
					 ,ISNULL(task.HM_ISGETDO,0)    as isGetDOTask
					 ,ISNULL(task.HM_NUMBERATTACHEDPAGES, 0)   as numberattachedPages
					 ,ISNULL(task.HM_PAYMENTAMOUNT, 0.0)   as paymentAmount
					 ,ISNULL(task.HM_PAYMENTAMOUNTUSD, 0.0)   as paymentAmountUSD
					 ,ISNULL(task.HM_CARRIERNAME, '')   as carrierName
					 ,ISNULL(task.HM_GETBILLADDRESS, '')   as getBillAddress
					 ,ISNULL(task.HM_GETBILLDISTRICT, '')   as getBilldistrict
					 ,ISNULL(task.HM_GETBILLCONTACT, '')   as getBillcontact " +
                    "FROM TASK AS task " +
                    "LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID " +
                    "LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID " +
                    "LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT " +
                    "LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID " +
                    "LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK " +
                    "LEFT JOIN Partners as customer ON customer.PartnerId = transDetail.ShipperID " +
                    "WHERE task.IDKEYSHIPMENT IS NOT NULL ");
            if (isFinished != null)
            {
                
                    if (isFinished)
                    {
                        query += "AND task.[STATUS] = 2 ";
                    }
                    else
                    {
                        query += "AND (task.[STATUS] = 1) ";
                    }

            }
            else
            {
                query += "AND ISNULL(task.[STATUS],0) = 0 ";
            }
            if (null != isMobile)
            {
                if (isMobile)
                {
                    query += "AND task.ASSIGNMOBILE = 1 ";
                }
                else
                {
                    query += "AND task.ASSIGNMOBILE = 0 ";
                }
            }
            if (!String.IsNullOrEmpty(jobId))
            {
                query += "AND trans.TransID LIKE '%" + jobId + "%' ";
            }
            if (!String.IsNullOrEmpty(cdsNo))
            {
                query += "AND ISNULL(cd.TKSo,'') LIKE '%" + cdsNo + "%' ";
            }
            if (String.IsNullOrEmpty(cdsNo) && String.IsNullOrEmpty(jobId))
            {
                if (null != isFinished)
                {
                    if (isFinished)
                    {
                        if (!firstDay.IsNullOrEmpty())
                            query += "AND task.FINISHDATE >= '"+firstDay+"'";
                        if (!lastDay.IsNullOrEmpty())
                            query += "AND task.FINISHDATE <= '" + lastDay + "'";
                    }
                    else
                    {
                        if (!firstDay.IsNullOrEmpty())
                            query += "AND task.ASSIGNEDON >= '" + firstDay + "'";
                        if (!lastDay.IsNullOrEmpty())
                            query += "AND task.ASSIGNEDON <= '" + lastDay + "'";
                    }
                }
                else
                {
                    if (!firstDay.IsNullOrEmpty())
                        query += "AND task.ASSIGNEDON >= '" + firstDay + "'";
                    if (!lastDay.IsNullOrEmpty())
                        query += "AND task.ASSIGNEDON <= '" + lastDay + "'";
                }
                if (!staffId.IsNullOrEmpty())
                {
                    query +=string.Format( "AND task.STAFFID = '{0}' ",staffId);
                }
                query += " ORDER BY task.IDKEYSHIPMENT, task.TASKORDER ASC";

            }
            return query;
        } 
        public static string getDetailTask(string taskId)
        {
            string query = string.Format(@" SELECT task.TASKID, task.TASKREGISTERID, task.TASKNAME, task.[STATUS], task.TASKORDER as 'order', task.STAFFID, staff.ENGLISHNAME as staffName, task.IDKEYSHIPMENT, ISNULL(task.HBLNo,transDetail.HWBNO) as hwbno, task.FINISHDATE, task.[DESCRIPTION], ISNULL(task.FILECONTENTBASE64,'')as fileContentBase64_1, cd.TKSo, task.ASSIGNEDON as ngayGui, ISNULL(cd.CucHQ,trans.Consolidatater) as cucHQ
							 , assigner.ContactName as nguoiGui
							 , customer.PartnerName as nguoiNhan,  
								 CASE WHEN IsDate(trans.[LoadingDate])<>0 THEN trans.[LoadingDate] ELSE trans.[TransDate] END AS shipmentDate, task.TRANSACTIONID as transactionId,  
								 cd.Loaihinh as loaiHinh, cd.Maloaihinh, ISNULL(cd.PLUONG,trans.MarksRegistration) as phanLuong, ISNULL(transDetail.ShipperID,'')as shipperId, ISNULL(task.createTruck,0) as createTruck, task.deadlineOn, ISNULL(task.Notes,'') as notes  
								 , ISNULL(CASE WHEN trans.Noofpieces > 0 THEN  CAST (trans.Noofpieces AS VARCHAR) + ' ' +  trans.UnitPieaces ELSE '' END ,'')  as noPieces
								 ,ISNULL(task.HM_ISGETBILLTASK, 0)  as isGetBillTask 
								 ,ISNULL(task.HM_ISGETOTHERTASK, 0) as isGetOtherTask   
								 ,ISNULL(task.HM_REQUESTEREMAIL, '')   as requesterEmail
								 ,ISNULL(task.HM_REQUESTERNAME, '')   as requesterName
								 ,ISNULL(task.HM_NUMBERATTACHEDPAGES, 0)   as numberattachedPages
								 ,ISNULL(task.HM_PAYMENTAMOUNT, 0.0)   as paymentAmount
								 ,ISNULL(task.HM_PAYMENTAMOUNTUSD, 0.0)   as paymentAmountUSD
								 ,ISNULL(task.HM_CARRIERNAME, '')   as carrierName
								 ,ISNULL(task.HM_GETBILLADDRESS, '')  as getBillAddress 
								 ,ISNULL(task.HM_GETBILLDISTRICT, '')  as getBillDistrict 
								 ,ISNULL(task.HM_GETBILLCONTACT, '')   as getBillContact
								 ,ISNULL(task.HM_ONBOARDDATE, '')   as onboardDate
								 ,ISNULL(task.HM_INVOICENO, '')   as invoiceNo
								 ,ISNULL(task.HM_BILLTYPE, '')   as billType
								 ,ISNULL(task.HM_PACKAGE, '') as packages  
								 ,ISNULL(task.HM_20QTY, 0)   as qty20
								 ,ISNULL(task.HM_40QTY, 0)   as qty40
								 , ISNULL(ISNULL(task.HM_GW,trans.GrossWeight),0)  as gw
								 , ISNULL(ISNULL(task.HM_CBM,trans.SeaCBM),0)  as cbm
								 ,ISNULL(task.HM_GETBILLCELLNO, '')   as getBillCellNo
								 ,ISNULL(task.HBLNO, '')   as billNo
								 ,ISNULL(trans.RouteDelivery,'')  as warehouse 
								 ,ISNULL(task.ProblemName,'')  as problemName
								 ,ISNULL(task.resolution,'')   as resolution
								 ,ISNULL(task.FILENAMEFIREBASE,'')   AS fileNameFirebase_1
								 ,ISNULL(task.FileName,'')   as fileName_1
								 ,ISNULL(task.FileExt,'')   as fileExt_1
								 ,ISNULL(task.FILECONTENTBASE64_1,'') as fileContentBase64_2
								 ,ISNULL(task.FILENAMEFIREBASE_1,'')   fileNameFirebase_2
								 ,ISNULL(task.FileName_1,'')   as fileName_2
								 ,ISNULL(task.FileExt_1,'')   as fileExt_2
								 ,ISNULL(task.FILECONTENTBASE64_2,'') as fileContentBase64_3
								 ,ISNULL(task.FILENAMEFIREBASE_2,'')   as fileNameFirebase_3
								 ,ISNULL(task.FileName_2,'')   as fileName_3
								 ,ISNULL(task.FileExt_2,'')   as fileExt_3
								 ,ISNULL(task.FILECONTENTBASE64_3,'') as fileContentBase64_4
								 ,ISNULL(task.FILENAMEFIREBASE_3,'')   as fileNameFirebase_4
								 ,ISNULL(task.FileName_3,'')   as fileName_4
								 ,ISNULL(task.FileExt_3,'')   as  fileExt_4
								 ,ISNULL(task.FILECONTENTBASE64_4,'') as fileContentBase64_5
								 ,ISNULL(task.FILENAMEFIREBASE_4,'')   as fileNameFirebase_5
								 ,ISNULL(task.FileName_4,'')   as fileName_5
								 ,ISNULL(task.FileExt_4,'')   as fileExt_5
								 ,ISNULL(task.HM_ISGETDO, 0)   as isGetDOTask
								 ,ISNULL(trans.FlghtNo, '')   as vehicleNo
								 ,ISNULL(transDetail.Notes, '')   as specialRequest
								 FROM TASK AS task  
								 LEFT JOIN DEPARTMENTS AS dept ON dept.DEPTID = task.DEPARTMENTID  
								 LEFT JOIN CONTACTSLIST AS staff ON staff.CONTACTID = task.STAFFID  
								 LEFT JOIN CONTACTSLIST AS assigner ON assigner.CONTACTID = task.ASSIGNEDBY  
								 LEFT JOIN TRANSACTIONDETAILS AS transDetail ON transDetail.IDKEYSHIPMENT = task.IDKEYSHIPMENT  
								 LEFT JOIN PARTNERS AS customer ON customer.PartnerId = transDetail.ShipperID  
								 LEFT JOIN Transactions as trans ON transDetail.TransID = trans.TransID  
								 LEFT JOIN CustomsDeclaration as cd ON transDetail.CustomsID = cd.MasoTK  
								 WHERE task.TASKID = '{0}'", taskId);
            return query;
        }
        public static string getInfoTransactionInfoDetail(string hblNo)
        {
            string query = string.Format(@"SELECT FieldKey, HAWBNO, Description, FileName, FileExt FROM TransactionInfoDetail WHERE TransactionInfoDetail.HAWBNO = '{0}'",hblNo);
            return query;
        }
        public static string getInfoTransactionInfoDetailByIDKeyShipment(string idKeyShipment)
        {
            string query = string.Format(@"SELECT transInfo.FieldKey, transInfo.HAWBNO, transInfo.Description, transInfo.FileName, transInfo.FileExt FROM TransactionInfoDetail AS transInfo LEFT JOIN TransactionDetails AS transDetail ON transDetail.HWBNO = transInfo.hawbNo WHERE transDetail.IDKeyShipment = '{0}' ",idKeyShipment);
            return query;
        }        public static string Template()
        {
            string query = string.Format(@"SELECT * FROM DEPARTMENTS");
            return query;
        }

    }
}
