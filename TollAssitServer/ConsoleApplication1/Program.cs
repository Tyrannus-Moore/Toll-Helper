using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDbHandler;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            SqliteHandler handler = SqliteHandler.Instance("TollAssist.db");
            if (handler == null) 
            {
                Console.WriteLine("加载数据库TollAssist.db失败");
                Console.ReadLine();
                return;
            }
            System.Threading.Thread InsertCarTableThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InsertCarTableThreadFunc));
            InsertCarTableThread.IsBackground = true;
            InsertCarTableThread.Name = "InsertCarTableThread";

            System.Threading.Thread InsertCustomRecordThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InsertCustomRecordThreadFunc));
            InsertCustomRecordThread.IsBackground = true;
            InsertCustomRecordThread.Name = "InsertCustomRecordThread";

            System.Threading.Thread InsertNewPlatteThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InsertNewPlatteThreadFunc));
            InsertNewPlatteThread.IsBackground = true;
            InsertNewPlatteThread.Name = "InsertNewPlatteThread";

            System.Threading.Thread QueryCarTableThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(QueryCarTableThreadFunc));
            QueryCarTableThread.IsBackground = true;
            QueryCarTableThread.Name = "QueryCarTableThread";


            Console.WriteLine("输入任意键开始测试");

            InsertCarTableThread.Start(handler);
            InsertCustomRecordThread.Start(handler);
            InsertNewPlatteThread.Start(handler);
            QueryCarTableThread.Start(handler);


            Console.ReadLine();

        }

        static void InsertCarTableThreadFunc(object stat) 
        {
            SqliteHandler handler = stat as SqliteHandler;

            ASSISTICE.CarTable car = new ASSISTICE.CarTable();
            List<ASSISTICE.CarTable> cars = new List<ASSISTICE.CarTable>() { car };
            string error;
            while (true) 
            {
                car.platte = DateTime.Now.ToString("HHmmss");
                car.carclass = "测";
                car.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                try
                {
                    if (!handler.BatchAddCarTable(cars, out error))
                    {
                        Console.WriteLine("InsertCarTableThreadFunc()异常:{0}", error);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("调用异常{0}",ex.Message);
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        static void InsertCustomRecordThreadFunc(object stat)
        {
            SqliteHandler handler = stat as SqliteHandler;

            ASSISTICE.CustomRecord record = new ASSISTICE.CustomRecord();
            record.node = new ASSISTICE.TollNode();
            List<ASSISTICE.CustomRecord> records = new List<ASSISTICE.CustomRecord>() { record };
            string error;
            while (true)
            {
                record.platte = DateTime.Now.ToString("HHmmss");
                record.cartype = "测";
                record.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                try
                {
                    if (!handler.BatchAddCustomRecord(records, out error))
                    {
                        Console.WriteLine("InsertCustomRecordThreadFunc()异常:{0}", error);
                    }
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("调用异常{0}",ex.Message);
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        static void InsertNewPlatteThreadFunc(object stat)
        {
            SqliteHandler handler = stat as SqliteHandler;

            ASSISTICE.NewPlatte record = new ASSISTICE.NewPlatte();
            List<ASSISTICE.NewPlatte> records = new List<ASSISTICE.NewPlatte>() { record };
            string error;
            while (true)
            {
                record.platte = DateTime.Now.ToString("HHmmss");
                record.cartype = "测";
                record.dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                

                try
                {
                    if (!handler.BatchAddNewPlatte(records, out error))
                    {
                        Console.WriteLine("InsertNewPlatteThreadFunc()异常:{0}", error);
                    }
                }
                catch (Exception ex)
                {
                    
                     Console.WriteLine("调用异常{0}",ex.Message);
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        static void QueryCarTableThreadFunc(object stat)
        {
            SqliteHandler handler = stat as SqliteHandler;
            List<ASSISTICE.CarTable> records;
            string error;
            while (true)
            {
                //生成sqlite语法形式的sql语句
                string sqlite_query = string.Format("select * from cartable where platte='{0}' limit 1", DateTime.Now.ToString("HHmmss"));

                try
                {
                    if (handler.SearcherCarTable(sqlite_query, out error) == null)
                    {
                        Console.WriteLine("InsertNewPlatteThreadFunc()异常:{0}", error);
                    }
                }
                catch (Exception ex)
                {
                    
                   Console.WriteLine("调用异常{0}",ex.Message);
                }

                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
