using System.IO.Ports;
using System.Text.Json;
using System.Text.RegularExpressions;

public class Program
{

    public static string RecMsg = "";
    public static int Main()
    {
        SerialPort mySerialPort = new SerialPort("COM4");

        mySerialPort.BaudRate = 2400;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;

        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

        mySerialPort.Open();

        Console.WriteLine("Приложение сериализует в 'фоне'...");
        Console.WriteLine();
        Console.ReadKey();

        mySerialPort.Close();

        return 0;
    }

    public static void MessageHandling(string msg)
    {
        /*if (String.IsNullOrEmpty(msg))
            return;
*/
        if (msg.Contains('$') && !RecMsg.Contains('$'))
        {
            int startIndex = msg.IndexOf("$");
            RecMsg = msg[startIndex..];
            return;
        }

        RecMsg += msg;

        string pattern = @"^\$(\d+)\.(\d\d?),(\d+)\.(\d\d?)";
        Regex rx = new Regex(pattern, RegexOptions.Compiled);
        MatchCollection matchCollection = rx.Matches(RecMsg);

        if (matchCollection.Count > 0)
        {
            RecMsg = matchCollection.First().Value;

            ParseFinalStringAndSerialize();
        }
    }

    public static void ParseFinalStringAndSerialize()
    {
        List<string> data = RecMsg.Replace("$", "").Split(',').ToList();

        double windSpeed = double.Parse(data[0], System.Globalization.CultureInfo.InvariantCulture);

        double winDirection = double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture);

        WriteToFile(windSpeed, winDirection);

        RecMsg = "";
    }


    private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();

        MessageHandling(indata);
    }

    public static void WriteToFile(double WindSpeed, double WinDirection)
    {
        using (StreamWriter sw = File.AppendText("Meteo.json"))
        {
            Data dt = new Data(WindSpeed, WinDirection);

            string dateToJson = JsonSerializer.Serialize<Data>(dt);

            sw.WriteLine(dateToJson);
        }
    }

    public static bool CheckData(string indata)
    {
        string pattern = @"^\$(\d+)\.(\d+),(\d+)\.(\d+)";
        if (Regex.IsMatch(indata, pattern))
        {
            return true;
        }

        return false;
    }

    [Serializable]
    public class Data
    {
        public string Name { get; set; } = "WMT700";
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public String Time { get; set; }

        public Data(double wSpeed, double wDirection)
        {
            WindSpeed = wSpeed;
            WindDirection = wDirection;
            Time = DateTime.Now.ToString("G"); // dd.mm.yyyy hh.mm.ss
        }

        public Data() { }
    }
}
