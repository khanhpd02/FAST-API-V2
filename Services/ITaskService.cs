using FAST_API_V2.ViewModels;

namespace FAST_API_V2.Services
{
    public interface ITaskService
    {
        Task<Dictionary<string, object>> ProccessGetUnassignedTask(ContactIdAndIsMobileInputVM vm, string connect);
        Task<Dictionary<string, object>> ProccessAssignTask(AssignTaskInputVM vm, string connect);
        Task<Dictionary<string, object>> ProccessGetMyTask(GetMyTaskInputVM vm, string connect);
        Task<Dictionary<string, object>> ProccessGetDetailTask(TaskIdInputV vm, string connect);
    }
}
