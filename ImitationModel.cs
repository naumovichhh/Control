using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    internal class ImitationModel
    {
        private const int steps = 1000000;
        private readonly double pi1;
        private readonly double pi2;
        private int denialCount;
        // для получения среднего времени нахождения заявки в системе подсчитывается общее количество заявок requestCount, прошедших через систему,
        // и общее количество нахождения заявки в системе в тактовый интервал requestInSystemCount, которое делится на requestCount
        private int requestCount;
        private int requestInSystemCount;  
        private int queueCount;
        private Dictionary<string, int> stateCount = new Dictionary<string, int>()
        {
            { "2000", 0 }, { "1000", 0 }, { "2010", 0 }, { "1010", 0 }, { "2110", 0 }, { "1110", 0 }, { "2210", 0 }, { "1210", 0 },
            { "1001", 0 }, { "2011", 0 }, { "1011", 0 }, { "2111", 0 }, { "1111", 0 }, { "2211", 0 }, { "1211", 0 }
        };
        private Dictionary<string, double> stateProbability = new Dictionary<string, double>();
        private double denialProbability;
        private double avgTimeInSystem;
        private double avgQueueLength;

        public ImitationModel(double pi1, double pi2)
        {
            this.pi1 = pi1;
            this.pi2 = pi2;
        }

        public double this[string s]
        {
            get => stateProbability[s];
        }

        public double DenialProbability => denialProbability;
        public double AvgTimeInSystem => avgTimeInSystem;
        public double AvgQueueLength => avgQueueLength;
        
        public void Simulate()
        {
            Random random1 = new Random((int)DateTime.Now.Ticks);
            // прибавляется число для того, чтобы сгенерированные величины не были одинаковы
            Random random2 = new Random((int)DateTime.Now.Ticks + 120121);
            int stepsToSupply = 3;
            int channel1 = 0;
            int channel2 = 0;
            int queue = 0;
            List<int> list = new List<int>();
            double wc2 = 0;
            double w1 = 0;
            double w2 = 0;
            double wqueue = 0;
            int channel1Count = 0, channel2Count = 0;
            int channel1RequestCount = 0;
            int queuePassedCount = 0;

            for (int i = 0; i < steps; ++i)
            {
                if (channel2 == 1)
                {
                    if (random2.NextDouble() <= 1 - pi2)
                    {
                        channel2 = 0;
                        ++requestCount;
                        wc2 = (wc2 * (requestCount - 1) + list[0]) / (double)requestCount;
                        w2 = (w2 * (requestCount - 1) + channel2Count) / (double)requestCount;
                        channel2Count = 0;
                        list.RemoveAt(0);
                        Console.WriteLine("Request processed");
                    }
                }

                if (channel1 == 1)
                {
                    if (random1.NextDouble() <= 1 - pi1)
                    {
                        ++queuePassedCount;
                        channel1 = 0;
                        ++channel1RequestCount;
                        w1 = (w1 * (channel1RequestCount - 1) + channel1Count) / (double)channel1RequestCount;
                        channel1Count = 0;
                        if (channel2 == 0)
                        {
                            channel2 = 1;
                        }
                        else
                        {
                            ++denialCount;
                            requestInSystemCount -= list[1];
                            list.RemoveAt(1);
                            Console.WriteLine("Denial after channel");
                        }
                    }
                }

                if (queue > 0 && channel1 == 0)
                {
                    channel1 = 1;
                    --queue;
                }

                if (--stepsToSupply == 0)
                {
                    stepsToSupply = 2;
                    if (queue == 1)
                    {
                        queue = 2;
                        Console.WriteLine("Request in");
                        list.Add(0);
                    }
                    else if (queue == 2)
                    {
                        ++denialCount;
                    }
                    else if (queue == 0)
                    {
                        if (channel1 == 1)
                            queue = 1;
                        else
                            channel1 = 1;
                        Console.WriteLine("Request in");
                        list.Add(0);
                    }
                }

                ++stateCount[$"{stepsToSupply}{queue}{channel1}{channel2}"];
                requestInSystemCount += queue + channel1 + channel2;
                for (int j = 0; j < list.Count; ++j)
                    ++list[j];
                queueCount += queue;
                channel1Count += channel1;
                channel2Count += channel2;

                Console.WriteLine($"{queue.ToString()}{channel1.ToString()}{channel2.ToString()}  {(list.Count > 0 ? list.Select(e => e.ToString()).Aggregate((s, n) => s + "," + n) : "-")}");
                Console.WriteLine($"Wsystem2: {wc2}");
                Console.WriteLine("<step>");
            }

            Console.WriteLine("Wc2: " + wc2);
            Console.WriteLine("Wchannel1: " + w1);
            Console.WriteLine("Wchannel2: " + w2);
            Console.WriteLine("Wqueue: " + queueCount / (double)queuePassedCount);
            Summarize();
        }

        private void Summarize()
        {
            denialProbability = (double)denialCount / (double)(steps / 2);
            avgQueueLength = (double)queueCount / (double)steps;
            avgTimeInSystem = (double)requestInSystemCount / (double)requestCount;
            foreach (var pair in stateCount)
            {
                stateProbability[pair.Key] = (double)pair.Value / (double)steps;
            }
        }
    }
}
