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

        public static void Main()
        {
            string contentText;
            var ocr = new AzureOcr.AzureOcr(AzureOcrKey);
            if (!UseExistingOcrResult)
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

            AzureSpeech.AzureSpeech speach = new AzureSpeech.AzureSpeech(AzureSpeechKey, @"C:\Temp\noobsmuc.AzureCognitivesServices.mp3");
            speach.DoTts(builder.ToString());

            Console.ReadKey();
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
