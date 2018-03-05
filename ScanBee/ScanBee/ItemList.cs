using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace ScanBee
{
    public class ItemList : ContentPage
    {
        public ContentPage Context;
        public string Filter;
        public Good refGood;
        public bool Taped = false;
        public ItemsRequestStructure Parent;
        public ObservableCollection<TablePart> lv_Items = new ObservableCollection<TablePart>();
        private string Mode;
        
        public ItemList(Good reflocGood, string locFilter, ItemsRequestStructure locParent, ContentPage locContext, string locMode)
        {
            refGood = reflocGood;
            Filter = locFilter;
            Parent = locParent;
            Context = locContext;
            Mode = locMode;

            StackLayout stack = new StackLayout
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            //1.Get Element list from server (if this is a good, then fill additional info, like serial, description, Units)


            switch (locMode)
            {
                case "RequestServerForItems":
                    Title = "Найденные объекты";
                    Device.BeginInvokeOnMainThread(MethodInvoker);
                    break;

                case "DisplayItemListForSelection":
                    Title = "Выберите 1 из дублей ШК";
                    break;

                case "OnlyDisplayItemList":
                    Title = "Товары по отбору";
                    break;

                default:
                    break;
            }


            //lv_Items.Add(new TablePart
            //{
            //    Value0 = "Good",
            //    Value1 = "Unit",
            //    Value3 = "Description",
            //    Value5 = "Serial",
            //    Value7 = "Quality",
            //});

            //2.Build page layout
            ListView lv_Main = new ListView
            {
                ItemsSource = lv_Items,
                ItemTemplate = new DataTemplate(() =>

                {

                    Grid G = new Grid
                    {
                        ColumnSpacing = 1,
                        BackgroundColor = (Color)App.Current.Resources["textColor"],
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        ColumnDefinitions =
                    {
                            //new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                            //new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            //new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                            //new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            //new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },

                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                        RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(42, GridUnitType.Absolute) }
                    },
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    
                    //======== Field 0
                    
                    Label l = new Label
                    {
                        BackgroundColor = (Color)App.Current.Resources["backColor"],
                        TextColor = (Color)App.Current.Resources["textColor"],
                        FontSize = 13,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,

                    };

                    TapGestureRecognizer tgr = new TapGestureRecognizer
                    {
                        NumberOfTapsRequired = 1,
                        CommandParameter = lv_Items.Count - 1
                    };
                    tgr.Tapped += Tgr_Tapped;

                    l.GestureRecognizers.Add(tgr);

                    l.SetBinding(Label.TextProperty, "Value0");
                    //f.Content = l;
                    G.Children.Add(l, 0, 0);


                    ////======== Field 1
                    
                    //Label l1 = new Label
                    //{
                    //    BackgroundColor = (Color)App.Current.Resources["backColor"],
                    //    TextColor = (Color)App.Current.Resources["textColor"],
                    //    FontSize = 10,
                    //    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //    VerticalOptions = LayoutOptions.FillAndExpand,

                    //};
                    //l1.SetBinding(Label.TextProperty, "Value1");

                    //TapGestureRecognizer tgr1 = new TapGestureRecognizer
                    //{
                    //    NumberOfTapsRequired = 1,
                    //    CommandParameter = lv_Items.Count - 1
                    //};
                    //tgr1.Tapped += Tgr_Tapped;

                    //l.GestureRecognizers.Add(tgr1);

                    ////f1.Content = l1;
                    //G.Children.Add(l1, 1, 0);

                    ////======== Field 2
                    
                    //Label l2 = new Label
                    //{
                    //    BackgroundColor = (Color)App.Current.Resources["backColor"],
                    //    TextColor = (Color)App.Current.Resources["textColor"],
                    //    FontSize = 10,
                    //    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //    VerticalOptions = LayoutOptions.FillAndExpand,

                    //};
                    //l2.SetBinding(Label.TextProperty, "Value3");
                    //TapGestureRecognizer tgr2 = new TapGestureRecognizer
                    //{
                    //    NumberOfTapsRequired = 1,
                    //    CommandParameter = lv_Items.Count - 1
                    //};
                    //tgr2.Tapped += Tgr_Tapped;

                    //l.GestureRecognizers.Add(tgr2);

                    ////f2.Content = l2;
                    //G.Children.Add(l2, 2, 0);

                    ////======== Field 3
                    
                    //Label l3 = new Label
                    //{
                    //    BackgroundColor = (Color)App.Current.Resources["backColor"],
                    //    TextColor = (Color)App.Current.Resources["textColor"],
                    //    FontSize = 10,
                    //    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //    VerticalOptions = LayoutOptions.FillAndExpand,

                    //};
                    //l3.SetBinding(Label.TextProperty, "Value5");
                    //TapGestureRecognizer tgr3 = new TapGestureRecognizer
                    //{
                    //    NumberOfTapsRequired = 1,
                    //    CommandParameter = lv_Items.Count - 1
                    //};
                    //tgr3.Tapped += Tgr_Tapped;

                    //l.GestureRecognizers.Add(tgr3);
                    ////f3.Content = l3;
                    //G.Children.Add(l3, 3, 0);

                    ////======== Field 4
                    
                    //Label l4 = new Label
                    //{
                    //    BackgroundColor = (Color)App.Current.Resources["backColor"],
                    //    TextColor = (Color)App.Current.Resources["textColor"],
                    //    FontSize = 10,
                    //    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //    VerticalOptions = LayoutOptions.FillAndExpand,
                    //};

                    //TapGestureRecognizer tgr4 = new TapGestureRecognizer
                    //{
                    //    NumberOfTapsRequired = 1,
                    //    CommandParameter = lv_Items.Count - 1
                    //};
                    //tgr4.Tapped += Tgr_Tapped;

                    //l4.GestureRecognizers.Add(tgr4);

                    //l4.SetBinding(Label.TextProperty, "Value7");
                    ////f4.Content = l4;
                    //G.Children.Add(l4, 4, 0);


                    StackLayout s = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        BackgroundColor = (Color)App.Current.Resources["textColor"],
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    s.Children.Add(G);
                    
                    return new ViewCell { View = s};
                })
            };

            stack.Children.Add(lv_Main);
            Content = stack;

        }

        private void Tgr_Tapped(object sender, EventArgs e)
        {
            int lvSElectedIndex = (int)((TappedEventArgs)e).Parameter;
            if (!Taped)
            {
                switch(Mode)
                    {
                    case "RequestServerForItems":
                        if (Parent.Intent == "Good")
                        {
                            Navigation.PopAsync();
                            Taped = true;

                            refGood.Metadata = lv_Items[lvSElectedIndex].Metadata;
                            refGood.uidGood = lv_Items[lvSElectedIndex].UID;
                            refGood.nGood = lv_Items[lvSElectedIndex].Value0;

                            refGood.uidUnit = lv_Items[lvSElectedIndex].Value1;
                            refGood.nUnit = lv_Items[lvSElectedIndex].Value2;
                            refGood.mUnit = lv_Items[lvSElectedIndex].Value3;

                            if (lv_Items[lvSElectedIndex].Value4.Length > 0)
                            {
                                refGood.uidGoodDescription = lv_Items[lvSElectedIndex].Value4;
                                refGood.nGoodDescription = lv_Items[lvSElectedIndex].Value5;
                                refGood.mGoodDescription = lv_Items[lvSElectedIndex].Value6;
                            };

                            if (lv_Items[lvSElectedIndex].Value7.Length > 0)
                            {
                                refGood.uidSerial = lv_Items[lvSElectedIndex].Value7;
                                refGood.nSerial = lv_Items[lvSElectedIndex].Value8;
                                refGood.mSerial = lv_Items[lvSElectedIndex].Value9;
                            };

                            if (lv_Items[lvSElectedIndex].Value10.Length > 0)
                            {
                                refGood.uidQuality = lv_Items[lvSElectedIndex].Value10;
                                refGood.nQuality = lv_Items[lvSElectedIndex].Value11;
                                refGood.mQuality = lv_Items[lvSElectedIndex].Value12;
                            }

                            refGood.Barcode = lv_Items[lvSElectedIndex].Value19;
                        }
                        else if (Parent.Intent == "GoodDescription")
                        {
                            Navigation.PopAsync();
                            Taped = true;

                            refGood.uidGoodDescription = lv_Items[lvSElectedIndex].UID;
                            refGood.nGoodDescription = lv_Items[lvSElectedIndex].Value0;
                            refGood.mGoodDescription = lv_Items[lvSElectedIndex].Metadata;

                        }
                        else if (Parent.Intent == "Serial")
                        {
                            Navigation.PopAsync();
                            Taped = true;

                            refGood.uidSerial = lv_Items[lvSElectedIndex].UID;
                            refGood.nSerial = lv_Items[lvSElectedIndex].Value0;
                            refGood.mSerial = lv_Items[lvSElectedIndex].Metadata;
                        }
                        else if (Parent.Intent == "Unit")
                        {
                            Navigation.PopAsync();
                            Taped = true;

                            refGood.uidUnit = lv_Items[lvSElectedIndex].UID;
                            refGood.nUnit = lv_Items[lvSElectedIndex].Value0;
                            refGood.mUnit = lv_Items[lvSElectedIndex].Metadata;
                        }
                        else if (Parent.Intent == "Quality")
                        {
                            Navigation.PopAsync();
                            Taped = true;

                            refGood.uidQuality = lv_Items[lvSElectedIndex].UID;
                            refGood.nQuality = lv_Items[lvSElectedIndex].Value0;
                            refGood.mQuality = lv_Items[lvSElectedIndex].Metadata;
                        }
                        break;

                    case "DisplayItemListForSelection":
                        foreach (Good g in ((Page_GoodsList)Context).lv_Goods)
                        {
                            if (g.uidGood == lv_Items[lvSElectedIndex].UID)
                            {
                                ((Page_GoodsList)Context).Navigation.InsertPageBefore(new Page_EnterValue(g, (Page_GoodsList)Context, null, "EnterAmount"), this);
                                break;
                            }
                        }
                        Navigation.PopAsync();
                        break;

                    case "OnlyDisplayItemList":

                        break;
                    default:
                        break;
                }

            }
        }

        private async void MethodInvoker()
        {
            try
            {
                Exchange EE = new Exchange(this);
                
                bool result = await EE.GetItemListByFilter(this, Parent, Filter);
            }
            catch (Exception e) // handle whatever exceptions you expect
            {
                //Handle exceptions
            }
        }

        protected override void OnAppearing()
        {
            AppGlobals.refCurrentPageContext = this;
        }
    }
    public class ItemsRequestStructure
    {
        public string Intent = "";
        public string Metadata = "";
        public string ParentUID = "";
        public string ParentMetadata = "";
        public string VarName = "";
    }
}
