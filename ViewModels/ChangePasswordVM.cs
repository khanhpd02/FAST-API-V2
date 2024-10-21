namespace FAST_API_V2.ViewModels
{
    public class ChangePasswordVM
    {
        public byte[] username { get; set; }
        public bool isCustomer { get; set; }
        public byte[] oldPassword { get; set; }
        public byte[] newPassword { get; set; }
    }
}
