using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningBacktester
{
    class Program
    {
        static void Main(string[] args)
        {
            
           // createBacktestReport();
          // createADV();
          //  createQua();
            createliveReport();
            readdaily2(new decimal(1, 0, 0, false, 2), 100000, "2014/04/17");
           // backtest(20141, (decimal)0.01, 100000);
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
                    fileHelperEngine1.WriteFile(string.Format("Daily\\DBadv\\{0}", strArrays[12]), dailyData1s);
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
                    if (File.Exists((string.Format("Daily\\DailyQua\\{0}{1}.csv", num, i + 1))))
                    {
                        File.Delete(string.Format("Daily\\DailyQua\\{0}{1}.csv", num, i + 1));
                    }
                    string[] files = Directory.GetFiles("Daily\\DBadv");
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
                            fileHelperEngine.AppendToFile(string.Format("Daily\\DailyQua\\{0}{1}.csv", num, i + 1), dailyData1s);
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
                            fileHelperEngine.AppendToFile(string.Format("Daily\\DailyQua\\{0}{1}.csv", num, i + 1), dailyData1s);
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
            if (File.Exists("Daily\\finalall.csv"))
            {
                File.Delete("Daily\\finalall.csv");
            }
            Dictionary<string, DateTime> strs3 = new Dictionary<string, DateTime>();
            List<EarningData> earningDatas = new List<EarningData>();
            FileStream fileStream = new FileStream("holiday.txt", FileMode.Open);
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
                          (new FileHelperEngine(typeof(unionearingdate))).AppendToFile("Daily\\finalall.csv", unionearingdates);
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
            FileStream fileStream = new FileStream("bio.csv", FileMode.Open);
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
            DailyData1[] dailyData1Array = (DailyData1[])fileHelperEngine.ReadFile(string.Format("Daily\\DailyQua\\20141.csv", new object[0]));
            FileHelperEngine fileHelperEngine2 = new FileHelperEngine(typeof(unionearingdate));
            unionearingdate[] unionearingdateArray = (unionearingdate[])fileHelperEngine2.ReadFile("Daily\\finalall.csv");
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
            fileHelperEngine2.WriteFile("Daily\\test1.csv", unionearingdates1.Where<unionearingdate>((unionearingdate dd) =>
            {
                bool flag = dd.high >= new decimal(5);
                return flag;
            }));
            unionearingdate[] unionearingdateArray1 = (unionearingdate[])fileHelperEngine2.ReadFile("Daily\\test1.csv");
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
            fileHelperEngine3.WriteFile("Daily\\test.csv", dailyDataouts2);
            DailyDataout[] dailyDataoutArray = (DailyDataout[])fileHelperEngine3.ReadFile("Daily\\test.csv");
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
            fileHelperEngine4.HeaderText="Symbol,date,AlreadyOpen,AlreadyOpenSize,Side,EnterPrice,ExitPrice,size,adv";
            fileHelperEngine4.WriteFile("Daily\\Update.D.csv", livedataReports1);
            TimeSpan timeSpan = DateTime.Now.TimeOfDay;
            Console.WriteLine(timeSpan - timeOfDay);
        }


        private static void createBacktestReport()
        {
            if (File.Exists("DailyReport\\R.csv"))
            {
                File.Delete("DailyReport\\R.csv");
            }
            try
            {
                int num = 2007;
                int num1 = 2014;
                while (num <= num1)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        int num2 = num * 10 + i;
                        decimal num3 = new decimal(1, 0, 0, false, 2);
                        Program.backtest(num2, num3, 100000);
                        Console.WriteLine(num2);
                    }
                    num++;
                }
            }
            catch
            {
            }
            Program.anlquater();
        }

        private static void backtest(int quarter,decimal threshold, int buyinpower)
        {
            TimeSpan st = DateTime.Now.TimeOfDay;
             #region hoilday dictionary
              bool flag3;
               Dictionary<string, DateTime> strs3 = new Dictionary<string, DateTime>();
            List<EarningData> earningDatas = new List<EarningData>();
            FileStream fileStream1 = new FileStream("holiday.txt", FileMode.Open);
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

            #region get the quarter

            int year = quarter / 10;
            string begindate = year.ToString();
            string enddate = year.ToString();
            if (quarter % 10 == 1)
            {
                begindate = string.Concat(begindate, "0101");
                enddate = string.Concat(enddate, "0315");
            }
            if (quarter % 10 == 2)
            {

                begindate = string.Concat(begindate, "0401");
                enddate = string.Concat(enddate, "0615");
            }
            if (quarter % 10 == 3)
            {

                begindate = string.Concat(begindate, "0701");
                enddate = string.Concat(enddate, "0915");
            }
            if (quarter % 10 == 4)
            {

                begindate = string.Concat(begindate, "1001");
                enddate = string.Concat(enddate, "1215");
            }

            #endregion

            #region create bio tech symbols Dic

            bool flag2;
            Dictionary<string, string> bioArry = new Dictionary<string, string>();
            FileStream fileStream = new FileStream("bio.csv", FileMode.Open);
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
                bioArry[str2] = str2;
                str2 = streamReader.ReadLine();
            }
            streamReader.Close();
            streamReader.Dispose();
            fileStream.Close();
            fileStream.Dispose();

            #endregion

            #region get right the earingdata

            FileHelperEngine EngineDBq = new FileHelperEngine(typeof(DailyData1));
            DailyData1[] resDB = (DailyData1[])EngineDBq.ReadFile(string.Format("Daily\\DailyQua\\{0}.csv", quarter));
            FileHelperEngine EngineEarning = new FileHelperEngine(typeof(unionearingdate));
            unionearingdate[] unionearingdateArray = (unionearingdate[])EngineEarning.ReadFile("Daily\\final.csv");

            var selectEarningdata = unionearingdateArray.Where(d =>
                d.date >= DateTime.ParseExact(begindate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None) &&
                d.date <= DateTime.ParseExact(enddate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)).
                Where(d => Math.Abs(d.threshold) >= threshold).
                Where(d => (d.high >= d.maxhigh && d.threshold > 0) || (d.low <= d.minlow && d.threshold < 0)).
                Where(d => { 
                    bool symbolExit;
                    symbolExit = !bioArry.ContainsKey(d.symbol);
                    return symbolExit;
                }).
                Where(d=>d.high>=5)
                ;

            EngineEarning.WriteFile("Daily\\temp.csv", selectEarningdata);
            unionearingdate[] earningdata = (unionearingdate[])EngineEarning.ReadFile("Daily\\temp.csv");

            #endregion

            #region combin earning and spy in daily report

            #region select data from DB
            var spy=resDB.Where(d=>d.symbol=="SPY");

            var selectDB = from db in resDB
                           from e in earningdata
                           where (db.symbol == e.symbol) && (db.date >= e.date) 
                           select new DailyDataout
                           {
                               symbol=db.symbol,
                               open=db.open,
                               high=db.high,
                               low=db.low,
                               close=db.close,
                               volume=db.volume,
                               adv=db.adv,
                               threshold=e.threshold,
                               date=db.date
                           };

            selectDB = selectDB.Where(dd => dd.date <= DateTime.ParseExact(enddate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None));

            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));
            EngineOut.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            EngineOut.WriteFile("Daily\\temp.csv", selectDB);
            #endregion

            #region combine spy
            DailyDataout[] selectD = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            var combinspy = from db in selectD
                            join e in spy on db.date equals e.date
                            select new DailyDataout
                            {
                                symbol = db.symbol,
                                open = db.open,
                                high = db.high,
                                low = db.low,
                                close = db.close,
                                volume = db.volume,
                                adv = db.adv,
                                threshold = db.threshold,
                                date = db.date,
                                spyclose=e.close,
                                spyopen=e.open,
                                spylow=e.low,
                                spyhigh=e.high
                            };
            #endregion

           
            bool yc2tc = false;

            #region add beta
            if (yc2tc)
            {

                #region get beta close to close

                List<DailyDataout> newcombinspy = new List<DailyDataout>();
                string symbol = "";
                decimal beta = 0;
                decimal yesterdayClose = 0;
                decimal yesterdaySPYClose = 0;
                foreach (var data in combinspy)
                {
                    DailyDataout tempdailyout = new DailyDataout();
                    tempdailyout = data;

                    if (tempdailyout.symbol == symbol)
                    {
                        beta = ((tempdailyout.close / yesterdayClose) / (tempdailyout.spyclose / yesterdaySPYClose) - 1) * 100;
                        tempdailyout.beta = beta;
                        yesterdayClose = tempdailyout.close;
                        yesterdaySPYClose = tempdailyout.spyclose;
                    }
                    else
                    {
                        symbol = tempdailyout.symbol;
                        yesterdayClose = tempdailyout.close;
                        yesterdaySPYClose = tempdailyout.spyclose;
                    }
                    newcombinspy.Add(tempdailyout);
                }


            #endregion

                EngineOut.WriteFile("Daily\\temp.csv", newcombinspy);
            }
            else
            {

              #region get beta close to open

            List<DailyDataout> newcombinspy = new List<DailyDataout>();
            string symbol = "";
            decimal beta = 0;
            foreach (var data in combinspy)
            {
                DailyDataout tempdailyout = new DailyDataout();
                tempdailyout = data;

                if (tempdailyout.symbol == symbol)
                {
                    beta = ((tempdailyout.close / tempdailyout.open) / (tempdailyout.spyclose / tempdailyout.spyopen) - 1) * 100;
                    tempdailyout.beta = beta;
                }
                else
                {
                    symbol = tempdailyout.symbol;
                    beta = ((tempdailyout.close / tempdailyout.open) / (tempdailyout.spyclose / tempdailyout.spyopen) - 1) * 100;
                }
                newcombinspy.Add(tempdailyout);
            }


            #endregion

              EngineOut.WriteFile("Daily\\temp.csv", newcombinspy);
            }
            #endregion


            #endregion

            #region date thrshold

            DateTime lastDate = DateTime.ParseExact(enddate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (lastDate.DayOfWeek == DayOfWeek.Sunday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-2).ToString("yyyyMMdd")) || strs3.ContainsKey(lastDate.AddDays(-3).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-4);
                }
                else
                lastDate = lastDate.AddDays(-3);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Saturday)
            {
                if (strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")) || strs3.ContainsKey(lastDate.AddDays(-2).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-3);
                }
                else
                    lastDate = lastDate.AddDays(-2);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Monday)
            {
                if (strs3.ContainsKey(lastDate.ToString("yyyyMMdd")) || strs3.ContainsKey(lastDate.AddDays(-3).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-4);
                }
                    else
                lastDate = lastDate.AddDays(-3);
            }
            else if (lastDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                if (strs3.ContainsKey(lastDate.ToString("yyyyMMdd")) || strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-4);
                }
                else
                    lastDate = lastDate.AddDays(-1);

            }
            else 
            {
                if (strs3.ContainsKey(lastDate.ToString("yyyyMMdd")) || strs3.ContainsKey(lastDate.AddDays(-1).ToString("yyyyMMdd")))
                {
                    lastDate = lastDate.AddDays(-2);
                }
                else
                    lastDate = lastDate.AddDays(-1);

            }

            #endregion

            strategy2ExitbySumBeta(quarter, threshold, buyinpower, (decimal)5, lastDate);

            DailyGroupbysymbol(quarter, threshold);

            #region create daily report

            //FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            //enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            DailyDataout[] ReportEaring = (DailyDataout[])EngineOut.ReadFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))));

            var DailyReport = ReportEaring.GroupBy(d => d.date).Select(d => new DailyReport
            {
              date=d.Key,
              pnl=d.Sum(dd=>dd.pnl)+d.Sum(dd=>dd.spypnl),
              pnlovernight=d.Sum(dd=>dd.overnightpnl)+d.Sum(dd=>dd.overnightspypnl),
              pnlday=d.Sum(dd=>dd.daypnl)+d.Sum(dd=>dd.dayspypnl),
              longcount=d.Where(dd=>dd.threshold>0).Count(),
              shortcount=d.Where(dd=>dd.threshold<0).Count(),
              longpnl=d.Where(dd=>dd.threshold>0).Sum(dd=>dd.pnl),
              shortpnl = d.Where(dd => dd.threshold < 0).Sum(dd => dd.pnl),
              qua=quarter,
              lose10=d.Sum(dd=>dd.lose10),
              win10=d.Sum(dd=>dd.win10),
              hspypnl = d.Sum(dd => dd.spypnl),
              
              
            }).OrderBy(d=>d.date);
            #endregion

            #region get the bp and accpnl daily
            List<DailyReport> newDailyReport = new List<DailyReport>();
            decimal accPnl = 0;
            foreach(var date in DailyReport)
            {
                DailyReport tempdailyreport = new DailyReport();
                tempdailyreport = date;

                accPnl = accPnl + date.pnl;
                tempdailyreport.accPnl = accPnl;
                tempdailyreport.bp = Math.Max(date.longcount, date.shortcount) * 2 * buyinpower;
                newDailyReport.Add(tempdailyreport);
            }
            #endregion

            #region get the dollar sharpe
            decimal avgPnl = newDailyReport.Select(d => d.accPnl).Last() / newDailyReport.Where(d => d.pnl != 0).Count();

            List<DailyReport> finalDailyReport = new List<DailyReport>();
            decimal SD = 0;
            int count = 1;
            foreach(var date in newDailyReport)
            {
                DailyReport tempdailyreport = new DailyReport();
                tempdailyreport = date;

                SD = SD + (date.pnl - avgPnl) * (date.pnl - avgPnl);

                if (count != 1)
                    date.dollarSharp = avgPnl / (decimal)(Math.Sqrt(Convert.ToDouble(SD) / ((count - 1)*252)));
                finalDailyReport.Add(tempdailyreport);
                count++;
            }

            FileHelperEngine fileHelperEngine5 = new FileHelperEngine(typeof(DailyReport));
            fileHelperEngine5.HeaderText = "date,longpnl,shortpnl,longcount,shortcount,quarter,dollarsharp,bp,win10,lose10,spy hedge pnl,pnl overnight, pnl day,pnl,acc pnl";
            if (!File.Exists("DailyReport\\R.csv"))
            {
                fileHelperEngine5.WriteFile(string.Format("DailyReport\\R.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), finalDailyReport);
            }
            else
            {
                fileHelperEngine5.AppendToFile(string.Format("DailyReport\\R.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), finalDailyReport);
            }

            #endregion

            TimeSpan en = DateTime.Now.TimeOfDay;
            Console.WriteLine(en-st);
        }

        private static void anlquater()
        {
            FileHelperEngine engion = new FileHelperEngine(typeof(DailyReport));
            DailyReport[] dailyReportArray = (DailyReport[])engion.ReadFile("DailyReport\\R.csv");

            var quaterRe = dailyReportArray.GroupBy(d => d.qua).Select(d => new quaterReport

                {
                    qua=d.Key,
                    lastestExitday=d.Select(dd=>dd.date).Last(),
                    DollarSharpe=d.Select(dd=>dd.dollarSharp).Last(),
                    AccPnl=d.Select(dd=>dd.accPnl).Last(),
                    AccPnlday=d.Sum(dd=>dd.pnlday),
                    AccPnlovernight=d.Sum(dd=>dd.pnlovernight),
                    gaplos10=d.Sum(dd=>dd.lose10),
                    gapsWin10=d.Sum(dd=>dd.win10),
                    highestPnl=d.Max(dd=>dd.accPnl),
                    lowestPnl=d.Min(dd=>dd.accPnl),
                    maxbp=d.Max(dd=>dd.bp),
                    maxbplongcount=d.OrderBy(dd=>dd.bp).Select(dd=>dd.longcount).Last(),
                    maxbpshortcount = d.OrderBy(dd => dd.bp).Select(dd => dd.shortcount).Last(),
                }

                );
            FileHelperEngine fileHelperEngine1 = new FileHelperEngine(typeof(quaterReport));
            fileHelperEngine1.HeaderText="qua,lastestExitday,Dallar sharpe,AccPnlday,AccPnlovernight,AccPnl,lowest pnl,highest pnl,max bp,max bp long count,max bp short count,win10,lose10";
            fileHelperEngine1.WriteFile("DailyReport\\QR.csv", quaterRe);
        }

        #region old
        #region strategy 1
        private static void strategyCloseIn(int quarter, decimal threshold, int buyinpower, decimal bate, DateTime lastdate)
        {
            #region get symbols & spy hedge pnl
            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));

            DailyDataout[] FinalDB = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            #region definition var
            List<DailyDataout> ReportEaring = new List<DailyDataout>();
            List<AdjustEaringdata> adjE = new List<AdjustEaringdata>();

            Queue<decimal> Beta5Ds = new Queue<decimal>();
            decimal avgBeta = 777;

            string symbol = "";
            decimal spyearingdate = 0;

            decimal highEaring = 0;
            decimal lowEaring = 0;

            decimal pricein = 0;
            decimal priceout = 0;

            decimal enterhigh = 0;
            decimal enterlow = 0;

            bool findin = false;
            bool findout = false;
            bool entrydate = false;
            bool exitdate = false;

            int size = 0;

            decimal spyin = 0;
            // decimal spyout = 0;
            int spysize = 0;
            #endregion

            foreach (var item in FinalDB)
            {

                DailyDataout tempitem = new DailyDataout();
                tempitem = item;
                AdjustEaringdata adjear = new AdjustEaringdata();
                if (tempitem.threshold > 0)
                {
                    #region long
                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {
                            #region get pnl

                            #region entry pnl
                            if (entrydate)
                            {
                                //pricein = Math.Max(pricein, item.open);
                                size = Convert.ToInt32(buyinpower / pricein);
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = size * (item.close - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                enterlow = item.low;
                                tempitem.pricein = pricein;
                                #region short spy hedge worst condition
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                Beta5Ds.Enqueue(tempitem.beta);
                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                                entrydate = false;
                            }
                            #endregion

                            #region exit pnl
                            else if (exitdate)
                            {
                                priceout = Math.Min(priceout, item.close);
                                priceout = item.open;

                                tempitem.pnl = size * (priceout - pricein);
                                tempitem.daypnl = size * (priceout - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = priceout;
                                #region short spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regular pnl
                            else
                            {
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = size * (item.close - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = item.close;
                                #region short spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                            #region exit logic
                            if (!findout && (item.close < (lowEaring * item.spyclose / spyearingdate)
                                //|| avgBeta<(decimal)-2
                                 || item.date == lastdate
                                ))
                            {
                                priceout = lowEaring * item.spyclose / spyearingdate;
                                findout = true;
                                exitdate = true;
                            }
                            #endregion
                        }

                        #region entry logic

                        if (!findin && item.close > (highEaring * item.spyclose / spyearingdate))
                        {
                            //pricein = (highEaring * item.spyclose / spyearingdate);
                            findin = true;
                            entrydate = true;
                            spyin = item.spyclose;
                            pricein = item.close;
                            //if (item.symbol == "MTW")
                            //    Console.WriteLine(item.date);
                            tempitem.close = pricein;
                           // ReportEaring.Add(tempitem);

                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);

                        }

                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                    }
                    else
                    {
                        #region earnign date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        entrydate = false;
                        exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = 10000000;
                        Beta5Ds.Clear();

                        #endregion
                    }
                    #endregion
                }
                if (tempitem.threshold < 0)
                {
                    #region short

                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {


                            #region get pnl

                            #region entry pnl
                            if (entrydate)
                            {
                                pricein = Math.Min(pricein, item.open);
                                size = Convert.ToInt32(buyinpower / pricein);
                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = size * (-item.close + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                enterhigh = item.high;
                                tempitem.pricein = pricein;
                                #region long spy hedge worst condition
                                spysize = Convert.ToInt32(buyinpower / item.spyopen);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                Beta5Ds.Enqueue(tempitem.beta);
                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                entrydate = false;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #region exit pnl
                            else if (exitdate)
                            {
                                priceout = Math.Max(priceout, item.close);
                                priceout = item.open;

                                tempitem.pnl = size * (-priceout + pricein);
                                tempitem.daypnl = size * (-priceout + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = priceout;

                                #region long spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regual pnl

                            else
                            {

                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = size * (-item.close + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = item.close;
                                #region long spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.5)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                            #region exit logic
                            if (!findout && (item.close > (highEaring * item.spyclose / spyearingdate)
                                //||avgBeta > (decimal)2
                                 || item.date == lastdate
                                ))
                            {
                                priceout = highEaring * item.spyclose / spyearingdate;
                                findout = true;
                                exitdate = true;
                            }
                            #endregion
                        }

                        #region entry logic
                        if (!findin && item.close < (lowEaring * item.spyclose / spyearingdate))
                        {
                            //pricein = (lowEaring* item.spyclose / spyearingdate);
                            findin = true;
                            entrydate = true;
                            spyin = item.spyclose;
                            pricein = item.close;

                            tempitem.close = pricein;
                        //    ReportEaring.Add(tempitem);

                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion
                    }
                    else
                    {
                        #region earning date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        entrydate = false;
                        exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = -10000000;
                        Beta5Ds.Clear();
                        #endregion
                    }

                    #endregion
                }

            }

            #region write to file
            FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            enginDR.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            FileHelperEngine enginadj = new FileHelperEngine(typeof(AdjustEaringdata));
            enginadj.HeaderText = "symbol,date,threshold,adj earning high, adj earing low";
            enginadj.WriteFile(string.Format("DailyReport\\{0}.{1}.E.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), adjE);
            #endregion

            #endregion
        }

        private static void strategyBeta(int quarter, decimal threshold, int buyinpower,decimal bate,DateTime lastdate)
        {
            #region get symbols & spy hedge pnl

            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));

            DailyDataout[] FinalDB = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            #region definition var
            List<DailyDataout> ReportEaring = new List<DailyDataout>();
            List<AdjustEaringdata> adjE = new List<AdjustEaringdata>();

            Queue<decimal> Beta5Ds = new Queue<decimal>();
            decimal avgBeta = 777;

            string symbol = "";
            decimal spyearingdate = 0;

            decimal highEaring = 0;
            decimal lowEaring = 0;

            decimal pricein = 0;
            decimal priceout = 0;

            decimal enterhigh = 0;
            decimal enterlow = 0;

            bool findin = false;
            bool findout = false;
            bool entrydate = false;
            bool exitdate = false;

            int size = 0;

            decimal spyin = 0;
            // decimal spyout = 0;
            int spysize = 0;
            #endregion

            foreach (var item in FinalDB)
            {

                DailyDataout tempitem = new DailyDataout();
                tempitem = item;
                AdjustEaringdata adjear = new AdjustEaringdata();

                if (tempitem.threshold > 0)
                {
                    #region long
                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {
                            #region get pnl

                            #region entry pnl
                            if (entrydate)
                            {
                                pricein = Math.Max(pricein, item.open);
                                size = Convert.ToInt32(buyinpower / pricein);
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = tempitem.pnl;
                                tempitem.overnightpnl = 0;
                                tempitem.pricein = pricein;
                                tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;
                                enterlow = item.low;

                                #region short spy hedge worst condition
                                spysize = Convert.ToInt32(buyinpower / item.spyopen);
                                tempitem.spypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.dayspypnl = tempitem.spypnl;
                                tempitem.overnightspypnl = 0;
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                Beta5Ds.Enqueue(tempitem.beta);
                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                                entrydate = false;
                            }
                            #endregion

                            #region exit pnl
                            else if (exitdate)
                            {
                                //if (avgBeta < -bate)
                                //{
                                //    priceout = item.open;
                                //}
                                //else
                                //{
                                    priceout = Math.Min(priceout, item.close);
                                    priceout = item.open;
                               // }
                                tempitem.pnl = size * (priceout - pricein);
                                tempitem.daypnl = size * (priceout - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = priceout;

                                #region short spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regular pnl
                            else
                            {
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = size * (item.close - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;
                                #region short spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                            #region exit logic
                            if (!findout && (item.close < (lowEaring * item.spyclose / spyearingdate)
                                || avgBeta<-bate 
                                ||item.date==lastdate
                                ))
                            {
                                priceout = lowEaring * item.spyclose / spyearingdate;
                                findout = true;
                                exitdate = true;
                            }
                            #endregion
                        }

                        #region entry logic

                        if (!findin && item.close > (highEaring * item.spyclose / spyearingdate))
                        {
                            pricein = (highEaring * item.spyclose / spyearingdate);
                            findin = true;
                            entrydate = true;
                            spyin = item.spyclose;
                            //if (item.symbol == "FB")
                            //    Console.WriteLine(item.date);
                            tempitem.pricein = pricein;
                            //     ReportEaring.Add(tempitem);

                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);

                       }

                        #endregion

                        #region adj
                        if (!findin)
                        {
                        adjear.symbol = item.symbol;
                        adjear.date = item.date;
                        adjear.threshold = item.threshold;
                        adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                        adjE.Add(adjear);
                        }
                        #endregion

                    }
                    else
                    {
                        #region earnign date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        entrydate = false;
                        exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = 10000000;
                        Beta5Ds.Clear();

                        #endregion
                    }
                    #endregion
                }
                if (tempitem.threshold < 0)
                {
                    #region short

                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {

                            #region get pnl

                            #region entry pnl
                            if (entrydate)
                            {
                                pricein = Math.Min(pricein, item.open);
                                size = Convert.ToInt32(buyinpower / pricein);
                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = tempitem.pnl;
                                tempitem.overnightpnl = 0;
                                enterhigh = item.high;
                                tempitem.pricein = pricein;
                                tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;
                                #region long spy hedge worst condition
                                spysize = Convert.ToInt32(buyinpower / item.spyopen);
                                tempitem.spypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.dayspypnl = tempitem.spypnl;
                                tempitem.overnightspypnl = 0;
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                Beta5Ds.Enqueue(tempitem.beta);
                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                entrydate = false;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #region exit pnl
                            else if (exitdate)
                            {
                                //if (avgBeta > bate)
                                //{
                                //    priceout=item.open;
                                //}
                                //else
                                //{
                                priceout = Math.Max(priceout, item.close);
                                priceout = item.open;
                               // }
                                tempitem.pnl = size * (-priceout + pricein);
                                tempitem.daypnl = size * (-priceout + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = priceout;
                                #region long spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regular pnl
                            else
                            {
                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = size * (-item.close + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;
                                #region long spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.5)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                            #region exit logic
                            if (!findout && (item.close > (highEaring * item.spyclose / spyearingdate)
                                ||avgBeta >bate 
                                ||item.date==lastdate
                                ))
                            {
                                priceout = highEaring * item.spyclose / spyearingdate;
                                findout = true;
                                exitdate = true;
                            }
                            #endregion
                        }

                        #region entry logic
                        if (!findin && item.close < (lowEaring * item.spyclose / spyearingdate))
                        {
                            pricein = (lowEaring * item.spyclose / spyearingdate);
                            findin = true;
                            entrydate = true;
                            spyin = item.spyclose;
                            tempitem.pricein = pricein;
                            tempitem.close = pricein;
                         //   ReportEaring.Add(tempitem);

                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);

                        }
                        #endregion

                        #region adj
                        if (!findin)
                        {
                        adjear.symbol = item.symbol;
                        adjear.date = item.date;
                        adjear.threshold = item.threshold;
                        adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                        adjE.Add(adjear);
                        }
                        #endregion
                    }
                    else
                    {
                        #region earning date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        entrydate = false;
                        exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = -10000000;
                        Beta5Ds.Clear();
                        #endregion
                    }

                    #endregion
                }

            }

            #region create file
            FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            enginDR.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            FileHelperEngine enginadj = new FileHelperEngine(typeof(AdjustEaringdata));
            enginadj.HeaderText = "symbol,date,threshold,adj earning high, adj earing low";
            enginadj.WriteFile(string.Format("DailyReport\\{0}.{1}.E.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), adjE);
            #endregion

            #endregion
        }
        #endregion

        private static void strategy2Beta(int quarter, decimal threshold, int buyinpower, decimal bate, DateTime lastdate)
        {
            #region get symbols & spy hedge pnl

            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));

            DailyDataout[] FinalDB = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            #region definition var
            List<DailyDataout> ReportEaring = new List<DailyDataout>();
            List<AdjustEaringdata> adjE = new List<AdjustEaringdata>();

            Queue<decimal> Beta5Ds = new Queue<decimal>();
            decimal avgBeta = 777;

            string symbol = "";
            decimal spyearingdate = 0;

            decimal highEaring = 0;
            decimal lowEaring = 0;

            decimal pricein = 0;
            decimal priceout = 0;

            decimal enterhigh = 0;
            decimal enterlow = 0;

            bool findin = false;
            bool findout = false;
          //  bool entrydate = false;
            bool exitdate = false;


            decimal adjustEarningHigh = 0;
            decimal adjustEarningLow = 0;

            int size = 0;

            decimal spyin = 0;
            // decimal spyout = 0;
            int spysize = 0;
            #endregion

            foreach (var item in FinalDB)
            {

                DailyDataout tempitem = new DailyDataout();
                tempitem = item;
                AdjustEaringdata adjear = new AdjustEaringdata();

                if (tempitem.threshold > 0)
                {
                    #region long
                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {
                            #region get pnl

                            #region exit pnl
                            if (exitdate)
                            {
                                //if (avgBeta < -bate)
                                //{
                                //    priceout = item.open;
                                //}
                                //else
                                //{
                                priceout = Math.Min(priceout, item.close);
                                priceout = item.open;
                                // }
                                tempitem.pnl = size * (priceout - pricein);
                                tempitem.daypnl = size * (priceout - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = priceout;

                                #region short spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regular pnl
                            else
                            {
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = size * (item.close - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = enterlow * item.spyclose / spyearingdate;
                                #region short spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                        }

                        #region entry logic

                        if (!findin && item.high >= adjustEarningHigh)
                        {
                            pricein = Math.Max(adjustEarningHigh, item.open);
                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (item.close - pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterlow = item.low;

                            tempitem.ExitSignalprice = enterlow * item.spyclose / spyearingdate;
                           


                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;

                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;
                            findin = true;
                           // spyin = item.spyclose;
                          

                          

                        }

                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjustEarningHigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion


                        #region exit logic
                        if ((findin && !findout) && (item.close < (enterlow * item.spyclose / spyearingdate)
                            //                || avgBeta < -bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = enterlow * item.spyclose / spyearingdate;
                            findout = true;
                            exitdate = true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region earnign date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        findin = false;
                        findout = false;
                        //entrydate = false;
                        exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = 10000000;
                        Beta5Ds.Clear();

                        #endregion
                    }
                    #endregion
                }
                if (tempitem.threshold < 0)
                {
                    #region short

                    if (item.symbol == symbol)
                    {

                        if ((findin && !findout) || exitdate)
                        {

                            #region get pnl


                            #region exit pnl
                             if (exitdate)
                            {
                                //if (avgBeta > bate)
                                //{
                                //    priceout=item.open;
                                //}
                                //else
                                //{
                                priceout = Math.Max(priceout, item.close);
                                priceout = item.open;
                                // }
                                tempitem.pnl = size * (-priceout + pricein);
                                tempitem.daypnl = size * (-priceout + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = priceout;
                                #region long spy hedge on close
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                ReportEaring.Add(tempitem);
                                exitdate = false;
                            }
                            #endregion

                            #region regular pnl
                            else
                            {
                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = size * (-item.close + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = enterhigh * item.spyclose / spyearingdate;
                                #region long spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.5)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;
                            }
                            #endregion

                            #endregion

                      
                        }

                        #region entry logic
                        if (!findin && item.low<= adjustEarningLow)
                        {
                            pricein = Math.Min(adjustEarningLow, item.open); ;

                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (-item.close + pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterhigh = item.high;
                            tempitem.pricein = pricein;
                            tempitem.ExitSignalprice = enterhigh * item.spyclose / spyearingdate;


                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;
                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;

                            findin = true;
                         

                         

                        }
                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjustEarningLow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                        #region exit logic
                        if ((findin && !findout) && (item.close > (enterhigh * item.spyclose / spyearingdate)
                            //    || avgBeta > bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = enterhigh * item.spyclose / spyearingdate;
                            findout = true;
                            exitdate = true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region earning date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        //entrydate = false;
                        exitdate = false;
                        size = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = -10000000;
                        Beta5Ds.Clear();
                        #endregion
                    }

                    #endregion
                }

            }

            #region create file
            FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            enginDR.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            FileHelperEngine enginadj = new FileHelperEngine(typeof(AdjustEaringdata));
            enginadj.HeaderText = "symbol,date,threshold,adj earning high, adj earing low";
            enginadj.WriteFile(string.Format("DailyReport\\{0}.{1}.E.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), adjE);
            #endregion

            #endregion
        }
        #endregion

        /// <summary>
        /// if daily price is match the yesterday signal price, then in
        /// if meet any condition as follows, then out
        /// 1. reach the end of date
        /// 2. 5 day windows betas avg is smaller than -5 (long) or bigger than 5 (short)
        /// 3. daily close is smaller adj earning low (long) or  bigger than adj earning high (short)
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="threshold"></param>
        /// <param name="buyinpower"></param>
        /// <param name="bate"></param>
        /// <param name="lastdate"></param>
        private static void strategy2BetaExit(int quarter, decimal threshold, int buyinpower, decimal bate, DateTime lastdate)
        {
            #region get symbols & spy hedge pnl

            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));

            DailyDataout[] FinalDB = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            #region definition var
            List<DailyDataout> ReportEaring = new List<DailyDataout>();
            List<AdjustEaringdata> adjE = new List<AdjustEaringdata>();

            Queue<decimal> Beta5Ds = new Queue<decimal>();
            decimal avgBeta = 777;

            string symbol = "";
            decimal spyearingdate = 0;

            decimal highEaring = 0;
            decimal lowEaring = 0;

            decimal pricein = 0;
            decimal priceout = 0;

            decimal enterhigh = 0;
            decimal enterlow = 0;

            bool findin = false;
            bool findout = false;
            //  bool entrydate = false;
           // bool exitdate = false;


            decimal adjustEarningHigh = 0;
            decimal adjustEarningLow = 0;

            int size = 0;

            decimal spyin = 0;
            // decimal spyout = 0;
            int spysize = 0;
            #endregion

            foreach (var item in FinalDB)
            {

                DailyDataout tempitem = new DailyDataout();
                tempitem = item;
                AdjustEaringdata adjear = new AdjustEaringdata();

                if (tempitem.threshold > 0)
                {
                    #region long
                    if (item.symbol == symbol)
                    {
                        #region exit logic
                        if ((findin && !findout) && (item.close < (lowEaring * item.spyclose / spyearingdate)
                                            || avgBeta < -bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = Math.Min(lowEaring * item.spyclose / spyearingdate, item.close);
                            priceout = item.close;
                            tempitem.pnl = size * (priceout - pricein);
                            tempitem.daypnl = size * (priceout - item.open);
                            tempitem.overnightpnl = size * (item.open - pricein);
                            tempitem.priceout = priceout;

                            #region short spy hedge on close
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = -spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            ReportEaring.Add(tempitem);
                            findout = true;
                        }
                        #endregion

                        #region get pnl

                        if ((findin && !findout))
                        {
                           
                                tempitem.pnl = size * (item.close - pricein);
                                tempitem.daypnl = size * (item.close - item.open);
                                tempitem.overnightpnl = size * (item.open - pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;

                                #region short spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = -spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;


                        }
                        #endregion

                        #region entry logic

                        if (!findin && item.high >= adjustEarningHigh)
                        {
                            pricein = Math.Max(adjustEarningHigh, item.open);
                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (item.close - pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterlow = item.low;

                            tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;



                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;

                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;
                            findin = true;
                            // spyin = item.spyclose;




                        }

                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjustEarningHigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                    }
                    else
                    {
                        #region earnign date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        findin = false;
                        findout = false;
                        //entrydate = false;
                       // exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = 10000000;
                        Beta5Ds.Clear();

                        #endregion
                    }
                    #endregion
                }
                if (tempitem.threshold < 0)
                {
                    #region short

                    if (item.symbol == symbol)
                    {
                        #region exit logic
                        if ((findin && !findout) && (item.close > (highEaring * item.spyclose / spyearingdate)
                                || avgBeta > bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = Math.Max((highEaring * item.spyclose / spyearingdate), item.close);
                            priceout = item.close;

                            tempitem.pnl = size * (-priceout + pricein);
                            tempitem.daypnl = size * (-priceout + item.open);
                            tempitem.overnightpnl = size * (-item.open + pricein);
                            tempitem.priceout = priceout;

                            #region long spy hedge on close
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            ReportEaring.Add(tempitem);
                            findout = true;
                        }
                        #endregion

                        #region get pnl
                        if ((findin && !findout))
                        {
                           

                                tempitem.pnl = size * (-item.close + pricein);
                                tempitem.daypnl = size * (-item.close + item.open);
                                tempitem.overnightpnl = size * (-item.open + pricein);
                                tempitem.priceout = item.close;
                                tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;
                                #region long spy hedge
                                spysize = Convert.ToInt32(buyinpower / spyin);
                                tempitem.spypnl = spysize * (item.spyclose - spyin);
                                tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                                tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                                #endregion

                                #region win10 or lose10
                                if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                                {
                                    tempitem.win10 = 1;
                                }
                                else
                                {
                                    tempitem.win10 = 0;
                                }
                                if (tempitem.pnl / (pricein * size) <= -(decimal)0.5)
                                {
                                    tempitem.lose10 = 1;
                                }
                                else
                                {
                                    tempitem.lose10 = 0;
                                }
                                #endregion

                                #region 5 days beta array
                                if (Beta5Ds.Count == 5)
                                {
                                    avgBeta = Beta5Ds.Average();
                                    Beta5Ds.Dequeue();
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }
                                else
                                {
                                    Beta5Ds.Enqueue(tempitem.beta);
                                }

                                #endregion

                                ReportEaring.Add(tempitem);
                                pricein = item.close;
                                spyin = item.spyclose;

                        }
                        #endregion

                        #region entry logic
                        if (!findin && item.low <= adjustEarningLow)
                        {
                            pricein = Math.Min(adjustEarningLow, item.open); ;

                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (-item.close + pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterhigh = item.high;
                            tempitem.pricein = pricein;
                            tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;


                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;
                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;

                            findin = true;




                        }
                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjustEarningLow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                     
                    }
                    else
                    {
                        #region earning date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        //entrydate = false;
                        //exitdate = false;
                        size = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = -10000000;
                        Beta5Ds.Clear();
                        #endregion
                    }

                    #endregion
                }

            }

            #region create file
            FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            enginDR.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            FileHelperEngine enginadj = new FileHelperEngine(typeof(AdjustEaringdata));
            enginadj.HeaderText = "symbol,date,threshold,adj earning high, adj earing low";
            enginadj.WriteFile(string.Format("DailyReport\\{0}.{1}.E.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), adjE);
            #endregion

            #endregion
        }

        /// <summary>
        /// if daily price is match the yesterday signal price, then in
        /// if meet any condition as follows, then out
        /// sum up all the betas intraday  grab the hightest point don’t let it retrace more than 50% or 35%
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="threshold"></param>
        /// <param name="buyinpower"></param>
        /// <param name="bate"></param>
        /// <param name="lastdate"></param>
        private static void strategy2ExitbySumBeta(int quarter, decimal threshold, int buyinpower, decimal bate, DateTime lastdate)
        {
            #region get symbols & spy hedge pnl

            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));

            DailyDataout[] FinalDB = (DailyDataout[])EngineOut.ReadFile("Daily\\temp.csv");

            #region definition var
            List<DailyDataout> ReportEaring = new List<DailyDataout>();
            List<AdjustEaringdata> adjE = new List<AdjustEaringdata>();

            Queue<decimal> Beta5Ds = new Queue<decimal>();

            decimal sumBeta = 0;
            decimal maxBeta = 0;

            decimal avgBeta = 777;

            string symbol = "";
            decimal spyearingdate = 0;

            decimal highEaring = 0;
            decimal lowEaring = 0;

            decimal pricein = 0;
            decimal priceout = 0;

            decimal enterhigh = 0;
            decimal enterlow = 0;

            bool findin = false;
            bool findout = false;
            //  bool entrydate = false;
            // bool exitdate = false;


            decimal adjustEarningHigh = 0;
            decimal adjustEarningLow = 0;

            int size = 0;

            decimal spyin = 0;
            // decimal spyout = 0;
            int spysize = 0;
            #endregion

            foreach (var item in FinalDB)
            {

                DailyDataout tempitem = new DailyDataout();
                tempitem = item;
                AdjustEaringdata adjear = new AdjustEaringdata();

                if (tempitem.threshold > 0)
                {
                    #region long
                    if (item.symbol == symbol)
                    {
                        #region exit logic
                        if ((findin && !findout) && (
                             ((sumBeta + tempitem.beta) <= (maxBeta * (decimal)0.65))
                            //item.close < (lowEaring * item.spyclose / spyearingdate)
                                          //  || avgBeta < -bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = Math.Min(lowEaring * item.spyclose / spyearingdate, item.close);
                            priceout = item.close;
                            tempitem.pnl = size * (priceout - pricein);
                            tempitem.daypnl = size * (priceout - item.open);
                            tempitem.overnightpnl = size * (item.open - pricein);
                            tempitem.priceout = priceout;

                            #region short spy hedge on close
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = -spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            ReportEaring.Add(tempitem);
                            findout = true;
                        }
                        #endregion

                        #region get pnl

                        if ((findin && !findout))
                        {

                            tempitem.pnl = size * (item.close - pricein);
                            tempitem.daypnl = size * (item.close - item.open);
                            tempitem.overnightpnl = size * (item.open - pricein);
                            tempitem.priceout = item.close;
                            tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;

                            #region short spy hedge
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = -spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = -spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = -spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            if (Beta5Ds.Count == 5)
                            {
                                avgBeta = Beta5Ds.Average();
                                Beta5Ds.Dequeue();
                                Beta5Ds.Enqueue(tempitem.beta);
                            }
                            else
                            {
                                Beta5Ds.Enqueue(tempitem.beta);
                            }

                            #endregion

                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;

                            #region beta issue
                            sumBeta = sumBeta + tempitem.beta;
                            if (sumBeta > maxBeta)
                                maxBeta = sumBeta;
                            #endregion
                        }
                        #endregion

                        #region entry logic

                        if (!findin && item.high >= adjustEarningHigh)
                        {
                            pricein = Math.Max(adjustEarningHigh, item.open);
                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (item.close - pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterlow = item.low;

                            tempitem.ExitSignalprice = lowEaring * item.spyclose / spyearingdate;

                            #region beta issue
                            sumBeta = tempitem.beta;
                            maxBeta = tempitem.beta;
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;

                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;
                            findin = true;
                            // spyin = item.spyclose;




                        }

                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninghigh = highEaring * item.spyclose / spyearingdate;
                            adjustEarningHigh = highEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion

                    }
                    else
                    {
                        #region earnign date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        findin = false;
                        findout = false;
                        //entrydate = false;
                        // exitdate = false;
                        size = 0;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = 10000000;
                        Beta5Ds.Clear();

                        sumBeta = 0;
                        maxBeta = 0;
                        #endregion
                    }
                    #endregion
                }
                if (tempitem.threshold < 0)
                {
                    #region short

                    if (item.symbol == symbol)
                    {
                        #region exit logic
                        if ((findin && !findout) && (
                             ((sumBeta+tempitem.beta)>=(maxBeta*(decimal)0.65))
                           // item.close > (highEaring * item.spyclose / spyearingdate)
                              //  || avgBeta > bate
                            || item.date == lastdate
                            ))
                        {
                            priceout = Math.Max((highEaring * item.spyclose / spyearingdate), item.close);
                            priceout = item.close;

                            tempitem.pnl = size * (-priceout + pricein);
                            tempitem.daypnl = size * (-priceout + item.open);
                            tempitem.overnightpnl = size * (-item.open + pricein);
                            tempitem.priceout = priceout;

                            #region long spy hedge on close
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            ReportEaring.Add(tempitem);
                            findout = true;
                        }
                        #endregion

                        #region get pnl
                        if ((findin && !findout))
                        {


                            tempitem.pnl = size * (-item.close + pricein);
                            tempitem.daypnl = size * (-item.close + item.open);
                            tempitem.overnightpnl = size * (-item.open + pricein);
                            tempitem.priceout = item.close;
                            tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;
                            #region long spy hedge
                            spysize = Convert.ToInt32(buyinpower / spyin);
                            tempitem.spypnl = spysize * (item.spyclose - spyin);
                            tempitem.dayspypnl = spysize * (item.spyclose - item.spyopen);
                            tempitem.overnightspypnl = spysize * (item.spyopen - spyin);
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.5)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            if (Beta5Ds.Count == 5)
                            {
                                avgBeta = Beta5Ds.Average();
                                Beta5Ds.Dequeue();
                                Beta5Ds.Enqueue(tempitem.beta);
                            }
                            else
                            {
                                Beta5Ds.Enqueue(tempitem.beta);
                            }

                            #endregion

                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;

                            #region beta issue
                            sumBeta = sumBeta + tempitem.beta;
                            if (sumBeta < maxBeta)
                                maxBeta = sumBeta;
                            #endregion
                        }
                        #endregion

                        #region entry logic
                        if (!findin && item.low <= adjustEarningLow)
                        {
                            pricein = Math.Min(adjustEarningLow, item.open); ;

                            size = Convert.ToInt32(buyinpower / pricein);
                            tempitem.pnl = size * (-item.close + pricein);
                            tempitem.daypnl = tempitem.pnl;
                            tempitem.overnightpnl = 0;
                            enterhigh = item.high;
                            tempitem.pricein = pricein;
                            tempitem.ExitSignalprice = highEaring * item.spyclose / spyearingdate;

                            #region beta issue
                            sumBeta = tempitem.beta;
                            maxBeta = tempitem.beta;
                            #endregion

                            #region win10 or lose10
                            if (tempitem.pnl / (pricein * size) >= (decimal)0.1)
                            {
                                tempitem.win10 = 1;
                            }
                            else
                            {
                                tempitem.win10 = 0;
                            }
                            if (tempitem.pnl / (pricein * size) <= -(decimal)0.1)
                            {
                                tempitem.lose10 = 1;
                            }
                            else
                            {
                                tempitem.lose10 = 0;
                            }
                            #endregion

                            #region 5 days beta array
                            Beta5Ds.Enqueue(tempitem.beta);
                            #endregion

                            tempitem.pricein = pricein;
                            ReportEaring.Add(tempitem);
                            pricein = item.close;
                            spyin = item.spyclose;

                            findin = true;




                        }
                        #endregion

                        #region adj
                        if (!findin)
                        {
                            adjear.symbol = item.symbol;
                            adjear.date = item.date;
                            adjear.threshold = item.threshold;
                            adjear.adjearninglow = lowEaring * item.spyclose / spyearingdate;
                            adjustEarningLow = lowEaring * item.spyclose / spyearingdate;
                            adjE.Add(adjear);
                        }
                        #endregion


                    }
                    else
                    {
                        #region earning date

                        symbol = item.symbol;
                        spyearingdate = item.spyclose;
                        highEaring = item.high;
                        lowEaring = item.low;
                        enterhigh = 0;
                        enterlow = 0;
                        pricein = 0;
                        priceout = 0;
                        findin = false;
                        findout = false;
                        //entrydate = false;
                        //exitdate = false;
                        size = 0;

                        adjustEarningHigh = item.high;
                        adjustEarningLow = item.low;

                        spyin = 0;
                        spysize = 0;
                        avgBeta = -10000000;
                        Beta5Ds.Clear();

                        sumBeta = 0;
                        maxBeta = 0;
                        #endregion
                    }

                    #endregion
                }

            }

            #region create file
            FileHelperEngine enginDR = new FileHelperEngine(typeof(DailyDataout));
            enginDR.HeaderText = "symbol,date,open,high,low,close,volume,Exist signal price,pricein,priceout,threshold,spy close,spy open,spy high, spy low,adv,win10,lose10,overnight pnl,day pnl,pnl,spy overnight pnl,spy day pnl, spy pnl,bp,beta";
            enginDR.WriteFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), ReportEaring);

            FileHelperEngine enginadj = new FileHelperEngine(typeof(AdjustEaringdata));
            enginadj.HeaderText = "symbol,date,threshold,adj earning high, adj earing low";
            enginadj.WriteFile(string.Format("DailyReport\\{0}.{1}.E.csv", quarter, Convert.ToInt32(threshold * new decimal(100))), adjE);
            #endregion

            #endregion
        }





        private static void DailyGroupbysymbol(int quarter, decimal threshold)
        {
            FileHelperEngine EngineOut = new FileHelperEngine(typeof(DailyDataout));
             DailyDataout[] ReportEaring = (DailyDataout[])EngineOut.ReadFile(string.Format("DailyReport\\{0}.{1}.D.csv", quarter, Convert.ToInt32(threshold * new decimal(100))));

             var result = ReportEaring.GroupBy(d => d.symbol).Select(d => new DailyReportSymbol
             {
                 symbol=d.Key,
                 entrydate=d.OrderBy(dd=>dd.date).Select(dd=>dd.date).First(),
                 Exitdate=d.OrderBy(dd=>dd.date).Select(dd=>dd.date).Last(),
                 pricein = d.OrderBy(dd => dd.date).Select(dd => dd.pricein).First(),
                 priceout = d.OrderBy(dd => dd.date).Select(dd => dd.priceout).Last(),
                 lose10 = d.Sum(dd => dd.lose10),
                 win10 = d.Sum(dd => dd.win10),
                 threshold=d.Select(dd=>dd.threshold).First(),
                 pnl = d.Sum(dd => dd.pnl),
                 pnlovernight = d.Sum(dd => dd.overnightpnl) ,
                 pnlday = d.Sum(dd => dd.daypnl)  ,
                 dayspypnl=d.Sum(dd => dd.dayspypnl),
                 overnightspypnl = d.Sum(dd => dd.overnightspypnl),
                 spypnl = d.Sum(dd => dd.spypnl),
                 hedgePnl = d.Sum(dd => dd.pnl) + d.Sum(dd => dd.spypnl),
             });

            FileHelperEngine EngineDailySymbol = new FileHelperEngine(typeof(DailyReportSymbol));
            EngineDailySymbol.HeaderText = "symbol,entrydate,exitdate,pricein,pricnout,thrreshold,win10,lose10,spyovernight pnl,spy day pnl, spy pnl,pnl over night,pnl day,pnl,total pnl";
            EngineDailySymbol.WriteFile(string.Format("DailyReport\\{0}.{1}.Ds.csv", quarter, Convert.ToInt32(threshold * new decimal(100))),result);

        }

    }

    #region class
    [DelimitedRecord(",")]
	[IgnoreFirst(1)]
	public class quaterReport
	{
		public int qua;

        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime lastestExitday;

		public decimal DollarSharpe;

        public decimal AccPnlday;

        public decimal AccPnlovernight;

		public decimal AccPnl;

       

		public decimal lowestPnl;

		public decimal highestPnl;

		public decimal maxbp;

		public int maxbplongcount;

		public int maxbpshortcount;

		public int gapsWin10;

		public int gaplos10;

		public quaterReport()
		{
		}
	}

     [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class DailyReport
	{
       
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        private DateTime _date;

		private decimal _longpnl;

		private decimal _shortpnl;

		private int _longcount;

		private int _shortcount;

		//private decimal _spyclose;

		

		private int _qua;

		private decimal _dollarSharp;

		private decimal _bp;

		private int _win10;

		private int _lose10;
         private decimal _hspypnl;
        private decimal _pnlovernight;

        private decimal _pnlday;

        private decimal _pnl;

      


        //private decimal _accpnlovernight;

        //private decimal _accpnlday;

        private decimal _accPnl;

		public decimal accPnl
		{
			get
			{
				decimal num = this._accPnl;
				return num;
			}
			set
			{
				this._accPnl = value;
			}
		}

		public decimal bp
		{
			get
			{
				decimal num = this._bp;
				return num;
			}
			set
			{
				this._bp = value;
			}
		}

		public DateTime date
		{
			get
			{
				DateTime dateTime = this._date;
				return dateTime;
			}
			set
			{
				this._date = value;
			}
		}

		public decimal dollarSharp
		{
			get
			{
				decimal num = this._dollarSharp;
				return num;
			}
			set
			{
				this._dollarSharp = value;
			}
		}

		public decimal hspypnl
		{
			get
			{
				decimal num = this._hspypnl;
				return num;
			}
			set
			{
				this._hspypnl = value;
			}
		}

		public int longcount
		{
			get
			{
				int num = this._longcount;
				return num;
			}
			set
			{
				this._longcount = value;
			}
		}

		public decimal longpnl
		{
			get
			{
				decimal num = this._longpnl;
				return num;
			}
			set
			{
				this._longpnl = value;
			}
		}

		public int lose10
		{
			get
			{
				int num = this._lose10;
				return num;
			}
			set
			{
				this._lose10 = value;
			}
		}

		public decimal pnl
		{
			get
			{
				decimal num = this._pnl;
				return num;
			}
			set
			{
				this._pnl = value;
			}
		}

		public int qua
		{
			get
			{
				int num = this._qua;
				return num;
			}
			set
			{
				this._qua = value;
			}
		}

		public int shortcount
		{
			get
			{
				int num = this._shortcount;
				return num;
			}
			set
			{
				this._shortcount = value;
			}
		}

		public decimal shortpnl
		{
			get
			{
				decimal num = this._shortpnl;
				return num;
			}
			set
			{
				this._shortpnl = value;
			}
		}

       

        //public decimal spyclose
        //{
        //    get
        //    {
        //        decimal num = this._spyclose;
        //        return num;
        //    }
        //    set
        //    {
        //        this._spyclose = value;
        //    }
        //}

		public int win10
		{
			get
			{
				int num = this._win10;
				return num;
			}
			set
			{
				this._win10 = value;
			}
		}

        public decimal pnlovernight
        {
            get
            {
                decimal num = this._pnlovernight;
                return num;
            }
            set
            {
                this._pnlovernight = value;
            }
        }

        public decimal pnlday
        {
            get
            {
                decimal num = this._pnlday;
                return num;
            }
            set
            {
                this._pnlday = value;
            }
        }


        //public decimal accpnlovernight
        //{
        //    get
        //    {
        //        decimal num = this._accpnlovernight;
        //        return num;
        //    }
        //    set
        //    {
        //        this._accpnlovernight = value;
        //    }
        //}

        //public decimal accpnlday
        //{
        //    get
        //    {
        //        decimal num = this._accpnlday;
        //        return num;
        //    }
        //    set
        //    {
        //        this._accpnlday = value;
        //    }
        //}
	}


     [DelimitedRecord(",")]
     [IgnoreFirst(1)]
     public class DailyReportSymbol
     {
         public string symbol;
         [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime entrydate;
         [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
         public DateTime Exitdate;
         public decimal pricein;
         public decimal priceout;
         public decimal threshold;
         public int win10;
         public int lose10;
         public decimal overnightspypnl;
         public decimal dayspypnl;
         public decimal spypnl;
         public decimal pnlovernight;
         public decimal pnlday;
         public decimal pnl;
         public decimal hedgePnl;
     }

     [DelimitedRecord(",")]
     [IgnoreFirst(1)]
     public class AdjustEaringdata
     {
         public string symbol;
         [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
         public DateTime date;
         public decimal threshold;
         public decimal adjearninghigh;
         public decimal adjearninglow;
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
[IgnoreFirst(1)]
public class RowDailyData
{
    public string ID;

    [FieldConverter(typeof(RemoveQuotFromString))]
    public string symbol;

    [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]   
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

#endregion

}
