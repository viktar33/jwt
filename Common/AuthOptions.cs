using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common
{
    public class AuthOptions
    {
        public string  Issuer { get; set; }

        public string Audience { get; set; }

        public string Secret { get; set; }

        public int TokenLifeTime { get; set; }

        public SymmetricSecurityKey GetSummetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}