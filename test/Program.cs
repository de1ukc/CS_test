using System.IO.Ports;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MeteoProj
{
    public class Program
    {
        public static string RecMsg = "";
        private const string _pattern = @"\$(\d+)\.(\d\d?),(\d+)\.(\d\d?)";

        public static int Main()
        {
            SerialPort mySerialPort = new("COM4")
            {
                BaudRate = 2400,
                StopBits = StopBits.One,
                DataBits = 8
            };

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            mySerialPort.Open();

            Console.WriteLine("Приложение сериализует в 'фоне'...");
            Console.WriteLine();
            Console.ReadKey();

            mySerialPort.Close();

            return 0;
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            try
            {
                MessageHandling(indata);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void MessageHandling(string msg)
        {
            if (msg.Contains('$') && !RecMsg.Contains('$'))
            {
                int startIndex = msg.IndexOf("$");
                RecMsg = msg[startIndex..];
            }
            else
            {
                RecMsg += msg;
            }

            Regex rx = new Regex(_pattern, RegexOptions.Compiled);
            MatchCollection matchCollection = rx.Matches(RecMsg);

            if (matchCollection.Count > 0)
            {
                foreach (Match match in matchCollection.Cast<Match>())
                {
                    ParseFinalStringAndSerialize(match.ToString());
                }
            }
        }

        public static void ParseFinalStringAndSerialize(string match)
        {
            List<string> data = match.Replace("$", "").Split(',').ToList();

            double windSpeed = double.Parse(data[0], System.Globalization.CultureInfo.InvariantCulture);

            double winDirection = double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture);

            WriteToFile(windSpeed, winDirection);

            RecMsg = RecMsg.Remove(RecMsg.IndexOf(match), match.Length);
        }

        public static void WriteToFile(double WindSpeed, double WinDirection)
        {
            Data dt = new(WindSpeed, WinDirection);

            List<Data> lst = ReadFromFile();

            lst.Add(dt);

            //string dateToJson = "{" + JsonSerializer.Serialize<List<Data>>(lst) + "}";
            string dateToJson = JsonSerializer.Serialize<List<Data>>(lst);

            using (StreamWriter sw = new("Meteo.json", false))
            {
                sw.Write(dateToJson);
            }
        }

        public static List<Data> ReadFromFile()
        {
            var jsonStrList = new List<Data>();

            using (StreamReader sr = new("Meteo.json"))
            {
                string lst = sr.ReadToEnd();

                var indexes = (lst.IndexOf('['), lst.IndexOf("]") + 1);
                if (indexes.Item1 >= 0 && indexes.Item2 >= 0)
                {
                    lst = lst[indexes.Item1..indexes.Item2];

                    jsonStrList = JsonSerializer.Deserialize<List<Data>>(lst);
                }
            }
            return jsonStrList;
        }

        [Serializable]
        public class Data
        {
            public string Name { get; set; } = "WMT700";
            public double WindSpeed { get; set; }
            public double WindDirection { get; set; }
            public string Time { get; set; }

            public Data(double wSpeed, double wDirection)
            {
                WindSpeed = wSpeed;
                WindDirection = wDirection;
                Time = DateTime.Now.ToString("G"); // dd.mm.yyyy hh.mm.ss
            }

            public Data() { }
        }
    }
}