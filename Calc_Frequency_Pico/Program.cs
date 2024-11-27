using Calc_Frequency_Pico.Service;
using System.Collections.Concurrent;

namespace Calc_Frequency_Pico
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Please provide path to the folder containing wave files to process.");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Invalid path");
                await Task.Delay(TimeSpan.FromSeconds(4));
                Environment.Exit(0);
            }
            var files = FileService.GetCsvFiles(path);
            var output = AnalyseAllFiles(files);
            //parallel processing did not show any significant gain in performance for given sample size
            //var output = AnalyseAllFiles_Parallel(files);

            await FileService.WriteResultsToFile(output, path);
            Console.WriteLine($"Output file has been generated under \"{Path.Combine(path, "Output")}\"");
            Environment.Exit(0);
        }

        public static Dictionary<string, double> AnalyseAllFiles(List<FileInfo> files)
        {
            var output = new Dictionary<string, double>();
            foreach (var file in files)
            {
                var waveform = FileService.GetWaveform(file.FullName);
                if(waveform.Samples.Count < 2)
                {
                    Console.WriteLine($"{file.Name} has insufficient samples. Skipping file.");
                    continue;
                }

                var frequency = AnalysisService.GetFrequency(waveform);

                output.Add(file.Name, frequency);
            }

            output = output.OrderBy(x => x.Value).ToDictionary();

            return output;
        }

        public static Dictionary<string, double> AnalyseAllFiles_Parallel(List<FileInfo> files)
        {

            var concurrencyLevel = Environment.ProcessorCount * 2;
            var output = new ConcurrentDictionary<string, double>(concurrencyLevel, files.Count + 20);
            Parallel.ForEach(files, file =>
            {
                var waveform = FileService.GetWaveform(file.FullName);

                var frequency = AnalysisService.GetFrequency(waveform);

                output.TryAdd(file.Name, frequency);
            });

            return  output.ToDictionary().OrderBy(x => x.Value).ToDictionary();
        }
    }
}