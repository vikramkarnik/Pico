using Calc_Frequency_Pico.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calc_Frequency_Pico.Service;

public class FileService 
{
    public static Waveform GetWaveform(string filePath)
    {
        var waveform = new Waveform();
        using (var reader = new StreamReader(filePath))
        {
            var header = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var values = (reader.ReadLine() ?? string.Empty).Split(',');
                if (values.Length == 0)
                    continue;

                if (!Double.TryParse(values[0], out var seconds))
                    throw new FormatException("Invalid value in Seconds column");

                if (!Double.TryParse(values[1], out var volts))
                    throw new FormatException("Invalid value in Volts column");
                waveform.Samples.Add(new Sample { Seconds = seconds, Volts = volts });
            }
        }
        return waveform;
    }

    public static List<FileInfo> GetCsvFiles(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        return di.GetFiles().Where(_ => string.Equals(_.Extension, ".csv", StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public static async Task WriteResultsToFile(Dictionary<string, double> output,string path)
    {
        string csv = string.Join(Environment.NewLine, output.Select(d => $"{d.Key},{d.Value},"));
        var fileString = $"File Name, Waveform Frequency{Environment.NewLine}{csv}";
        
        Directory.CreateDirectory(Path.Combine(path, "Output"));
        var filePath = Path.Combine(path, "Output", $"output_{DateTime.UtcNow.Ticks}.csv");
        await File.WriteAllTextAsync(filePath, fileString);
    }
}
