using FAST_API_V2.ViewModels;

namespace FAST_API_V2.Services
{
    public interface IContactListService
    {
        Task<Dictionary<string, object>> GetAllContactList( string connect); 
        Task<Dictionary<string, object>> GetAllDeparment( string connect);
        Task<Dictionary<string, object>> GetAllDriver( string connect);
        Task<Dictionary<string, object>> GetContactListByDeptId( DeptIdInputVM deptId,string connect );
        Task<Dictionary<string, object>> getContactsByDeptCodeLogistics(ContactIdInputVM contactId, string connect);
        Task<Dictionary<string, object>> getContactsByDeptCodeOperation(ContactIdInputVM contactId, string connect);
        Task<Dictionary<string, object>> getContactsByDeptCodeAccountant(ContactIdInputVM contactId, string connect);


    }
}
