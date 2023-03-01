
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Encrypt_Decrypt_Database_MVC_Core.Models;

using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace PasswordEncrypt.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration;

        public HomeController(IConfiguration _configuration)
        {
         
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View(GetUsers());
        }

        [HttpPost]
        public IActionResult Index1(string encryption, string password1)
        {
            string constr = this.Configuration.GetConnectionString("MyConn");
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "INSERT INTO Users VALUES (@Username, @Password)";
                string query = "INSERT INTO Users VALUES (@Encryptname, @Password1,null,null)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    //cmd.Parameters.AddWithValue("@Username", userName);
                    //cmd.Parameters.AddWithValue("@Password", Encrypt(password));
                    cmd.Parameters.AddWithValue("@Encryptname", Encrypt(encryption));
                    cmd.Parameters.AddWithValue("@Password1", Encrypt(password1));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View("Index", GetUsers());
        }
        [HttpPost]
        public IActionResult Index2(string decryption, string password2)
        {
            string constr = this.Configuration.GetConnectionString("MyConn");
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "INSERT INTO Users VALUES (@Username, @Password)";
                string query = "INSERT INTO Users VALUES (null, null,@Decryptname,@Password2)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    //cmd.Parameters.AddWithValue("@Username", userName);
                    //cmd.Parameters.AddWithValue("@Password", Encrypt(password));
                    cmd.Parameters.AddWithValue("@Decryptname", Decrypt(decryption));
                    cmd.Parameters.AddWithValue("@Password2", Decrypt(password2));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View("Index", GetUsers());
        }


        private List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string constr = this.Configuration.GetConnectionString("MyConn");
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Encryptname, Password1,Decryptname, Password2 FROM Users"))
                //using (SqlCommand cmd = new SqlCommand("SELECT Username, Password FROM Users"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            users.Add(new UserModel
                            {
                                Username = sdr["Encryptname"].ToString(),
                            
                                //   DecryptedUsername = Decrypt(sdr["Encryptname"].ToString()),
                                EncryptedPassword = sdr["Password1"].ToString(),
                                DecryptedUsername = sdr["Decryptname"].ToString(),
                                DecryptedPassword = sdr["Password2"].ToString(),
                                //   DecryptedPassword = Decrypt(sdr["Password1"].ToString())
                            }); ;
                        }
                    }
                    con.Close();
                }
            }
            return users;
        }

        private string Encrypt(string clearText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }
    }
}