using FAST_API_V2.Helpers;
using FAST_API_V2.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NanoidDotNet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

public interface IJwtUtils
{ 
    string ValidateJwtTokenPasswordFirebase(string jwtToken);
    string GenerateJwtToken(LoginOutputVM user);
    Guid? ValidateJwtToken(string token);
    string? ValidateJwtTokenEmail(string token);

    //RefreshToken GenerateRefreshToken();
}

public class JwtUtils : IJwtUtils
{

    private readonly AppSettings _appSettings;

    public JwtUtils(
        
        IOptions<AppSettings> appSettings)
    {
        
        _appSettings = appSettings.Value;
    }
    
    public string GenerateJwtToken(LoginOutputVM loginOutputVM)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var role = "";
        if(loginOutputVM.isAdmin)
        {
            role = "Admin";
        }
        else
        {
            role = "Customer";
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", loginOutputVM.contactName.ToString()),
                new Claim("Role", role),
                
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Guid? ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return userId;
        }
        catch (Exception)
        {
            // Log or handle the exception
            return null;
        }
    }
    public string? ValidateJwtTokenEmail(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userEmailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;
                return userEmail;
            }

            return null;
        }
        catch (Exception)
        {
            // Log or handle the exception
            return null;
        }
    }
    public string? ValidateJwtTokenPasswordFirebase(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userEmailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "passFirebase");

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;
                return userEmail;
            }

            return null;
        }
        catch (Exception)
        {
            // Log or handle the exception
            return null;
        }
    }


    /* public RefreshToken GenerateRefreshToken()
     {
         var refreshToken = new RefreshToken
         {
             Token = GenerateUniqueToken(),
             Expires = DateTime.UtcNow.AddDays(7),
             Created = DateTime.UtcNow,
         };

         return refreshToken;
     }*/

    private string GenerateUniqueToken()
    {
        const int length = 10; // Độ dài token cần tạo

        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var uniqueToken = Nanoid.Generate(alphabet, length);
        /*
                // Kiểm tra token có duy nhất hay không bằng cách kiểm tra trong cơ sở dữ liệu
                var tokenIsUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == uniqueToken));

                if (!tokenIsUnique)
                    return GenerateUniqueToken(); // Nếu không duy nhất, thử tạo token mới lần nữa*/

        return uniqueToken;
    }
}
