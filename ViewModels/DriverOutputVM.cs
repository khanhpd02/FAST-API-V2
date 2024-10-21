namespace FAST_API_V2.ViewModels
{
    public class DriverOutputVM
    {
        public List<DriverDeTail> driverDeTails { get; set; }= new List<DriverDeTail>();   
    }
    public class DriverDeTail
    {
        public string vehicleNo { get; set; }
        public string contactId { get; set; }
        public string contactName { get; set; }
    }
}
