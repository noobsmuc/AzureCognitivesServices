using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using noobsmuc.AzureCognitivesServices.AzureOcr;

namespace noobsmuc.AzureCognitivesServices
{
    public class Program
    {
        private const bool UseExistingOcrResult = false;

        private const string AzureOcrKey = "3xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxb";
        private const string AzureSpeechKey = "8xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx8";
        private const string ServiceRegion = "westeurope";

        public static void Main()
        {

            Console.WriteLine("1. TTS with exiting OcrResult");
            Console.WriteLine("2. STT (microphone needed)");
            Console.WriteLine("3. Let's play 'whisper down the lane'");

            Console.WriteLine("");
            Console.Write("Your choice (0: Stop.): ");

            ConsoleKeyInfo x;
            do
            {
                x = Console.ReadKey();
                Console.WriteLine("");
                switch (x.Key)
                {
                    case ConsoleKey.D1:
                        RunTts(true);
                        break;

                    case ConsoleKey.D2:
                        RunStt();
                        break;

                    case ConsoleKey.D3:
                        Play();
                        break;

                    case ConsoleKey.D0:
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
                Console.WriteLine("\nRecognition done. Your Choice (0: Stop): ");
            } while (x.Key != ConsoleKey.D0);

            
            Console.ReadKey();
        }

        private static void Play()
        {
            Console.WriteLine("");

            string defaultText = "Give papa a cup of proper coffee in a copper coffee cup.";
            Console.WriteLine(defaultText);

            Console.WriteLine("Press Enter to start or input your own text");
            string inputString = Console.ReadLine();

            if (string.IsNullOrEmpty(inputString))
                inputString = defaultText;

            Console.WriteLine("");
            Console.WriteLine("Press Enter to start '5 round' or input round count");
            string roundCount = Console.ReadLine();

            var rounds = 5;
            if (!string.IsNullOrEmpty(roundCount))
            {
                int.TryParse(roundCount, out rounds);
            }
            
            PlayWhisperDownTheLane play = new PlayWhisperDownTheLane(AzureSpeechKey, ServiceRegion, rounds);
            play.LetsPlay(inputString);
        }

        private static void RunStt()
        {
            AzureSpeech.Azure_SpeechToText stt = new AzureSpeech.Azure_SpeechToText(AzureSpeechKey,ServiceRegion, @"C:\Temp\noobsmuc.AzureCognitivesServices.txt");
            stt.RecognizeSpeechAsync().Wait();

            Console.WriteLine("Please press a key to continue.");
            Console.ReadLine();
        }

        private static void RunTts(bool useExistingOcrResult)
        {
            string contentText;
            var ocr = new AzureOcr.AzureOcr(AzureOcrKey);
            if (!useExistingOcrResult)
            {
                string imageFullPathName = "exampleTextJpg.jpg";
                byte[] image = GetImageAsByteArray(imageFullPathName);

                contentText = ocr.MakeOcrRequest(image).Result;

                File.WriteAllText("exampleOcrResult.json", contentText);
            }
            else
            {
                contentText = File.ReadAllText("exampleOcrResult.json");
            }

            OcrResult ocrObject = JsonConvert.DeserializeObject<OcrResult>(contentText);
            StringBuilder builder = new StringBuilder();

            foreach (var line in ocrObject.regions[0].lines)
            {
                foreach (Word lineWord in line.words)
                {
                    builder.Append(lineWord.text + " ");
                }
            }

            AzureSpeech.Azure_TextToSpeech speech = new AzureSpeech.Azure_TextToSpeech(AzureSpeechKey, @"C:\Temp\noobsmuc.AzureCognitivesServices.mp3");
            speech.DoTts(builder.ToString());
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
