namespace FAST_API_V2.ViewModels
{
    public class LoginOutputVM
    {
        public string contactId { get; set; }
        public string userName { get; set; }
        public string deptId { get; set; }
        public string cmpId { get; set; }
        public string topic { get; set; }
        public string deptName { get; set; }
        public string contactName { get; set; }
        public string version { get; set; }
        public string result { get; set; }
        public string content { get; set; }
        public string role { get; set; }
        public string vehicleNo { get; set; }
        public string accessKpiReport { get; set; }
        public bool importDoc { get; set; }
        public bool exportDoc { get; set; }
        public string isShowHandling { get; set; }
        public string isShowLogistics { get; set; }
        public string isAuthorizedSetlement { get; set; }
        public string isAuthorizedAdvance { get; set; }
        public string isManager { get; set; }
        public string countAdvance { get; set; }
        public string countSetle { get; set; }
        public string countUnlockRequest { get; set; }
        public string isLog { get; set; }
        public string countTask { get; set; }
        public string countProcessingTask { get; set; }
        public string isTruck { get; set; }
        public string countTruck { get; set; }
        public string idKeyShipment { get; set; }
        public string countBooking { get; set; }
        public string countMyAdvance { get; set; }
        public string countMySetle { get; set; }
        public List<Depts> listDepts { get; set; }
        public string adminList { get; set; }
        public string pushNotifyUrl { get; set; }
        public bool isDeleteHandlingTask { get; set; }
        public string isTruckisDeleteHandlingTask { get; set; }
        public string accessRight { get; set; }
        public string isAdmin { get; set; }
        public string resetTime { get; set; }
        public List<TroubleLabel> listTroubleLabel { get; set; }
        public string apiKeyGps { get; set; }
        public string idGps { get; set; }
        public bool isCustomer { get; set; }
        public string partnerId { get; set; }
        public string partnerName { get; set; }
        public bool sendEmail { get; set; }
        public bool assignOpsStaff { get; set; }
        public bool saleProfitReport { get; set; }
        public bool sheetDebtReport { get; set; }
        public bool isAgent { get; set; }
        public bool isColoader { get; set; }
        public string yourCompany { get; set; }
        public bool isAssignRight { get; set; }
    }

    public class Depts
    {
        public string deptId { get; set; }
        public string deptName { get; set; }
        public string mangerId { get; set; }
    }

    public class TroubleLabel
    {
        public string label { get; set; }
    }
}
