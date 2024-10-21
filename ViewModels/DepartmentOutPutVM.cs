namespace FAST_API_V2.ViewModels
{
    public class DepartmentOutPutVM
    {
        public List<DepartmentDetail> departmentDetailList { get; set; } = new List<DepartmentDetail>();
    }
    public class DepartmentDetail 
    {
        public string deptName { get; set; }
        public string deptManagerId { get; set; }
        public string deptId { get; set; }
    }
}
