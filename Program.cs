using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;

namespace Parser
{
    class Program
    {
        private static Repository _repository = new Repository();
        static void Main(string[] args)
        {
            string q = "База данных успешно загружена!";
            Console.WriteLine(q);
            var repository = _repository;
            for(int i=0; i<repository.Cars.Count;i++ )
            {
                Console.WriteLine(repository.Cars[i].name);
            }
            Console.ReadKey();
        }
       
    }
}
