using FileHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UpdateEarning2
{
    class Program
    {
        static void Main(string[] args)
        {
            #region hoilday dictionary
            bool flag3;
            Dictionary<string, DateTime> strs3 = new Dictionary<string, DateTime>();
            List<EarningData> earningDatas = new List<EarningData>();
            FileStream fileStream1 = new FileStream(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\holiday.txt", FileMode.Open);
            StreamReader streamReader1 = new StreamReader(fileStream1);
            do
            {
                string str1 = streamReader1.ReadLine();
                if (!(str1 == ""))
                {
                    strs3[str1] = DateTime.ParseExact(str1, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    flag3 = (str1 == null ? false : str1 != "");
                }
                else
                {
                    break;
                }
            } while (flag3);

            streamReader1.Close();
            streamReader1.Dispose();
            fileStream1.Close();
            fileStream1.Dispose();
            #endregion

           createEaringdate();
           Console.WriteLine("Earning data done!");

           ProcessStartInfo startInfo = new ProcessStartInfo();
           startInfo.FileName = @"C:\Program Files\R\R-3.0.3\bin\R.exe";
           startInfo.Arguments = "CMD BATCH \"C:\\Users\\Jiahai\\Downloads\\Download Yahoo Data for Earnings Project.R\"";
           Process cmd=Process.Start(startInfo);
           cmd.WaitForExit();
           Console.WriteLine("DB done!");

            createPerioddata();
            Console.WriteLine("E.csv done!");

            createADV();
            Console.WriteLine("adv done!");
            createQua();
            Console.WriteLine("period data update done!");
            createliveReport();
            Console.WriteLine("live earning data done!");
            DateTime lastDate = DateTime.Now;

            #region date thrshold

            if (lastDate.DayOfWeek == DayOfWeek.Sunday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-2).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-3);
                }
                else
                lastDate = lastDate.AddDays(-2);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Saturday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")) )
                {
                    lastDate = lastDate.AddDays(-2);
                }
                else
                    lastDate = lastDate.AddDays(-1);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Monday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-3).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-4);
                }
                    else
                lastDate = lastDate.AddDays(-3);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-4);
                }
                else
                    lastDate = lastDate.AddDays(-1);

            }
            else 
            {
                if (strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-2);
                }
                else
                    lastDate = lastDate.AddDays(-1);

            }

            #endregion

            readdaily2(new decimal(1, 0, 0, false, 2), 100000, lastDate.ToString("yyyy/MM/dd"));
        }

         static void createEaringdate()
        {
            FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(symbollist));
            symbollist[] symbollistArray = (symbollist[])fileHelperEngine.ReadFile(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\symbollist.csv");
            WebClient webClient = new WebClient();
            int num = 0;
            string str = "";
            int num1 = 1;
            symbollist[] symbollistArray1 = symbollistArray;
            for (int i = 0; i < (int)symbollistArray1.Length; i++)
            {
                symbollist _symbollist = symbollistArray1[i];
                if (num >= 400)
                {
                    str = str.Remove(0, 1);
                    webClient.Credentials = new NetworkCredential("CSTL_387287_SERVICES", "1DCUhct5I7Q8R4TP");
                    webClient.DownloadFile(string.Format("https://datadirect.factset.com/services/fastfetch?factlet=ExtractFormulaHistory&items=CS_EARN_REL_DATE(01/01/2014,NOW)&date=20140403&ids={0}&format=csv&cal=local", str), string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\UpdateEarning2\\UpdateEarning2\\bin\\Debug\\symbol\\symbollist{0}.csv", num1));
                    num = 0;
                    str = "";
                    Console.WriteLine(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\UpdateEarning2\\UpdateEarning2\\bin\\Debug\\symbol\\symbollist{0}.csv", num1));
                    num1++;
                }
                else
                {
                    str = string.Concat(str, ",", _symbollist.symbol);
                    num++;
                }
            }
            str = str.Remove(0, 1);
            webClient.Credentials = new NetworkCredential("CSTL_387287_SERVICES", "1DCUhct5I7Q8R4TP");
            webClient.DownloadFile(string.Format("https://datadirect.factset.com/services/fastfetch?factlet=ExtractFormulaHistory&items=CS_EARN_REL_DATE(01/01/2014,NOW)&date=20140403&ids={0}&format=csv&cal=local", str), string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\UpdateEarning2\\UpdateEarning2\\bin\\Debug\\symbol\\symbollist{0}.csv", num1));
            Program.combineCSV();
           // Console.WriteLine("Earingdate update done!");
        }

         static void combineCSV()
        {
            if (File.Exists(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv"))
            {
                File.Delete(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv");
            }
            string[] files = Directory.GetFiles(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\symbol\");
            for (int i = 0; i < (int)files.Length; i++)
            {
                string str = files[i];
                FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(symbollistCombine));
                symbollistCombine[] symbollistCombineArray = (symbollistCombine[])fileHelperEngine.ReadFile(str);
                IEnumerable<symbollistCombine> symbollistCombines = ((IEnumerable<symbollistCombine>)symbollistCombineArray).Where<symbollistCombine>((symbollistCombine d) => d.Earingdate != DateTime.Parse("1999/01/01"));
                fileHelperEngine.AppendToFile(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv", symbollistCombines);
            }
            if (File.Exists("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\updateEarning.csv"))
            {
                File.Delete("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\updateEarning.csv");
                File.Copy(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv", "C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\updateEarning.csv");
            }
            else
            {
                File.Copy(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv", "C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\updateEarning.csv");

            }
        }

         private static void createPerioddata()
         {
             string str;
             string str1;
             if (File.Exists(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\E.csv"))
             {
                 File.Delete(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\E.csv");
             }
             Dictionary<string, DateTime> strs = new Dictionary<string, DateTime>();
             List<earingData> earingDatas = new List<earingData>();
             FileStream fileStream = new FileStream(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\holiday.txt", FileMode.Open);
             StreamReader streamReader = new StreamReader(fileStream);
             do
             {
                 str = streamReader.ReadLine();
                 if (str == "")
                 {
                     break;
                 }
                 strs[str] = DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
             }
             while (str != null && str != "");
             FileStream fileStream1 = new FileStream(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\Earingdate.n.csv", FileMode.Open);
             StreamReader streamReader1 = new StreamReader(fileStream1);
             do
             {
                 str1 = streamReader1.ReadLine();
                 if (str1 == null)
                 {
                     break;
                 }
                 char[] chrArray = new char[] { ',' };
                 string[] strArrays = str1.Split(chrArray);
                 earingData earingDatum = new earingData();
                 earingData earingDatum1 = new earingData();
                 earingDatum.symbol = strArrays[0];
                 earingDatum1.symbol = strArrays[0];
                 DateTime dateTime = DateTime.Parse(strArrays[2]);
                 if (dateTime.DayOfWeek == DayOfWeek.Friday)
                 {
                     earingDatum.date = dateTime;
                     earingDatas.Add(earingDatum);
                     earingDatum1.quarter = earingDatum.quarter;
                     if (!strs.ContainsKey(dateTime.AddDays(3).ToString("yyyyMMdd")))
                     {
                         earingDatum1.date = dateTime.AddDays(3);
                         earingDatas.Add(earingDatum1);
                     }
                     else
                     {
                         earingDatum1.date = dateTime.AddDays(4);
                         earingDatas.Add(earingDatum1);
                     }
                 }
                 else if (dateTime.DayOfWeek == DayOfWeek.Monday || dateTime.DayOfWeek == DayOfWeek.Tuesday || dateTime.DayOfWeek == DayOfWeek.Wednesday)
                 {
                     earingDatum.date = dateTime;
                     earingDatas.Add(earingDatum);
                     earingDatum1.quarter = earingDatum.quarter;
                     if (!strs.ContainsKey(dateTime.AddDays(1).ToString("yyyyMMdd")))
                     {
                         earingDatum1.date = dateTime.AddDays(1);
                         earingDatas.Add(earingDatum1);
                     }
                     else
                     {
                         earingDatum1.date = dateTime.AddDays(2);
                         earingDatas.Add(earingDatum1);
                     }
                 }
                 else
                 {
                     if (dateTime.DayOfWeek != DayOfWeek.Thursday)
                     {
                         continue;
                     }
                     earingDatum.date = dateTime;
                     earingDatas.Add(earingDatum);
                     earingDatum1.quarter = earingDatum.quarter;
                     if (!strs.ContainsKey(dateTime.AddDays(1).ToString("yyyyMMdd")))
                     {
                         earingDatum1.date = dateTime.AddDays(1);
                         earingDatas.Add(earingDatum1);
                     }
                     else
                     {
                         earingDatum1.date = dateTime.AddDays(4);
                         earingDatas.Add(earingDatum1);
                     }
                 }
             }
             while (str1 != null && str1 != "");
             streamReader.Close();
             streamReader.Dispose();
             fileStream.Close();
             fileStream.Dispose();
             streamReader1.Close();
             streamReader1.Dispose();
             fileStream1.Close();
             fileStream1.Dispose();
             string[] files = Directory.GetFiles("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB");
             for (int i = 0; i < (int)files.Length; i++)
             {
                 string str2 = files[i];
                 try
                 {
                     if (str2 != "C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB\\SPY.csv")
                     {
                         FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(RowDailyData));
                         RowDailyData[] rowDailyDataArray = (RowDailyData[])fileHelperEngine.ReadFile(str2);
                         RowDailyData[] rowDailyDataArray1 = (RowDailyData[])fileHelperEngine.ReadFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB\\SPY.csv");
                        // RowDailyData[] rowDailyDataArray2 = rowDailyDataArray;
                         List<earingData> earingDatas1 = earingDatas;

                         var rowDailyDataArray2 = rowDailyDataArray.Where(d => d.date >= DateTime.Parse("2014/01/01"));


                         var dailyEaringDatas = from db in rowDailyDataArray2
                                               from e in earingDatas1
                                               where db.symbol == e.symbol && db.date == e.date
                                               select new DailyEaringData
                                               {
                                                   date=db.date,
                                                   close=db.close,
                                                   open=db.open,
                                                   high=db.high,
                                                   low=db.low,
                                                   qua=e.quarter,
                                                   symbol=db.symbol,
                                                   volume=db.Volume

                                               };

                       

                         //var variabl = (RowDailyData t) => new { symbol = t.symbol, date = t.date };
                         //var fun = (earingData x) => new { symbol = x.symbol, date = x.date };
                         //IEnumerable<DailyEaringData> dailyEaringDatas = ((IEnumerable<RowDailyData>)rowDailyDataArray2).Join(earingDatas1, variabl, fun, (RowDailyData t, earingData x) =>
                         //{
                         //    DailyEaringData dailyEaringDatum = new DailyEaringData();
                         //    dailyEaringDatum.symbol = t.symbol;
                         //    dailyEaringDatum.date = t.date;
                         //    dailyEaringDatum.volume = t.Volume;
                         //    return dailyEaringDatum;
                         //});
                         IEnumerable<DailyEaringData> dailyEaringDatas1 = dailyEaringDatas;
                         IOrderedEnumerable<DailyEaringData> dailyEaringDatas2 = dailyEaringDatas1.OrderBy<DailyEaringData, long>((DailyEaringData dd) => dd.volume);
                         DateTime dateTime1 = dailyEaringDatas2.Select<DailyEaringData, DateTime>((DailyEaringData dd) => dd.date).Last<DateTime>();
                         DateTime dateTime2 = new DateTime();
                         if (dateTime1.DayOfWeek == DayOfWeek.Monday)
                         {
                             dateTime2 = (!strs.ContainsKey(dateTime1.AddDays(-3).ToString("yyyyMMdd")) ? dateTime1.AddDays(-3) : dateTime1.AddDays(-4));
                         }
                         else if (dateTime1.DayOfWeek != DayOfWeek.Tuesday)
                         {
                             dateTime2 = (!strs.ContainsKey(dateTime1.AddDays(-1).ToString("yyyyMMdd")) ? dateTime1.AddDays(-1) : dateTime1.AddDays(-2));
                         }
                         else
                         {
                             dateTime2 = (!strs.ContainsKey(dateTime1.AddDays(-1).ToString("yyyyMMdd")) ? dateTime1.AddDays(-1) : dateTime1.AddDays(-4));
                         }
                         IEnumerable<RowDailyData> rowDailyDatas = rowDailyDataArray.Where<RowDailyData>((RowDailyData d) => d.date >= dateTime2);
                         IEnumerable<RowDailyData> rowDailyDatas1 = rowDailyDatas;
                         RowDailyData[] rowDailyDataArray3 = rowDailyDataArray1;
                         Func<RowDailyData, DateTime> func1 = (RowDailyData db) => db.date;
                         Func<RowDailyData, DateTime> func2 = (RowDailyData e) => e.date;
                         IEnumerable<DailyDataReport> dailyDataReports = rowDailyDatas1.Join<RowDailyData, RowDailyData, DateTime, DailyDataReport>((IEnumerable<RowDailyData>)rowDailyDataArray3, func1, func2, (RowDailyData db, RowDailyData e) =>
                         {
                             DailyDataReport dailyDataReport = new DailyDataReport();
                             dailyDataReport.Symbol = db.symbol;
                             dailyDataReport.Date = db.date;
                             dailyDataReport.Open = db.open;
                             dailyDataReport.High = db.high;
                             dailyDataReport.Low = db.low;
                             dailyDataReport.Close = db.close;
                             dailyDataReport.Volume = db.Volume;
                             dailyDataReport.Spyclose = e.close;
                             dailyDataReport.Spyopen = e.open;
                             dailyDataReport.Symbolspardtc2to = db.close / db.open;
                             dailyDataReport.Spyspardtc2to = e.close / e.open;
                             dailyDataReport.Betatc2to = ((db.close / db.open) / (e.close / e.open) - 1) * 100;
                             return dailyDataReport;
                         });
                         ObservableCollection<DailyDataReport> observableCollection = new ObservableCollection<DailyDataReport>();
                         string symbol = "";
                         decimal num = new decimal(0);
                         decimal spyclose = new decimal(0);
                         foreach (DailyDataReport dailyDataReport1 in dailyDataReports)
                         {
                             DailyDataReport close = new DailyDataReport();
                             if (dailyDataReport1.Symbol != symbol)
                             {
                                 symbol = dailyDataReport1.Symbol;
                                 num = dailyDataReport1.Close;
                                 spyclose = dailyDataReport1.Spyclose;
                             }
                             else
                             {
                                 close = dailyDataReport1;
                                 close.Symbolspardtc2yc = dailyDataReport1.Close / num;
                                 close.Spyspardtc2yc = dailyDataReport1.Spyclose / spyclose;
                                 close.Betatc2yc = (close.Symbolspardtc2yc / close.Spyspardtc2yc-1)*100;
                                 close.Symbolspardto2yc = dailyDataReport1.Open / num;
                                 close.Spyspardto2yc = dailyDataReport1.Spyopen / spyclose;
                                 close.Betato2yc = (close.Symbolspardto2yc / close.Spyspardto2yc-1)*100;
                                 observableCollection.Add(close);
                                 num = dailyDataReport1.Close;
                                 spyclose = dailyDataReport1.Spyclose;
                             }
                         }
                         (new FileHelperEngine(typeof(DailyDataReport))).AppendToFile(@"C:\Users\Jiahai\Documents\Visual Studio 2012\Projects\UpdateEarning2\UpdateEarning2\bin\Debug\E.csv", observableCollection);
                     }
                 }
                 catch
                 {
                 }
             }
         }



         private static void createADV()
         {
             bool flag;
             bool flag1;
             string[] files = Directory.GetFiles("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB");
             for (int i = 0; i < (int)files.Length; i++)
             {
                 string str = files[i];
                 char[] chrArray = new char[] { '\\' };
                 string[] strArrays = str.Split(chrArray);
                 FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(RowDailyData));
                 RowDailyData[] rowDailyDataArray = (RowDailyData[])fileHelperEngine.ReadFile(str);
                 Queue<long> nums = new Queue<long>();
                 List<DailyData1> dailyData1s = new List<DailyData1>();
                 int num = -1;
                 decimal num1 = new decimal(0);
                 decimal num2 = new decimal(0);
                 bool flag2 = true;
                 decimal num3 = new decimal(0);
                 bool flag3 = true;
                 RowDailyData[] rowDailyDataArray1 = rowDailyDataArray;
                 for (int j = 0; j < (int)rowDailyDataArray1.Length; j++)
                 {
                     RowDailyData rowDailyDatum = rowDailyDataArray1[j];
                     DailyData1 dailyData1 = new DailyData1();
                     if (flag2)
                     {
                         flag2 = false;
                     }
                     else if (!(num2 == new decimal(0)))
                     {
                         num3 = rowDailyDatum.adjusted / num2;
                         flag1 = (num3 > new decimal(49) ? false : !(num3 < new decimal(2, 0, 0, false, 2)));
                         if (!flag1)
                         {
                             flag3 = false;
                             break;
                         }
                     }
                     else
                     {
                         flag3 = false;
                         break;
                     }
                     num2 = rowDailyDatum.adjusted;
                     if (rowDailyDatum.close != new decimal(0))
                     {
                         num1 = rowDailyDatum.adjusted / rowDailyDatum.close;
                     }
                     flag = (num1 <= new decimal(6, 0, 0, false, 1) ? false : !(num1 >= new decimal(14, 0, 0, false, 1)));
                     if (flag)
                     {
                         dailyData1.open = rowDailyDatum.open;
                         dailyData1.high = rowDailyDatum.high;
                         dailyData1.low = rowDailyDatum.low;
                         dailyData1.close = rowDailyDatum.close;
                     }
                     else
                     {
                         dailyData1.open = rowDailyDatum.open * num1;
                         dailyData1.high = rowDailyDatum.high * num1;
                         dailyData1.low = rowDailyDatum.low * num1;
                         dailyData1.close = rowDailyDatum.close * num1;
                     }
                     dailyData1.symbol = rowDailyDatum.symbol;
                     dailyData1.date = rowDailyDatum.date;
                     dailyData1.volume = rowDailyDatum.Volume;
                     if (nums.Count == 20)
                     {
                         num = (int)nums.Average();
                         nums.Dequeue();
                         nums.Enqueue(rowDailyDatum.Volume);
                     }
                     else
                     {
                         nums.Enqueue(rowDailyDatum.Volume);
                     }
                     dailyData1.adv = num;
                     dailyData1s.Add(dailyData1);
                 }
                 if (flag3)
                 {
                     FileHelperEngine fileHelperEngine1 = new FileHelperEngine(typeof(DailyData1));
                     fileHelperEngine1.WriteFile(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DBadv\\{0}", strArrays[12]), dailyData1s);
                 }
             }
         }

         private static void createQua()
         {
             IEnumerable<DailyData1> dailyData1s;
             int num = 2014;
             int num1 = 2014;
             string[] strArrays = new string[] { "0101", "0401", "0701", "1001" };
             string[] strArrays1 = strArrays;
             strArrays = new string[] { "0501", "0801", "1101", "0201" };
             string[] strArrays2 = strArrays;
             while (num <= num1)
             {
                 for (int i = 0; i < 4; i++)
                 {
                     if (File.Exists((string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DailyQua\\{0}{1}.csv", num, i + 1))))
                     {
                         File.Delete(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DailyQua\\{0}{1}.csv", num, i + 1));
                     }
                     string[] files = Directory.GetFiles("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DBadv");
                     for (int j = 0; j < (int)files.Length; j++)
                     {
                         string str = files[j];
                         FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(DailyData1));
                         DailyData1[] dailyData1Array = (DailyData1[])fileHelperEngine.ReadFile(str);
                         if (i == 3)
                         {
                             DailyData1[] dailyData1Array1 = dailyData1Array;
                             IEnumerable<DailyData1> dailyData1s1 = ((IEnumerable<DailyData1>)dailyData1Array1).Where<DailyData1>((DailyData1 d) =>
                             {
                                 bool flag = d.date >= DateTime.ParseExact(string.Format("{0}{1}", num, strArrays1[i]), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                                 return flag;
                             });
                             dailyData1s = dailyData1s1.Where<DailyData1>((DailyData1 d) =>
                             {
                                 bool flag = d.date < DateTime.ParseExact(string.Format("{0}{1}", num + 1, strArrays2[i]), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                                 return flag;
                             });
                             fileHelperEngine.AppendToFile(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DailyQua\\{0}{1}.csv", num, i + 1), dailyData1s);
                         }
                         else
                         {
                             DailyData1[] dailyData1Array2 = dailyData1Array;
                             IEnumerable<DailyData1> dailyData1s2 = ((IEnumerable<DailyData1>)dailyData1Array2).Where<DailyData1>((DailyData1 d) =>
                             {
                                 bool flag = d.date >= DateTime.ParseExact(string.Format("{0}{1}", num, strArrays1[i]), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                                 return flag;
                             });
                             dailyData1s = dailyData1s2.Where<DailyData1>((DailyData1 d) =>
                             {
                                 bool flag = d.date < DateTime.ParseExact(string.Format("{0}{1}", num, strArrays2[i]), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                                 return flag;
                             });
                             fileHelperEngine.AppendToFile(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DailyQua\\{0}{1}.csv", num, i + 1), dailyData1s);
                         }
                     }
                 }
                 num++;
             }
         }

         private static void createliveReport()
         {
             bool flag2;
             bool flag3;
             bool flag4;
             if (File.Exists("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\finalall.csv"))
             {
                 File.Delete("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\finalall.csv");
             }
             Dictionary<string, DateTime> strs3 = new Dictionary<string, DateTime>();
             List<EarningData> earningDatas = new List<EarningData>();
             FileStream fileStream = new FileStream("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\holiday.txt", FileMode.Open);
             StreamReader streamReader = new StreamReader(fileStream);
             do
             {
                 string str1 = streamReader.ReadLine();
                 if (!(str1 == ""))
                 {
                     strs3[str1] = DateTime.ParseExact(str1, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                     flag2 = (str1 == null ? false : str1 != "");
                 }
                 else
                 {
                     break;
                 }
             }
             while (flag2);
             FileStream fileStream1 = new FileStream("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\updateEarning.csv", FileMode.Open);
             StreamReader streamReader1 = new StreamReader(fileStream1);
             do
             {
                 string str2 = streamReader1.ReadLine();
                 if (str2 != null)
                 {
                     char[] chrArray = new char[] { ',' };
                     string[] strArrays = str2.Split(chrArray);
                     EarningData earningDatum = new EarningData();
                     EarningData earningDatum1 = new EarningData();
                     earningDatum.symbol = strArrays[0];
                     earningDatum1.symbol = strArrays[0];
                     DateTime dateTime1 = DateTime.Parse(strArrays[2]);
                     if (dateTime1.DayOfWeek != DayOfWeek.Friday)
                     {
                         flag3 = (dateTime1.DayOfWeek == DayOfWeek.Monday || dateTime1.DayOfWeek == DayOfWeek.Tuesday ? false : dateTime1.DayOfWeek != DayOfWeek.Wednesday);
                         if (!flag3)
                         {
                             earningDatum.date = dateTime1;
                             earningDatas.Add(earningDatum);
                             if (!strs3.ContainsKey(dateTime1.AddDays(1).ToString("yyyyMMdd")))
                             {
                                 earningDatum1.date = dateTime1.AddDays(1);
                                 earningDatas.Add(earningDatum1);
                             }
                             else
                             {
                                 earningDatum1.date = dateTime1.AddDays(2);
                                 earningDatas.Add(earningDatum1);
                             }
                         }
                         else if (dateTime1.DayOfWeek == DayOfWeek.Thursday)
                         {
                             earningDatum.date = dateTime1;
                             earningDatas.Add(earningDatum);
                             if (!strs3.ContainsKey(dateTime1.AddDays(1).ToString("yyyyMMdd")))
                             {
                                 earningDatum1.date = dateTime1.AddDays(1);
                                 earningDatas.Add(earningDatum1);
                             }
                             else
                             {
                                 earningDatum1.date = dateTime1.AddDays(4);
                                 earningDatas.Add(earningDatum1);
                             }
                         }
                     }
                     else
                     {
                         earningDatum.date = dateTime1;
                         earningDatas.Add(earningDatum);
                         if (!strs3.ContainsKey(dateTime1.AddDays(3).ToString("yyyyMMdd")))
                         {
                             earningDatum1.date = dateTime1.AddDays(3);
                             earningDatas.Add(earningDatum1);
                         }
                         else
                         {
                             earningDatum1.date = dateTime1.AddDays(4);
                             earningDatas.Add(earningDatum1);
                         }
                     }
                     flag4 = (str2 == null ? false : str2 != "");
                 }
                 else
                 {
                     break;
                 }
             }
             while (flag4);
             streamReader.Close();
             streamReader.Dispose();
             fileStream.Close();
             fileStream.Dispose();
             streamReader1.Close();
             streamReader1.Dispose();
             fileStream1.Close();
             fileStream1.Dispose();
             string[] files = Directory.GetFiles("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB");
             for (int i = 0; i < (int)files.Length; i++)
             {
                 string str3 = files[i];
                 try
                 {
                     if (str3 != "C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB\\SPY.csv")
                     {
                         FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(RowDailyData));
                         RowDailyData[] rowDailyDataArray = (RowDailyData[])fileHelperEngine.ReadFile(str3);
                         RowDailyData[] rowDailyDataArray1 = (RowDailyData[])fileHelperEngine.ReadFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\earingdataProj\\earingdataProj\\bin\\Debug\\Daily\\DB\\SPY.csv");
                         // RowDailyData[] rowDailyDataArray2 = rowDailyDataArray;
                         List<EarningData> earningDatas1 = earningDatas;

                         var rowDailyDataArray2 = rowDailyDataArray.Where(d => d.date > DateTime.Parse("2014/01/01"));

                         var dailyDatas = from db in rowDailyDataArray2
                                          from e in earningDatas1
                                          where db.symbol == e.symbol && db.date == e.date
                                          select new DailyData
                                          {
                                              date = db.date,
                                              close = db.close,
                                              open = db.open,
                                              high = db.high,
                                              low = db.low,
                                              qua = e.que,
                                              symbol = db.symbol,
                                              volume = db.Volume

                                          };


                         //var func = (RowDailyData t) =>
                         //{
                         //    var variable = new { symbol = t.symbol, date = t.date };
                         //    return variable;
                         //};
                         //var func1 = (EarningData x) =>
                         //{
                         //    var variable = new { symbol = x.symbol, date = x.date };
                         //    return variable;
                         //};
                         //IEnumerable<DailyData> dailyDatas = ((IEnumerable<RowDailyData>)rowDailyDataArray2).Join(earningDatas1, func, func1, (RowDailyData t, EarningData x) =>
                         //{
                         //    DailyData dailyDatum = new DailyData();
                         //    dailyDatum.symbol = t.symbol;
                         //    dailyDatum.open = t.open;
                         //    dailyDatum.date = t.date;
                         //    dailyDatum.high = t.high;
                         //    dailyDatum.low = t.low;
                         //    dailyDatum.close = t.close;
                         //    dailyDatum.volume = t.Volume;
                         //    DailyData dailyDatum1 = dailyDatum;
                         //    return dailyDatum1;
                         //});
                         IEnumerable<DailyData> dailyDatas1 = dailyDatas;
                         IOrderedEnumerable<DailyData> dailyDatas2 = dailyDatas1.OrderBy<DailyData, long>((DailyData dd) =>
                         {
                             long num = dd.volume;
                             return num;
                         });
                         DateTime dateTime2 = dailyDatas2.Select<DailyData, DateTime>((DailyData dd) =>
                         {
                             DateTime dateTime = dd.date;
                             return dateTime;
                         }).Last<DateTime>();
                         DateTime dateTime3 = dateTime2;
                         for (int j = 0; j < 252; j++)
                         {
                             if (dateTime3.DayOfWeek == DayOfWeek.Friday)
                             {
                                 dateTime3 = (!strs3.ContainsKey(dateTime3.AddDays(-1).ToString("yyyyMMdd")) ? dateTime3.AddDays(-1) : dateTime3.AddDays(-2));
                             }
                             else if (dateTime3.DayOfWeek == DayOfWeek.Monday)
                             {
                                 dateTime3 = (!strs3.ContainsKey(dateTime3.AddDays(-3).ToString("yyyyMMdd")) ? dateTime3.AddDays(-3) : dateTime3.AddDays(-4));
                             }
                             else if (dateTime3.DayOfWeek == DayOfWeek.Tuesday)
                             {
                                 dateTime3 = (!strs3.ContainsKey(dateTime3.AddDays(-1).ToString("yyyyMMdd")) ? dateTime3.AddDays(-1) : dateTime3.AddDays(-4));
                             }
                             else if (dateTime3.DayOfWeek == DayOfWeek.Thursday)
                             {
                                 dateTime3 = (!strs3.ContainsKey(dateTime3.AddDays(-1).ToString("yyyyMMdd")) ? dateTime3.AddDays(-1) : dateTime3.AddDays(-2));
                             }
                             else if (dateTime3.DayOfWeek == DayOfWeek.Wednesday)
                             {
                                 dateTime3 = (!strs3.ContainsKey(dateTime3.AddDays(-1).ToString("yyyyMMdd")) ? dateTime3.AddDays(-1) : dateTime3.AddDays(-2));
                             }
                         }
                         IEnumerable<RowDailyData> rowDailyDatas9 = rowDailyDataArray.Where<RowDailyData>((RowDailyData d) =>
                         {
                             bool flag = d.date <= dateTime2;
                             return flag;
                         });
                         IEnumerable<RowDailyData> rowDailyDatas10 = rowDailyDatas9.OrderByDescending<RowDailyData, DateTime>((RowDailyData d) =>
                         {
                             DateTime dateTime = d.date;
                             return dateTime;
                         }).Take<RowDailyData>(2);
                         IEnumerable<RowDailyData> rowDailyDatas11 = rowDailyDatas10;
                         var groupings = rowDailyDatas11.GroupBy((RowDailyData data) =>
                         {
                             var variable = new { symbo = data.symbol };
                             return variable;
                         });
                         IEnumerable<DailyData3> dailyData3s = groupings.Select((d) =>
                         {
                             DailyData3 dailyData3 = new DailyData3();
                             dailyData3.symbol = d.Key.symbo;
                             DailyData3 dailyData31 = dailyData3;
                             var collection = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas = collection.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData31.close = rowDailyDatas.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.close;
                                 return num;
                             }).Last<decimal>();
                             DailyData3 dailyData32 = dailyData3;
                             var collection1 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas1 = collection1.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData32.open = rowDailyDatas1.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.open;
                                 return num;
                             }).Last<decimal>();
                             DailyData3 dailyData33 = dailyData3;
                             var collection2 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas2 = collection2.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData33.high = rowDailyDatas2.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.high;
                                 return num;
                             }).Last<decimal>();
                             DailyData3 dailyData34 = dailyData3;
                             var collection3 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas3 = collection3.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData34.low = rowDailyDatas3.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.low;
                                 return num;
                             }).Last<decimal>();
                             DailyData3 dailyData35 = dailyData3;
                             var collection4 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas4 = collection4.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData35.volume = rowDailyDatas4.Select<RowDailyData, long>((RowDailyData dd) =>
                             {
                                 long volume = dd.Volume;
                                 return volume;
                             }).Last<long>();
                             DailyData3 dailyData36 = dailyData3;
                             var collection5 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas5 = collection5.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             decimal num1 = rowDailyDatas5.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.open;
                                 return num;
                             }).Last<decimal>();
                             var collection6 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas6 = collection6.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             decimal num2 = num1 - rowDailyDatas6.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.close;
                                 return num;
                             }).First<decimal>();
                             var collection7 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas7 = collection7.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData36.threshold = num2 / rowDailyDatas7.Select<RowDailyData, decimal>((RowDailyData dd) =>
                             {
                                 decimal num = dd.close;
                                 return num;
                             }).First<decimal>();
                             DailyData3 dailyData37 = dailyData3;
                             var collection8 = d;
                             IOrderedEnumerable<RowDailyData> rowDailyDatas8 = collection8.OrderBy<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             });
                             dailyData37.date = rowDailyDatas8.Select<RowDailyData, DateTime>((RowDailyData dd) =>
                             {
                                 DateTime dateTime = dd.date;
                                 return dateTime;
                             }).Last<DateTime>();
                             DailyData3 dailyData38 = dailyData3;
                             return dailyData38;
                         });
                         IOrderedEnumerable<DailyData3> dailyData3s1 = dailyData3s.OrderBy<DailyData3, string>((DailyData3 data) =>
                         {
                             string str = data.symbol;
                             return str;
                         });
                         IOrderedEnumerable<DailyData3> dailyData3s2 = dailyData3s1.ThenBy<DailyData3, int>((DailyData3 data) =>
                         {
                             int num = data.qua;
                             return num;
                         });
                         IEnumerable<RowDailyData> rowDailyDatas12 = rowDailyDataArray.Where<RowDailyData>((RowDailyData d) =>
                         {
                             bool flag;
                             flag = (d.date >= dateTime2 ? false : d.date >= dateTime3);
                             bool flag1 = flag;
                             return flag1;
                         });
                         IEnumerable<IGrouping<string, RowDailyData>> groupings1 = rowDailyDatas12.GroupBy<RowDailyData, string>((RowDailyData d) =>
                         {
                             string str = d.symbol;
                             return str;
                         });
                         var collection9 = groupings1.Select((IGrouping<string, RowDailyData> da) =>
                         {
                             string key = da.Key;
                             IGrouping<string, RowDailyData> strs = da;
                             decimal num1 = strs.Max<RowDailyData>((RowDailyData d) =>
                             {
                                 decimal num = d.high;
                                 return num;
                             });
                             IGrouping<string, RowDailyData> strs1 = da;
                             decimal num2 = strs1.Min<RowDailyData>((RowDailyData d) =>
                             {
                                 decimal num = d.low;
                                 return num;
                             });
                             IGrouping<string, RowDailyData> strs2 = da;
                             var variable = new
                             {
                                 symbol = key,
                                 Maxhigh = num1,
                                 minclose = num2,
                                 date252 = strs2.Min<RowDailyData, DateTime>((RowDailyData d) =>
                                 {
                                     DateTime dateTime = d.date;
                                     return dateTime;
                                 })
                             };
                             return variable;
                         });
                         IOrderedEnumerable<DailyData3> dailyData3s3 = dailyData3s2;
                         var collection10 = collection9;

                         var unionearingdates = from t in dailyData3s3
                                                join x in collection10 on t.symbol equals x.symbol
                                                select new unionearingdate
                                                {
                                                    symbol = t.symbol,
                                                    open = t.open,
                                                    date = t.date,
                                                    high = t.high,
                                                    low = t.low,
                                                    close = t.close,
                                                    volume = t.volume,
                                                    qua = t.qua,
                                                    threshold = t.threshold,
                                                    maxhigh = x.Maxhigh,
                                                    minlow = x.minclose,
                                                    week52date = x.date252,
                                                };




                         //var func2 = (DailyData3 t) =>
                         //{
                         //    var variable = new { symbol = t.symbol };
                         //    return variable;
                         //};
                         //var func3 = (x) =>
                         //{
                         //    var variable = new { symbol = x.symbol };
                         //    return variable;
                         //};
                         //IEnumerable<unionearingdate> unionearingdates = dailyData3s3.Join(collection10, func2, func3, (t, x) =>
                         //{
                         //    unionearingdate _unionearingdate = new unionearingdate();
                         //    _unionearingdate.symbol = t.symbol;
                         //    _unionearingdate.open = t.open;
                         //    _unionearingdate.date = t.date;
                         //    _unionearingdate.high = t.high;
                         //    _unionearingdate.low = t.low;
                         //    _unionearingdate.close = t.close;
                         //    _unionearingdate.volume = t.volume;
                         //    _unionearingdate.qua = t.qua;
                         //    _unionearingdate.threshold = t.threshold;
                         //    _unionearingdate.maxhigh = x.Maxhigh;
                         //    _unionearingdate.minlow = x.minclose;
                         //    _unionearingdate.week52date = x.date252;
                         //    unionearingdate _unionearingdate1 = _unionearingdate;
                         //    return _unionearingdate1;
                         //});
                         (new FileHelperEngine(typeof(unionearingdate))).AppendToFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\finalall.csv", unionearingdates);
                     }
                 }
                 catch
                 {
                 }
             }
         }

         private static void readdaily2(decimal threshold, int buyinpower, string date)
         {
             bool flag2;
             bool flag3;
             bool flag4;
             bool flag5;
             bool flag6;
             bool flag7;
             bool flag8;
             bool flag9;
             bool flag10;
             TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
             string str1 = "20140401";
             Dictionary<string, string> strs = new Dictionary<string, string>();
             FileStream fileStream = new FileStream("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\bio.csv", FileMode.Open);
             StreamReader streamReader = new StreamReader(fileStream);
             streamReader.BaseStream.Seek((long)0, SeekOrigin.Begin);
             string str2 = streamReader.ReadLine();
             while (true)
             {
                 flag2 = (str2 == null ? false : str2 != "");
                 if (!flag2)
                 {
                     break;
                 }
                 strs[str2] = str2;
                 str2 = streamReader.ReadLine();
             }
             streamReader.Close();
             streamReader.Dispose();
             fileStream.Close();
             fileStream.Dispose();
             FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(DailyData1));
             //FileHelperEngine fileHelperEngine1 = new FileHelperEngine(typeof(DailyDataReport));
             DailyData1[] dailyData1Array = (DailyData1[])fileHelperEngine.ReadFile(string.Format("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\DailyQua\\20141.csv", new object[0]));
             FileHelperEngine fileHelperEngine2 = new FileHelperEngine(typeof(unionearingdate));
             unionearingdate[] unionearingdateArray = (unionearingdate[])fileHelperEngine2.ReadFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\finalall.csv");
             IEnumerable<unionearingdate> unionearingdates1 = unionearingdateArray.Where<unionearingdate>((unionearingdate data) =>
             {
                 bool flag = data.date >= DateTime.ParseExact(str1, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                 return flag;
             }).Where<unionearingdate>((unionearingdate data) =>
             {
                 bool flag;
                 if (Math.Abs(data.threshold) < threshold)
                 {
                     flag = false;
                 }
                 else if (!(data.threshold > new decimal(0)) || !(data.high >= data.maxhigh))
                 {
                     flag = (data.threshold >= new decimal(0) ? false : data.low <= data.minlow);
                 }
                 else
                 {
                     flag = true;
                 }
                 bool flag1 = flag;
                 return flag1;
             });
             fileHelperEngine2.WriteFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\test1.csv", unionearingdates1.Where<unionearingdate>((unionearingdate dd) =>
             {
                 bool flag = dd.high >= new decimal(5);
                 return flag;
             }));
             unionearingdate[] unionearingdateArray1 = (unionearingdate[])fileHelperEngine2.ReadFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\test1.csv");
             List<unionearingdate> unionearingdates2 = new List<unionearingdate>();
             unionearingdate[] unionearingdateArray2 = unionearingdateArray1;
             for (int i = 0; i < (int)unionearingdateArray2.Length; i++)
             {
                 unionearingdate _unionearingdate = unionearingdateArray2[i];
                 if (!strs.ContainsKey(_unionearingdate.symbol))
                 {
                     unionearingdates2.Add(_unionearingdate);
                 }
             }
             DailyData1[] dailyData1Array1 = dailyData1Array;
             Func<DailyData1, IEnumerable<unionearingdate>> func = (DailyData1 db) =>
             {
                 IEnumerable<unionearingdate> unionearingdates = unionearingdates2;
                 return unionearingdates;
             };
             var collection = ((IEnumerable<DailyData1>)dailyData1Array1).SelectMany(func, (DailyData1 db, unionearingdate e) =>
             {
                 var variable = new { db = db, e = e };
                 return variable;
             });
             var collection1 = collection.Where((argument0) =>
             {
                 bool flag;
                 flag = (argument0.db.symbol != argument0.e.symbol ? false : argument0.db.date >= argument0.e.date);
                 bool flag1 = flag;
                 return flag1;
             });
             IEnumerable<DailyDataout> dailyDataouts = collection1.Select((argument1) =>
             {
                 DailyDataout dailyDataout = new DailyDataout();
                 dailyDataout.symbol = argument1.db.symbol;
                 dailyDataout.date = argument1.db.date;
                 dailyDataout.open = argument1.db.open;
                 dailyDataout.high = argument1.db.high;
                 dailyDataout.low = argument1.db.low;
                 dailyDataout.close = argument1.db.close;
                 dailyDataout.volume = argument1.db.volume;
                 dailyDataout.threshold = argument1.e.threshold;
                 dailyDataout.adv = argument1.db.adv;
                 DailyDataout dailyDataout1 = dailyDataout;
                 return dailyDataout1;
             });
             DailyData1[] dailyData1Array2 = dailyData1Array;
             IEnumerable<DailyData1> dailyData1s = ((IEnumerable<DailyData1>)dailyData1Array2).Where<DailyData1>((DailyData1 d) =>
             {
                 bool flag = d.symbol == "SPY";
                 return flag;
             });
             IEnumerable<DailyDataout> dailyDataouts1 = dailyDataouts;
             IEnumerable<DailyData1> dailyData1s1 = dailyData1s;

             var dailyDataouts2 = from db in dailyDataouts1
                                  join x in dailyData1s on db.date equals x.date
                                  select new DailyDataout
                                  {
                                      symbol = db.symbol,
                                      date = db.date,
                                      open = db.open,
                                      high = db.high,
                                      low = db.low,
                                      close = db.close,
                                      volume = db.volume,
                                      threshold = db.threshold,
                                      adv = db.adv,
                                      spyclose = x.close,
                                      spyopen = x.open,
                                  };

             //var func1 = (DailyDataout db) =>
             //{
             //    var variable = new { date = db.date };
             //    return variable;
             //};
             //var func2 = (DailyData1 x) =>
             //{
             //    var variable = new { date = x.date };
             //    return variable;
             //};
             //IEnumerable<DailyDataout> dailyDataouts2 = dailyDataouts1.Join(dailyData1s1, func1, func2, (DailyDataout db, DailyData1 x) =>
             //{
             //    DailyDataout dailyDataout = new DailyDataout();
             //    dailyDataout.symbol = db.symbol;
             //    dailyDataout.date = db.date;
             //    dailyDataout.open = db.open;
             //    dailyDataout.high = db.high;
             //    dailyDataout.low = db.low;
             //    dailyDataout.close = db.close;
             //    dailyDataout.volume = db.volume;
             //    dailyDataout.threshold = db.threshold;
             //    dailyDataout.adv = db.adv;
             //    dailyDataout.spyclose = x.close;
             //    dailyDataout.spyopen = x.open;
             //    DailyDataout dailyDataout1 = dailyDataout;
             //    return dailyDataout1;
             //});
             FileHelperEngine fileHelperEngine3 = new FileHelperEngine(typeof(DailyDataout));
             fileHelperEngine3.WriteFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\test.csv", dailyDataouts2);
             DailyDataout[] dailyDataoutArray = (DailyDataout[])fileHelperEngine3.ReadFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\test.csv");
             IOrderedEnumerable<DailyDataout> dailyDataouts3 = ((IEnumerable<DailyDataout>)dailyDataoutArray).OrderBy<DailyDataout, string>((DailyDataout d) =>
             {
                 string str = d.symbol;
                 return str;
             });
             IOrderedEnumerable<DailyDataout> dailyDataouts4 = dailyDataouts3.ThenBy<DailyDataout, DateTime>((DailyDataout d) =>
             {
                 DateTime dateTime = d.date;
                 return dateTime;
             });
             List<livedataReport> livedataReports = new List<livedataReport>();
             string str3 = "";
             decimal num = new decimal(0);
             decimal num1 = new decimal(0);
             decimal num2 = new decimal(0);
             decimal num3 = new decimal(0);
             decimal num4 = new decimal(0);
             bool flag11 = false;
             bool flag12 = false;
             int num5 = 0;
             string str4 = "NO";
             int num6 = 0;
             foreach (DailyDataout dailyDataout2 in dailyDataouts4)
             {
                 livedataReport _livedataReport = new livedataReport();
                 _livedataReport.date = dailyDataout2.date;
                 _livedataReport.adv = dailyDataout2.adv;
                 if (!(dailyDataout2.symbol == str3))
                 {
                     str3 = dailyDataout2.symbol;
                     num = dailyDataout2.spyclose;
                     num5 = 0;
                     num1 = new decimal(0);
                     num2 = new decimal(0);
                     num3 = dailyDataout2.high;
                     num4 = dailyDataout2.low;
                     flag12 = false;
                     flag11 = false;
                     if (dailyDataout2.threshold > new decimal(0))
                     {
                         flag4 = (flag11 ? true : !(dailyDataout2.close > num3 * dailyDataout2.spyclose / num));
                         if (!flag4)
                         {
                             num1 = Math.Max(dailyDataout2.open, (num3 * dailyDataout2.spyclose) / num);
                             num5 = Convert.ToInt32(buyinpower / num1);
                             str4 = "Yes";
                             num6 = num5;
                             flag11 = true;
                         }
                         _livedataReport.symbol = str3;
                         _livedataReport.side = "Long";
                         _livedataReport.pricein = (num3 * dailyDataout2.spyclose) / num;
                         _livedataReport.priceout = (dailyDataout2.low * dailyDataout2.spyclose) / num;
                         _livedataReport.size = Convert.ToInt32(buyinpower / _livedataReport.pricein);
                         _livedataReport.AlreadyOpenSize = num6;
                         _livedataReport.AlreadyOpen = str4;
                     }
                     else if (dailyDataout2.threshold < new decimal(0))
                     {
                         flag3 = (flag11 ? true : !(dailyDataout2.close < num4 * dailyDataout2.spyclose / num));
                         if (!flag3)
                         {
                             num1 = Math.Min(dailyDataout2.open, (num4 * dailyDataout2.spyclose) / num);
                             num5 = Convert.ToInt32(buyinpower / num1);
                             flag11 = true;
                             str4 = "Yes";
                             num6 = num5;
                         }
                         _livedataReport.symbol = str3;
                         _livedataReport.side = "Short";
                         _livedataReport.pricein = (num4 * dailyDataout2.spyclose) / num;
                         _livedataReport.priceout = (dailyDataout2.high * dailyDataout2.spyclose) / num;
                         _livedataReport.size = Convert.ToInt32(buyinpower / _livedataReport.pricein);
                         _livedataReport.AlreadyOpenSize = num6;
                         _livedataReport.AlreadyOpen = str4;
                     }
                     livedataReports.Add(_livedataReport);
                 }
                 else if (dailyDataout2.threshold > new decimal(0))
                 {
                     flag8 = (!flag11 ? true : flag12);
                     if (!flag8)
                     {
                         if (dailyDataout2.close < ((dailyDataout2.low * dailyDataout2.spyclose) / num))
                         {
                             num2 = (dailyDataout2.low * dailyDataout2.spyclose) / num;
                             flag12 = true;
                         }
                     }
                     flag9 = (!flag11 ? true : flag12);
                     if (!flag9)
                     {
                         num1 = new decimal(-1);
                     }
                     flag10 = (flag11 ? true : !(dailyDataout2.close > num3 * dailyDataout2.spyclose / num));
                     if (!flag10)
                     {
                         num1 = (num3 * dailyDataout2.spyclose) / num;
                         num5 = Convert.ToInt32(buyinpower / num1);
                         str4 = "Yes";
                         num6 = num5;
                         flag11 = true;
                     }
                     _livedataReport.symbol = str3;
                     _livedataReport.side = "Long";
                     if (flag11)
                     {
                         _livedataReport.pricein = num1;
                     }
                     else
                     {
                         _livedataReport.pricein = (num3 * dailyDataout2.spyclose) / num;
                     }
                     if (flag12)
                     {
                         _livedataReport.priceout = num2;
                     }
                     else
                     {
                         _livedataReport.priceout = (dailyDataout2.low * dailyDataout2.spyclose) / num;
                     }
                     _livedataReport.size = Convert.ToInt32(buyinpower / _livedataReport.pricein);
                     _livedataReport.AlreadyOpenSize = num6;
                     _livedataReport.AlreadyOpen = str4;
                     if (!flag12)
                     {
                         livedataReports.Add(_livedataReport);
                     }
                 }
                 else if (dailyDataout2.threshold < new decimal(0))
                 {
                     flag5 = (!flag11 ? true : flag12);
                     if (!flag5)
                     {
                         if (dailyDataout2.close > ((dailyDataout2.high * dailyDataout2.spyclose) / num))
                         {
                             num2 = (dailyDataout2.high * dailyDataout2.spyclose) / num;
                             flag12 = true;
                         }
                     }
                     flag6 = (!flag11 ? true : flag12);
                     if (!flag6)
                     {
                         num1 = new decimal(-1);
                     }
                     flag7 = (flag11 ? true : !(dailyDataout2.close < num4 * dailyDataout2.spyclose / num));
                     if (!flag7)
                     {
                         num1 = (num4 * dailyDataout2.spyclose) / num;
                         num5 = Convert.ToInt32(buyinpower / num1);
                         flag11 = true;
                         str4 = "Yes";
                         num6 = num5;
                     }
                     _livedataReport.symbol = str3;
                     _livedataReport.side = "Short";
                     if (flag11)
                     {
                         _livedataReport.pricein = num1;
                     }
                     else
                     {
                         _livedataReport.pricein = (num4 * dailyDataout2.spyclose) / num;
                     }
                     if (flag12)
                     {
                         _livedataReport.priceout = num2;
                     }
                     else
                     {
                         _livedataReport.priceout = (dailyDataout2.high * dailyDataout2.spyclose) / num;
                     }
                     _livedataReport.size = Convert.ToInt32(buyinpower / _livedataReport.pricein);
                     _livedataReport.AlreadyOpenSize = num6;
                     _livedataReport.AlreadyOpen = str4;
                     if (!flag12)
                     {
                         livedataReports.Add(_livedataReport);
                     }
                 }
             }
             IEnumerable<livedataReport> livedataReports1 = livedataReports.Where<livedataReport>((livedataReport d) =>
             {
                 bool flag = d.date == DateTime.Parse(date);
                 return flag;
             });
             FileHelperEngine fileHelperEngine4 = new FileHelperEngine(typeof(livedataReport));
             fileHelperEngine4.HeaderText = "Symbol,date,AlreadyOpen,AlreadyOpenSize,Side,EnterPrice,ExitPrice,size,adv";
             fileHelperEngine4.WriteFile("C:\\Users\\Jiahai\\Documents\\Visual Studio 2012\\Projects\\EarningBacktester\\EarningBacktester\\bin\\Debug\\Daily\\Update.D.csv", livedataReports1);
             TimeSpan timeSpan = DateTime.Now.TimeOfDay;
             Console.WriteLine(timeSpan - timeOfDay);
         }
    }

    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class livedataReport
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public string AlreadyOpen;

        public int AlreadyOpenSize;

        public string side;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal pricein;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal priceout;

        public int size;

        public int adv;


    }

    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class DailyDataout
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public decimal open;

        public decimal high;

        public decimal low;

        public decimal close;

        public long volume;

        public decimal ExitSignalprice;

        public decimal pricein;

        public decimal priceout;


        public decimal threshold;

        public decimal spyclose;

        public decimal spyopen;

        public decimal spyhigh;

        public decimal spylow;

        public int adv;

        public byte win10;

        public byte lose10;

        public decimal overnightpnl;

        public decimal daypnl;

        public decimal pnl;

        public decimal overnightspypnl;

        public decimal dayspypnl;

        public decimal spypnl;

        public decimal bp;

        public decimal beta;


    }
    [DelimitedRecord(",")]
    public class DailyData
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public decimal open;

        public decimal high;

        public decimal low;

        public decimal close;

        public long volume;

        public int qua;

        public DailyData()
        {
        }
    }


    [DelimitedRecord(",")]
    public class DailyData3
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public decimal open;

        public decimal high;

        public decimal low;

        public decimal close;

        public long volume;

        public int qua;

        public decimal threshold;

        public DailyData3()
        {
        }
    }

    [DelimitedRecord(",")]
    public class EarningData
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public int que;


    }


    [DelimitedRecord(",")]
    public class unionearingdate
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        public decimal open;

        public decimal high;

        public decimal low;

        public decimal close;

        public long volume;

        public int qua;

        public decimal threshold;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime week52date;

        public decimal maxhigh;

        public decimal minlow;


    }

    [DelimitedRecord(",")]
    public class DailyData1
    {
        public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal open;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal high;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal low;

        [FieldConverter(typeof(DecimalkeeptwoString))]
        public decimal close;

        public long volume;

        public int adv;

    }

    [DelimitedRecord(",")]
    public class symbollist
    {
        public string symbol;
       
    }

    [DelimitedRecord(",")]
	[IgnoreEmptyLines]
	[IgnoreFirst(4)]
	public class symbollistCombine
	{
		public string symbol;
         
		public string getDate;

        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]  
		[FieldNullValue(typeof(DateTime), "01/01/1999")]
		public DateTime Earingdate;

	
	}

    [DelimitedRecord(",")]
	public class earingData
	{
		public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]    
		public DateTime date;

		public int quarter;

	
	}

    	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	public class RowDailyData
	{
		public string ID;

		[FieldConverter(typeof(RemoveQuotFromString))]
		public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]    // The parameters of the attribute are missing, because its assembly was not found
		public DateTime date;

		public decimal open;

		public decimal high;

		public decimal low;

		public decimal close;

		[FieldConverter(typeof(LongConverterFromString))]
		public long Volume;

		[FieldConverter(typeof(DecimalConverterFromString))]
		public decimal adjusted;
        }

    [DelimitedRecord(",")]
	public class DailyDataReport
	{
		public string Symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime Date;

		public decimal Open;

		public decimal High;

		public decimal Low;

		public decimal Close;

		public long Volume;

		public decimal Symbolspardtc2to;

		public decimal Spyspardtc2to;

		public decimal Betatc2to;

		public decimal Spyopen;

		public decimal Spyclose;

		public decimal Symbolspardtc2yc;

		public decimal Spyspardtc2yc;

		public decimal Betatc2yc;

		public decimal Symbolspardto2yc;

		public decimal Spyspardto2yc;

		public decimal Betato2yc;

	}

    [DelimitedRecord(",")]
	public class DailyEaringData
	{
		public string symbol;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime date;

		public decimal open;

		public decimal high;

		public decimal low;

		public decimal close;

		public long volume;

		public int qua;

		
	}

        public class RemoveQuotFromString : ConverterBase
        {
            public RemoveQuotFromString()
            {
            }

            public override string FieldToString(object from)
            {
                return ((string)from).ToString();
            }

            public override object StringToField(string from)
            {
                return from.Replace("\"", "");
            }
        }

        public class LongConverterFromString : ConverterBase
        {
            public LongConverterFromString()
            {
            }

            public override string FieldToString(object from)
            {
                return ((long)from).ToString();
            }

            public override object StringToField(string from)
            {
                long num = (long)0;
                if (!from.Contains("e+"))
                {
                    num = Convert.ToInt64(from);
                }
                else
                {
                    char[] chrArray = new char[] { 'e' };
                    string[] strArrays = from.Split(chrArray);
                    num = (long)(Convert.ToDouble(strArrays[0]) * Math.Pow(10, (double)Convert.ToInt32(strArrays[1])));
                }
                return num;
            }
        }

        public class DecimalConverterFromString : ConverterBase
        {
            public DecimalConverterFromString()
            {
            }

            public override string FieldToString(object from)
            {
                return ((decimal)from).ToString();
            }

            public override object StringToField(string from)
            {
                decimal num = new decimal(0);
                if (!from.Contains("e+"))
                {
                    num = Convert.ToDecimal(from);
                }
                else
                {
                    char[] chrArray = new char[] { 'e' };
                    string[] strArrays = from.Split(chrArray);
                    num = (long)(Convert.ToDouble(strArrays[0]) * Math.Pow(10, (double)Convert.ToInt32(strArrays[1])));
                }
                return num;
            }
        }

        public class DecimalkeeptwoString : ConverterBase
        {
            public DecimalkeeptwoString()
            {
            }

            public override string FieldToString(object from)
            {
                string str;
                decimal num = (decimal)from;
                str = (!(num != new decimal(0)) ? num.ToString() : num.ToString("#.##"));
                return str;
            }

            public override object StringToField(string from)
            {
                decimal num = new decimal(0);
                object obj = Convert.ToDecimal(from);
                return obj;
            }
        }

	

}
