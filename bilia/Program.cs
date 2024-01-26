using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Security.AccessControl;


namespace IBauto
{
    internal class Program
    {
        public static string login="";
        public static string password="";
        public static bool b = false;
        public ushort secretKey = 0x0088;
        static void Main(string[] args)
        {
        //string[] loginBD = { };
        //string[] passwordBD = { };
        start:
            Console.Clear();
            Console.WriteLine("1)Авторизация");
            Console.WriteLine("2)Регистрация");
            int number = Convert.ToInt32(Console.ReadLine());
            switch (number)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Авторизация");
                    Console.WriteLine(" Введите логин");
                    login = Console.ReadLine();
                    Console.WriteLine(" Введите пароль");
                    password = Console.ReadLine();
                    password = CalcPassword(password);
                    ConnectBD();
                    Console.WriteLine(b);
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Регистрация");
                    Console.WriteLine(" Введите логин");
                    login = Console.ReadLine();
                    Console.WriteLine(" Введите пароль");
                    password = Console.ReadLine();
                    password = CalcPassword(password);
                    RegistorBD();
                    goto start;
            }
        }
        public static void ConnectBD()
        {
            ///string myConnectionString = @"DataSource = DESKTOP-3G79H5S\SQLEXPRESS; InitialCatolog=IB;Trusted_Connection=True;TrustServerCertificate=true";
            string connectionString = @"Data Source=DESKTOP-3G79H5S\SQLEXPRESS;Initial Catalog=IB;Integrated Security=True; TrustServerCertificate=true";
            string mySelectQuery = "SELECT * FROM Auto";
            SqlConnection myConnection = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(mySelectQuery, myConnection);
            myConnection.Open();
            SqlDataReader myReader = myCommand.ExecuteReader();
            try
            {
                while (myReader.Read())
                {
                    string str1 = myReader.GetValue(1).ToString();
                    string str2 = myReader.GetValue(2).ToString();
                    //Console.WriteLine(str1);
                    //Console.WriteLine(str2);
                    if ((login == str1) && (password == str2))
                    {
                        b = true;
                        break;
                    }

                }
            }
            finally
            {
                myReader.Close();
                myConnection.Close();
            }
        }
        public static void RegistorBD()
        {
            ///string myConnectionString = @"DataSource = DESKTOP-3G79H5S\SQLEXPRESS; InitialCatolog=IB;Trusted_Connection=True;TrustServerCertificate=true";
            string connectionString = @"Data Source=DESKTOP-3G79H5S\SQLEXPRESS;Initial Catalog=IB;Integrated Security=True; TrustServerCertificate=true";
            string mySelectQuery = $@"INSERT INTO Auto (Login, Password) VALUES ('{login}', '{password}')";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(mySelectQuery, connection);
                int number = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static string CalcPassword(string password)
        {
            var result = MD5.HashData(Encoding.ASCII.GetBytes(password));
            return result.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
        }
    }
}
