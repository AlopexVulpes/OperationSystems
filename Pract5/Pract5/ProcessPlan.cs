using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Pract5
{
    class ProcessPlan
    {
        Process myProcess;
        bool isStarted;
        int timeSlot;
        Thread thread;

        /// Конструктор для инициализации процесса
        /// <param name="name"></param>
        public ProcessPlan(object name)
        {
            myProcess = new Process();
            myProcess.StartInfo.FileName = (string)name;
        }

        /// Процесс
        public Process MyProcess { get => myProcess; set => myProcess = value; }
        /// Признак запуска процесса
        public bool IsStarted { get => isStarted; set => isStarted = value; }
        /// Квант времени
        public int TimeSlot { get => timeSlot; set => timeSlot = value; }
        /// Поток, ожидающий завершения процесса
        public Thread Thread { get => thread; set => thread = value; }


        // Импорт методов для управления сторонними программными модулями

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        
        /// Перечисление состояния потока. Необходимо для методов паузы и возобновления потоков
        private enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
        /// Возобновление работы процесса
        public void Resume()
        {
            try
            {
                foreach (ProcessThread thread in myProcess.Threads)
                {
                    var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        break;
                    }
                    ResumeThread(pOpenThread);
                }
            }
            catch
            {

            }
        }

        /// Приостановка процесса
        public void Suspend()
        {
            try
            {
                foreach (ProcessThread thread in myProcess.Threads)
                {
                    var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        break;
                    }
                    SuspendThread(pOpenThread);
                }
            }
            catch
            {

            }
        }
    }
}
