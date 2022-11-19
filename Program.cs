using System;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;

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
        public static int StatustoInt(string status)
        {
            switch (status)
            {
                case "aktiv": return Active;
                case "väntande": return Waiting;
                case "avklarad": return Ready;
                default: return 0;
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
                set
                {
                    try
                    {
                        if (value > 0 && value <= 3) status = value;
                        else throw new IndexOutOfRangeException();
                    }
                    catch (System.IndexOutOfRangeException) { Console.WriteLine("Status måste vara 1-3"); }
                }
            }
            public int Priority
            {
                get { return priority; }
                set
                {
                    try
                    {
                        if (value > 0 && value <= 4) priority = value;
                        else throw new IndexOutOfRangeException();
                    }
                    catch (System.IndexOutOfRangeException) { Console.WriteLine("Priority måste vara 1-4"); }

                }
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
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-47}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-79}|");
                else
                    Console.WriteLine();
            }
        }
        public static void RedigeraKopieraUppgift(string redigeraElKopiera, string uppgift)
        {
            if (redigeraElKopiera == "redigera")
            {
                foreach (TodoItem item in list)
                {
                    if (item.Task == uppgift)
                    {

                        Console.WriteLine("Tryck [enter] om du inte vill ändra!");
                        Console.Write($"uppgiftens namn ({uppgift}): ");
                        string newtask = Console.ReadLine();
                        if (newtask != "")
                            item.Task = newtask;
                        Console.Write($"status ({StatusToString(item.Status)}): ");
                        string newstatus = Console.ReadLine();
                        if (newstatus != "")
                        {
                            if (newstatus == "avklarad" || newstatus == "väntande" || newstatus == "aktiv")
                                item.Status = StatustoInt(newstatus);
                            else
                                item.Status = Convert.ToInt32(newstatus);
                        }
                        Console.Write($"Priority ({item.Priority}): ");
                        string newpriority = Console.ReadLine();
                        if (newpriority != "")
                            item.Priority = Convert.ToInt32(newpriority);
                        Console.Write($"beskrivning ({item.TaskDescription}): ");
                        string newTaskdescription = Console.ReadLine();
                        if (newTaskdescription != "")
                            item.TaskDescription = newTaskdescription;
                        Console.WriteLine($"uppgifterna sparades till {uppgift}");
                    }
                }
            }
            else if (redigeraElKopiera == "kopiera")
            {
                int count = 1;
                foreach (TodoItem item in list)
                {

                    if (item.Task == uppgift)
                    {
                        TodoItem newitem = new TodoItem(2, uppgift);
                        list.Insert(count, newitem);
                        list[count].Status = Active;
                        list[count].Task = uppgift + $", 2";
                        list[count].Priority = list[count - 1].Priority;
                        list[count].TaskDescription = list[count - 1].TaskDescription;
                        break;
                    }
                    count++;
                }
            }
        }

        public static void AndraStatus(string status, string namnuppgift)
        {
            if (status == "klar")
            {
                foreach (TodoItem item in list)
                    if (item.Task == namnuppgift)
                        item.Status = Ready;
            }

            else if (status == "vänta")
            {
                foreach (TodoItem item in list)
                    if (item.Task == namnuppgift)
                        item.Status = Waiting;
            }

            else if (status == "aktivera")
            {
                foreach (TodoItem item in list)
                    if (item.Task == namnuppgift)
                        item.Status = Active;
            }
            else
            {
                Console.WriteLine("Något blev fel!");
            }

        }
        public static void Nyuppgift(string task = "")
        {
            if (task == "")
            {
                Console.Write("prioritet:");
                int priority = Convert.ToInt16(Console.ReadLine());
                Console.Write("namn på uppgiften?");
                task = Console.ReadLine();
                TodoItem nyuppgift = new TodoItem(priority, task);
                Console.Write("status:");
                string newstatus = Console.ReadLine();
                if (newstatus == "avklarad" || newstatus == "väntande" || newstatus == "aktiv")
                    nyuppgift.Status = StatustoInt(newstatus);
                else
                    nyuppgift.Status = Convert.ToInt16(newstatus);
                Console.Write("Beskrvning:");
                nyuppgift.TaskDescription = Console.ReadLine();
                Console.WriteLine("Din uppgift har lagts till i listan");
            }

            else
            {
                Console.Write("prioritet:");
                int priority = Convert.ToInt16(Console.ReadLine());
                TodoItem nyuppgift = new TodoItem(priority, task);
                nyuppgift.Status = Active;
                Console.Write("Beskrvning:");
                nyuppgift.TaskDescription = Console.ReadLine();
                list.Add(nyuppgift);
                Console.WriteLine("Din uppgift har lagts till i listan");
            }

        }
        public static void ReadListFromFile(string todoFileName)
        {
            try
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
            catch (Exception)
            {
                Console.WriteLine($"Något blev fel med filen som lästes in..");
                Console.WriteLine("Titta över filen och försök igen");
            }
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
                Console.Write("|status      |prio  |namn                                           |");
                if (verbose) Console.WriteLine("beskrivning                                                                    |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|-----------------------------------------------|");
            if (verbose) Console.WriteLine("-------------------------------------------------------------------------------|");
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
        public static void PrintTodoList(string allt, bool verbose = false)
        {
            PrintHead(verbose);
            if (allt == null)
                WhatToPrint(Active, verbose);

            else if (allt == "klara")
                WhatToPrint(Ready, verbose);

            else if (allt == "väntande")
                WhatToPrint(Waiting, verbose);
            else
                foreach (TodoItem item in list)
                {
                    item.Print(verbose);
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
            Console.WriteLine("hjälp                lista denna hjälp");
            Console.WriteLine("lista                lista alla aktiva uppgfiter från att-göra-listan");
            Console.WriteLine("lista väntande       lista alla väntande uppgifter från att-göra-listan");
            Console.WriteLine("lista klara          lista alla klara uppgifter från att-göra-listan");
            Console.WriteLine("lista allt           lista alla uppgifter från att-göra-listan");
            Console.WriteLine("beskriv              lista aktiva uppgifter från att-göra-listan med beskrivning");
            Console.WriteLine("beskriv allt         lista alla uppgifter från att-göra-listan med beskrivning");
            Console.WriteLine("aktivera    /uppgift/   sätt status på uppgift till aktiv");
            Console.WriteLine("vänta    /uppgift/   sätt status på uppgift till väntande");
            Console.WriteLine("klar     /uppgift/   sätt status på uppgift till avklarad");
            Console.WriteLine("redigera /uppgift/   redigera en uppgift med namnet /uppgift/");
            Console.WriteLine("kopiera  /uppgift/   redigera en uppgift med namnet /uppgift/ till namnet /uppgift, 2/");
            Console.WriteLine("ladda                Laddar listan 'attgöralistan.txt'");
            Console.WriteLine("ladda /fil/          Laddar filen /fil/ ");
            Console.WriteLine("spara                sparar listan 'attgöralistan.txt'");
            Console.WriteLine("spara /fil/          sparar listan till /fil/(om inte finns, skapar)");
            Console.WriteLine("sluta                spara att-göra-listan och sluta");
        }

    }
    class MainClass
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            string[] command;
            string Filename = "attgöralistan.txt";
            string lastFilename = "";
            do
            {
                command = MyIO.ReadCommand();
                if (MyIO.Equals(command[0], "hjälp"))
                    Todo.PrintHelp();
                else if (MyIO.Equals(command[0], "sluta"))
                {
                    Todo.WritelistTofile(lastFilename);
                    Console.WriteLine("Hej då!");
                    break;
                }
                else if (command[0] == "lista")
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
                else if (command[0] == "beskriv")
                    if (command.Length < 2)
                        Todo.PrintTodoList(allt: null, verbose: true);
                    else
                        Todo.PrintTodoList("allt", verbose: true);
                else if (command[0] == "ladda")
                {
                    if (command.Length < 2)
                    {
                        lastFilename = Filename;
                        Todo.ReadListFromFile(Filename);
                    }
                    else
                    {
                        lastFilename = command[1];
                        Todo.ReadListFromFile(command[1]);
                    }
                }
                else if (command[0] == "spara")
                    if (command.Length < 2)
                        Todo.WritelistTofile(Filename);
                    else
                        Todo.WritelistTofile(command[1]);
                else if (command[0] == "ny")
                    if (command.Length < 2)
                        Todo.Nyuppgift();
                    else
                        Todo.Nyuppgift(command[1]);
                else if (command[0] == "klar" || command[0] == "aktivera" || command[0] == "vänta")
                    if (command.Length < 2)
                        Console.WriteLine($"Vilken uppgift ska sättas till {command[0]}?");
                    else
                        Todo.AndraStatus(command[0], command[1]);

                else if (command[0] == "redigera")
                    if (command.Length < 2)
                        Console.WriteLine("Vilken uppgift ska redigeras?");
                    else
                        Todo.RedigeraKopieraUppgift(command[0], command[1]);
                else if (command[0] == "kopiera")
                    if (command.Length < 2)
                        Console.WriteLine("Vilken uppgift ska kopieras?");
                    else
                        Todo.RedigeraKopieraUppgift(command[0], command[1]);
                else
                    Console.WriteLine($"Okänt kommando: {command[0]}"); //felsök
            }
            while (true);
        }
    }
    class MyIO
    {
        static public string[] ReadCommand()
        {
            Console.Write("> ");
            string command = Convert.ToString(Console.ReadLine()); ;
            string[] rawcommand = command.Trim().Split(" ");
            for (int i = 2; i < rawcommand.Length; i++)
                rawcommand[1] = rawcommand[1] + " " + rawcommand[i];
            return rawcommand;
        }
    }
}