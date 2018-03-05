using System;
using System.Net;
using System.Net.Http;
using Plugin.Connectivity;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScanBee
{
    public class Exchange
    {
        private ContentPage Context;
        private JsonSerialization SerialObj_ThisClass = new JsonSerialization();
        public Exchange(ContentPage MP)
        {
            Context = MP;
        }
        public async Task<bool> GetDocumentsList()
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "GetDocs";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            
            if (!CrossConnectivity.Current.IsConnected)
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            JsonSerialization JS = new JsonSerialization();
            string OutputString = JS.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), (ContentPage)Context);
            if (!res.MajorStatus)
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }
            try
            {
                FillDocumentsList(new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus), (MainPage)Context);
            }
            catch
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return false;
            }

            return true;
        }
        public async Task<bool> GetGoodList(string DocGuid)
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "GetGoods";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            OS.Value0 = DocGuid;
            

            if (!CrossConnectivity.Current.IsConnected)
            {
                await ((Page_GoodsList)Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            JsonSerialization JS = new JsonSerialization();
            string OutputString = JS.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), (ContentPage)Context);
            if (!res.MajorStatus)
            {
                await ((Page_GoodsList)Context).DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }
            try
            {
                FilGoodsList(new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus), (Page_GoodsList)Context);
            }
            catch
            {
                await ((Page_GoodsList)Context).DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return false;
            }
            return true;
        }
        public async Task<bool> SendGoodListToServer(ObservableCollection<Good> refGoodList, Document refDocument)
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "AquiringOk";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            OS.Value0 = refDocument.UID;
            OS.Value1 = refDocument.Metadata;
            foreach (Good g in refGoodList)
            {
                OS.Table0.Add(new TablePart
                {
                    Metadata = g.Metadata,
                    Value0 = g.nGood,
                    UID = g.uidGood,
                    //Value1 = g.nUnit,
                    Value2 = g.uidUnit,
                    //Value3 = g.nGoodDescription,
                    Value4 = g.uidGoodDescription,
                    //Value5 = g.nSerial,
                    Value6 = g.uidSerial,
                    //Value7 = g.nQuality,
                    Value8 = g.uidQuality,
                    Value9 = g.Amount,
                    Value10 = g.AmountAquired,
                    Value11 = g.Value0,
                    Value12 = g.Value1,
                    Value13 = g.Value2,
                    Value18 = g.StringID,
                    Value19 = g.Barcode
                });
            }

            if (!CrossConnectivity.Current.IsConnected)
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            string OutputString = SerialObj_ThisClass.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), (ContentPage)Context);
            if (!res.MajorStatus)
            {
                await ((ContentPage)Context).DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }

            if (((string)res.MinorStatus) != "\"\"")
            {
                await ((ContentPage)Context).DisplayAlert("Error!", (string)res.MinorStatus.ToString(),"ok");
                return false;
            }

            //Update docs list at main page
            Device.BeginInvokeOnMainThread(()=> {
                AppGlobals.refMainPageContext.UpdateList();
            });

            return true;
        }
        public async Task<bool> DeleteItem(OutputStructure locOS)
        {
            
            if (!CrossConnectivity.Current.IsConnected)
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            string OutputString = SerialObj_ThisClass.Serialize(locOS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), (ContentPage)Context);
            if (!res.MajorStatus)
            {
                await ((ContentPage)Context).DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }

            try
            {
                InputStructure IS = new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus);
                if (IS.Status != "ok")
                {
                    await ((ContentPage)Context).DisplayAlert("Error!", (string)res.MinorStatus.ToString(), "ok");
                    return false;
                }
            }
            catch
            {
                await Context.DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return false;
            }
            
            return true;
        }
        public async Task<bool> GetItemListByFilter(ContentPage Context, ItemsRequestStructure P, string Filter)
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "GetItemListByFilter";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            
            OS.Value0 = P.Metadata;
            OS.Value1 = Filter;
            OS.Value2 = P.ParentMetadata;
            OS.Value3 = P.ParentUID;
            

            if (!CrossConnectivity.Current.IsConnected)
            {
                await ((MainPage)Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            JsonSerialization JS = new JsonSerialization();
            string OutputString = JS.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), Context);
            if (!res.MajorStatus)
            {
                await Context.DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }
            try
            {
                FillObjectsList(new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus), (ItemList)Context,P);
            }
            catch
            {
                await Context.DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return false;
            }

            return true;
        }
        public async Task<bool> GetGoodByBarcode(string Barcode)
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "GetGoodByBarcode";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            OS.Value0 = Barcode;
            
            if (!CrossConnectivity.Current.IsConnected)
            {
                await (Context).DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return false;
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            JsonSerialization JS = new JsonSerialization();
            string OutputString = JS.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), Context);
            if (!res.MajorStatus)
            {
                await Context.DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return false;
            }
            try
            {
                ReplaceObjectInGoodRedactingPage(new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus));
            }
            catch
            {
                await Context.DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return false;
            }

            return true;
        }
        public async Task<InputStructure> GetGoodListWithBarcode(string Barcode)
        {
            OutputStructure OS = new OutputStructure();
            OS.Intent = "GetGoodListWithBarcode";
            OS.DeviceID = new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID");
            OS.Value0 = Barcode;

            if (!CrossConnectivity.Current.IsConnected)
            {
                await Context.DisplayAlert("Error!", "Нет подключения к сети.", "ok");
                return new InputStructure();
            }
            object ip = "";
            App.Current.Properties.TryGetValue("ext_Address", out ip);

            string FullServiceAddr = ip.ToString();
            JsonSerialization JS = new JsonSerialization();
            string OutputString = JS.Serialize(OS);

            Status res = await HttpRequest(OutputString, FullServiceAddr,
                new CommonProcs().GetProperty("ext_Login"), new CommonProcs().GetProperty("ext_Password"), (ContentPage)Context);
            if (!res.MajorStatus)
            {
                await Context.DisplayAlert("Error!", "Ошибка при обмене с сервисом: " + res.MinorStatus.ToString(), "ok");
                return new InputStructure();
            }
            try
            {
                return new JsonSerialization().deserializeJSON<InputStructure>((string)res.MinorStatus);
            }
            catch
            {
                await Context.DisplayAlert("Error!", "Ошибка при разборке полученного пакета!", "ok");
                return new InputStructure();
            }
        }
        public void FilGoodsList(InputStructure IS, Page_GoodsList Context)
        {
            //Context.lv_Goods = new ObservableCollection<Good>();
            Context.lv_Goods.Clear();
            foreach (TablePart TP in IS.Table0)
            {
                Context.lv_Goods.Add(new Good
                {
                    Metadata = TP.Metadata,
                    nGood = TP.Value0,
                    uidGood = TP.UID,
                    nUnit = TP.Value1,
                    uidUnit = TP.Value2,
                    nGoodDescription = TP.Value3,
                    uidGoodDescription = TP.Value4,
                    nSerial = TP.Value5,
                    uidSerial = TP.Value6,
                    nQuality = TP.Value7,
                    uidQuality = TP.Value8,
                    Amount = TP.Value9,
                    AmountAquired = TP.Value10,
                    Value0 = TP.Value11,
                    Value1 = TP.Value12,
                    Value2 = TP.Value13,
                    mUnit = TP.Value14,
                    mGoodDescription = TP.Value15,
                    mSerial = TP.Value16,
                    mQuality = TP.Value17,
                    StringID = TP.Value18,
                    Barcode = TP.Value19

                });
            }
        }
        public async Task<Status> HttpRequest(string Params, string HttpAddr, string Login, string Password, ContentPage Context)
        {
            Status ret = new Status();
            ret.MajorStatus = false;

            DataCompression DC = new DataCompression();
            string B64 = DC.CompressString(Params);
            

            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(Convert.ToInt32(new CommonProcs().GetProperty("ext_Timeout"))) })
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(HttpAddr);
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("Accept", "application/json");
                    request.Content = new StringContent(B64);
                    request.Headers.Add("Authorization", CreateCredentialsHeader(Login, Password));
                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            HttpContent responseContent = response.Content;
                            ret.MinorStatus = DC.Extract(await responseContent.ReadAsStringAsync());
                            ret.MajorStatus = true;
                        }
                        else
                        {
                            try
                            {
                                HttpContent ResponseContent = response.Content;
                                ret.Description = response.StatusCode.ToString();
                                ret.MinorStatus = DC.Extract(await ResponseContent.ReadAsStringAsync());
                            }
                            catch
                            {
                                ret.Description = "ServerResponseError";
                                ret.MinorStatus = response.StatusCode.ToString();
                            }
                            
                        }

                    }
                    catch (Exception e)
                    {
                        ret.Description = "ServerResponseError";
                        ret.MinorStatus = e.Message;
                    }
                }
            }
            return ret;
        }
        public string CreateCredentialsHeader(string Login, string Password)
        {
            string _auth = string.Format("{0}:{1}", Login, Password);
            string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
            return string.Format("{0} {1}", "Basic", _enc);
        }
        public void FillDocumentsList(InputStructure IS, MainPage Context)
        {
            Context.lv_Documents.Clear();
            foreach (TablePart TP in IS.Table0)
            {
                Context.lv_Documents.Add(new Document {
                    Metadata = TP.Metadata,
                    UID = TP.UID,
                    Number = TP.Value0,
                    Date = TP.Value1,
                    Value0 = TP.Value2,
                    Value1 = TP.Value3,
                    Value2 = TP.Value4});
            }
        }
        public void FillObjectsList(InputStructure IS, ItemList Page_IL, ItemsRequestStructure locParent)
        {
            Page_IL.lv_Items.Clear();
            foreach (TablePart TP in IS.Table0)
            {
                Page_IL.lv_Items.Add(new TablePart
                {   Metadata = TP.Metadata,
                    UID = TP.UID,
                    Value0 = TP.Value0,
                    Value1 = TP.Value1,
                    Value2 = TP.Value2,
                    Value3 = TP.Value3,
                    Value4 = TP.Value4,
                    Value5 = TP.Value5,
                    Value6 = TP.Value6,
                    Value7 = TP.Value7,
                    Value8 = TP.Value8,
                    Value9 = TP.Value9,
                    Value10 = TP.Value10,
                    Value11 = TP.Value11,
                    Value12 = TP.Value12,
                    Value13 = TP.Value13,
                    Value14 = TP.Value14,
                    Value15 = TP.Value15,
                    Value16 = TP.Value16,
                    Value17 = TP.Value17,
                    Value18 = TP.Value18,
                    Value19 = TP.Value19,
                });

            }
        }
        public void ReplaceObjectInGoodRedactingPage(InputStructure IS)
        {
            foreach (TablePart TP in IS.Table0)
            {
                //((Page_GoodRedacting)Context).GoodItem = new Good();

                ((Page_GoodRedacting)Context).GoodItem.Metadata = TP.Metadata;
                ((Page_GoodRedacting)Context).GoodItem.uidGood = TP.UID;
                ((Page_GoodRedacting)Context).GoodItem.nGood = TP.Value0;

                ((Page_GoodRedacting)Context).GoodItem.uidUnit = TP.Value1;
                ((Page_GoodRedacting)Context).GoodItem.nUnit = TP.Value2;
                ((Page_GoodRedacting)Context).GoodItem.mUnit = TP.Value3;

                ((Page_GoodRedacting)Context).GoodItem.uidGoodDescription = TP.Value4;
                ((Page_GoodRedacting)Context).GoodItem.nGoodDescription = TP.Value5;
                ((Page_GoodRedacting)Context).GoodItem.mGoodDescription = TP.Value6;

                ((Page_GoodRedacting)Context).GoodItem.uidSerial = TP.Value7;
                ((Page_GoodRedacting)Context).GoodItem.nSerial = TP.Value8;
                ((Page_GoodRedacting)Context).GoodItem.mSerial = TP.Value9;

                ((Page_GoodRedacting)Context).GoodItem.uidQuality = TP.Value10;
                ((Page_GoodRedacting)Context).GoodItem.nQuality = TP.Value11;
                ((Page_GoodRedacting)Context).GoodItem.mQuality = TP.Value12;

                ((Page_GoodRedacting)Context).GoodItem.Barcode = TP.Value19;
                ((Page_GoodRedacting)Context).os_barcodes.Clear();
                foreach (string BC in TP.Value19)
                {
                    ((Page_GoodRedacting)Context).os_barcodes.Add(new BarcodeItem { Barcode = BC });
                }
                    
            }
        }
    }
       

    [DataContract]
    public class OutputStructure
    {
        public OutputStructure()
        {
            Table0 = new List<TablePart>();
        }

        [DataMember]
        public string Intent;
        [DataMember]
        public string DeviceID;
        [DataMember]
        public string Value0;
        [DataMember]
        public string Value1;
        [DataMember]
        public string Value2;
        [DataMember]
        public string Value3;
        [DataMember]
        public List<TablePart> Table0;
    }
    [DataContract]
    public class InputStructure
    {
        public InputStructure()
        {
            Table0 = new List<TablePart>();
            return;
        }
        [DataMember]
        public string Status;
        [DataMember]
        public string Value0;
        [DataMember]
        public string Value1;
        [DataMember]
        public string Value2;
        [DataMember]
        public string Value3;
        [DataMember]
        public List<TablePart> Table0;
    }
    
    [DataContract]
    public class TablePart
    {
        public int Count = 22;

        public object this [int index]
        {
            set
            {
                switch (index)
                {
                    case 0:
                        UID = (string)value;
                        return;
                    case 1:
                        Metadata = (string)value;
                        return;
                    case 2:
                        Value0 = (string)value;
                        return;
                    case 3:
                        Value1 = (string)value;
                        return;
                    case 4:
                        Value2 = (string)value;
                        return;
                    case 5:
                        Value3 = (string)value;
                        return;
                    case 6:
                        Value4 = (string)value;
                        return;
                    case 7:
                        Value5 = (string)value;
                        return;
                    case 8:
                        Value6 = (string)value;
                        return;
                    case 9:
                        Value7 = (string)value;
                        return;
                    case 10:
                        Value8 = (string)value;
                        return;
                    case 11:
                        Value9 = (string)value;
                        return;
                    case 12:
                        Value10 = (string)value;
                        return;
                    case 13:
                        Value11 = (string)value;
                        return;
                    case 14:
                        Value12 = (string)value;
                        return;
                    case 15:
                        Value13 = (string)value;
                        return;
                    case 16:
                        Value14 = (string)value;
                        return;
                    case 17:
                        Value15 = (string)value;
                        return;
                    case 18:
                        Value16 = (string)value;
                        return;
                    case 19:
                        Value17 = (string)value;
                        return;
                    case 20:
                        Value18 = (string)value;
                        return;
                    case 21:
                        Value19 = (string[])value;
                        return;
                    default: return;
                }

            }
            get
            {
                switch (index)
                {
                    case 0: return UID;
                    case 1: return Metadata;
                    case 2: return Value0;
                    case 3: return Value1;
                    case 4: return Value2;
                    case 5: return Value3;
                    case 6: return Value4;
                    case 7: return Value5;
                    case 8: return Value6;
                    case 9: return Value7;
                    case 10: return Value8;
                    case 11: return Value9;
                    case 12: return Value10;
                    case 13: return Value11;
                    case 14: return Value12;
                    case 15: return Value13;
                    case 16: return Value14;
                    case 17: return Value15;
                    case 18: return Value16;
                    case 19: return Value17;
                    case 20: return Value18;
                    case 21: return Value19;
                    default: return null;
                }
            }
        }

        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string Metadata { get; set; }
        [DataMember]
        public string Value0 { get; set; }
        [DataMember]
        public string Value1 { get; set; }
        [DataMember]
        public string Value2 { get; set; }
        [DataMember]
        public string Value3 { get; set; }
        [DataMember]
        public string Value4 { get; set; }
        [DataMember]
        public string Value5 { get; set; }
        [DataMember]
        public string Value6 { get; set; }
        [DataMember]
        public string Value7 { get; set; }
        [DataMember]
        public string Value8 { get; set; }
        [DataMember]
        public string Value9 { get; set; }
        [DataMember]
        public string Value10 { get; set; }
        [DataMember]
        public string Value11 { get; set; }
        [DataMember]
        public string Value12 { get; set; }
        [DataMember]
        public string Value13 { get; set; }
        [DataMember]
        public string Value14 { get; set; }
        [DataMember]
        public string Value15 { get; set; }
        [DataMember]
        public string Value16 { get; set; }
        [DataMember]
        public string Value17 { get; set; }
        [DataMember]
        public string Value18 { get; set; }
        [DataMember]
        public string[] Value19 { get; set; }
    }

}
