using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VkNet;
using VkNet.Enums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Windows;

namespace Vk_Bot
{
    class Program
    {
        static VkApi vkapi = new VkApi();
        static long userID = 0;
        static ulong? Ts;
        static ulong? Pts;
        static bool IsActive;
        static Timer WatchTimer = null;
        static byte MaxSleepSteps = 3;
        static int StepSleepTime = 333;
        static byte CurrentSleepSteps = 1;
        delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<Message> messages);
        static event MessagesRecievedDelegate NewMessages;
        static Random _Random = new Random();
        static string CommandsPath = "";

        private static Repository _repository = new Repository();
        private static List<Counter> _counter = new List<Counter>();
        static void Main(string[] args)
        {
            string KEY = "13b47b7e3145e07700571bf78b25c33525b09fde14ee170842f964a1f2b04698fd7f5752344d00affc1bf";
            string q = "База данных успешно загружена!";
            ColorMessage(q, ConsoleColor.Green);
            Topic();
            Console.WriteLine("Попытка авторизации...");

            if (Auth(KEY))
            {

                ColorMessage("Авторизация успешно завершена.", ConsoleColor.Green);



                Eye();
                ColorMessage("Слежение за сообщениями активировано.", ConsoleColor.Green);
            }
            else
            {
                ColorMessage("Не удалось произвести авторизацию!", ConsoleColor.Red);
            }

            Console.WriteLine("Нажмите ENTER чтобы выйти...");
            Console.ReadLine();
        }

        static private void Topic()
        {
            Console.Title = "Car-Searcher Bot";
            ColorMessage("Car-Searcher Bot", ConsoleColor.DarkYellow);
        }





        static private bool Auth(string GroupID)
        {
            try
            {
                vkapi.Authorize(new ApiAuthParams { AccessToken = GroupID });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /*
        static private List<string> Commands()
        {
            var repo = ParseHtmlToRepository();
            var all
            for (int i = 0; i < repo.Count; i++)
            {
                allCars.add()
            }
        }
        */
        
        static private bool CheckMark(string Message, bool boolFunction)
        {
            
                for (int i = 0; i < _repository.Cars.Count; i++)
                {
                    if ((_repository.Cars[i].name.ToLower() == Message))
                    {
                        boolFunction = true;
                       
                    }

                }
            
            if (boolFunction == true)
                return boolFunction;
            else
                return boolFunction = false;
        }
        static private bool CheckModel(int n, string Message, bool boolFunction)
        {

           
                for (int i = 0; i < _repository.Cars.Count; i++)
                {
                    if (_repository.Cars[i].name.ToLower() == _counter[n].Storage[0])
                    {
                        for (int m = 0; m < _repository.Cars[i].models.Count; m++)
                        {
                            if (_repository.Cars[i].models[m].Name.ToLower() == Message)
                            {
                                boolFunction = true;

                            }

                        }
                    }
                }
            
            if (boolFunction == true)
                return boolFunction;
            else
                return boolFunction = false;

        }
        static private bool CheckVarieties(int n, string Message, bool boolFunction)

        {
            int number;
            bool result = Int32.TryParse(Message, out number);
            if (result)
            {
                
                    for (int i = 0; i < _repository.Cars.Count; i++)
                    {
                        if (_repository.Cars[i].name.ToLower() == _counter[n].Storage[0])
                        {
                            for (int m = 0; m < _repository.Cars[i].models.Count; m++)
                            {
                                if ((_repository.Cars[i].models[m].Name.ToLower() == _counter[n].Storage[1]) && (_repository.Cars[i].models[m].Varieties.Count >= number))
                                {
                                                                     
                                              boolFunction = true;
                                            
                                        
                                    
                                }

                            }
                        }
                    }
 
                
                if (boolFunction == true)
                    return boolFunction;
                else
                    return boolFunction = false;
            }
            else
            {
                return boolFunction = false;
            }
        }

        static private void Command(string Message, User Sender)
        {
            bool boolFunction = false;           
            Message = Message.ToLower();

            long idOfPerson = Sender.Id;
            var counter = _counter;

            if (counter.Count == 0)
            {
                var storage = new Counter
                {
                    IdOfPerson = idOfPerson,
                };
                counter.Add(storage);
            }
            else if (counter.Count != 0)
            {
                for (int n = 0; n < counter.Count; n++)
                {
                    if (counter[n].IdOfPerson != idOfPerson)

                    {
                        var storage = new Counter
                        {
                            IdOfPerson = idOfPerson,
                        };
                        counter.Add(storage);
                    }

                }
            }
            

            for (int n = 0; n < counter.Count; n++)
            {
                if (counter[n].IdOfPerson == idOfPerson)
                {
                    if (_counter[n].Storage.Count == 0)
                    {
                        boolFunction = CheckMark(Message, boolFunction);
                    }
                    else if (_counter[n].Storage.Count == 1)
                    {
                        boolFunction = CheckModel(n, Message, boolFunction);
                    }
                    else if (_counter[n].Storage.Count == 2)
                    {
                        boolFunction = CheckVarieties(n, Message, boolFunction);
                    }
                    
                        if ((Message == "help") && (counter[n].Storage.Count == 0))
                    {
                        SendMessage(_repository.helperForSpeed[0].CarsInformation);
                    }
                    else if ((Message == "привет" || Message == "здравствуй" || Message == "добрый день") && (counter[n].Storage.Count == 0))
                    {
                        SendMessage("Привет, назови мне любую марку машины и я расскажу тебе о ней поподробнее!)");
                    }
                    else if ((boolFunction == true) && (counter[n].Storage.Count == 0))
                    {
                        for (int i = 0; i < _repository.Cars.Count; i++)
                        {
                            if (_repository.Cars[i].name.ToLower() == Message)
                            {
                                string msg = "";
                                for (int j = 0; j < _repository.Cars[i].models.Count; j++)
                                {
                                    if (j == _repository.Cars[i].models.Count - 1)
                                        msg += _repository.Cars[i].models[j].Name;
                                    else
                                        msg += _repository.Cars[i].models[j].Name + ", " +"\n";

                                }
                                SendMessage(msg);
                                counter[n].Storage.Add(_repository.Cars[i].name.ToLower());

                            }
                        }
                    }
                    else if (counter[n].Storage.Count == 0)
                    {
                        SendMessage("Неизвестная команда. Напишите \"Help\" , чтобы получить полный список машин.");
                    }
                    else if ((Message == "back") && ((counter[n].Storage.Count == 1) || (counter[n].Storage.Count == 2) || (counter[n].Storage.Count == 3)))
                    {
                        if (counter[n].Storage.Count == 1)
                        {
                            // counter[n].Storage.Remove("Первый");
                            SendMessage(_repository.helperForSpeed[0].CarsInformation);
                            counter[n].Storage.RemoveAt(0);
                        }
                        else if (counter[n].Storage.Count == 2)
                        {
                            string msg = "";
                            for (int i = 0; i < _repository.Cars.Count; i++)
                            {
                                if (_repository.Cars[i].name.ToLower() == counter[n].Storage[0])
                                {
                                    for (int v = 0; v < _repository.Cars[i].models.Count; v++)
                                    {
                                        if (_repository.Cars[i].models[v].Name.ToLower() == counter[n].Storage[1])
                                        {
                                            for (int j = 0; j < _repository.Cars[i].models.Count; j++)
                                            {
                                                if (j == _repository.Cars[i].models.Count - 1)
                                                    msg += _repository.Cars[i].models[j].Name;
                                                else
                                                    msg += _repository.Cars[i].models[j].Name + ", " + "\n";

                                            }
                                            SendMessage(msg);
                                        }
                                    }
                                }
                            }
                            counter[n].Storage.RemoveAt(1);
                        }
                        else
                        {
                            string msg = "";
                            for (int i = 0; i < _repository.Cars.Count; i++)
                            {
                                if (_repository.Cars[i].name.ToLower() == counter[n].Storage[0])
                                {
                                    for (int v = 0; v < _repository.Cars[i].models.Count; v++)
                                    {
                                        if (_repository.Cars[i].models[v].Name.ToLower() == counter[n].Storage[1])
                                        {
                                            for (int j = 0; j < _repository.Cars[i].models[v].Varieties.Count; j++)
                                            {
                                                if (j == _repository.Cars[i].models[v].Varieties[j].MainCharacteristics.Count - 1)
                                                    msg += String.Format("{0}){1} with: {2}", j + 1, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].Motor, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].CarKit);
                                                else
                                                    msg += String.Format("{0}){1} with: {2};{3}", j + 1, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].Motor, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].CarKit, "\n");



                                            }
                                            SendMessage(msg);
                                            
                                        }
                                    }

                                }
                            }
                            counter[n].Storage.RemoveAt(2);
                        }
                    }
                    else if ((boolFunction == true) && (counter[n].Storage.Count == 1))
                    {
                        string msg = "";
                        for (int i = 0; i < _repository.Cars.Count; i++)
                        {
                            if (_repository.Cars[i].name.ToLower() == counter[n].Storage[0])
                            {
                                for (int v = 0; v < _repository.Cars[i].models.Count; v++)
                                {
                                    if (_repository.Cars[i].models[v].Name.ToLower() == Message)
                                    {
                                        for (int j = 0; j < _repository.Cars[i].models[v].Varieties.Count; j++)
                                        {
                                            
                                                if (j == _repository.Cars[i].models[v].Varieties.Count - 1)
                                                    msg += String.Format("{0}){1} with: {2}", j + 1, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].Motor, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].CarKit);
                                                else
                                                    msg += String.Format("{0}){1} with: {2};{3}", j + 1, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].Motor, _repository.Cars[i].models[v].Varieties[j].MainCharacteristics[0].CarKit, "\n");

                                            



                                        }
                                        SendMessage(msg);
                                        counter[n].Storage.Add(_repository.Cars[i].models[v].Name.ToLower());
                                    }
                                }

                            }
                        }
                    }
                    else if ((boolFunction == true) && (counter[n].Storage.Count == 2))
                    {
                        int number = Int32.Parse(Message);
                        string msg = "";
                        for (int i = 0; i < _repository.Cars.Count; i++)
                        {
                            if (_repository.Cars[i].name.ToLower() == counter[n].Storage[0])
                            {
                                for (int v = 0; v < _repository.Cars[i].models.Count; v++)
                                {
                                    if (_repository.Cars[i].models[v].Name.ToLower() == counter[n].Storage[1])
                                    {
                                        

                                            msg = String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}",
                                                _repository.Cars[i].models[v].Varieties[number-1].MainCharacteristics[0].Motor
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].Transmission
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].Turbo
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].Arrangement
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].HorsePower
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].CarKit
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].Fuel
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].DriveUnit
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].PrimerelyPrice
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].FuelConsumption
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].AccelerationToHundreds
                                                , _repository.Cars[i].models[v].Varieties[number - 1].MainCharacteristics[0].MaxSpeed);


                                        
                                        SendMessage(msg);
                                        number = number - 1;
                                        var numberToString = number.ToString();
                                        counter[n].Storage.Add(numberToString);
                                    }
                                }

                            }
                        }
                    }
                   
                    else if ((counter[n].Storage.Count == 1) || (counter[n].Storage.Count == 2) || (counter[n].Storage.Count == 3))
                    {
                        if (counter[n].Storage.Count == 1)
                            SendMessage("Извините, но я не знаю такой команды. Выберите одну из моделей или напишите \"Back\", чтобы выбрать другую марку");
                        else if (counter[n].Storage.Count == 2)
                            SendMessage("Извините, но я не знаю такой команды. Выберите одну из комплектаций, для этого введите ее порядковый номер. Или напишите \"Back\", чтобы выбрать другую модель");
                        else
                            SendMessage("Напишиет \"Back\", чтобы вернуться назад к выбору модификаций");
                    }
                    else
                    {
                        SendMessage("Something Wrong! Приносим свои извинения");
                    }
                }
                else
                {
                    SendMessage("Something Wromg! Приносим свои извинения");
                }
            }
        }

        static public void SendMessage(string Body)
        {
            try
            {
                vkapi.Messages.Send(new MessagesSendParams
                {
                    UserId = userID,
                    Message = Body
                });
            }
            catch (Exception e)
            {
                ColorMessage("Ошибка! " + e.Message, ConsoleColor.Red);
            }

        }
        static private void Eye()
        {
            LongPollServerResponse Pool = vkapi.Messages.GetLongPollServer(true);
            StartAsync( lastPts: Pool.Pts);
            NewMessages += Watcher_NewMessages;
        }
        static private void Watcher_NewMessages(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type != MessageType.Sended)
                {
                    User Sender = vkapi.Users.Get(messages[i].UserId.Value);
                    Console.WriteLine("Новое сообщение: {0} {1}: {2}", Sender.FirstName, Sender.LastName, messages[i].Body);
                    userID = messages[i].UserId.Value;
                    Console.Beep();
                                      
                    Command(messages[i].Body, Sender);
                    

                }
            }
        }
        static private LongPollServerResponse GetLongPoolServer(ulong? lastPts = null)
        {
            LongPollServerResponse response = vkapi.Messages.GetLongPollServer(false, lastPts == null);
            Ts = response.Ts;
            Pts = Pts == null ? response.Pts : lastPts;
            return response;
        }
        static private Task<LongPollServerResponse> GetLongPoolServerAsync(ulong? lastPts = null)
        {
            return Task.Run(() =>
            {
                return GetLongPoolServer(lastPts);
            });
        }
        static private LongPollHistoryResponse GetLongPoolHistory()
        {
            if (!Ts.HasValue) GetLongPoolServer(null);
            MessagesGetLongPollHistoryParams rp = new MessagesGetLongPollHistoryParams();
            rp.Ts = Ts.Value;
            rp.Pts = Pts;
            int i = 0;
            LongPollHistoryResponse history = null;
            string errorLog = "";

            while (i < 5 && history == null)
            {
                i++;
                try
                {
                    history = vkapi.Messages.GetLongPollHistory(rp);
                }
                catch (TooManyRequestsException)
                {
                    Thread.Sleep(150);
                    i--;
                }
                catch (Exception ex)
                {
                    errorLog += string.Format("{0} - {1}{2}", i, ex.Message, Environment.NewLine);
                }
            }

            if (history != null)
            {
                Pts = history.NewPts;
                foreach (var m in history.Messages)
                {
                    m.FromId = m.Type == MessageType.Sended ? vkapi.UserId : m.UserId;
                }
            }
            else ColorMessage(errorLog, ConsoleColor.Red);
            return history;
        }
        static private Task<LongPollHistoryResponse> GetLongPoolHistoryAsync()
        {
            return Task.Run(() => { return GetLongPoolHistory(); });
        }
        static async void WatchAsync(object state)
        {
            LongPollHistoryResponse history = await GetLongPoolHistoryAsync();
            if (history.Messages.Count > 0)
            {
                CurrentSleepSteps = 1;
                NewMessages?.Invoke(vkapi, history.Messages);
            }
            else if (CurrentSleepSteps < MaxSleepSteps) CurrentSleepSteps++;
            WatchTimer.Change(CurrentSleepSteps * StepSleepTime, Timeout.Infinite);
        }
        static async void StartAsync( ulong? lastPts = null)
        {

            await GetLongPoolServerAsync(lastPts);
            WatchTimer = new Timer(new TimerCallback(WatchAsync), null, 0, Timeout.Infinite);
        }
       
        public static void Restart()
        {
            Process.Start((Process.GetCurrentProcess()).ProcessName);
            Environment.Exit(0);
        }
        static private void ColorMessage(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }


     
    }
}

