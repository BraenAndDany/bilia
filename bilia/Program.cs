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
using System.Runtime.Intrinsics.X86;


namespace IBauto
{
    internal class Program
    {
        public static string login="";
        public static string password="";
        public static string ID = "";
        public static bool b = false;
        public static bool c = false;
        public ushort secretKey = 0x0088;
        public static int TryLogin = 0;
        static void Main(string[] args)
        {
        //string[] loginBD = { };
        //string[] passwordBD = { };
        start:
            Console.Clear();
            Console.WriteLine("1)Авторизация");
            Console.WriteLine("2)Регистрация");
            Console.WriteLine("3)Удаление");
            int number = Convert.ToInt32(Console.ReadLine());
            switch (number)
            {
                case 1:
                    Login:
                    Console.Clear();
                    Console.WriteLine("Авторизация");
                    Console.WriteLine(" Введите логин");
                    login = Console.ReadLine();
                    Console.WriteLine(" Введите пароль");
                    password = Console.ReadLine();
                    password = CalcPassword(password);
                    try
                    {
                        ConnectBD();
                        if (TryLogin > 2)
                        {
                            Console.Clear() ;
                            Console.WriteLine("Несанкционированный доступ");
                            StreamWriter sw = new StreamWriter("log.txt", true);
                            sw.WriteLine("Превышено количество попыток входа в аккаунт {0}.", login);
                            sw.Close();
                        }
                        if(b==false&&TryLogin<=2)
                        {
                                TryLogin++;
                                goto Login;
                        }
                    }
                    catch (Exception e)
                    {
                        StreamWriter sw = new StreamWriter("log.txt", true);
                        sw.WriteLine("Exception: " + e.Message);
                        sw.Close();
                    }

                    if (c == true)
                    {
                        ConnectSA();
                    }
                    try
                    {
                        StreamWriter sw = new StreamWriter("log.txt", true);
                        sw.WriteLine("Попытка входа в аккаунт {0}. Результат {1}", login, b);
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Регистрация");
                    Console.WriteLine(" Введите логин");
                    login = Console.ReadLine();
                    Console.WriteLine(" Введите пароль");
                    password = Console.ReadLine();
                    password = CalcPassword(password);
                    try
                    {
                        RegistorBD();
                        StreamWriter sw = new StreamWriter("log.txt", true);
                        sw.WriteLine("Попытка создания в аккаунт {0}.", login);
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                        StreamWriter sw = new StreamWriter("log.txt", true);
                        sw.WriteLine("Exception: " + e.Message);
                        sw.Close();
                    }
                    finally
                    {
                        Console.WriteLine("Executing finally block.");
                    }
                    goto start;

                case 3:
                    Console.Clear();
                    Console.WriteLine("Введите логи и пароль админа");
                    Console.WriteLine(" Введите логин");
                    login = Console.ReadLine();
                    Console.WriteLine(" Введите пароль");
                    password = Console.ReadLine();
                    password = CalcPassword(password);
                    ConnectBD();
                    if (c == true)
                    {
                        Console.WriteLine("Удаление аккаунта");
                        Console.WriteLine("Введите ID аккаунта, который хотите удалить");
                        ID = Console.ReadLine();
                        if (ID == "1")
                        {
                            Console.WriteLine("Ошибка ввода, нельзя удалить админа");
                        }
                        else
                        {
                            try
                            {
                                DeleteLog();
                                StreamWriter sw = new StreamWriter("log.txt", true);
                                sw.WriteLine("Удаление аккаунта {0}.", login);
                                sw.Close();

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception: " + e.Message);
                                StreamWriter sw = new StreamWriter("log.txt", true);
                                sw.WriteLine("Exception: " + e.Message);
                                sw.Close();
                            }


                        }
                    }
                    else
                    {
                        Console.WriteLine("Неправильные данные");
                    }
                    goto start;


            }
        }

        private static void ConnectSA()
        {
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
                    Console.WriteLine("Логин: {0} | Пароль: {1}", str1, str2);
                }
            }
            finally
            {
                myReader.Close();
                myConnection.Close();
            }
        }

        public static void DeleteLog()
        {
            string connectionString = @"Data Source=DESKTOP-3G79H5S\SQLEXPRESS;Initial Catalog=IB;Integrated Security=True; TrustServerCertificate=true";

            string sqlExpression = $"DELETE FROM Auto WHERE ID='{ID}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                connection.Close();
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
                        if ("admin" == str1)
                        {
                            c = true;
                        }
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
            var result = SHA256.HashData(Encoding.ASCII.GetBytes(password));
            return result.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
        }
    }
}
