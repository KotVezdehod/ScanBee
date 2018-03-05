using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace ScanBee
{
    public class BarcodeItem
    {
        public string Barcode { get; set; }
    }
    public class Page_GoodRedacting : ContentPage
    {
        public Good GoodItem = new Good();
        public Good GoodItemRef;
        public ObservableCollection<Good> RefLv_Goods;
        public Document refDocument;
        public ObservableCollection<BarcodeItem> os_barcodes = new ObservableCollection<BarcodeItem>();
        public Page_GoodRedacting(Good locGoodItem, ObservableCollection<Good> locLv_Goods, Document locDocument)
        {
            GoodItemRef = locGoodItem;
            RefLv_Goods = locLv_Goods;
            refDocument = locDocument;
            for (int cur = 0; cur < locGoodItem.Count; cur++)
            {
                GoodItem[cur] = locGoodItem[cur];
            }
            
            StackLayout stack = new StackLayout {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            CommonProcs CP = new CommonProcs();

            //========== GoodName
            if (CP.GetProperty("ext_GoodListView_GoodName_IsVivsible") == "True")
            {
                AddCluster(stack, "ТМЦ: ", new ItemsRequestStructure { Intent = "Good", VarName = "nGood", Metadata = GoodItem.Metadata});
            }

            //========== Description
            if (CP.GetProperty("ext_GoodListView_GoodDescription_IsVivsible") == "True")
            {
                AddCluster(stack, "Хар-ка: ", new ItemsRequestStructure { Intent = "GoodDescription", Metadata = GoodItem.mGoodDescription, VarName = "nGoodDescription", ParentMetadata = GoodItem.Metadata });
            }
            //========== S/N
            if (CP.GetProperty("ext_GoodListView_GoodSerial_IsVivsible") == "True")
            {
                AddCluster(stack, "С/Н: ", new ItemsRequestStructure { Intent = "Serial", Metadata = GoodItem.mSerial, VarName = "nSerial", ParentMetadata = GoodItem.Metadata });
            }
            //========== UnitName
            if (CP.GetProperty("ext_GoodListView_UnitName_IsVivsible") == "True")
            {
                AddCluster(stack, "Ед.: ", new ItemsRequestStructure { Intent = "Unit", Metadata = GoodItem.mUnit, VarName = "nUnit", ParentMetadata = GoodItem.Metadata });
            }
            //========== Quality
            if (CP.GetProperty("ext_GoodListView_GoodQuality_IsVivsible") == "True")
            {
                AddCluster(stack, "Кач-во: ", new ItemsRequestStructure { Intent = "Quality", VarName = "nQuality", Metadata = GoodItem.mQuality});
            }
            //========== Amount
            if (CP.GetProperty("ext_GoodListView_GoodAmount_IsVivsible") == "True")
            {
                AddCluster(stack, "Кол.: ", new ItemsRequestStructure { Intent = "Amount", VarName = "Amount" });
            }
            //========== AmountAquired
            if (CP.GetProperty("ext_GoodListView_GoodAmountAquired_IsVivsible") == "True")
            {
                AddCluster(stack, "Набрано: ", new ItemsRequestStructure { Intent = "AmountAquired", VarName = "AmountAquired" });
            }
            //========== Value0
            if (CP.GetProperty("ext_GoodListView_Value0_IsVivsible") == "True")
            {
                AddCluster(stack, "Д/З_0: ", new ItemsRequestStructure { Intent = "Value0", VarName = "Value0" });
            }
            //========== Value1
            if (CP.GetProperty("ext_GoodListView_Value1_IsVivsible") == "true")
            {
                AddCluster(stack, "Д/З_1: ", new ItemsRequestStructure { Intent = "Value1", VarName = "Value1" });
            }
            //========== Value2
            if (CP.GetProperty("ext_GoodListView_Value2_IsVivsible") == "True")
            {
                AddCluster(stack, "Д/З_2: ", new ItemsRequestStructure { Intent = "Value2", VarName = "Value2" });
            }

            Label l = new Label
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                TextColor = Color.Cyan,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = 14,
                Text = "Штрих-коды товара:"
            };

            stack.Children.Add(l);

            ListView lv_barcodes = new ListView
            {
                ItemsSource = os_barcodes,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                ItemTemplate = new DataTemplate(()=>
                {
                    StackLayout s = new StackLayout
                    {
                        BackgroundColor = (Color)App.Current.Resources["textColor"],
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        //VerticalOptions = LayoutOptions.FillAndExpand,
                    };
                    
                    l = new Label
                    {
                        BackgroundColor = (Color)App.Current.Resources["backColor"],
                        TextColor = Color.LightYellow,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        FontSize = 14,
                    };

                    s.Children.Add(l);

                    l.SetBinding(Label.TextProperty,"Barcode");

                    return new ViewCell { View = s };
                })
                
            };

            lv_barcodes.ItemSelected += Lv_barcodes_ItemSelected;
            
            stack.Children.Add(lv_barcodes);

            Button B;
            if (new CommonProcs().GetProperty("ext_ScanHardWare") == "1")
            {
                B = new Button
                {
                    Text = "Сканировать",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End
                };
                B.Clicked += Scan_Clicked;
                stack.Children.Add(B);
            }

            B = new Button
            {
                Text = "Удалить",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                
            };

            B.Clicked += B_Clicked_Delete;

            stack.Children.Add(B);

            B = new Button
            {
                Text = "Ok",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };
            B.Clicked += Ok_Clicked;

            stack.Children.Add(B);

            this.Content = stack;

        }

        private void Scan_Clicked(object sender, EventArgs e)
        {
            //ScanSupport.BarcodeScanned("020217/0009");          //!!!!DEBUG!!!!!
            Scaner scaner = new Scaner();
            scaner.ScanBarcode(this,null);
        }

        private async void Lv_barcodes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string[] actions = { "Удалить" };
                string res = await DisplayActionSheet("Что сделать?", "Отмена",null, actions);
                if (res == "Удалить")
                {
                    os_barcodes.Remove((BarcodeItem)e.SelectedItem);

                }
                ((ListView)sender).SelectedItem = null;
            }
        }
        

        private async void B_Clicked_Delete(object sender, EventArgs e)
        {
            string[] actions = { "УДАЛИТЬ" };
            string res = await DisplayActionSheet("Удалить строку документа?", "ОТМЕНА", null, actions);
            if (res == "УДАЛИТЬ")
            {
                Device.BeginInvokeOnMainThread(MethodInvoker);
            }
        }

        public async void MethodInvoker()
        {
            Exchange EE = new Exchange(this);
            OutputStructure OS = new OutputStructure
            {
                DeviceID = (string)new CommonProcs().GetProperty("ext_TabSel_Other_DeviceID"),
                Intent = "DeleteItem",
                Value0 = "Good",
                Value1 = refDocument.UID,
                Value2 = GoodItemRef.StringID,
            };
            if (await EE.DeleteItem(OS))
            {
                RefLv_Goods.Remove(GoodItemRef);
                Navigation.PopAsync();
            }
        }

        private void Ok_Clicked(object sender, EventArgs e)
        {
            for (int cur = 0; cur < GoodItem.Count; cur++)
            {
                GoodItemRef[cur] = GoodItem[cur];
            }

            GoodItemRef.Barcode = new string[os_barcodes.Count];
            if (os_barcodes.Count > 0)
            {
                int counter = 0;
                foreach (BarcodeItem bci in os_barcodes)
                {
                    GoodItemRef.Barcode[counter] = bci.Barcode;
                    counter++;
                };
            }
            
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            AppGlobals.refCurrentPageContext = this;
            os_barcodes.Clear();
            foreach (string bc in GoodItem.Barcode)
            {
                os_barcodes.Add(new BarcodeItem { Barcode = bc });
            }
        }

        protected override void OnDisappearing()
        {
            
        }

        public void AddCluster(StackLayout stack, string ClusterName, ItemsRequestStructure locParent)
        {
            Grid grid = CreateGrid(90);
            Frame f = new Frame
            {
                OutlineColor = Color.White,
                Padding = new Thickness(1),
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            f.Content = new Label
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                TextColor = Color.Gray,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Text = ClusterName,
            };

            grid.Children.Add(f, 0, 0);

            Label l = new Label
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                TextColor = (Color)App.Current.Resources["textColor"],
                FontSize = 14,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                BindingContext = GoodItem,

            };
            l.SetBinding(Label.TextProperty, locParent.VarName);
            TapGestureRecognizer tgr = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                CommandParameter = locParent
            };
            tgr.Tapped += Tgr_Tapped;
            l.GestureRecognizers.Add(tgr);
            
            Frame f1 = new Frame
            {
                OutlineColor = Color.White,
                Padding = new Thickness(1),
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            f1.Content = l;

            grid.Children.Add(f1, 1, 0);
            
            //Image I1 = new Image
            //{
            //    Source = "loop_72_72.PNG"
            //};
            //TapGestureRecognizer tgr1 = new TapGestureRecognizer { NumberOfTapsRequired = 1, CommandParameter = locParent };
            //tgr1.Tapped += Tgr1_Tapped;
            //I1.GestureRecognizers.Add(tgr1);

            //grid.Children.Add(I1,2,0);

            stack.Children.Add(grid);
        }

        private void Tgr1_Tapped(object sender, EventArgs e)
        {
            if (((TappedEventArgs)e).Parameter == "Good")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Unit")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "GoodDescription")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Serial")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Quality")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Amount")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "AmountAquired")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Value0")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Value1")
            {

            }
            else if (((TappedEventArgs)e).Parameter == "Value2")
            {

            }
        }

        private void Tgr_Tapped(object sender, EventArgs e)
        {
            ItemsRequestStructure P = (ItemsRequestStructure)((TappedEventArgs)e).Parameter;
            if (P.Intent == "AmountAquired")
            {
                Navigation.PushAsync(new Page_EnterValue(GoodItem, this, P, "EnterAmount"));
                return;
            }

            if (P.Intent == "GoodDescription")
            {
                P.ParentUID = GoodItem.uidGood;
            };

            if (P.Intent == "Serial")
            {
                P.ParentUID = GoodItem.uidGood;
            };

            if (P.Intent == "Unit")
            {
                P.ParentUID = GoodItem.uidGood;
            };

            Navigation.PushAsync(new Page_EnterValue(GoodItem, this, P, "Filter"));
        }

        public Grid CreateGrid(double H0)
        {

            Grid g = new Grid
            {
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.Start,
                ColumnSpacing = 1,
                ColumnDefinitions =
                    {
                    new ColumnDefinition { Width = new GridLength(H0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(40, GridUnitType.Absolute) },
                    },

                RowDefinitions =
                    {
                    new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
                    },
            };

            return g;
        }
    }
    
}
