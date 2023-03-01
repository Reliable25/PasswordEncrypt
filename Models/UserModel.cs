using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Encrypt_Decrypt_Database_MVC_Core.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string DecryptedUsername { get; set; }
        public string EncryptedPassword{ get; set; }
        public string DecryptedPassword { get; set; }
    }
}
