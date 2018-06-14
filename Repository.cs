using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    class Repository
    {
        public List<AllCars> Cars { get; set; }
        public List<HelperForSpeed> helperForSpeed { get; set; }



        public Repository()
        {
            Cars = new List<AllCars>();
            helperForSpeed = new List<HelperForSpeed>();
            Console.WriteLine("Происходит создание базы данных. Это может занять пару минут(в зависимости от скорости вашего интернет соединения). Пожалуйста, подождите...");
            ParseHtmlToRepository(Cars, helperForSpeed);
        }
        static void AllCarsToUser(List<AllCars> allCars, List<HelperForSpeed> helperForSpeed)
        {

            string msg = "";
            for (int j = 0; j < allCars.Count; j++)
            {
                if (j == allCars.Count - 1)
                    msg += allCars[j].name;
                else
                    msg += allCars[j].name + ", ";

            }
            var helper = new HelperForSpeed
            {
                CarsInformation = msg
            };
            helperForSpeed.Add(helper);

        }
        static void ParseHtmlToRepository(List<AllCars> Cars, List<HelperForSpeed> helperForSpeed)
        {

            string URL = "http://www.motorpage.ru";
            string allCarsUrl = URL.Insert(URL.Length, "/select-auto/by-mark.html");

            var htmlDocument = LoadHtmlDocument(allCarsUrl);

            string carsInf = FindSpecialBlockInHtmlDocument(htmlDocument, "div", "id", "all-marks");

            AddAllCarsInRepo(carsInf, URL, Cars);

            AllCarsToUser(Cars, helperForSpeed);


            for (int j = 0; j < Cars.Count; j++)
            {
                htmlDocument = LoadHtmlDocument(Cars[j].url);

                string modelsInf = FindSpecialBlockInHtmlDocument(htmlDocument, "div", "id", "all-models");

                AddAllModelsInRepo(Cars, modelsInf, URL, j);


                for (int g = 0; g < Cars[j].models.Count; g++)
                {
                    htmlDocument = LoadHtmlDocument(Cars[j].models[g].Url);

                    string characteristicsInf = FindSpecialBlocksInHtmlDocument(htmlDocument, "tbody");

                    AddAllVarietiesInRepo(Cars, characteristicsInf, j, g);


                    MatchCollection NumberOfColumns = CountNumberOfColumns(Cars, j, g);

                    for (int h = 0; h < Cars[j].models[g].Varieties.Count; h++)
                    {
                        int remainder = NumberOfColumns.Count % 8;

                        string sourceStr = UTF8ToWin1251(Cars[j].models[g].Varieties[h].Text);

                        MatchCollection m0 = Regex.Matches(sourceStr, @"<td>(.+?)<\/td>", RegexOptions.Singleline);
                        MatchCollection m1 = Regex.Matches(m0[0].Groups[1].Value, @":\s(\w-?\w*)\W", RegexOptions.Singleline);
                        MatchCollection m2 = Regex.Matches(sourceStr, @">(.*?)<", RegexOptions.Singleline);
                        var listCharacteristics = new List<Characteristics>();

                        if (0 == remainder)
                        {
                            MatchCollection m3 = Regex.Matches(m2[10].Groups[1].Value, @";(.+?)$", RegexOptions.Singleline);
                            if (m2.Count == 21)
                            {
                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string a = "Cars without price:";
                                ColorMessage(a, ConsoleColor.Blue);
                                */
                                WithoutPrice(Cars, j, g, h, m1, m2, m3, listCharacteristics);
                            }

                            else if (m2.Count == 22)
                            {
                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string a = "Everything is good)";
                                ColorMessage(a, ConsoleColor.White);
                                */
                                WhenEverythingIsOk(Cars, j, g, h, m1, m2, m3, listCharacteristics);

                            }

                            else
                            {
                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string a = "Something Wrong";
                                ColorMessage(a, ConsoleColor.Red);
                                */
                            }



                        }
                        else
                        {
                            /*
                            string a = "Отсутсвует столбец комплектации!";
                            ColorMessage(a, ConsoleColor.Yellow);
                            */
                            MatchCollection m3 = Regex.Matches(m2[8].Groups[1].Value, @";(.+?)$", RegexOptions.Singleline);
                            if (m2.Count == 20)
                            {
                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string b = "Есть цена";
                                ColorMessage(b, ConsoleColor.DarkYellow);
                                */
                                WithoutCarKit(Cars, j, g, h, m1, m2, m3, listCharacteristics);
                            }
                            else if (m2.Count == 19)
                            {


                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string b = "Цена отсутсвует";
                                ColorMessage(b, ConsoleColor.Green);
                                */
                                WithoutCarKitAndPrice(Cars, j, g, h, m1, m2, m3, listCharacteristics);
                            }
                            else
                            {
                                /*
                                Console.WriteLine("Марка: {0}, Модель: {1}, idМарки: {2}, idМодели: {3}, h: {4}", allCars[j].name, allCars[j].models[g].Name, j, g, h);
                                string b = "Something Wrong";
                                ColorMessage(b, ConsoleColor.Red);
                                */

                            }
                        }
                    }


                }


            }


        }
        static string UTF8ToWin1251(string source)
        {
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.GetEncoding("windows-1251");

            byte[] win1251Bytes = utf8.GetBytes(source);
            byte[] utf8Bytes = Encoding.Convert(utf8, win1251, win1251Bytes);
            source = utf8.GetString(utf8Bytes);
            return source;

        }
        static MatchCollection WithoutCarKitHtml(string htmlSourse)
        {
            MatchCollection m = Regex.Matches(htmlSourse, @"<th(.+?)(<\/th>)", RegexOptions.Singleline);
            return m;
        }
        static void WithoutPrice(List<AllCars> allCars, int j, int g, int h, MatchCollection m1, MatchCollection m2, MatchCollection m3, List<Characteristics> listCharacteristics)
        {

            /*
            Console.WriteLine(m2[2].Groups[1].Value.Insert(0, "Мотор: "));
            Console.WriteLine(m1[0].Groups[1].Value.Insert(0, "КПП: "));
            Console.WriteLine(m1[1].Groups[1].Value.Insert(0, "Турбонадув: "));
            Console.WriteLine(m1[2].Groups[1].Value.Insert(0, "Компановка: "));
            Console.WriteLine(m2[5].Groups[1].Value.Insert(0, "Лошадиные силы: "));
            Console.WriteLine(m2[8].Groups[1].Value.Insert(0, "Комплектация: "));
            Console.WriteLine(m2[12].Groups[1].Value.Insert(0, "Топливо: "));
            Console.WriteLine(m2[14].Groups[1].Value.Insert(0, "Привод: "));
            Console.WriteLine(m2[16].Groups[1].Value.Insert(0, "Расход топлива: "));
            Console.WriteLine(m2[18].Groups[1].Value.Insert(0, "Разгон до 100 км/ч: "));
            Console.WriteLine(m2[20].Groups[1].Value.Insert(0, "Максимальная скорость: "));
            Console.WriteLine("Примерная цена: к сожалению отсутствует");
            Console.WriteLine(" ");
            */
            // Console.WriteLine(m3[0].Groups[1].Value.Insert(m3[0].Groups[1].Value.Length, m2[11].Groups[1].Value).Insert(0, "Примерная цена: "));

            string motor = m2[2].Groups[1].Value;
            string transmission = m1[0].Groups[1].Value;
            string turbo = m1[1].Groups[1].Value;
            string arrangement = m1[2].Groups[1].Value;
            string horsePower = m2[5].Groups[1].Value;
            string carKit = m2[8].Groups[1].Value;
            string fuel = m2[12].Groups[1].Value;
            string driveUnit = m2[14].Groups[1].Value;
            string fuelConsumption = m2[16].Groups[1].Value;
            string accelerationToHundreds = m2[18].Groups[1].Value;
            string maxSpeed = m2[20].Groups[1].Value;
            string primerelyPrice = ("Примерная цена: к сожалению отсутствует");


            var characteristics = new Characteristics
            {
                Motor = motor.Insert(0, "Мотор: "),
                Transmission = transmission.Insert(0, "КПП: "),
                Turbo = turbo.Insert(0, "Турбонадув: "),
                Arrangement = arrangement.Insert(0, "Компановка: "),
                HorsePower = horsePower.Insert(0, "Лошадиные силы: "),
                CarKit = carKit.Insert(0, "Комплектация: "),
                Fuel = fuel.Insert(0, "Топливо: "),
                DriveUnit = driveUnit.Insert(0, "Привод: "),
                FuelConsumption = fuelConsumption.Insert(0, "Расход топлива: "),
                AccelerationToHundreds = accelerationToHundreds.Insert(0, "Разгон до 100 км/ч: "),
                MaxSpeed = maxSpeed.Insert(0, "Максимальная скорость: "),
                PrimerelyPrice = primerelyPrice

            };
            allCars[j].models[g].Varieties[h].MainCharacteristics.Add(characteristics);
        }
        static void WhenEverythingIsOk(List<AllCars> allCars, int j, int g, int h, MatchCollection m1, MatchCollection m2, MatchCollection m3, List<Characteristics> listCharacteristics)
        {
            /*
            Console.WriteLine(m2[2].Groups[1].Value.Insert(0, "Мотор: "));
            Console.WriteLine(m1[0].Groups[1].Value.Insert(0, "КПП: "));
            Console.WriteLine(m1[1].Groups[1].Value.Insert(0, "Турбонадув: "));
            Console.WriteLine(m1[2].Groups[1].Value.Insert(0, "Компановка: "));
            Console.WriteLine(m2[5].Groups[1].Value.Insert(0, "Лошадиные силы: "));
            Console.WriteLine(m2[8].Groups[1].Value.Insert(0, "Комплектация: "));
            Console.WriteLine(m2[13].Groups[1].Value.Insert(0, "Топливо: "));
            Console.WriteLine(m2[15].Groups[1].Value.Insert(0, "Привод: "));
            Console.WriteLine(m2[17].Groups[1].Value.Insert(0, "Расход топлива: "));
            Console.WriteLine(m2[19].Groups[1].Value.Insert(0, "Разгон до 100 км/ч: "));
            Console.WriteLine(m2[21].Groups[1].Value.Insert(0, "Максимальная скорость: "));
            Console.WriteLine(m3[0].Groups[1].Value.Insert(m3[0].Groups[1].Value.Length, m2[11].Groups[1].Value).Insert(0, "Примерная цена: "));
            Console.WriteLine(" ");
            */
            string motor = m2[2].Groups[1].Value;
            string transmission = m1[0].Groups[1].Value;
            string turbo = m1[1].Groups[1].Value;
            string arrangement = m1[2].Groups[1].Value;
            string horsePower = m2[5].Groups[1].Value;
            string carKit = m2[8].Groups[1].Value;
            string fuel = m2[13].Groups[1].Value;
            string driveUnit = m2[15].Groups[1].Value;
            string fuelConsumption = m2[17].Groups[1].Value;
            string accelerationToHundreds = m2[19].Groups[1].Value;
            string maxSpeed = m2[21].Groups[1].Value;
            string primerelyPrice = m3[0].Groups[1].Value.Insert(m3[0].Groups[1].Value.Length, m2[11].Groups[1].Value);
            var characteristics = new Characteristics
            {
                Motor = motor.Insert(0, "Мотор: "),
                Transmission = transmission.Insert(0, "КПП: "),
                Turbo = turbo.Insert(0, "Турбонадув: "),
                Arrangement = arrangement.Insert(0, "Компановка: "),
                HorsePower = horsePower.Insert(0, "Лошадиные силы: "),
                CarKit = carKit.Insert(0, "Комплектация: "),
                Fuel = fuel.Insert(0, "Топливо: "),
                DriveUnit = driveUnit.Insert(0, "Привод: "),
                FuelConsumption = fuelConsumption.Insert(0, "Расход топлива: "),
                AccelerationToHundreds = accelerationToHundreds.Insert(0, "Разгон до 100 км/ч: "),
                MaxSpeed = maxSpeed.Insert(0, "Максимальная скорость: "),
                PrimerelyPrice = primerelyPrice.Insert(0, "Примерная цена: ")

            };
            allCars[j].models[g].Varieties[h].MainCharacteristics.Add(characteristics);
        }
        static void WithoutCarKit(List<AllCars> allCars, int j, int g, int h, MatchCollection m1, MatchCollection m2, MatchCollection m3, List<Characteristics> listCharacteristics)
        {
            /*
            Console.WriteLine(m2[2].Groups[1].Value.Insert(0, "Мотор: "));
            Console.WriteLine(m1[0].Groups[1].Value.Insert(0, "КПП: "));
            Console.WriteLine(m1[1].Groups[1].Value.Insert(0, "Турбонадув: "));
            Console.WriteLine(m1[2].Groups[1].Value.Insert(0, "Компановка: "));
            Console.WriteLine(m2[5].Groups[1].Value.Insert(0, "Лошадиные силы: "));
            Console.WriteLine("Комплектация: отсутствует");
            Console.WriteLine(m2[11].Groups[1].Value.Insert(0, "Топливо: "));
            Console.WriteLine(m2[13].Groups[1].Value.Insert(0, "Привод: "));
            Console.WriteLine(m2[15].Groups[1].Value.Insert(0, "Расход топлива: "));
            Console.WriteLine(m2[17].Groups[1].Value.Insert(0, "Разгон до 100 км/ч: "));
            Console.WriteLine(m2[19].Groups[1].Value.Insert(0, "Максимальная скорость: "));
            Console.WriteLine(m3[0].Groups[1].Value.Insert(m3[0].Groups[1].Value.Length, m2[9].Groups[1].Value).Insert(0, "Примерная цена: "));
            Console.WriteLine(" ");
            */
            string motor = m2[2].Groups[1].Value;
            string transmission = m1[0].Groups[1].Value;
            string turbo = m1[1].Groups[1].Value;
            string arrangement = m1[2].Groups[1].Value;
            string horsePower = m2[5].Groups[1].Value;
            string carKit = "Отсутствует";
            string fuel = m2[10].Groups[1].Value;
            string driveUnit = m2[12].Groups[1].Value;
            string fuelConsumption = m2[14].Groups[1].Value;
            string accelerationToHundreds = m2[16].Groups[1].Value;
            string maxSpeed = m2[18].Groups[1].Value;
            string primerelyPrice = m3[0].Groups[1].Value.Insert(m3[0].Groups[1].Value.Length, m2[11].Groups[1].Value);
            var characteristics = new Characteristics
            {
                Motor = motor.Insert(0, "Мотор: "),
                Transmission = transmission.Insert(0, "КПП: "),
                Turbo = turbo.Insert(0, "Турбонадув: "),
                Arrangement = arrangement.Insert(0, "Компановка: "),
                HorsePower = horsePower.Insert(0, "Лошадиные силы: "),
                CarKit = carKit.Insert(0, "Комплектация: "),
                Fuel = fuel.Insert(0, "Топливо: "),
                DriveUnit = driveUnit.Insert(0, "Привод: "),
                FuelConsumption = fuelConsumption.Insert(0, "Расход топлива: "),
                AccelerationToHundreds = accelerationToHundreds.Insert(0, "Разгон до 100 км/ч: "),
                MaxSpeed = maxSpeed.Insert(0, "Максимальная скорость: "),
                PrimerelyPrice = primerelyPrice.Insert(0, "Примерная цена: ")

            };
            allCars[j].models[g].Varieties[h].MainCharacteristics.Add(characteristics);
        }
        static void WithoutCarKitAndPrice(List<AllCars> allCars, int j, int g, int h, MatchCollection m1, MatchCollection m2, MatchCollection m3, List<Characteristics> listCharacteristics)
        {
            /*
            Console.WriteLine(m2[2].Groups[1].Value.Insert(0, "Мотор: "));
            Console.WriteLine(m1[0].Groups[1].Value.Insert(0, "КПП: "));
            Console.WriteLine(m1[1].Groups[1].Value.Insert(0, "Турбонадув: "));
            Console.WriteLine(m1[2].Groups[1].Value.Insert(0, "Компановка: "));
            Console.WriteLine(m2[5].Groups[1].Value.Insert(0, "Лошадиные силы: "));
            Console.WriteLine("Комплектация: отсутствует");
            Console.WriteLine(m2[10].Groups[1].Value.Insert(0, "Топливо: "));
            Console.WriteLine(m2[12].Groups[1].Value.Insert(0, "Привод: "));
            Console.WriteLine(m2[14].Groups[1].Value.Insert(0, "Расход топлива: "));
            Console.WriteLine(m2[16].Groups[1].Value.Insert(0, "Разгон до 100 км/ч: "));
            Console.WriteLine(m2[18].Groups[1].Value.Insert(0, "Максимальная скорость: "));
            Console.WriteLine("Цена отсутсвует");
            Console.WriteLine(" ");
            */
            string motor = m2[2].Groups[1].Value;
            string transmission = m1[0].Groups[1].Value;
            string turbo = m1[1].Groups[1].Value;
            string arrangement = m1[2].Groups[1].Value;
            string horsePower = m2[5].Groups[1].Value;
            string carKit = "Отсутствует";
            string fuel = m2[10].Groups[1].Value;
            string driveUnit = m2[12].Groups[1].Value;
            string fuelConsumption = m2[14].Groups[1].Value;
            string accelerationToHundreds = m2[16].Groups[1].Value;
            string maxSpeed = m2[18].Groups[1].Value;
            string primerelyPrice = "Отсутсвует";
            var characteristics = new Characteristics
            {
                Motor = motor.Insert(0, "Мотор: "),
                Transmission = transmission.Insert(0, "КПП: "),
                Turbo = turbo.Insert(0, "Турбонадув: "),
                Arrangement = arrangement.Insert(0, "Компановка: "),
                HorsePower = horsePower.Insert(0, "Лошадиные силы: "),
                CarKit = carKit.Insert(0, "Комплектация: "),
                Fuel = fuel.Insert(0, "Топливо: "),
                DriveUnit = driveUnit.Insert(0, "Привод: "),
                FuelConsumption = fuelConsumption.Insert(0, "Расход топлива: "),
                AccelerationToHundreds = accelerationToHundreds.Insert(0, "Разгон до 100 км/ч: "),
                MaxSpeed = maxSpeed.Insert(0, "Максимальная скорость: "),
                PrimerelyPrice = primerelyPrice.Insert(0, "Примерная цена: ")

            };
            allCars[j].models[g].Varieties[h].MainCharacteristics.Add(characteristics);
        }
        static HtmlDocument LoadHtmlDocument(string allCarsUrl)
        {
            var webClient = new System.Net.WebClient();
            string HTML = webClient.DownloadString(allCarsUrl);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(HTML);
            return htmlDocument;
        }
        static string FindSpecialBlockInHtmlDocument(HtmlDocument htmlDocument, string block, string initializer, string wordInInitializer)
        {
            var carsHtml = htmlDocument.DocumentNode.Descendants(block).Where(node => node.GetAttributeValue(initializer, "").Equals(wordInInitializer)).ToList();
            var carsList = carsHtml[0].InnerHtml.ToArray();
            string carsInf = string.Join("", carsList);
            return carsInf;
        }
        static void AddAllCarsInRepo(string carsInf, string URL, List<AllCars> Cars)
        {

            MatchCollection m1 = Regex.Matches(carsInf, "<a \\s*(.+?)\\s*</a>", RegexOptions.Singleline);

            foreach (Match m in m1)
            {
                string carHtml = m.Groups[1].Value;
                string line;
                string[] parts;

                line = carHtml.ToString();
                parts = line.Split('"');

                var car = new AllCars
                {
                    url = URL.Insert(URL.Length, parts[1]),
                    name = UTF8ToWin1251(parts[2].Trim('>'))
                };
                Cars.Add(car);
            }

        }
        static void AddAllModelsInRepo(List<AllCars> allCars, string modelsInf, string URL, int j)
        {
            MatchCollection m1 = Regex.Matches(modelsInf, "<a \\s*(.+?)\\s*</a>", RegexOptions.Singleline);
            foreach (Match m in m1)
            {
                string cAr = m.Groups[1].Value;
                var line = cAr.ToString();
                string[] parts = line.Split('"');
                MatchCollection mc1 = Regex.Matches(parts[2], "(?<=<h3>)(.*)(?=</h3>)", RegexOptions.Singleline);
                foreach (Match mc in mc1)
                {
                    var a = URL.Insert(URL.Length, parts[1]);
                    var b = UTF8ToWin1251(mc.Groups[1].Value);
                    var model = new Models
                    {
                        Url = a,
                        Name = b
                    };

                    allCars[j].models.Add(model);

                }
            }
        }
        static string FindSpecialBlocksInHtmlDocument(HtmlDocument htmlDocument, string block)
        {
            var characteristicsHtml = htmlDocument.DocumentNode.Descendants(block).ToList();
            var characteristicsList = characteristicsHtml[0].InnerHtml.ToArray();
            string characteristicsInf = string.Join("", characteristicsList);
            return characteristicsInf;
        }
        static void AddAllVarietiesInRepo(List<AllCars> allCars, string characteristicsInf, int j, int g)
        {
            MatchCollection m1 = Regex.Matches(characteristicsInf, "<tr \\s*(.+?)\\s*</tr>", RegexOptions.Singleline);
            var varieties = new List<Varieties>();
            foreach (Match m in m1)
            {
                string variety = m.Groups[1].Value;
                var mainCharacteristics = new Varieties
                {
                    Text = variety
                };
                allCars[j].models[g].Varieties.Add(mainCharacteristics);
            }
        }
        static MatchCollection CountNumberOfColumns(List<AllCars> allCars, int j, int g)
        {
            var webClient = new System.Net.WebClient();
            var HTML = webClient.DownloadString(allCars[j].models[g].Url);
            var NumberOfColumns = WithoutCarKitHtml(UTF8ToWin1251(HTML));
            return NumberOfColumns;
        }
        static string CheckWhiteSpaces(string word)
        {
            bool boolFunction = String.IsNullOrWhiteSpace(word);
            if (boolFunction == true)
            {
                return "Отсутсвует";
            }
            else
                return word;
        }
    }
}
