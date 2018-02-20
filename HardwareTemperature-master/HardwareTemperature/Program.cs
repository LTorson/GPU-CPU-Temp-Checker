/*
  Supported Hardware
  -CPU core sensors
   Intel Core 2, Core i3/i5/i7, Atom, Sandy Bridge, Ivy Bridge, Haswell, Broadwell, Silvermont, Skylake, Kaby Lake, Airmont
   AMD K8 (0Fh family), K10 (10h, 11h family), Llano (12h family), Fusion (14h family), Bulldozer (15h family), Jaguar (16h family)

  -GPU sensors
   Nvidia
   AMD (ATI)
*/

using OpenHardwareMonitor.Hardware;
using System;
using System.IO;
using System.Timers;

namespace HardwareTemperature
{
    class Program
    {
        static void Main(string[] args)
        {
            // add CPU and GPU as hardware
            // note that, CPU temperature data requires 'highestAvailable' permission.
            Computer computer = new Computer() { CPUEnabled = true, GPUEnabled = true };
            computer.Open();

            Timer timer = new Timer() { Enabled = true, Interval = 1000 };
            timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                Console.Clear();
                Console.WriteLine("{0}\n", DateTime.Now);

                foreach (IHardware hardware in computer.Hardware)
                {
                    hardware.Update();

                    Console.WriteLine("{0}: {1}", hardware.HardwareType, hardware.Name);
                    // Set a variable to the My Documents path.
                    string myFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Overclocking.txt";
                    new FileInfo(myFilePath).Directory.Create();

                    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                    double len = new FileInfo(myFilePath).Length;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }

                    // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                    // show a single decimal place, and no space.
                    string result = String.Format("{0:0.##} {1}", len, sizes[order]);

                    // Append text to an existing file named "WriteLines.txt".
                    using (StreamWriter outputFile = new StreamWriter(myFilePath, true))
                    {
                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            // Celsius is default unit
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Console.WriteLine("File Saved {0} - {1}: {2}°C/{3}°F", result, myFilePath, sensor.Value, sensor.Value * 1.8 + 32);
                                if (sensor.Value <= 60)
                                {
                                    outputFile.WriteLine("Within Temperature Range - {0}: {1}°C/{2}°F", sensor.Name, sensor.Value, sensor.Value * 1.8 + 32);
                                    // Console.WriteLine("{0}: {1}°F", sensor.Name, sensor.Value*1.8+32);
                                }
                                else
                                {
                                    outputFile.WriteLine("Exceeding Temperature Range - {0}: {1}°C/{2}°F", sensor.Name, sensor.Value, sensor.Value * 1.8 + 32);
                                }
                            }

                        }
                        outputFile.WriteLine("----------------------------------------------");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Press Enter to exit");
            };
            // press enter to exit
            Console.ReadLine();
        }
    }
}
