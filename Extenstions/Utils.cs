using System.Security.Cryptography;
using System.Text;
namespace FAST_API_V2.Extenstions
{
    public static class Utils
    {
        private static readonly string SecretKey = "0302385187";
        private static readonly string Salt = "COD-211023-000042";

        public static string Encrypt(this string strToEncrypt)
        {
            try
            {
                byte[] iv = new byte[16]; // AES block size is 16 bytes
                using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(SecretKey, Encoding.UTF8.GetBytes(Salt), 15888, HashAlgorithmName.SHA256))
                {
                    byte[] key = rfc2898DeriveBytes.GetBytes(32); // AES key size is 256 bits (32 bytes)
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                        {
                            byte[] plainTextBytes = Encoding.UTF8.GetBytes(strToEncrypt);
                            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                            return Convert.ToBase64String(encryptedBytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while encrypting: " + ex.ToString());
                return null;
            }
        }

        public static T MapTo<T>(this Dictionary<string, object> keyVal) where T : class
        {
            if (keyVal is null) return null;
            var instance = (T)Activator.CreateInstance(typeof(T));
            var props = typeof(T).GetProperties();
            foreach (var key in keyVal.Keys)
            {
                var prop = props.FirstOrDefault(x => x.Name == key);
                if (prop is null || !prop.CanWrite) continue;
                // var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                // var parsedVal = converter.ConvertFromInvariantString(keyVal[key]?.ToString());

                object value = keyVal[key];

                // Kiểm tra nếu thuộc tính là kiểu bool
                if (prop.PropertyType == typeof(bool))
                {
                    // Chuyển đổi từ int sang bool
                    value = (value is int intValue) ? (intValue != 0) : false;
                }
                prop.SetValue(instance, keyVal[key]);
            }
            return instance;
        }
        public static Dictionary<string, object> ToDictionary<T>(this T obj) where T : class
        {
            var dictionary = new Dictionary<string, object>();

            if (obj != null)
            {
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    dictionary[prop.Name] = prop.GetValue(obj);
                }
            }

            return dictionary;
        }



    }
}
