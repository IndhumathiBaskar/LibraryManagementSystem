using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role {  get; set; }
        public string RefreshToken { get; set; }  // 🔹 Store Refresh Token
        public DateTime RefreshTokenExpiry { get; set; }  // 🔹 Expiry Date

    }
}
