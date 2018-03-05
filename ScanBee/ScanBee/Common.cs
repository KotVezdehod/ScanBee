using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace ScanBee
{
    public class Status
    {
        public bool MajorStatus;
        public object MinorStatus;
        public string Description;
    }

    public class CommonProcs
    {
        public string GetProperty(string Name)
        {
            object p = "";
            App.Current.Properties.TryGetValue(Name, out p);
            return p.ToString();
        }
        public void DefaultSettingsToPtopertyes()
        {
            foreach (KeyValuePair<string, object> kv in new Parameters())
            {
                object v = "";
                if (!(App.Current.Properties.TryGetValue(kv.Key, out v)))
                {
                    App.Current.Properties.Add(kv.Key, kv.Value);
                }
            }
        }
        
    }

    public class ScanSupport
    {
        public ContentPage refCurrentPageContext;
        public ScanSupport()
        {
            
        }
        public async void BarcodeScanned(string Barcode, string ScanPurpose)
        {
            refCurrentPageContext = (ContentPage)AppGlobals.refCurrentPageContext;
            if (refCurrentPageContext.GetType() == typeof(Page_GoodsList))       //Наборка товаров
            {

                ObservableCollection<Good> FoundedGoods = new ObservableCollection<Good>();
                
                foreach (Good g in ((Page_GoodsList)refCurrentPageContext).lv_Goods)
                {
                    
                    foreach (string BC in g.Barcode)
                    {
                        if (Barcode == BC)
                        {
                            FoundedGoods.Add(g);
                            break;
                        }
                    }
                }
                if (FoundedGoods.Count == 0)
                {
                    await refCurrentPageContext.DisplayAlert("штрихкод не найден!", "", "OK");
                }
                else if (FoundedGoods.Count == 1)
                {
                    await refCurrentPageContext.Navigation.PushAsync(new Page_EnterValue(FoundedGoods[0], refCurrentPageContext, null, "EnterAmount"));
                }
                else
                {
                    ItemList IL = new ItemList(null, null, null, refCurrentPageContext, "DisplayItemListForSelection");
                    foreach (Good g in FoundedGoods)
                    {
                        IL.lv_Items.Add(new TablePart {Metadata = g.Metadata, UID = g.uidGood, Value0 = g.nGood });
                    }

                    await refCurrentPageContext.Navigation.PushAsync(IL);
                }
            }
            if (refCurrentPageContext.GetType() == typeof(Page_GoodRedacting))       //Добавление ШК
            {
                object CheckForBKUniquness;
                App.Current.Properties.TryGetValue("ext_TabSel_Other_CheckForBarcodeUniqueness", out CheckForBKUniquness);

                if ((bool)CheckForBKUniquness)
                {

                    //Поищем на стороне ИБ
                    Exchange EE = new Exchange(refCurrentPageContext);
                    InputStructure BarcodeList = await EE.GetGoodListWithBarcode(Barcode);


                    //Поищем здесь
                    List < TablePart > tmpList = new List<TablePart>();
                    foreach (Good locGood in AppGlobals.refPage_GoodList.lv_Goods)
                    {
                        bool boolAllreadyAtList = false;
                        foreach (TablePart tp in BarcodeList.Table0)
                        {
                            if (tp.UID == locGood.uidGood)
                            {
                                boolAllreadyAtList = true;
                                break;
                            } ;
                        }
                        if (boolAllreadyAtList) continue;

                        foreach (string locBC in locGood.Barcode)
                        {
                            if (Barcode == locBC)
                            {
                                //BarcodeList.Table0.Add(new TablePart { Metadata = locGood.Metadata, UID = locGood.uidGood, Value0 = locGood.nGood });
                                tmpList.Add(new TablePart { Metadata = locGood.Metadata, UID = locGood.uidGood, Value0 = locGood.nGood });
                                break;
                            }
                        }
                    }

                    foreach (TablePart locTP in tmpList)
                    {
                        BarcodeList.Table0.Add(new TablePart { Metadata = locTP.Metadata, UID = locTP.UID, Value0 = locTP.Value0 });
                    }

                    if (BarcodeList.Table0.Count > 0)
                    {
                        var Answer = await ((Page_GoodRedacting)refCurrentPageContext).DisplayAlert("Scan Bee", "Штрихкод не уникален по базе данных! Показать список совпадений?", "Да", "Нет");
                        if (Answer)
                        {
                            ItemList IL = new ItemList(null, null, null, refCurrentPageContext, "OnlyDisplayItemList");
                            foreach (TablePart tp in BarcodeList.Table0)
                            {
                                IL.lv_Items.Add(new TablePart { Metadata = tp.Metadata, UID = tp.UID, Value0 = tp.Value0 });
                            }
                            
                            await refCurrentPageContext.Navigation.PushAsync(IL);
                        }
                        return;
                    }
                }

                Page_GoodRedacting Page_GoodRedacting = (Page_GoodRedacting)refCurrentPageContext;

                bool BarcodesIsEqual = false;
                foreach (string BC in Page_GoodRedacting.GoodItem.Barcode)
                {
                    if (BC == Barcode)
                    {
                        BarcodesIsEqual = true;
                        break;
                    }
                }

                if (!BarcodesIsEqual)
                {
                    string[] NewString = new string[Page_GoodRedacting.GoodItem.Barcode.Length + 1];

                    int counter = 0;
                    foreach (string BC in Page_GoodRedacting.GoodItem.Barcode)
                    {
                        NewString[counter] = BC;
                        counter++;
                    };
                    NewString[NewString.Length - 1] = Barcode;
                    Page_GoodRedacting.GoodItem.Barcode = NewString;

                    Page_GoodRedacting.os_barcodes.Clear();
                    foreach (string BC in Page_GoodRedacting.GoodItem.Barcode)
                    {
                        Page_GoodRedacting.os_barcodes.Add(new BarcodeItem { Barcode = BC });
                    }
                    Page_GoodRedacting.DisplayAlert("Штрихкод добавлен", "", "Ok");
                }
                else
                {
                    Page_GoodRedacting.DisplayAlert("ScanBee", "Штрихкод уже в списке!", "", "Ok");
                };
                
            }

            if (refCurrentPageContext.GetType() == typeof(Page_EnterValue))          //Поиск товара по ШК
            {
                await ((Page_EnterValue)refCurrentPageContext).Navigation.PopAsync();        //ждем, пока в стеке верхняя не будет страница Page_GoodRedacting, потом дергаем сервер
                Exchange EE = new Exchange((Page_GoodRedacting)AppGlobals.refCurrentPageContext);
                await EE.GetGoodByBarcode(Barcode);
            }
            if (refCurrentPageContext.GetType() == typeof(Page_Settings))                //Загрузка настроек по qr-коду
            {
                if (ScanPurpose == "SettingsBarcodeScan")
                {
                    try
                    {
                        string Json = new DataCompression().Extract(Barcode);
                        Settings_From_QRCode SFQ = new JsonSerialization().deserializeJSON<Settings_From_QRCode>(Json);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            foreach (KeyValuePair<string, object> kv in ((Page_Settings)refCurrentPageContext).Parameters)
                            {
                                if (((Page_Settings)refCurrentPageContext).Parameters[kv.Key] != null)
                                {
                                    //if (((Page_Settings)refCurrentPageContext).Parameters[kv.Key].GetType() == typeof(string))
                                    //{
                                        ((Page_Settings)refCurrentPageContext).Parameters[kv.Key] = SFQ[kv.Key.Replace("ext_", "")];
                                    //}
                                    //if (((Page_Settings)refCurrentPageContext).Parameters[kv.Key].GetType() == typeof(int))
                                    //{
                                    //    ((Page_Settings)refCurrentPageContext).Parameters[kv.Key] = SFQ[kv.Key.Replace("ext_", "")];
                                    //}
                                    //if (((Page_Settings)refCurrentPageContext).Parameters[kv.Key].GetType() == typeof(bool))
                                    //{
                                    //    ((Page_Settings)refCurrentPageContext).Parameters[kv.Key] = SFQ[kv.Key.Replace("ext_", "")];
                                    //}

                                }
                                
                            }
                        });
                        
                    }
                    catch
                    {
                        ((Page)refCurrentPageContext).DisplayAlert("Не удалось распознать входную структуру настроек!", "", "OK");
                    };

                }

            }
            
        }
        
    }
}
