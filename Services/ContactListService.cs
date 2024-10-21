
using FAST_API_V2.Extenstions;
using FAST_API_V2.ViewModels;
using System.Xml.Linq;

namespace FAST_API_V2.Services
{
    public class ContactListService : IContactListService
    {
        private const int LOGISTICS_DEPT_TYPE = 2;
        private const int OPERATION_DEPT_TYPE = 4;
        private const int ACCOUNTANT_DEPT_TYPE = 6;

        public async Task<Dictionary<string, object>> GetAllContactList(string connect)
        {
            
            ContactListOutputVM contactListOutputVMs =new ContactListOutputVM ();
            var contactList = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.getAllContactsList(),connect);
            /*foreach (var contact in contactList)
            {
                foreach (var contact2 in contact)
                {
                    var ct = new ContactDetailVM
                    {
                        ContactID = contact2["ContactID"].ToString(),
                        ContactName = contact2["ContactName"].ToString(),
                        Username = contact2["Username"].ToString()

                    };
                    contactListOutputVMs.listcontactdetail.Add(ct);
                }
            }*/
            contactListOutputVMs.contactLists=contactList;
            return contactListOutputVMs.ToDictionary();
        }

        public async Task<Dictionary<string, object>> GetAllDeparment(string connect)
        {
            DepartmentOutPutVM departmentoutput = new DepartmentOutPutVM();
            var departmentlist = await SqlService.ReadDataSet(SqlQuery.getAllDepartmentsList(), connect);
            foreach (var department in departmentlist)
            {
                foreach (var rps in department)
                {
                    var ct = new DepartmentDetail
                    {
                        deptId = rps["DeptID"].ToString(),
                        deptName = rps["Department"].ToString(),
                        deptManagerId = rps["ManagerContact"].ToString()

                    };
                    departmentoutput.departmentDetailList.Add(ct);
                }
            }

            return departmentoutput.ToDictionary();
        }

        public async Task<Dictionary<string, object>> GetAllDriver(string connect)
        {
            DriverOutputVM driverOutputVM = new DriverOutputVM();
            var driverList = await SqlService.ReadDataSet(SqlQuery.getAllDriver(), connect);
            foreach (var drive in driverList)
            {
                foreach (var rps in drive)
                {
                    var ct = new DriverDeTail
                    {
                        contactId = rps["ContactID"].ToString(),
                        contactName = rps["ContactName"].ToString(),
                        vehicleNo = rps["LinkedTruckNo"].ToString()

                    };
                    driverOutputVM.driverDeTails.Add(ct);
                }
            }

            return driverOutputVM.ToDictionary();
        }

        public async Task<Dictionary<string, object>> getContactsByDeptCodeLogistics(ContactIdInputVM contactId, string connect)
        {
            var ContactId = new ParametersVM
            {
                Field = "contactId",
                Value = contactId.contactId
            };
            var youconpany = await SqlService.ReadDataSet(SqlQuery.getYourCompanyByContactId(), connect, ContactId);
            string cmpId = youconpany[0][0]["CmpID"].ToString();
            var listContact = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.getContacsListByCode(LOGISTICS_DEPT_TYPE,cmpId),connect);
            var output= new ContactListOutputVM { contactLists = listContact };
            return output.ToDictionary();
        }

        public async Task<Dictionary<string, object>> GetContactListByDeptId(DeptIdInputVM deptId, string connect)
        {
            ContactListOutputVM contactListOutputVMs = new ContactListOutputVM();
            var contactList = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.GetContactByDeptID(deptId.deptId), connect);
            /*foreach (var contact in contactList)
            {
                foreach (var contact2 in contact)
                {
                    var ct = new ContactDetailVM
                    {
                        contactID = contact2["ContactID"].ToString(),
                        contactName = contact2["ContactName"].ToString(),
                        userName = contact2["Username"].ToString()

                    };
                    contactListOutputVMs.listcontactdetail.Add(ct);
                }
            }*/
            var output = new ContactListOutputVM { contactLists = contactList };
            return output.ToDictionary();
        }

        public async Task<Dictionary<string, object>> getContactsByDeptCodeOperation(ContactIdInputVM contactId, string connect)
        {
            var contactID = new ParametersVM
            {
                Field = "contactId",
                Value = contactId.contactId
            };
            var youconpany = await SqlService.ReadDataSet(SqlQuery.getYourCompanyByContactId(), connect, contactID);
            string cmpId = youconpany[0][0]["CmpID"].ToString();
            var listContact = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.getContacsListByCode(OPERATION_DEPT_TYPE, cmpId), connect);
            var output = new ContactListOutputVM { contactLists = listContact };
            if(youconpany[0][0]["CmpID"].ToString()== "INTERLINK/HCM"||
                youconpany[0][0]["CmpID"].ToString() == "INTERSKY/HCM" ||
                youconpany[0][0]["CmpID"].ToString() == "INTERLINK/HPH")
            {
                var listContact2 = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.getContacsListByCode(ACCOUNTANT_DEPT_TYPE, cmpId), connect);
                output.contactLists = listContact2;
            }
            return output.ToDictionary();
        }

        public async Task<Dictionary<string, object>> getContactsByDeptCodeAccountant(ContactIdInputVM contactId, string connect)
        {
            var ContactId = new ParametersVM
            {
                Field = "contactId",
                Value = contactId.contactId
            };
            var youconpany = await SqlService.ReadDataSet(SqlQuery.getYourCompanyByContactId(), connect, ContactId);
            string cmpId = youconpany[0][0]["CmpID"].ToString();
            var listContact = await SqlService.ReadDsAsList<ContactDetailVM>(SqlQuery.getContacsListByCode(ACCOUNTANT_DEPT_TYPE, cmpId), connect);
            var output = new ContactListOutputVM { contactLists = listContact };
            return output.ToDictionary();
        }
    }
}
