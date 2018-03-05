using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Xamarin.Forms;

namespace ScanBee
{
    public class Page_EnterValue : ContentPage, INotifyPropertyChanged
    {
        public ContentPage ParentPage;
        public Good refGood;
        public string filter { get; set; }
        public ItemsRequestStructure Parent;
        private string Mode;
                
        public Page_EnterValue(Good locGood, ContentPage locPage, ItemsRequestStructure locParent, string locMode)
        {
            Mode = locMode;
            ParentPage = locPage;
            Parent = locParent;
            refGood = locGood;
            switch (Mode)
            {
                case "Filter":
                    Title = "Введите фильтр/сканируйте ШК";
                    break;
                case "EnterAmount":
                    Title = "Введите число";
                    Filter = refGood.AmountAquired == "0" ? "": refGood.AmountAquired;
                    break;
                case "Enlight":
                    Title = "Введите строку поиска";
                    break;
                default:
                    break;

            }
            
            StackLayout stack = new StackLayout
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BindingContext = this

            };

            StackLayout StackVertical = new StackLayout
            {
                BackgroundColor = (Color)App.Current.Resources["backColor"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                BindingContext = this

            };
            
            Entry E = new Entry
            {
                BackgroundColor = Color.Gray,
                TextColor = (Color)App.Current.Resources["textColor"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BindingContext = this
            };
            E.SetBinding(Entry.TextProperty, "Filter");
            //E.Focus();

            StackVertical.Children.Add(E);

            if (AppGlobals.refMicrophoneFeature)
            {
                Image I1 = new Image
                {
                    Source = "VoiceRec.PNG",
                    HorizontalOptions = LayoutOptions.End
                };

                TapGestureRecognizer tgr = new TapGestureRecognizer
                {
                    NumberOfTapsRequired = 1

                };
                tgr.Tapped += Tgr__Image1_Tapped;
                I1.GestureRecognizers.Add(tgr);
                StackVertical.Children.Add(I1);
            }

            //stack.Children.Add(E);
            stack.Children.Add(StackVertical);

            Button B;
            
            if (Mode == "Filter")
            {
                if (new CommonProcs().GetProperty("ext_ScanHardWare") == "1")
                {
                    B = new Button
                    {
                        Text = "Сканировать",
                    };
                    B.Clicked += Сканировать_Clicked;
                    stack.Children.Add(B);
                }
            }
            
            B = new Button
            {
                Text = "Ok"
            };
            B.Clicked += Ok_Clicked;
            stack.Children.Add(B);

            Content = stack;
        }

        private void Tgr__Image1_Tapped(object sender, EventArgs e)
        {
            IVoice IV = DependencyService.Get<IVoice>();
            IV.StartVoceIntent(this);
        }

        private void Сканировать_Clicked(object sender, EventArgs e)
        {
            //ScanSupport.BarcodeScanned("020217/0009");          //!!!!DEBUG!!!!!
            Scaner scaner = new Scaner();
            scaner.ScanBarcode(this, null);

        }

        private void Ok_Clicked(object sender, EventArgs e)
        {
            if (Mode == "Filter")
            {
                if (Parent.Intent == "Good" || Parent.Intent == "GoodDescription"
                || Parent.Intent == "Serial"
                || Parent.Intent == "Unit"
                || Parent.Intent == "Quality")
                {
                    Navigation.InsertPageBefore(new ItemList(refGood, Filter, Parent, this, "RequestServerForItems"), this);
                    Navigation.PopAsync();
                }
                else
                {
                    refGood[Parent.VarName] = Filter;
                    Navigation.PopAsync();
                }
            }
            if (Mode == "EnterAmount")
            {
                refGood.AmountAquired = Filter;
                Navigation.PopAsync();
            }
            if (Mode == "Enlight")
            {
                if (typeof(Page_GoodsList) == ParentPage.GetType())
                {
                    ((Page_GoodsList)ParentPage).PatternForLocalFilter = Filter;
                }
                Navigation.PopAsync();
            }

        }
        protected override void OnAppearing()
        {
            AppGlobals.refCurrentPageContext = this;
            
        }
        //=============== INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public string Filter
        {
            get { return filter; }
            set
            {
                if (filter != value)
                {
                    filter = value;
                    OnPropertyChanged("Filter");
                }
            }
        }
        
    }
}
