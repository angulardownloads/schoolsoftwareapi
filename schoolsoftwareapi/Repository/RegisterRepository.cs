using schoolsoftwareapi.Abstract;
using schoolsoftwareapi.Model;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Data.SqlClient;

namespace schoolsoftwareapi.Repository
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IConfiguration _config;

        public RegisterRepository(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("MySchoolSoftwareString"));
            }
        }

        public async Task<Register> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT ContactId, Name, Position, Age,Salary FROM Register WHERE ContactId = @ContactId";
                conn.Open();
                var result = await conn.QueryAsync<Register>(sQuery, new { ContactId = id });
                return result.FirstOrDefault();

            }
        }

        //public async Task<Register> GetUserType(string username)
        //{
        //  using (IDbConnection conn = Connection)
        //  {
        //    string sQuery = "SELECT usertype FROM Register WHERE username = @username";
        //    conn.Open();
        //    var result = await conn.QueryAsync<Register>(sQuery, new { username = username });
        //    return result.FirstOrDefault();

        //  }
        //}
        public async Task<IEnumerable<Register>> GetUserType(string username)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var res = await dbConnection.QueryAsync<Register>("SELECT * FROM Register where email='" + username + "'");
                return res;
            }
        }

        public async Task<IEnumerable<Register>> GetRegister()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<Register>("SELECT * FROM Register");
            }
        }

        public static string GenerateAPassKey(string passphrase)
        {
            // Pass Phrase can be any string
            string passPhrase = passphrase;
            // Salt Value can be any string(for simplicity use the same value as used for the pass phrase)
            string saltValue = passphrase;
            // Hash Algorithm can be "SHA1 or MD5"
            string hashAlgorithm = "SHA1";
            // Password Iterations can be any number
            int passwordIterations = 2;
            // Key Size can be 128,192 or 256
            int keySize = 256;
            // Convert Salt passphrase string to a Byte Array
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            // Using System.Security.Cryptography.PasswordDeriveBytes to create the Key
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            //When creating a Key Byte array from the base64 string the Key must have 32 dimensions.
            byte[] Key = pdb.GetBytes(keySize / 11);
            String KeyString = Convert.ToBase64String(Key);

            return KeyString;
        }

        //Save the keystring some place like your database and use it to decrypt and encrypt
        //any text string or text file etc. Make sure you dont lose it though.

        public static string Encrypt(string plainStr, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.ECB;
            aesEncryption.Padding = PaddingMode.ISO10126;
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText);
        }

        public static string Decrypt(string encryptedText, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.ECB;
            aesEncryption.Padding = PaddingMode.ISO10126;
            byte[] KeyInBytes = System.Text.Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length);
            return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }

        public string Add(Register Emp)
        {
            String KeyString = "Gv7L3V15jCdb9P5XGKiPnhHZ7JlKcmU=";
            String EncryptedPassword = Encrypt(Emp.Password, KeyString);

            //string password =Encrypt(Emp.Password, KeyString);
            String DecryptedPassword = Decrypt(EncryptedPassword, KeyString);
            Emp.Password = EncryptedPassword;
            Emp.Usertype = "0";
            Emp.Accounttype = "Free";
            Emp.License = "1 Year";
            string dateTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            Emp.Registereddate = dateTime;
            Emp.Lastloggedin = dateTime;
            string exists = this.checkuserexists(Emp.Email.ToString());
            if (exists == "false")
            {


                using (IDbConnection dbConnection = Connection)
                {
                    string sQuery = "INSERT INTO Register (username,phone,email,password,usertype,accounttype,license,registereddate,lastloggedin)"
                                    + " VALUES(@username,@phone,@email, @password,@usertype,@accounttype,@license,@registereddate,@lastloggedin)";
                    dbConnection.Open();
                    dbConnection.Execute(sQuery, Emp);

                    MimeMessage message = new MimeMessage();

                    MailboxAddress from = new MailboxAddress("HTML conversions",
                    "support@htmlconversions.com");
                    message.From.Add(from);

                    //MailboxAddress to = new MailboxAddress("User",
                    //"ravikumar289@gmail.com");
                    MailboxAddress to = new MailboxAddress("User",
                    Emp.Email);
                    message.To.Add(to);

                    message.Subject = "Registration successfull - 000schoolsoftware.com";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = "<h1>Thank you for registering!!</h1>" +
                      "<h1>Keep visiting our site</h1>" +
                      "<h1><a href='www.000schoolsoftware.com' target='_blank'>www.000schoolsoftware.com</a><h1>" +
                      "<h1>Email :" + Emp.Email + "<h1>" +
                      "<h1>Password :" + DecryptedPassword + "<h1>" +
                      "<h1>Want to change password??<a target='_blank' href='https://www.htmlconversions.com/changepassword/email=" + Emp.Email + "'>Change password</a> <h1>";
                    //bodyBuilder.TextBody = "Hello World!";
                    message.Body = bodyBuilder.ToMessageBody();
                    MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                    client.Connect("smtpout.secureserver.net", 465, true);
                    client.Authenticate("shopshoppywebsite@gmail.com", "Admin@1234");
                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();


                }
                return "true";
            }
            else
            {
                return "false";
            }

        }
        public string checkuserexists(string email)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "SELECT count(*) FROM register where email = " + "'" + email + "'";

                dbConnection.Open();
                var res = dbConnection.ExecuteScalar(sQuery);
                //return res;

                if (res.ToString() == "1")
                {
                    return "true";
                }
                else
                {
                    return "false";
                }

            }

        }
        public void Delete(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "DELETE FROM Register"
                             + " WHERE Id = @id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Register Emp)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "UPDATE Register SET Name = @Name,"
                               + " Position = @Position, Age= @Age,Salary=@Salary"
                               + " WHERE ContactId = @ContactId";
                dbConnection.Open();
                dbConnection.Query(sQuery, Emp);
            }
        }
    }
}

