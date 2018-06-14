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

namespace Vk_Bot
{
    class BotConnection
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

        static void Main(string[] args)
        {

            string KEY = "13b47b7e3145e07700571bf78b25c33525b09fde14ee170842f964a1f2b04698fd7f5752344d00affc1bf";
            ConsoleStyle();
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
        
        static void ConsoleStyle()
        {
            Console.Title = "Car-Searcher Bot";
            ColorMessage("Car-Searcher Bot", ConsoleColor.DarkYellow);
        }

     

        static bool Auth(string GroupID)
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

        static string[] Commands = { "Напиши любую марку машины","jj" };

        static void Command(string Message)
        {
            Message = Message.ToLower();
            
                if (Message == "help")
                {
                    string msg = "";
                    for (int j = 0; j < Commands.Length; j++) msg += Commands[j] + "/t ";
                    SendMessage(msg);
                }
                
                else if (Message == "привет" || Message == "приветик" || Message == "хай" )
                {
                    SendMessage("Привет, назови мне любую марку машины и я расскажу тебе о ней поподробнее!)");
                }
              
               
                else
                {
                    SendMessage("Неизвестная команда. Напишите 'Help' для получения списка команд.");
                }
               
            
        }

        static void SendMessage(string Body)
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

        static void Eye()
        {
            LongPollServerResponse Pool = vkapi.Messages.GetLongPollServer(true);
            StartAsync( lastPts: Pool.Pts);
            NewMessages += Watcher_NewMessages;
        }
        static void Watcher_NewMessages(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type != MessageType.Sended)
                {
                    User Sender = vkapi.Users.Get(messages[i].UserId.Value);
                    Console.WriteLine("Новое сообщение: {0} {1}: {2}", Sender.FirstName, Sender.LastName, messages[i].Body);
                    userID = messages[i].UserId.Value;
                    Console.Beep();
                   
                    
                        Command(messages[i].Body);
                    

                }
            }
        }
        static LongPollServerResponse GetLongPoolServer(ulong? lastPts = null)
        {
            LongPollServerResponse response = vkapi.Messages.GetLongPollServer(false, lastPts == null);
            Ts = response.Ts;
            Pts = Pts == null ? response.Pts : lastPts;
            return response;
        }
        static Task<LongPollServerResponse> GetLongPoolServerAsync(ulong? lastPts = null)
        {
            return Task.Run(() =>
            {
                return GetLongPoolServer(lastPts);
            });
        }
        static LongPollHistoryResponse GetLongPoolHistory()
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
        static Task<LongPollHistoryResponse> GetLongPoolHistoryAsync()
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

        static void ColorMessage(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

