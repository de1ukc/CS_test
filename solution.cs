using System.IO.Ports;
using System.Text.Json;
using System.Text.RegularExpressions;

public class Program{
    
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

    private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            var flag = CheckData(indata);

            if (flag)
            {
                List<string> data = indata.Replace("$", "").Split(',').ToList();

                double WindSpeed = double.Parse(data[0], System.Globalization.CultureInfo.InvariantCulture);

                double WinDirection = double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture);

                WriteToFile(WindSpeed, WinDirection);
            }
        }
        catch
        {

        }
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
        if (Regex.IsMatch(indata, pattern)){
            return true;
        }

        return false;
    }

   // [Serializable]
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
        
        public Data() { } // for deserialization
    }
}
