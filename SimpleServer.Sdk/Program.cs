﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace SimpleServer.Sdk
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SimpleServer SDK");
            Console.WriteLine("Copyright (C) Ultz");
            Console.WriteLine();
            Console.Write("Create SimpleServer project in this directory? [y/N] ");
            var yesorno = ("" + Console.ReadKey().KeyChar).ToUpper();
            Console.WriteLine();
            if (yesorno == "Y")
            {
                Console.WriteLine();
                Console.WriteLine("Looking for bundles...");
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync("https://hub.ultz.co.uk/dl/ss_sdk_root.txt").GetAwaiter().GetResult().Split(";").ToList();
                Console.WriteLine("Found "+response.Count +" bundle(s).");
                Console.WriteLine();
                Console.WriteLine("Pick your developer kit:");
                foreach (string bundle in response)
                {
                    Console.WriteLine((response.IndexOf(bundle)+1)+ ": "+bundle.Split('=')[0].Replace('_',' '));
                }
                Console.WriteLine();
                bool kitcorrect = false;
                int kitchoice = 0;
                while (!kitcorrect)
                {
                    Console.Write("Your choice: ");
                    int kitchoice1 = 0;
                    try
                    {
                        kitchoice1 = int.Parse(("" + Console.ReadKey().KeyChar).ToUpper()) - 1;
                    }
                    catch { Console.WriteLine(); Console.WriteLine("Parse error (invalid input)."); }
                    try
                    {
                        string str = response[kitchoice1];
                        str.Replace("BLAH", "BLEH"); // operate on str to check if it's null
                        kitchoice = kitchoice1;
                        kitcorrect = true;
                    }
                    catch (IndexOutOfRangeException) { Console.WriteLine("Invalid input."); }
                    catch { Console.WriteLine("Unspecified error."); }
                }
                Console.WriteLine("https://hub.ultz.co.uk/dl/ss_sdk_root.txt pointed at https://hub.ultz.co.uk/dl/"+response[kitchoice]+".txt");
                Console.WriteLine("Resolving SDK repository...");
                response = client.GetStringAsync("https://hub.ultz.co.uk/dl/"+response[kitchoice]+".txt").GetAwaiter().GetResult().Split(";").ToList();
                
            }
        }
    }
}