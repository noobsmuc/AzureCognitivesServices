using System;
using System.IO;
using noobsmuc.AzureCognitivesServices.AzureSpeech;

namespace noobsmuc.AzureCognitivesServices
{
    public class PlayWhisperDownTheLane
    {
        private readonly string _AzureSpeechKey;
        private readonly string _ServiceRegion;
        private readonly int _Rounds;

        private const string ttsStartFile = "start_tts.txt";
        private const string sttStartFile = "start_stt.wav";

        private const string ttsFile = "tts.txt";
        private const string sttFile = "stt.wav";

        public PlayWhisperDownTheLane(string azureSpeechKey, string serviceRegion, int rounds)
        {
            _AzureSpeechKey = azureSpeechKey;
            _ServiceRegion = serviceRegion;
            _Rounds = rounds;
        }

        public void LetsPlay(string text)
        {
            string startText = text;
            File.WriteAllText(ttsFile, text);
            File.Copy(ttsFile, ttsStartFile, true);

            Azure_TextToSpeech tts = new Azure_TextToSpeech(_AzureSpeechKey, sttFile);
            Azure_SpeechToText stt = new Azure_SpeechToText(_AzureSpeechKey, _ServiceRegion, ttsFile);
            
            for (int i = 0; i < _Rounds; i++)
            {
                string inputText = File.ReadAllText(ttsFile);
                tts.DoTts(inputText);

                if (i == 0)
                    File.Copy(sttFile, sttStartFile, true);

                stt.RecognizeSpeechAsync(sttFile).Wait();
            }

            string endtext = File.ReadAllText(ttsFile);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"Start text: {startText}");
            Console.WriteLine($"End text: {endtext}");
        }
    }
}
