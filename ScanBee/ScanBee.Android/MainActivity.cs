using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Android.Net;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using ZXing.Mobile;
using Android.Speech;

namespace ScanBee.Droid
{
    public static class Globals
    {
        public static object CurrentPageContext;
        //public static ConnectivityManager cm;
        public static string UpdateFileName = "ScanBee.Android.apk";
        public static bool MicrophoneFeature;
        public static string messageSpeakNow = "Говорите уже что-нибудь.";
        public static int VOICE = 63723;
        public static Xamarin.Forms.Platform.Android.FormsAppCompatActivity context;
        public static object refPage_;

    }

    [Activity(Label = "ScanBee", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        BroadcastReceiverLocal receiver;
        

        //BroadcastReceiverLocal_1 receiver1;           //DEBUG
        protected override void OnCreate(Bundle bundle)
        {
            

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            //Voice capabilities
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            Globals.MicrophoneFeature = rec == "android.hardware.microphone";

            MobileBarcodeScanner.Initialize(Application);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(ref Globals.CurrentPageContext, ref Globals.MicrophoneFeature));
            
            receiver = new BroadcastReceiverLocal();
           // receiver1 = new BroadcastReceiverLocal_1();            //DEBUG
            //Globals.cm = (ConnectivityManager)GetSystemService(Application.ConnectivityService);
            string AbslutePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
            Globals.UpdateFileName = Path.Combine(AbslutePath, Globals.UpdateFileName);

            Globals.context = this;
                        
            ////===== Тест перехвата броадкаста
            //TimerExampleState s = new TimerExampleState();
            //Timer T = new Timer((Object state) =>
            //{
            //    Intent intent = new Intent(RecognizerIntent.ActionGetLanguageDetails);

            //    //intent.SetAction(new CommonProcs().GetProperty("ext_IntentFilter"));
            //    //intent.PutExtra(new CommonProcs().GetProperty("ext_IntentExtra"), "340000614549");
            //    SendOrderedBroadcast(intent,null);
            //}, s, 5000, 1000000);
            ////test

        }


        protected override void OnResume()
        {
            base.OnResume();
            IntentFilter IF = new IntentFilter();
            //IF.AddAction("scan.rcv.enter");
            IF.AddAction(new CommonProcs().GetProperty("ext_IntentFilter"));
            RegisterReceiver(receiver, IF);


            ////DEBUG
            //IntentFilter IF1 = new IntentFilter();
            ////IF.AddAction("scan.rcv.enter");
            //IF1.AddAction(RecognizerIntent.ActionGetLanguageDetails);
            //RegisterReceiver(receiver1, IF1);

        }
        protected override void OnPause()
        {
            UnregisterReceiver(receiver);
            base.OnPause();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
                
        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == Globals.VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        if (Globals.refPage_.GetType() == typeof(Page_EnterValue))
                        {
                            ((Page_EnterValue)Globals.refPage_).Filter = (string)matches[0];
                        }
                    }
                    else
                    {
                    }
                }
                base.OnActivityResult(requestCode, resultVal, data);
            }
        }

    }
    public class BroadcastReceiverLocal : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            
                OutputScanData OutSD = new OutputScanData();
                string Barcode = intent.GetStringExtra(new CommonProcs().GetProperty("ext_IntentExtra"));

                if (Barcode != null)
                {
                    new ScanSupport().BarcodeScanned(Barcode,"");
                }
            
        }
    }

    //public class TimerExampleState
    //{

    //}

    //public class BroadcastReceiverLocal_1 : BroadcastReceiver               //DEBUG
    //{
    //    public override void OnReceive(Context context, Intent intent)
    //    {

    //        //ICollection<string> L = intent.Extras.KeySet();     //Let's look what extras in intent
    //        //int c = L.Count;
    //        //foreach (string cc in L)
    //        //{

    //        //}

    //        //OutputScanData OutSD = new OutputScanData();
    //        //string Barcode = intent.GetStringExtra(new CommonProcs().GetProperty("ext_IntentExtra"));

    //        //if (Barcode != null)
    //        //{
    //        //    new ScanSupport().BarcodeScanned(Barcode);
    //        //}

    //    }
    //}

    class UpdateApp
    {
        public async void doInBackground()
        {

            //try
            //{
            //    if (Globals.cm.ActiveNetworkInfo.IsConnected)
            //    {
            //        OutputScanData SD = new OutputScanData();
            //        SD.Purpose = "UpdateVersionRequest";
            //        string FullServiceAddr = new CommonProcs().GetProperty("ext_Address").ToString();
            //        ExchangeEngine EE = new ExchangeEngine();
            //        Status ts = await EE.UpdateRequest(SD);
            //        if (!ts.MajorStatus)
            //        {
            //            return;
            //        }
            //        UpdateStatus US = new JsonSerialization().deserializeJSON<UpdateStatus>((string)ts.MinorStatus);
            //        if (US.Version <= 100)
            //        {
            //            return;
            //        }

            //        SD.Purpose = "UpdateFileRequest";
            //        JsonSerialization JS = new JsonSerialization();
            //        string OutputString = JS.Serialize(SD);
            //        Status res = await EE.FileRequest(OutputString, FullServiceAddr,
            //        new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"));
            //        if (!res.MajorStatus)
            //        {
            //            return;
            //        }

            //        Intent intent = new Intent(Intent.ActionView);
            //        intent.SetDataAndType(Android.Net.Uri.Parse("file:///" + Globals.UpdateFileName), "application/vnd.android.package-archive");
            //        intent.SetFlags(ActivityFlags.NewTask);
            //        Application.Context.ApplicationContext.StartActivity(intent);

            //    }
            //}
            //catch (Exception e)
            //{
            //    return;
            //}
            return;
        }
    }

    class TimerState
    {
        public int counter = 0;
        public Timer tmr;
    }

}

