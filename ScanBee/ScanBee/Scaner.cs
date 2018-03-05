using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using System.Runtime.Serialization;

namespace ScanBee
{
    class Scaner
    {
        private bool SendingNow;
        private bool IsTorchOn = false;
        public async void ScanBarcode(ContentPage Context, string ScanPurpose)
        {

            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = false,
                //PureBarcode = true,
                UseNativeScanning = true,
                UseFrontCameraIfAvailable = true,
                TryHarder = true,
                DelayBetweenAnalyzingFrames = Convert.ToInt32(new CommonProcs().GetProperty("ext_DelayBetweenAnalyzingFrames")),
                InitialDelayBeforeAnalyzingFrames = Convert.ToInt32(new CommonProcs().GetProperty("ext_InitialDelayBeforeAnalyzingFrames")),
                DelayBetweenContinuousScans = Convert.ToInt32(new CommonProcs().GetProperty("ext_DelayBetweenContinuousScans")),
                PossibleFormats = new List<ZXing.BarcodeFormat>(),
            };

            SetUpBarcodeFormats(options);

            switch (ScanPurpose)
            {
                case "SettingsBarcodeScan":
                    options.PossibleFormats = new List<ZXing.BarcodeFormat>{ ZXing.BarcodeFormat.QR_CODE };
                    break;

                default:
                    break;
            }


            var ScanPage = new ZXingScannerPage()
            {
                DefaultOverlayShowFlashButton = true,
            };

            ScanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                ScanPage.IsScanning = false;
                if (Convert.ToBoolean(new CommonProcs().GetProperty("ext_Torch"))
                        && Convert.ToInt32(new CommonProcs().GetProperty("ext_ScanHardWare")) == 1)
                {
                    if (IsTorchOn)
                    {
                        ScanPage.ToggleTorch();
                        IsTorchOn = false;
                    }
                }
                SendingNow = false;
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Context.Navigation.PopAsync();
                    new ScanSupport().BarcodeScanned(result.Text, ScanPurpose);
                });
            };

            await Context.Navigation.PushAsync(ScanPage);
            SendingNow = true;
            if (Convert.ToBoolean(new CommonProcs().GetProperty("ext_Autofocus")))
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 2, 0);
                Device.StartTimer(ts, () =>
                {
                    if (SendingNow)
                    {

                        if (!IsTorchOn)
                        {
                            if (Convert.ToBoolean(new CommonProcs().GetProperty("ext_Torch"))
                            && Convert.ToInt32(new CommonProcs().GetProperty("ext_ScanHardWare")) == 1)
                            {
                                ScanPage.ToggleTorch();
                                IsTorchOn = true;
                            }

                        }
                        ScanPage.AutoFocus();
                    }
                    return true;
                });
            }

            

            return;
        }
        private void SetUpBarcodeFormats(MobileBarcodeScanningOptions options)
        {
            object v = "";
            if (App.Current.Properties.TryGetValue("ext_bk_format_All_1D", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.All_1D);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_AZTEC", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.AZTEC);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_CODABAR", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_EAN_8", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_8);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_EAN_13", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_13);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_CODE_128", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_CODE_39", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_CODE_93", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_93);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_DATA_MATRIX", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.DATA_MATRIX);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_IMB", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.IMB);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_ITF", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.ITF);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_MAXICODE", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.MAXICODE);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_MSI", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.MSI);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_PDF_417", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.PDF_417);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_PLESSEY", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.PLESSEY);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_QR_CODE", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_RSS_14", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.RSS_14);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_RSS_EXPANDED", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.RSS_EXPANDED);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_UPC_A", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_A);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_UPC_E", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_E);
                }
            }
            if (App.Current.Properties.TryGetValue("ext_bk_format_UPC_EAN_EXTENSION", out v))
            {
                if ((bool)v)
                {
                    options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_EAN_EXTENSION);
                }
            }

        }
    }

    [DataContract]
    public class OutputScanData
    {
        [DataMember]
        //public string Barcode = "0000000003400";
        public string Barcode;
        [DataMember]
        public string Purpose = "";
    }

    ////An exemple of using BeginInvokeOnMainThread
    //private void Test()
    //{
    //    Device.BeginInvokeOnMainThread(SomeMethod);
    //}

    //private async void SomeMethod()
    //{
    //    try
    //    {
    //        await SomeAsyncMethod();
    //    }
    //    catch (Exception e) // handle whatever exceptions you expect
    //    {
    //        //Handle exceptions
    //    }
    //}

    //private async Task SomeAsyncMethod()
    //{
    //    await Navigation.PushModalAsync(new ContentPage());
    //}
}
