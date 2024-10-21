namespace FAST_API_V2.ViewModels
{
    public class ContactListOutputVM
    {
        public List<ContactDetailVM> contactLists { get; set; } = new List<ContactDetailVM>();
    }
    public class ContactDetailVM
    {
        public string ContactID { get; set; }
        public string ContactName { get; set; }
        public string Username { get; set; }
    }
}
