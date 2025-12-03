using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RegIN_Kantuganov.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }

        public CorrectLogin HandlerCorrectLogin;
        public InCorrectLogin HandlerInCorrectLogin;

        public delegate void CorrectLogin();
        public delegate void InCorrectLogin();

        public void GetUserLogin(string login)
        {
            this.Id = -1;
            this.Login = String.Empty;
            this.Password = String.Empty;
            this.Name = String.Empty;
            this.Image = new byte[0];

            MySqlConnection connection = WorkingDB.OpenConnection();
            if (WorkingDB.OpenConnection(connection))
            {
                MySqlDataReader userData = WorkingDB.Query(
                    $"SELECT * FROM `users` WHERE `Login` = {Login}", connection);

                if (userData.HasRows)
                {
                    userData.Read();
                    this.Id = userData.GetInt32(0);
                    this.Login = userData.GetString(1);
                    this.Password = userData.GetString(2);
                    this.Name = userData.GetString(3);
                    if (userData.IsDBNull(4) == false)
                    {
                        this.Image = new byte[64 * 1024];
                        userData.GetBytes(4, 0, Image, 0, Image.Length);
                    }
                    this.DateUpdate = userData.GetDateTime(5);
                    this.DateCreate = userData.GetDateTime(6);

                    HandlerCorrectLogin.Invoke();
                }
                else HandlerInCorrectLogin.Invoke();
            }
            else HandlerInCorrectLogin.Invoke();
            WorkingDB.CloseConnection(connection);
        }

        public void Save() {
            MySqlConnection connection = WorkingDB.OpenConnection();
            if (WorkingDB.OpenConnection(connection))
            {
                MySqlCommand command = new MySqlCommand(
                    "INSERT INTO " +
                    "'users'(" +
                    "'Login', " +
                    "'Password', " +
                    "'Name', " +
                    "'Image', " +
                    "'DateUpdate', " +
                    "'DateCreate')" +
                    "VALUES (" +
                    "@Login, " +
                    "@Password, " +
                    "@Name, " +
                    "@Image, " +
                    "@DateUpdate, " +
                    "@DateCreate)", connection);
                command.Parameters.AddWithValue("@Login", this.Login);
                command.Parameters.AddWithValue("@Password", this.Password);
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@Image", this.Image);
                command.Parameters.AddWithValue("@DateUpdate", this.DateUpdate);
                command.Parameters.AddWithValue("@DateCreate", this.DateCreate);

                command.ExecuteNonQuery();
            }

            WorkingDB.CloseConnection(connection);
        }

        public void CreateNewPassword()
        {
            if (this.Login != String.Empty)
            {
                this.Password = CreatePassword();

                MySqlConnection connection = WorkingDB.OpenConnection();
                if (WorkingDB.OpenConnection(connection))
                {
                    WorkingDB.Query(
                        $"UPDATE " +
                        $"`users` " +
                        $"FROM " +
                        $"`Password`='{this.Password}` " +
                        $"WHERE " +
                        $"`Login`={this.Login};", connection);
                }

                WorkingDB.CloseConnection(connection);
                SendMail.SendMessage($"Your account password has been changed.\nNew password: {Password}", this.Login);
            }
        }

        private string CreatePassword()
        {
            List<char> NewPassword = new List<char>();
            Random random = new Random();
            char[] ArrNumbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            char[] ArrSymbols = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
            char[] ArrLowercase = {'q','w','e','r','t','y','u','i','o','p','a','s','d','f','g','h','j','k','l','z','x','c','v','b','n','m' };
            NewPassword.Add(ArrNumbers[random.Next(0, ArrNumbers.Length)]);
            NewPassword.Add(ArrSymbols[random.Next(0, ArrSymbols.Length)]);

            for (var i = 0; i < 2; i++)
                NewPassword.Add(char.ToUpper(ArrLowercase[random.Next(0, ArrLowercase.Length)]));
            for (var i = 0; i < 6; i++)
                NewPassword.Add(ArrLowercase[random.Next(0, ArrLowercase.Length)]);

            for (var i = 0; i < NewPassword.Count; i++)
            {
                int RandomSymbol = random.Next(0, NewPassword.Count);
                char Symbol = NewPassword[RandomSymbol];
                NewPassword[RandomSymbol] = NewPassword[i];
                NewPassword[i] = Symbol;
            }

            string NPassword = "";
            for (var i = 0; i < NewPassword.Count; i++)
                NPassword += NewPassword[i];

            return NPassword;
        }
    }
}
