using Xamarin.Forms;
using Android.Content;
using Android.Speech;
using Android.App;
using Android.Content.PM;
using Android.OS;


[assembly: Dependency(typeof(ScanBee.Droid.Voice))]
namespace ScanBee.Droid
{
    class Voice : ScanBee.IVoice
    {
        public void StartVoceIntent(ContentPage Page_)
        {
            Globals.refPage_ = Page_;
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, "ru_RU");
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "ru_RU");
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, "ru_RU");
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Globals.messageSpeakNow);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            Globals.context.StartActivityForResult(voiceIntent, Globals.VOICE);
        }

    }

}