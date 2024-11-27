using Calc_Frequency_Pico.Model;
using Calc_Frequency_Pico.Service;

namespace Calc_Frequency_Tests
{
    [TestClass]
    public sealed class AnalysisServiceTests
    {
        private Waveform _waveform;
        [TestInitialize]
        public void TestInit()
        {
            _waveform = GenerateWaveform();
        }

        [TestMethod]
        public void AnalysisService_GetFrequency_CorrectlyIdentifyFrequency()
        {
            // Act: Call the method to calculate the frequency
            double frequency = AnalysisService.GetFrequency(_waveform);

            // Assert: Verify the frequency is correct
            // The test signal is 1000 Hz, so the result should be close to 1000
            Assert.IsTrue(frequency >= 999.5 && frequency <= 1000.5,
                $"Expect frequency to be around 1000 Hz, but got {frequency} Hz");

        }

        private static Waveform GenerateWaveform()
        {
            const double frequency = 1000;
            const double samplingRate = 10000;
            const int samples = 1000;
            const double amplitude = 1.0;
            var timeStep = 1 / samplingRate;
            var waveform = new Waveform();

            for (int i = 0; i < samples; i++)
            {
                double seconds = i * timeStep;
                double voltage = amplitude * Math.Sin(2 * Math.PI * frequency * seconds);
                waveform.Samples.Add(new Sample { Seconds = seconds, Volts = voltage });

            }

            return waveform;
        }

    }
}
