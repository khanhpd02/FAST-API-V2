using FAST_API_V2.Middlewares;
using FAST_API_V2.Services;
using FAST_API_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FAST_API_V2.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    public class TaskController : Controller
    {

        private ITaskService _taskService;
        private IConfiguration _configuration;

        public TaskController(ITaskService taskService, IConfiguration configuration)
        {
            _taskService = taskService;
            _configuration = configuration;
        }

        [HttpPost("/get-unassigned-task.html")]
        public async Task<Dictionary<string, object>> proccessGetUnassignedTask([FromBody] ContactIdAndIsMobileInputVM vm)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _taskService.ProccessGetUnassignedTask(vm,connect);
        }
        [HttpPost("/assign-task.html")]
        public async Task<Dictionary<string, object>> proccessAssignTask([FromBody] AssignTaskInputVM vm)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _taskService.ProccessAssignTask(vm,connect);
        }
        [HttpPost("/get-my-task.html")]
        public async Task<Dictionary<string, object>> GetMyTask([FromBody] GetMyTaskInputVM vm)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _taskService.ProccessGetMyTask(vm,connect);
        }
        [HttpPost("/get-detail-task.html")]
        public async Task<Dictionary<string, object>> GetDetailTask([FromBody] TaskIdInputV vm)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _taskService.ProccessGetDetailTask(vm,connect);
        }
    }
}
