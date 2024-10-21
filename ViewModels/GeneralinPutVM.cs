namespace FAST_API_V2.ViewModels
{
    public class GeneralinPutVM
    {
    }
    public class DeptIdInputVM
    {
        public string deptId { get; set; }
    }
    public class ContactIdInputVM
    {
        public string contactId { get; set;}
    }
    public class ContactIdAndIsMobileInputVM
    {
        public string contactId { get; set; }
        public bool isMobile { get; set; }
    }
    public class AssignTaskInputVM
    {
        public string taskId { get; set; }
        public string staffId { get; set; }
        public string contactId { get; set; }
    }
    public class GetMyTaskInputVM
    {
        public string contactId { get; set; }
        public bool isFinished { get; set; }
        public bool isMobile { get; set; }
        public string jobId { get; set; }
        public string cdsNo { get; set; }
        public string? lastDay { get; set; }
        public string? firstDay { get; set; }
    }
    public class TaskIdInputV
    {
        public string taskId { get; set; }
    }
}
