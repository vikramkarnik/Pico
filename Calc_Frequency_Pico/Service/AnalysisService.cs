using Calc_Frequency_Pico.Model;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Calc_Frequency_Pico.Service;

public class AnalysisService
{
    public double GetFrequency_wrong(Waveform waveform)
    {
        //misunderstood problem at hand, algo is incorrect
        var countCoord = waveform.Samples.Count();
        var peaks = new HashSet<int>();
        var peak = 0;
        for (var i = 0; i < countCoord; i++)
        {
            if (waveform.Samples[i+1].Volts > waveform.Samples[i].Volts)
            {
                peak = i + 1;
            }
            else
            {
                peaks.Add(peak);
            }
            if (peaks.Count > 1)
                break;
        }
        var frequency = 1/(waveform.Samples[peaks.ElementAt(1)].Seconds - waveform.Samples[peaks.ElementAt(0)].Seconds);


        return frequency;
    }

    public static double GetFrequency(Waveform waveform)
    {
        if(waveform.Samples.Count < 2) 
            return 0;
        //correct way is to take fft of the sample voltages and then fetch max amplitude wave to calculate its frequency
        //assuming here that the sampling rate did not change
        var samplingInterval = waveform.Samples[1].Seconds - waveform.Samples[0].Seconds;
        var samplingRate = 1.0 / samplingInterval;
        
        var sampleCount = waveform.Samples.Count;
        var voltages = waveform.Samples.Select(_ => _.Volts).ToList();
        var fftSample = Array.ConvertAll(voltages.ToArray(), v => new Complex32((float)v, 0));
        Fourier.Forward(fftSample);

        var magnitudes = fftSample.Select(_ => _.Magnitude).Take(sampleCount / 2).ToList();

        var maxMagnitudeIndex = magnitudes.IndexOf(magnitudes.Max());

        var maxMagnitudeFrequency = maxMagnitudeIndex * samplingRate / sampleCount;

        return maxMagnitudeFrequency;
    }

}

