using System;
using System.IO;
using System.Security.Authentication;
using System.Threading;

namespace noobsmuc.AzureCognitivesServices.AzureSpeech
{
    public class Azure_TextToSpeech
    {
        const string UriBase = "https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        const string RequestUri = "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1";

        private readonly string _SubscriptionKey;
        private static string _DestinationFullFileName;

        public Azure_TextToSpeech(string subscriptionKey, string destinationFullFileName)
        {
            _SubscriptionKey = subscriptionKey;
            _DestinationFullFileName = destinationFullFileName;
        }

        public void DoTts(string message)
        {
            string accessToken;
            AzureAuthentication auth = new AzureAuthentication(UriBase, _SubscriptionKey);

            try
            {
                accessToken = auth.GetAccessToken();
            }
            catch (Exception ex)
            {
                 throw new AuthenticationException("Failed authentication.", ex);
            }

            
            var cortana = new Synthesize();
            cortana.OnAudioAvailable += SaveAudio;
            cortana.OnError += ErrorHandler;

            // Reuse Synthesize object to minimize latency
            cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
            {
                RequestUri = new Uri(RequestUri),
                // Text to be spoken.
                Text = message,
                VoiceType = Gender.Female,
                // Refer to the documentation for complete list of supported locales.
                Locale = "en-US", 
                // You can also customize the output voice. Refer to the documentation to view the different
                // voices that the TTS service can output.
                // VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)",
                VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Guy24KRUS)",
                // VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",

                // Service can return audio in different output format.
                OutputFormat = AudioOutputFormat.Riff24Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + accessToken,
            }).Wait();
        }

        private static void SaveAudio(object sender, GenericEventArgs<Stream> args)
        {
            Console.WriteLine(args.EventData);

            using (FileStream output = new FileStream(_DestinationFullFileName, FileMode.Create))
            {
                args.EventData.CopyTo(output);
            }

            // For SoundPlayer to be able to play the wav file, it has to be encoded in PCM.
            // Use output audio format AudioOutputFormat.Riff16Khz16BitMonoPcm to do that.
            // with this you can play directly
            //SoundPlayer player = new SoundPlayer(args.EventData);
            //player.PlaySync();

            args.EventData.Dispose();
        }

        private static void ErrorHandler(object sender, GenericEventArgs<Exception> ex)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", ex);
        }
    }
}