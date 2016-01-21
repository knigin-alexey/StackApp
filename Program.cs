using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourthApp
{
    /*
    * !!!При первом запуске файл входных данных генерируется автоматически!!!
    *
    * В течении дня в банк заходят люди, у каждого человека есть время 
    * захода в банк и время выхода. Всего за день у банка было N посетителей. 
    * Банк работает с 8:00 до 20:00. Человек посещает банк только один раз за день. 
    * Написать программу, которая определяет периоды времени, когда в банке было 
    * максимальное количество посетителей. Входные данные о посетителях программа 
    * должна получать из файла.
    */
    class Program
    {
        static void Main(string[] args)
        {
            object[] visits = GetResult();
            Console.WriteLine("В период с " + visits[1]  + " до " + visits[2] 
                + " было максимальное количество посетителей " + visits[0] + ".");
            Console.ReadKey();
        }

        static void SetData()
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            Dictionary<DateTime, string> dict = CreateDict();
            var result = from pair in dict
                         orderby pair.Key ascending
                         select pair;

            try
            {
                using (StreamWriter generate = new StreamWriter("input.txt"))
                {
                    foreach(KeyValuePair<DateTime, string> pair in result)
                    {
                        generate.WriteLine(pair.Key.ToString("HH:mm:ss", ci) + ";"
                            + pair.Value + ";");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Houston we have a problem: " + e.Message);
            }
        }

        static Dictionary<DateTime, string> CreateDict()
        {
            Random r = new Random();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int openHour = 9;
            int closeHour = 21;
            int inMinutes;
            int outMinutes;
            Dictionary<DateTime, string> genDict = new Dictionary<DateTime, string>();
            int vistors = 20;


            for (int i = 0; i < vistors; i++)
            {
                DateTime inTime = new DateTime(year, month, day);
                DateTime outTime = new DateTime(year, month, day);
                inMinutes = r.Next(openHour * 60, closeHour * 60);
                outMinutes = r.Next(inMinutes, closeHour * 60);
                inTime = inTime.AddMinutes(inMinutes);
                outTime = outTime.AddMinutes(outMinutes);
                bool isSame = true;
                bool inDict = false;

                //Так как Dictionary - несортированная коллекция, необходимо
                //обеспечивать уникальность ключа
                //Флаг inDict обеспечивает отслеживание элемента, на котором
                //произошло исключение. Если in-time еще не попало в словарь,
                //то можно просто перегенерировать, если перегенерировать без
                //флага, то на каждое исключение, вызыванное дублированием
                //out-time будет лишний in-time
                //А если этот флаг не ставить и действовать с помощью
                //genDict.ContainsKey(inTime)-Remove, то в случае исключения на 
                //in-time будет убираться оригинальный элемент.
                while (isSame)
                {
                    try
                    {                        
                        genDict.Add(inTime, "in");
                        inDict = true;
                        genDict.Add(outTime, "out");
                        isSame = false;
                    }
                    catch (ArgumentException e)
                    {
                        if (inDict)
                        {
                            genDict.Remove(inTime);
                        }
                        i--;
                        break;
                    }
                }
            }

            return genDict;
        }

        static object[] GetResult()
        {
            bool isInit = false;
            object[] result = new object[3];
            int counter = 0;
            int maxVisit = 0;
            CultureInfo ci = CultureInfo.InvariantCulture;

            while (!isInit)
            {
                try
                {
                    using (StreamReader input = new StreamReader("input.txt"))
                    {
                        string line;
                        isInit = true;
                        char[] separator = { ';' };
                        
                        DateTime timeOfMaxIn = new DateTime();
                        DateTime timeOfMaxOut = new DateTime();

                        while ((line = input.ReadLine()) != null)
                        {
                            string[] values = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            switch (values[1])
                            {
                                case "in":
                                    counter++;
                                    if (maxVisit <= counter)
                                    {
                                        maxVisit = counter;
                                        timeOfMaxIn = DateTime.Parse(values[0]); 
                                    }
                                    break;
                                case "out":
                                    if (counter == maxVisit)
                                    {
                                        timeOfMaxOut = DateTime.Parse(values[0]);
                                    }
                                    counter--;                                    
                                    break;
                                default:
                                    break;
                            }
                        }

                        result[0] = maxVisit;
                        result[1] = timeOfMaxIn.ToString("HH:mm:ss", ci);
                        result[2] = timeOfMaxOut.ToString("HH:mm:ss", ci);
                    }
                }
                catch (FileNotFoundException e)
                {
                    SetData();
                    Console.WriteLine("Массив сгенерирован.");
                }
            }
            return result;
        }
    }
}
