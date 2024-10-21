namespace FAST_API_V2.ViewModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ListDept
    {
        public string deptName { get; set; }
        public string deptId { get; set; }
        public string mangerId { get; set; }
    }

    public class ListTroubleLabel
    {
        public string label { get; set; }
    }

    public class LoginOutputVM
    {
        public string deptName { get; set; }
        public bool importDoc { get; set; }
        public string role { get; set; }
        public string adminList { get; set; }
        public bool isLog { get; set; }
        public bool exportDoc { get; set; }
        public string pushNotifyUrl { get; set; }
        public string content { get; set; }
        public string result { get; set; }
        public int countAdvance { get; set; }
        public int countSetle { get; set; }
        public bool sendEmail { get; set; }
        public int countUnlockRequest { get; set; }
        public bool isTruck { get; set; }
        public List<ListDept> listDepts { get; set; }
        public int countMySetle { get; set; }
        public bool isManager { get; set; }
        public bool isAssignRight { get; set; }
        public int resetTime { get; set; }
        public int countTruck { get; set; }
        public int accessRight { get; set; }
        public string contactId { get; set; }
        public int countTask { get; set; }
        public string partnerName { get; set; }
        public string contactName { get; set; }
        public string cmpId { get; set; }
        public bool accessKpiReport { get; set; }
        public string apiKeyGps { get; set; }
        public string deptId { get; set; }
        public List<ListTroubleLabel> listTroubleLabel { get; set; }
        public bool sheetDebtReport { get; set; }
        public bool isAdmin { get; set; }
        public string userName { get; set; }
        public string yourCompany { get; set; }
        public bool isShowLogistics { get; set; }
        public string version { get; set; }
        public bool isAuthorizedSetlement { get; set; }
        public bool isAuthorizedAdvance { get; set; }
        public bool assignOpsStaff { get; set; }
        public string token { get; set; }
        public string vehicleNo { get; set; }
        public bool isColoader { get; set; }
        public bool isCustomer { get; set; }
        public string idKeyShipment { get; set; }
        public string topic { get; set; }
        public bool isShowHandling { get; set; }
        public int countBooking { get; set; }
        public string idGps { get; set; }
        public bool isDeleteHandlingTask { get; set; }
        public bool isAgent { get; set; }
        public string partnerId { get; set; }
        public int countMyAdvance { get; set; }
        public bool saleProfitReport { get; set; }
    }


}
