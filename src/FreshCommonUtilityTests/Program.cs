using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Dapper;
using FreshCommonUtility.CoreModel;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.Security;
using FreshCommonUtility.SqlHelper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;

namespace FreshCommonUtilityTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Expression numA = Expression.Constant(6);
            //Console.WriteLine("NodeType: {0}, Type: {1}", numA.NodeType, numA.Type);
            //Expression numB = Expression.Constant(3);
            //Console.WriteLine("NodeType: {0}, Type: {1}", numB.NodeType, numB.Type);

            //BinaryExpression add = Expression.Add(numA, numB);
            //Console.WriteLine("NodeType: {0}, Type: {1}", add.NodeType, add.Type);

            //Console.WriteLine(add);
            //TimeCompare(UseTransReflection);
            //TimeCompare(JsonSerialize);
            //TimeCompare(LambdaExpressionV2);
            //TimeCompare(LambdaExpression);
            SetUp();
            RunTestSqlServer();

            SetupMySql();
            RunTestMySql();

            //var testStr = "FreshMan";
            //var enCodeStr = RsaHelper.RsaEncode(testStr);
            //var deCodeStr = RsaHelper.RsaDeCode(enCodeStr);
            //deCodeStr.IsEqualTo(testStr);

            var stopwatch = Stopwatch.StartNew();
            var pgtester = new TestClass();
            foreach (var method in typeof(TestClass).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var testwatch = Stopwatch.StartNew();
                Console.Write("Running " + method.Name + " in FreshCommonUtilityNetTest:");
                method.Invoke(pgtester, null);
                Console.WriteLine(" - OK! {0}ms", testwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.ReadKey();
        }

        /// <summary>
        /// SQL DbInit
        /// </summary>
        private static void SetUp()
        {
            using (var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=true"))
            {
                connection.Open();
                try
                {
                    connection.Execute(@" DROP DATABASE DapperSimpleCrudTestDb; ");
                }
                catch (Exception)
                { }

                connection.Execute(@" CREATE DATABASE DapperSimpleCrudTestDb; ");
            }

            using (var connection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog=DapperSimpleCrudTestDb;Integrated Security=True;MultipleActiveResultSets=true;"))
            {
                connection.Open();
                connection.Execute(@" create table Users (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime DEFAULT(getdate())) ");
                connection.Execute(@" create table Car (CarId int IDENTITY(1,1) not null, Id int null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table BigCar (CarId bigint IDENTITY(2147483650,1) not null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                connection.Execute(@" CREATE SCHEMA Log; ");
                connection.Execute(@" create table Log.CarLog (Id int IDENTITY(1,1) not null, LogNotes nvarchar(100) NOT NULL) ");
                connection.Execute(@" CREATE TABLE [dbo].[GUIDTest]([Id] [uniqueidentifier] NOT NULL,[name] [varchar](50) NOT NULL, CONSTRAINT [PK_GUIDTest] PRIMARY KEY CLUSTERED ([Id] ASC))");
                connection.Execute(@" create table StrangeColumnNames (ItemId int IDENTITY(1,1) not null Primary Key, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null)");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id int not null Primary Key, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(@" create table IgnoreColumns (Id int IDENTITY(1,1) not null Primary Key, IgnoreInsert nvarchar(100) null, IgnoreUpdate nvarchar(100) null, IgnoreSelect nvarchar(100)  null, IgnoreAll nvarchar(100) null) ");
                connection.Execute(@" CREATE TABLE GradingScale ([ScaleID] [int] IDENTITY(1,1) NOT NULL, [AppID] [int] NULL, [ScaleName] [nvarchar](50) NOT NULL, [IsDefault] [bit] NOT NULL)");
                connection.Execute(@" CREATE TABLE KeyMaster ([Key1] [int] NOT NULL, [Key2] [int] NOT NULL, CONSTRAINT [PK_KeyMaster] PRIMARY KEY CLUSTERED ([Key1] ASC, [Key2] ASC))");
            }
            Console.WriteLine("Created database");
        }

        /// <summary>
        /// Pg DbInit
        /// </summary>
        private static void SetupPg()
        {
            using (var connection = new NpgsqlConnection(String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", "localhost", "5432", "postgres", "postgrespass", "postgres")))
            {
                connection.Open();
                // drop  database 
                connection.Execute("DROP DATABASE IF EXISTS  testdb;");
                connection.Execute("CREATE DATABASE testdb  WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;");
            }
            System.Threading.Thread.Sleep(1000);

            using (var connection = new NpgsqlConnection(String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", "localhost", "5432", "postgres", "postgrespass", "testdb")))
            {
                connection.Open();
                connection.Execute(@" create table Users (Id SERIAL PRIMARY KEY, Name varchar not null, Age int not null, ScheduledDayOff int null, CreatedDate date not null default CURRENT_DATE) ");
                connection.Execute(@" create table Car (CarId SERIAL PRIMARY KEY, Id int null, Make varchar not null, Model varchar not null) ");
                connection.Execute(@" create table BigCar (CarId BIGSERIAL PRIMARY KEY, Make varchar not null, Model varchar not null) ");
                connection.Execute(@" alter sequence bigcar_carid_seq RESTART WITH 2147483650");
                connection.Execute(@" create table City (Name varchar not null, Population int not null) ");
                connection.Execute(@" CREATE SCHEMA Log; ");
                connection.Execute(@" create table Log.CarLog (Id SERIAL PRIMARY KEY, LogNotes varchar NOT NULL) ");
                connection.Execute(@" CREATE TABLE GUIDTest(Id uuid PRIMARY KEY,name varchar NOT NULL)");
                connection.Execute(@" create table StrangeColumnNames (ItemId Serial PRIMARY KEY, word varchar not null, colstringstrangeword varchar, keywordedproperty varchar) ");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id int PRIMARY KEY, Name varchar not null, Age int not null) ");

            }

        }

        /// <summary>
        /// MySQL DbInit
        /// </summary>
        private static void SetupMySql()
        {
            using (var connection = new MySqlConnection(
                $"Server={"localhost"};Port={"3306"};User Id={"FreshMan"};Password={"qinxianbo"};Database={"sys"};SslMode=None"))
            {
                connection.Open();
                // drop  database 
                connection.Execute("DROP DATABASE IF EXISTS testdb;");
                connection.Execute("CREATE DATABASE testdb;");
            }
            System.Threading.Thread.Sleep(1000);

            using (var connection = new MySqlConnection(
                $"Server={"localhost"};Port={"3306"};User Id={"FreshMan"};Password={"qinxianbo"};Database={"testdb"};SslMode=None"))
            {
                connection.Open();
                connection.Execute(@" create table Users (Id INTEGER PRIMARY KEY AUTO_INCREMENT, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime default current_timestamp ) ");
                connection.Execute(@" create table Car (CarId INTEGER PRIMARY KEY AUTO_INCREMENT, Id INTEGER null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table BigCar (CarId BIGINT PRIMARY KEY AUTO_INCREMENT, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" insert into BigCar (CarId,Make,Model) Values (2147483649,'car','car') ");
                connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                connection.Execute(@" CREATE TABLE GUIDTest(Id CHAR(38) NOT NULL,name varchar(50) NOT NULL, CONSTRAINT PK_GUIDTest PRIMARY KEY (Id ASC))");
                connection.Execute(@" create table StrangeColumnNames (ItemId INTEGER PRIMARY KEY AUTO_INCREMENT, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null) ");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id INTEGER PRIMARY KEY, Name nvarchar(100) not null, Age int not null) ");
                connection.Close();
            }

        }

        /// <summary>
        /// test sql server
        /// </summary>
        private static void RunTestSqlServer()
        {
            var stopwatch = Stopwatch.StartNew();
            var sqltester = new SimpleCrudTest(SimpleCRUD.Dialect.SQLServer);
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var testwatch = Stopwatch.StartNew();
                Console.Write($"Running {method.Name} in sql server");
                method.Invoke(sqltester, null);
                testwatch.Stop();
                Console.WriteLine($" -√ {testwatch.ElapsedMilliseconds}'ms ");
            }
            stopwatch.Stop();
            //write result
            Console.WriteLine($"Time elapsed :{stopwatch.Elapsed}");

            using (var connection = SqlConnectionHelper.GetOpenConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True"))
            {
                try
                {
                    connection.Execute(@" alter database DapperSimpleCrudTestDb set single_user with rollback immediate; DROP DATABASE DapperSimpleCrudTestDb; ");
                }
                catch (Exception)
                {
                }
            }
            Console.Write("SQL Server testing complete.");
        }

        /// <summary>
        /// test pg server
        /// </summary>
        private static void RunTestPg()
        {
            var stopwatch = Stopwatch.StartNew();
            var pgtester = new SimpleCrudTest(SimpleCRUD.Dialect.PostgreSQL);
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var testwatch = Stopwatch.StartNew();
                Console.Write("Running " + method.Name + " in PostgreSQL");
                method.Invoke(pgtester, null);
                Console.WriteLine(" - OK! {0}ms", testwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

            Console.Write("PostgreSQL testing complete.");
            Console.ReadKey();
        }

        private static void RunTestMySql()
        {
            var stopwatch = Stopwatch.StartNew();
            var mysqltester = new SimpleCrudTest(SimpleCRUD.Dialect.MySQL);
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                //skip schema tests
                if (method.Name.Contains("Schema")) continue;
                if (method.Name.Contains("Guid")) continue;
                if (method.Name.Contains("KeyMaster")) continue;
                if (method.Name.Contains("Ignore")) continue;
                var testwatch = Stopwatch.StartNew();
                Console.Write("Running " + method.Name + " in MySQL");
                method.Invoke(mysqltester, null);
                Console.WriteLine(" - OK! {0}ms", testwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

            Console.Write("MySQL testing complete.");
        }

        /// <summary>
        /// 模板调用
        /// </summary>
        /// <param name="function"></param>
        private static void TimeCompare(Action function)
        {
            var times = 1;
            var useTimes = 1;
            var timeWatch = new Stopwatch();
            timeWatch.Start();
            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < useTimes; j++)
                {
                    function();
                }
            }
            timeWatch.Stop();
            var time = timeWatch.ElapsedMilliseconds / times;
            Console.WriteLine($"{function.GetMethodInfo().Name} use time:{time} ms");
        }

        /// <summary>
        /// 反射拷贝对象
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="Tout"></typeparam>
        /// <param name="tIn"></param>
        /// <returns></returns>
        private static Tout TransReflection<TIn, Tout>(TIn tIn)
        {
            Tout tout = Activator.CreateInstance<Tout>();
            var tIntype = tIn.GetType();
            foreach (var itemout in tout.GetType().GetProperties())
            {
                var itemIn = tIntype.GetProperty(itemout.Name);
                if (itemIn != null)
                {
                    itemout.SetValue(tout, itemIn.GetValue(tIn));
                }
            }
            return tout;
        }

        /// <summary>
        /// 调用反射执行
        /// </summary>
        private static void UseTransReflection()
        {
            var u = new User { Age = 10, CreatedDate = DateTime.Now, Id = 100, Name = "FreshMan" };
            var t = TransReflection<User, User>(u);
        }

        /// <summary>
        /// 使用序列化的方式执行
        /// </summary>
        private static void JsonSerialize()
        {
            var u = new User { Age = 10, CreatedDate = DateTime.Now, Id = 100, Name = "FreshMan" };
            var t = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(u));
        }

        private static Dictionary<string, object> _Dic = new Dictionary<string, object>();

        /// <summary>
        /// Lambda表达式树拷贝
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tIn"></param>
        /// <returns></returns>
        private static TOut TransExp<TIn, TOut>(TIn tIn) where TOut : new() where TIn : new()
        {
            string key = $"trans_exp_{typeof(TIn).FullName}_{typeof(TOut).FullName}";
            if (!_Dic.ContainsKey(key))
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite) continue;
                    MemberExpression property = Expression.Property(parameterExpression,
                        typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)),
                    memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression,
                    new ParameterExpression[] { parameterExpression });
                Func<TIn, TOut> func = lambda.Compile();
                _Dic[key] = func;
            }
            return ((Func<TIn, TOut>)_Dic[key])(tIn);
        }

        /// <summary>
        /// Lambda表达式树拷贝
        /// </summary>
        private static void LambdaExpression()
        {
            var u = new User { Age = 10, CreatedDate = DateTime.Now, Id = 100, Name = "FreshMan" };
            var t = TransExp<User, User>(u);
        }

        /// <summary>
        /// 泛型优化之Lambda表达式树拷贝
        /// </summary>
        private static void LambdaExpressionV2()
        {
            //var u = new User { Age = 10, CreatedDate = DateTime.Now, Id = 100, Name = "FreshMan" };
            //var t = TransExpV2<User, User>.Trans(u);
            //var appSetting = new AppSettingsModel { EmailServerConfig = new EmailServerConfigModel { EmailSmtpServerAddress = "a", FromEmailAddress = "b", FromEmailPassword = "c", FromName = "d", PasswordEnabledSecret = false }, MySqlConnectionString = "e", RedisCaching = new RedisCaching { ConnectionString = "f", Enabled = false }, TestClasss = new List<Core.Model.TestClass> { new TestClass { name = "10" }, new TestClass { tests = "20" }, new TestClass { tests = "30" } } };
            //var t = TransExpV2<AppSettingsModel, AppSettingsModel>.Trans(appSetting);
            //appSetting.TestClasss.Clear();
        }
    }

    public static class TransExpV2<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = GetFunc();

        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite) continue;
                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)),
                memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, parameterExpression);
            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }
    }
}
