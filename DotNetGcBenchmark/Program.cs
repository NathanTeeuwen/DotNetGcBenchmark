// https://blog.pusher.com/golangs-real-time-gc-in-theory-and-practice/

namespace DotNetGcBenchmark
{
    class Program
    {
        const int windowSize = 200000;
        const int msgCount = 1000000;

        struct Message
        {
            public byte[] message;

            public static Message Construct(int size)
            {
                return new Message()
                {
                    message = new byte[size]
                };
            }
        };

        struct Buffer
        {
            public Message[] buffer;

            public static Buffer Construct()
            {
                return new Buffer()
                {
                    buffer = new Message[windowSize]
                };
            }
        }

        static long worst;

        static Message mkMessage(int n)
        {
            var m = Message.Construct(1024);
            for (int i = 0; i < m.message.Length; ++i)
            {
                m.message[i] = (byte)i;
            }

            return m;
        }

        static void pushMsg(Buffer b, int highID)
        {
            var start = System.Diagnostics.Stopwatch.StartNew();
            var m = mkMessage(highID);
            b.buffer[highID % windowSize] = m;
            long elapsed = start.ElapsedMilliseconds;
            if (elapsed > worst)
            {
                worst = elapsed;
            }
        }

        static void Main(string[] args)
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            var b = Buffer.Construct();
            for (int i = 0; i < msgCount; ++i)
            {
                pushMsg(b, i);
            }
            System.Console.WriteLine($"Worst push time: {worst}ms");
            System.Console.ReadKey();
        }
    }
}
