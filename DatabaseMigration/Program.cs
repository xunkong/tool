//using DatabaseMigration;

//Console.WriteLine("寻空数据库迁移工具");
//Console.WriteLine("适用版本 0.x -> 1.0.0");


//AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

//void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine("出现了错误");
//    Console.WriteLine((e.ExceptionObject as Exception)?.Message);
//    Console.ForegroundColor = ConsoleColor.Gray;
//    Console.WriteLine(e.ExceptionObject);
//    Console.WriteLine("按下回车键关闭程序");
//    Console.ReadLine();
//}


//var oldDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Xunkong\Data\XunkongData.db");

//if (File.Exists(DatabaseProvider.SqlitePath))
//{
//    Console.ForegroundColor = ConsoleColor.Yellow;
//    Console.WriteLine("迁移后的数据库已存在。。。");
//    Console.ForegroundColor = ConsoleColor.Green;
//    Console.WriteLine("按下回车键关闭程序");
//    Console.ReadLine();
//    return;
//}

//if (File.Exists(oldDbFile))
//{
//    Migration.Migrate();
//}
//else
//{
//    Console.ForegroundColor = ConsoleColor.Cyan;
//    Console.WriteLine("没有找到需要迁移的数据库");
//}

//Console.ForegroundColor = ConsoleColor.Green;
//Console.WriteLine("按下回车键关闭程序");
//Console.ReadLine();