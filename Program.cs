using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO.Enumeration;

namespace DTP_D17_hemtentamen
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            private int status;
            private int priority;
            private string task;
            private string taskDescription;

            public int Status
            {
                get { return status; }
                set { status = value; }
            }
            public int Priority
            {
                get { return priority; }
                set { priority = value; }
            }
            public string Task
            {
                get { return task; }
                set { task = value; }
            }
            public string TaskDescription
            {
                get { return taskDescription; }
                set { taskDescription = value; }
            }
            public TodoItem(int priority, string task)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile(string todoFileName)
        {           
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
            int numRead = 0;
            list.Clear();

            string line;
            while ((line = sr.ReadLine()) != null) 
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
        }
        public static void WritelistTofile(string Filename)
        {
            
            Console.WriteLine($"Sparar till filen: {Filename} ");
            StreamWriter sw = new StreamWriter(Filename);
            foreach (TodoItem uppgift in list)
            {
                if (uppgift != null)

                    sw.WriteLine($"{uppgift.Status}|{uppgift.Priority}|{uppgift.Task}|{uppgift.TaskDescription}");
            }
            sw.Close();

        }
        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.WriteLine();
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(string allt , bool verbose = false)
        {
            PrintHead(verbose);
            if(allt == null)
            WhatToPrint(Active,verbose);

            else if (allt == "klara")
            WhatToPrint(Ready,verbose);

            else if(allt == "väntande")
            WhatToPrint(Waiting,verbose);
            else
            {
              foreach (TodoItem item in list)
              {
                  item.Print(verbose);
              }
            }

            PrintFoot(verbose);
        }

        private static void WhatToPrint(int status, bool verbose)
        {
            foreach (TodoItem item in list)
            {
                if (item.Status == status)
                    item.Print(verbose);

            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("hjälp            lista denna hjälp");
            Console.WriteLine("lista            lista alla aktiva uppgfiter från att-göra-listan");
            Console.WriteLine("lista väntande   lista alla väntande uppgifter från att-göra-listan");
            Console.WriteLine("lista klara      lista alla klara uppgifter från att-göra-listan");
            Console.WriteLine("lista allt       lista alla uppgifter från att-göra-listan");
            Console.WriteLine("beskriv          lista aktiva uppgifter från att-göra-listan med beskrivning");
            Console.WriteLine("beskriv allt     lista alla uppgifter från att-göra-listan med beskrivning");
            Console.WriteLine("ladda            Laddar listan 'todo.lis'");
            Console.WriteLine("ladda /fil/      Laddar filen /fil/ ");
            Console.WriteLine("spara            sparar listan 'todo.lis'");
            Console.WriteLine("spara /fil/      sparar listan till /fil/(om inte finns, skapar)");
            Console.WriteLine("sluta            spara att-göra-listan och sluta");
        }

    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            string[] command;
            string Filename = "todo.lis";
            do
            {
                command = MyIO.ReadCommand();
                if (MyIO.Equals(command[0], "hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command[0], "sluta"))
                {
                    Console.WriteLine("Hej då!");
                    break;
                }
                else if (command[0] == "lista")
                {
                    if (command.Length < 2)
                        Todo.PrintTodoList(allt: null, verbose: false);
                    else if (command.Length >= 2 && command[1] == "allt")
                        Todo.PrintTodoList("allt", verbose: false);
                    else if (command.Length >= 2 && command[1] == "klara")
                        Todo.PrintTodoList("klara", verbose: false);
                    else if (command.Length >= 2 && command[1] == "väntande")
                        Todo.PrintTodoList("väntande", verbose: false);
                    else
                        Todo.PrintTodoList(allt: null, verbose: false);
                }
                else if (command[0] == "beskriv")
                {
                    if (command.Length < 2)
                        Todo.PrintTodoList(allt: null, verbose: true);
                    else
                        Todo.PrintTodoList("allt", verbose: true); ;
                }

                else if (command[0] == "ladda")
                {
                    if (command.Length < 2)
                    {
                        Todo.ReadListFromFile(Filename);
                    }
                    else
                        Todo.ReadListFromFile(command[1]);
                }
                else if (command[0] == "spara")
                {
                    if (command.Length < 2)
                        Todo.WritelistTofile(Filename);
                    else
                        Todo.WritelistTofile(command[1]);
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: {command[0]}");
                }               
            }
            while (true);
        }
    }
    class MyIO
    {
        static public string[] ReadCommand()
        {
            string command = Convert.ToString(Console.ReadLine());
            string[] rawcommand = command.Split(" ");
            Console.Write("> ");
            return rawcommand;
        }
    }
}