using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;


namespace Pract5
{
    class Program
    {
        /// Квант времени (в миллисекундах) 
        static readonly int timeSlotDefault = 800;
        /// Список процессов (приложений, программных модулей)
        static List<ProcessPlan> processes = new List<ProcessPlan>();
        /// Поток управления процессами

        static Thread threadControl;

        /// Поток для изменения кванта времени процесса

        static Thread threadTimeSlot;


        /// Таймер, отсчитывающий квант времени в миллисекундах

        /// <param name="milliseconds"></param>
        static void GoTimer(int milliseconds)
        {
            var timer = Stopwatch.StartNew();
            do
            {
            } while (timer.ElapsedMilliseconds != milliseconds);
            timer.Stop();
        }


        /// Удаление из списка завершенного процесса
        /// Выполняется в отдельном потоке
        /// <param name="obj"></param>
        static void WaitUntilProcessEnd(object obj)
        {
            ProcessPlan process = (ProcessPlan)obj;
            process.MyProcess.WaitForExit();
            processes.Remove(process);
            Console.WriteLine("Процесс " + Thread.CurrentThread.Name + " закончил работу");
        }

        /// Вывод квантов времени для всех процессов
        static void PrintTimeSlots()
        {
            Console.WriteLine("Кванты для потоков");
            foreach (var item in processes)
                Console.WriteLine("Поток " + item.Thread.Name + ": " + item.TimeSlot);
        }



        /// Событие. Изменение кванта времени для процесса
        static void ChangeTimeSlot()
        {
            // Событие
            while (processes.Count > 0)
            {
               Console.WriteLine("");
                int iProccess = int.Parse(Console.ReadLine());
                bool isChange = false;
                try
                {
                    foreach (var item in processes)
                    {
                        if (item.Thread.Name == iProccess.ToString())
                        {
                            item.TimeSlot += 200;
                            isChange = true;
                            break;
                        }
                    }
                }
                catch
                {
                    isChange = false;
                }
                if (isChange == false)
                {
                    Console.WriteLine("Такой поток не найден. Возможно, он уже завершен.");
                }
                PrintTimeSlots();
            }
        }


        ///  Запуск всех процессов, постановка их на паузу после истечения кванта времени, возобновление работы,
        ///  пока очередь процессов не опустеет

        static void ControlProcess()
        {
            for (int i = 0; processes.Count > 0;)
            {
                int nP = processes.Count;
                if (i > processes.Count - 1)
                    i = 0;
                if (processes[i].IsStarted == false)
                {
                    processes[i].MyProcess.Start();
                    processes[i].Thread.Start(processes[i]);
                    processes[i].IsStarted = true;
                }
                else
                {
                    processes[i].Resume();
                }
                Console.WriteLine("Запущен " + (i + 1) + " процесс");
                GoTimer(processes[i].TimeSlot);
                if (nP == processes.Count)
                {
                    processes[i].Suspend();
                    Console.WriteLine("Приостановлен " + (i + 1) + " процесс");
                    if (processes.Count != 0 && i == processes.Count - 1)
                        i = 0;
                    else
                        i++;
                }
            }
            // Остановка потока на изменение квантом времени процессов.
            // Все процессы уже выполнены, поэтому в нем нет необходимости
            threadTimeSlot.Abort();
            Console.WriteLine("Все процессы заверешены");
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                string line = "ConsoleApp" + (i + 1);
                line = "..\\..\\..\\" + line + "\\bin\\Debug\\" + line;
                ProcessPlan processPlan = new ProcessPlan(line);
                processPlan.TimeSlot = timeSlotDefault;
                processPlan.IsStarted = false;
                processPlan.Thread = new Thread(new ParameterizedThreadStart(WaitUntilProcessEnd));
                processPlan.Thread.Name = (i + 1).ToString();
                processes.Add(processPlan);
            }
            PrintTimeSlots();

            // Запуск потока, который контролирует программные модули
            threadControl = new Thread(new ThreadStart(ControlProcess));
            threadControl.Start();

            // Запуск потока, который контролирует событие изменения кванта
            threadTimeSlot = new Thread(new ThreadStart(ChangeTimeSlot));
            threadTimeSlot.Start();
        }
    }
}
