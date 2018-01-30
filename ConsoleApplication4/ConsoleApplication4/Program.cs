using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public class ProducerConsumer
    {
        private static object locker = new object();
        protected List<int> list = new List<int>();

        public void Test()
        {
            Action producer = () =>
            {
                Console.WriteLine("Producer");
                lock (locker)
                {
                    var counter = 0;
                    while (counter != 100)
                    {
                        list.Add(++counter);
                        Monitor.Pulse(locker);
                        Monitor.Wait(locker);
                    }
                }
            };

            Action consumer = () =>
            {
                Console.WriteLine("Consumer");
                lock (locker)
                {
                    while (true)
                    {
                        Monitor.Pulse(locker);
                        list.ForEach(Console.WriteLine);
                        list.Clear();
                        Thread.Sleep(50);
                        Monitor.Wait(locker);
                    }
                }
            };

            Task.Factory.StartNew(producer);
            Task.Factory.StartNew(consumer);
        }

        public void Test2()
        {
            BlockingCollection<int> blk = new BlockingCollection<int>(10);

            var producer = Task.Factory.StartNew(() =>
            {
                for(var i=0; i<50; ++i)
                {
                    blk.Add(i);
                    Console.WriteLine("Produce {0}", i);
                }
                blk.CompleteAdding();
            });

            var consumer = Task.Factory.StartNew(() =>
            {
                while(!blk.IsCompleted)
                {
                    if (blk != null)
                    {
                        var res = blk.Take();
                        Console.WriteLine("Consume {0}", res);
                    }
                }
            });

            Task.WaitAll(producer, consumer);
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {

            ProducerConsumer pc = new ProducerConsumer();
            pc.Test();
            pc.Test2();
            Console.ReadKey();
        }
    }
}
