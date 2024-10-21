using FAST_API_V2.Middlewares;
using FAST_API_V2.Services;
using FAST_API_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FAST_API_V2.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    public class ContactListController : Controller
    {
        private IContactListService _contactListService;
        private IConfiguration _configuration;
        public ContactListController(IContactListService contactListService, IConfiguration configuration)
        {
            _contactListService = contactListService;
            _configuration = configuration;
        }

        [HttpGet("/getAllContact.html")]
        public async Task<Dictionary<string, object>> getallContact()
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.GetAllContactList(connect);
        }

        [HttpGet("/get-contact-deptId.html")]
        public async Task<Dictionary<string, object>> GetcontactlistByDeptId([FromBody]DeptIdInputVM deptId)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.GetContactListByDeptId(deptId,connect);
        }
        [HttpGet("/get-all-dept.html")]
        public async Task<Dictionary<string, object>> getalldept()
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.GetAllDeparment(connect);
        }
        [HttpGet("/get-all-driver.html")]
        public async Task<Dictionary<string, object>> getalldriver()
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.GetAllDriver(connect);
        }
        [HttpGet("/get-contact-logistics.html")]
        public async Task<Dictionary<string, object>> getContactsByDeptCodeLogistics([FromBody] ContactIdInputVM contactid)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.getContactsByDeptCodeLogistics(contactid, connect);
        }

        [HttpGet("/get-contact-operation.html")]
        public async Task<Dictionary<string, object>> getContactsByDeptCodeOperation([FromBody] ContactIdInputVM contactid)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.getContactsByDeptCodeOperation(contactid, connect);
        }        
        [HttpGet("/get-contact-accountant.html")]
        public async Task<Dictionary<string, object>> getContactsByDeptCodeAccountant([FromBody] ContactIdInputVM contactid)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _contactListService.getContactsByDeptCodeAccountant(contactid, connect);
        }

    }
}
