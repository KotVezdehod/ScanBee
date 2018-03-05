using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScanBee
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page_GoodsList : ContentPage
    {
        private Page Context;
        private Document SelectedDocument;
        public ObservableCollection<Good> lv_Goods { get; set; }
        private string internal_PatternForLocalFilter = "";
        public Page_GoodsList(Page locContext, Document locSelectedDocument)
        {
            InitializeComponent();
            SetUpVisibility();
            
            Context = locContext;
            SelectedDocument = locSelectedDocument;
                        
            lv_Goods = new ObservableCollection<ScanBee.Good>();
            //{
            //    new ScanBee.Good
            //    {
            //        nGood = "GoodName",
            //        uidGood = "34523454563456456",
            //        nUnit = "шт.",
            //        uidUnit = "0099877667787777",
            //        nGoodDescription = "Характеристика",
            //        uidGoodDescription = "ertw45234srwertw",
            //        nSerial = "S/N",
            //        uidSerial = "fgsdfgwertwertsdfg",
            //        nQuality = "Новый",
            //        uidQuality = "cvbxcvbxcvbxcvbxcbv",
            //        Amount = "10",
            //        AmountAquired = "5",
            //        Value0 = "V0",
            //        Value1 = "V1",
            //        Value2 = "V2"
            //    }
            //};

            Device.BeginInvokeOnMainThread(MethodInvoker);
            
            this.BindingContext = this;

            ToolbarItems.Add(new ToolbarItem("Button_EnlightByString", "loop_72_72.PNG", async () =>
            {
                await Navigation.PushAsync(new Page_EnterValue(null, this, null, "Enlight"));
            }, ToolbarItemOrder.Default, 0));

            ToolbarItems.Add(new ToolbarItem("Button_AddItem", "Plus_72_72.PNG", async () =>
            {
                lv_Goods.Add(new Good { Metadata = "Справочник.Номенклатура" , Barcode = new string[0]});
                await Navigation.PushAsync(new Page_GoodRedacting(lv_Goods[lv_Goods.Count-1],lv_Goods, SelectedDocument));
            }, ToolbarItemOrder.Default, 1));


            //ToolbarItems.Add(new ToolbarItem("Button_FindItem", "loop_72_72.PNG", async () =>
            //{

            //}, ToolbarItemOrder.Default, 1));
  
        }
 
        protected override void OnAppearing()
        {
            AppGlobals.refCurrentPageContext = this;
            AppGlobals.refPage_GoodList = this;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(GetUserOppinion);

            return true;
        }

        public async void GetUserOppinion()
        {
            var Answer = await DisplayAlert("ScanBee", "Выйти (все изменения будут потеряны)?", "Да", "Нет");
            if ((bool)Answer)
            {
                Navigation.PopAsync();
            }

        }

        private async void MethodInvoker()
        {
            try
            {
                Exchange EE = new Exchange(this);
                bool result = await EE.GetGoodList(SelectedDocument.UID);
            }
            catch (Exception e) // handle whatever exceptions you expect
            {
                //Handle exceptions
            }
        }

        private async void Button_Clicked_Ok(object sender, EventArgs e)
        {
            var Answer = await DisplayAlert("ScanBee", "Завершить наборку?", "Да", "Нет");
            if ((bool)Answer)
            {
                Device.BeginInvokeOnMainThread(SendResultToServerInvoker);
                Navigation.PopAsync();
            }
            
        }

        private async void SendResultToServerInvoker()
        {

            try
            {
                Exchange EE = new Exchange(this);
                bool result = await EE.SendGoodListToServer(lv_Goods, SelectedDocument);
            }
            catch (Exception e) // handle whatever exceptions you expect
            {
                //Handle exceptions
            }
        }

        private void GoodList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Navigation.PushAsync(new Page_GoodRedacting((Good)((ListView)sender).SelectedItem,lv_Goods,SelectedDocument));
                ((ListView)sender).SelectedItem = null;
            }
        }

        private void SetUpVisibility()
        {
            CommonProcs CP = new CommonProcs();
            if (CP.GetProperty("ext_GoodListView_GoodName_IsVivsible") == "True")
            {
                Resources["lv_GoodName_col_width"] = CP.GetProperty("ext_GoodListView_GoodName_Width");
            }
            else
            {
                Resources["lv_GoodName_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_UnitName_IsVivsible") == "True")
            {
                Resources["lv_UnitName_col_width"] = CP.GetProperty("ext_GoodListView_UnitName_Width");
            }
            else
            {
                Resources["lv_UnitName_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_GoodDescription_IsVivsible") == "True")
            {
                Resources["lv_GoodDescription_col_width"] = CP.GetProperty("ext_GoodListView_GoodDescription_Width");
            }
            else
            {
                Resources["lv_GoodDescription_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_GoodSerial_IsVivsible") == "True")
            {
                Resources["lv_GoodSerial_col_width"] = CP.GetProperty("ext_GoodListView_GoodSerial_Width");
            }
            else
            {
                Resources["lv_GoodSerial_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_GoodQuality_IsVivsible") == "True")
            {
                Resources["lv_GoodQuality_col_width"] = CP.GetProperty("ext_GoodListView_GoodQuality_Width");
            }
            else
            {
                Resources["lv_GoodQuality_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_GoodAmount_IsVivsible") == "True")
            {
                Resources["lv_GoodAmount_col_width"] = CP.GetProperty("ext_GoodListView_GoodAmount_Width");
            }
            else
            {
                Resources["lv_GoodAmount_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_GoodAmountAquired_IsVivsible") == "True")
            {
                Resources["lv_GoodAmountAquired_col_width"] = CP.GetProperty("ext_GoodListView_GoodAmountAquired_Width");
            }
            else
            {
                Resources["lv_GoodAmountAquired_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_Value0_IsVivsible") == "True")
            {
                Resources["lv_Value0_col_width"] = CP.GetProperty("ext_GoodListView_Value0_Width");
            }
            else
            {
                Resources["lv_Value0_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_Value1_IsVivsible") == "True")
            {
                Resources["lv_Value1_col_width"] = CP.GetProperty("ext_GoodListView_Value1_Width");
            }
            else
            {
                Resources["lv_Value1_col_width"] = "0";
            }

            if (CP.GetProperty("ext_GoodListView_Value2_IsVivsible") == "True")
            {
                Resources["lv_Value2_col_width"] = CP.GetProperty("ext_GoodListView_Value2_Width");
            }
            else
            {
                Resources["lv_Value2_col_width"] = "0";
            }

            Resources["lv_GoodName_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodName_IsVivsible");
            Resources["lv_UnitName_IsVivsible"] = CP.GetProperty("ext_GoodListView_UnitName_IsVivsible");
            Resources["lv_GoodDescription_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodDescription_IsVivsible");
            Resources["lv_GoodSerial_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodSerial_IsVivsible");
            Resources["lv_GoodQuality_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodQuality_IsVivsible");
            Resources["lv_GoodAmount_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodAmount_IsVivsible");
            Resources["lv_GoodAmountAquired_IsVivsible"] = CP.GetProperty("ext_GoodListView_GoodAmountAquired_IsVivsible");
            Resources["lv_Value0_IsVivsible"] = CP.GetProperty("ext_GoodListView_Value0_IsVivsible");
            Resources["lv_Value1_IsVivsible"] = CP.GetProperty("ext_GoodListView_Value1_IsVivsible");
            Resources["lv_Value2_IsVivsible"] = CP.GetProperty("ext_GoodListView_Value2_IsVivsible");

            if (CP.GetProperty("ext_ScanHardWare") == "1")
            {
                Resources["btn_Scan_IsVivsible"] = true;
            }
            else
            {
                Resources["btn_Scan_IsVivsible"] = false;
            }

            
        }

        private void Page_GoodList_Scan_Clicked(object sender, EventArgs e)
        {
            //ScanSupport.BarcodeScanned("020217/0008");          //!!!!DEBUG!!!!!
            //Device.BeginInvokeOnMainThread(MethodInvoker_Scaning);
            Scaner scaner = new Scaner();
            scaner.ScanBarcode(this, null);
        }

        //public async void MethodInvoker_Scaning()
        //{

        //}

        public string PatternForLocalFilter
        {
            get { return internal_PatternForLocalFilter; }
            set
            {
                if (internal_PatternForLocalFilter != value)
                {
                    internal_PatternForLocalFilter = value;
                    OnPropertyChanged("PatternForLocalFilter");
                    string locGoodName = "";
                    string locPattern = value == null ? "" : value;
                    locPattern = locPattern.ToUpper();

                    foreach (Good g in lv_Goods)
                    {
                        locGoodName = g.nGood.ToUpper();
                        
                        if (value == null)
                        {
                            g.LocalFilter = "0";
                        }
                        else if (value == "")
                        {
                            g.LocalFilter = "0";
                        }
                        else
                        {
                            if (locGoodName.Contains(locPattern))
                            {
                                g.LocalFilter = "1";
                            }
                            else
                            {
                                g.LocalFilter = "0";
                            }
                        }
                        
                    }
                }
            }

        }
    }
    
    public class Good : IEnumerable, IEnumerator, INotifyPropertyChanged
    {
        private string int_Metadata;
        private string int_uidGood { get; set; }
        private string int_nGood { get; set; }
        private string int_uidUnit { get; set; }
        private string int_nUnit { get; set; }
        private string int_mUnit { get; set; }
        private string int_uidGoodDescription { get; set; }
        private string int_nGoodDescription { get; set; }
        private string int_mGoodDescription { get; set; }
        private string int_uidSerial { get; set; }
        private string int_nSerial { get; set; }
        private string int_mSerial { get; set; }
        private string int_uidQuality { get; set; }
        private string int_nQuality { get; set; }
        private string int_mQuality { get; set; }
        private string int_Amount { get; set; }
        private string int_AmountAquired { get; set; }
        private string int_Value0 { get; set; }
        private string int_Value1 { get; set; }
        private string int_Value2 { get; set; }
        private string int_Value3 { get; set; }
        private string int_StringID { get; set; }
        private string[] int_Barcode { get; set; }
        private string int_Amount_Equal_AmountEquired { get; set; }
        private string int_LocalFilter { get; set; }

        public Good()
        {
            int_uidGood = "00000000-0000-0000-0000-000000000000";
            int_uidUnit = "00000000-0000-0000-0000-000000000000";
            int_uidGoodDescription = "00000000-0000-0000-0000-000000000000";
            int_uidSerial = "00000000-0000-0000-0000-000000000000";
            int_uidQuality = "00000000-0000-0000-0000-000000000000";
            int_StringID = "00000000-0000-0000-0000-000000000000";
        }

        public string Metadata
        { get { return int_Metadata; }
          set
            {
                if (int_Metadata != value)
                {
                    int_Metadata = value;
                    OnPropertyChanged("Metadata");
                }
            }
        }
        public string uidGood
        {
            get { return int_uidGood; }
            set
            {
                if (int_uidGood != value)
                {
                    int_uidGood = value;
                    OnPropertyChanged("uidGood");
                }
            }
        }
        public string nGood
        {
            get { return int_nGood; }
            set
            {
                if (int_nGood != value)
                {
                    int_nGood = value;
                    OnPropertyChanged("nGood");
                }
            }
        }
        public string uidUnit
        {
            get { return int_uidUnit; }
            set
            {
                if (int_uidUnit != value)
                {
                    int_uidUnit = value;
                    OnPropertyChanged("uidUnit");
                }
            }
        }
        public string nUnit
        {
            get { return int_nUnit; }
            set
            {
                if (int_nUnit != value)
                {
                    int_nUnit = value;
                    OnPropertyChanged("nUnit");
                }
            }
        }
        public string mUnit
        {
            get { return int_mUnit; }
            set
            {
                if (int_mUnit != value)
                {
                    int_mUnit = value;
                    OnPropertyChanged("mUnit");
                }
            }
        }
        public string uidGoodDescription
        {
            get { return int_uidGoodDescription; }
            set
            {
                if (int_uidGoodDescription != value)
                {
                    int_uidGoodDescription = value;
                    OnPropertyChanged("uidGoodDescription");
                }
            }
        }
        public string nGoodDescription
        {
            get { return int_nGoodDescription; }
            set
            {
                if (int_nGoodDescription != value)
                {
                    int_nGoodDescription = value;
                    OnPropertyChanged("nGoodDescription");
                }
            }
        }
        public string mGoodDescription
        {
            get { return int_mGoodDescription; }
            set
            {
                if (int_mGoodDescription != value)
                {
                    int_mGoodDescription = value;
                    OnPropertyChanged("mGoodDescription");
                }
            }
        }
        public string uidSerial
        {
            get { return int_uidSerial; }
            set
            {
                if (int_uidSerial != value)
                {
                    int_uidSerial = value;
                    OnPropertyChanged("uidSerial");
                }
            }
        }
        public string nSerial
        {
            get { return int_nSerial; }
            set
            {
                if (int_nSerial != value)
                {
                    int_nSerial = value;
                    OnPropertyChanged("nSerial");
                }
            }
        }
        public string mSerial
        {
            get { return int_mSerial; }
            set
            {
                if (int_mSerial != value)
                {
                    int_mSerial = value;
                    OnPropertyChanged("mSerial");
                }
            }
        }
        public string uidQuality
        {
            get { return int_uidQuality; }
            set
            {
                if (int_uidQuality != value)
                {
                    int_uidQuality = value;
                    OnPropertyChanged("uidQuality");
                }
            }
        }
        public string nQuality
        {
            get { return int_nQuality; }
            set
            {
                if (int_nQuality != value)
                {
                    int_nQuality = value;
                    OnPropertyChanged("nQuality");
                }
            }
        }
        public string mQuality
        {
            get { return int_mQuality; }
            set
            {
                if (int_mQuality != value)
                {
                    int_mQuality = value;
                    OnPropertyChanged("mQuality");
                }
            }
        }
        public string Amount
        {
            get { return int_Amount; }
            set
            {
                if (this.AmountAquired == value)
                {
                    Amount_Equal_AmountEquired = "1";
                }
                else
                {
                    Amount_Equal_AmountEquired = "0";
                }
                if (int_Amount != value)
                {
                    int_Amount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }
        public string AmountAquired
        {
            get { return int_AmountAquired; }
            set
            {
                if (this.Amount == value)
                {
                    Amount_Equal_AmountEquired = "1";
                }
                else
                {
                    Amount_Equal_AmountEquired = "0";
                }

                if (int_AmountAquired != value)
                {
                    int_AmountAquired = value;
                    OnPropertyChanged("AmountAquired");
                }
            }
        }
        public string Value0
        {
            get { return int_Value0; }
            set
            {
                if (int_Value0 != value)
                {
                    int_Value0 = value;
                    OnPropertyChanged("Value0");
                }
            }
        }
        public string Value1
        {
            get { return int_Value1; }
            set
            {
                if (int_Value1 != value)
                {
                    int_Value1 = value;
                    OnPropertyChanged("Value1");
                }
            }
        }
        public string Value2
        {
            get { return int_Value2; }
            set
            {
                if (int_Value2 != value)
                {
                    int_Value2 = value;
                    OnPropertyChanged("Value2");
                }
            }
        }
        public string Value3
        {
            get { return int_Value3; }
            set
            {
                if (int_Value3 != value)
                {
                    int_Value3 = value;
                    OnPropertyChanged("Value3");
                }
            }
        }
        public string StringID
        {
            get { return int_StringID; }
            set
            {
                if (int_StringID != value)
                {
                    int_StringID = value;
                    OnPropertyChanged("StringID");
                }
            }
        }
        public string[] Barcode
        {
            get { return int_Barcode; }
            set
            {
                if (int_Barcode != value)
                {
                    int_Barcode = value;
                    OnPropertyChanged("Barcode");
                }
            }
        }
        public string Amount_Equal_AmountEquired
        {
            get { return int_Amount_Equal_AmountEquired; }
            set
            {
                if (int_Amount_Equal_AmountEquired != value)
                {
                    int_Amount_Equal_AmountEquired = value;
                    OnPropertyChanged("Amount_Equal_AmountEquired");
                }
            }
        }
        public string LocalFilter
        {
            get { return int_LocalFilter; }
            set
            {
                if (int_LocalFilter != value)
                {
                    int_LocalFilter = value;
                    OnPropertyChanged("LocalFilter");
                }
            }
        }


        //=============== INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        //=============== Get/Set by index
        public int Count =25;

        public object this[int Index]
        {
            set
            {
                switch (Index)
                {
                    case 0:
                        Metadata = (string)value;
                        return;
                    case 1:
                        uidGood = (string)value;
                        return;
                    case 2:
                        nGood = (string)value;
                        return;
                    case 3:
                        uidUnit = (string)value;
                        return;
                    case 4:
                        nUnit = (string)value;
                        return;
                    case 5:
                        mUnit = (string)value;
                        return;
                    case 6:
                        uidGoodDescription = (string)value;
                        return;
                    case 7:
                        nGoodDescription = (string)value;
                        return;
                    case 8:
                        mGoodDescription = (string)value;
                        return;
                    case 9:
                        uidSerial = (string)value;
                        return;
                    case 10:
                        nSerial = (string)value;
                        return;
                    case 11:
                        mSerial = (string)value;
                        return;
                    case 12:
                        uidQuality = (string)value;
                        return;
                    case 13:
                        nQuality = (string)value;
                        return;
                    case 14:
                        mQuality = (string)value;
                        return;
                    case 15:
                        Amount = (string)value;
                        return;
                    case 16:
                        AmountAquired = (string)value;
                        return;
                    case 17:
                        Value0 = (string)value;
                        return;
                    case 18:
                        Value1 = (string)value;
                        return;
                    case 19:
                        Value2 = (string)value;
                        return;
                    case 20:
                        Value3 = (string)value;
                        return;
                    case 21:
                        StringID = (string)value;
                        return;
                    case 22:
                        Barcode = (string[])value;
                        return;
                    case 23:
                        Amount_Equal_AmountEquired = (string)value;
                        return;
                    case 24:
                        LocalFilter = (string)value;
                        return;

                    default: return;
                }
            }

            get
            {
                switch (Index)
                {
                    case 0: return Metadata;
                    case 1: return uidGood;
                    case 2: return nGood;
                    case 3: return uidUnit;
                    case 4: return nUnit;
                    case 5: return mUnit;
                    case 6: return uidGoodDescription;
                    case 7: return nGoodDescription;
                    case 8: return mGoodDescription;
                    case 9: return uidSerial;
                    case 10: return nSerial;
                    case 11: return mSerial;
                    case 12: return uidQuality;
                    case 13: return nQuality;
                    case 14: return mQuality;
                    case 15: return Amount;
                    case 16: return AmountAquired;
                    case 17: return Value0;
                    case 18: return Value1;
                    case 19: return Value2;
                    case 20: return Value3;
                    case 21: return StringID;
                    case 22: return Barcode;
                    case 23: return Amount_Equal_AmountEquired;
                    case 24: return LocalFilter;
                    default: return null;
                }
            }
        }

        public object this[string Index]
        {
            set
            {
                switch (Index)
                {
                    case "Metadata":
                        Metadata = (string)value;
                        return;
                    case "uidGood":
                        uidGood = (string)value;
                        return;
                    case "nGood":
                        nGood = (string)value;
                        return;
                    case "uidUnit":
                        uidUnit = (string)value;
                        return;
                    case "nUnit":
                        nUnit = (string)value;
                        return;
                    case "mUnit":
                        mUnit = (string)value;
                        return;
                    case "uidGoodDescription":
                        uidGoodDescription = (string)value;
                        return;
                    case "nGoodDescription":
                        nGoodDescription = (string)value;
                        return;
                    case "mGoodDescription":
                        mGoodDescription = (string)value;
                        return;
                    case "uidSerial":
                        uidSerial = (string)value;
                        return;
                    case "nSerial":
                        nSerial = (string)value;
                        return;
                    case "mSerial":
                        mSerial = (string)value;
                        return;
                    case "uidQuality":
                        uidQuality = (string)value;
                        return;
                    case "nQuality":
                        nQuality = (string)value;
                        return;
                    case "mQuality":
                        mQuality = (string)value;
                        return;
                    case "Amount":
                        Amount = (string)value;
                        return;
                    case "AmountAquired":
                        AmountAquired = (string)value;
                        return;
                    case "Value0":
                        Value0 = (string)value;
                        return;
                    case "Value1":
                        Value1 = (string)value;
                        return;
                    case "Value2":
                        Value2 = (string)value;
                        return;
                    case "Value3":
                        Value3 = (string)value;
                        return;
                    case "StringID":
                        StringID = (string)value;
                        return;
                    case "Barcode":
                        Barcode = (string[])value;
                        return;
                    case "Amount_Equal_AmountEquired":
                        Amount_Equal_AmountEquired = (string)value;
                        return;
                    case "LocalFilter":
                        LocalFilter = (string)value;
                        return;
                    default: return;
                }
            }

            get
            {
                switch (Index)
                {
                    case "Metadata": return Metadata;
                    case "uidGood": return uidGood;
                    case "nGood": return nGood;
                    case "uidUnit": return uidUnit;
                    case "nUnit": return nUnit;
                    case "mUnit": return mUnit;
                    case "uidGoodDescription": return uidGoodDescription;
                    case "nGoodDescription": return nGoodDescription;
                    case "mGoodDescription": return mGoodDescription;
                    case "uidSerial": return uidSerial;
                    case "nSerial": return nSerial;
                    case "mSerial": return mSerial;
                    case "uidQuality": return uidQuality;
                    case "nQuality": return nQuality;
                    case "mQuality": return mQuality;
                    case "Amount": return Amount;
                    case "AmountAquired": return AmountAquired;
                    case "Value0": return Value0;
                    case "Value1": return Value1;
                    case "Value2": return Value2;
                    case "Value3": return Value3;
                    case "StringID": return StringID;
                    case "Barcode": return Barcode;
                    case "Amount_Equal_AmountEquired": return Amount_Equal_AmountEquired;
                    case "LocalFilter": return LocalFilter;
                    default: return null;
                }
            }
        }

        //=============== IEnumerable, IEnumerator

        int index = -1;

        public IEnumerator GetEnumerator()
        {
            return this;
        }
        public void Reset() { index = -1; }
        public bool MoveNext()
        {
            if (index == 25)
            {
                index = -1;
                return false;
            }
            index++;
            return true;

        }
        public object Current
        {
            get
            {
                switch (index)
                {
                    case 0: return Metadata;
                    case 1: return uidGood;
                    case 2: return nGood;
                    case 3: return uidUnit;
                    case 4: return nUnit;
                    case 5: return mUnit;
                    case 6: return uidGoodDescription;
                    case 7: return nGoodDescription;
                    case 8: return mGoodDescription;
                    case 9: return uidSerial;
                    case 10: return nSerial;
                    case 11: return mSerial;
                    case 12: return uidQuality;
                    case 13: return nQuality;
                    case 14: return mQuality;
                    case 15: return Amount;
                    case 16: return AmountAquired;
                    case 17: return Value0;
                    case 18: return Value1;
                    case 19: return Value2;
                    case 20: return Value3;
                    case 21: return StringID;
                    case 22: return Barcode;
                    case 23: return Amount_Equal_AmountEquired;
                    case 24: return LocalFilter;
                    default: return null;
                }
            }
        }
        
    }
    
    //Classes for renderers
 
    public class ViewCell_GoodPage : ViewCell
    {

    }

    public class Grid_GoodPage : Grid
    {
        
    }
    
    //Classes for IValueConverter
    public class myNumericColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((String)value)
            {
                case "0":
                    return Color.Red;
                case "":
                    return Color.Red;
                default:
                    return App.Current.Resources["backColor"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class Enlight_Color_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((String)value)
            {
                case null:
                    return App.Current.Resources["backColor"];
                case "0":
                    return App.Current.Resources["backColor"];
                case "":
                    return App.Current.Resources["backColor"];
                default:
                    return Color.ForestGreen;
                    
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        
    }
    
}
