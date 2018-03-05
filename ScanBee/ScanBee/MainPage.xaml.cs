//1. Кнопку "ок" на mainPage по которой будет передаваться список товаров обратно на сервер - ok
//2. Процедуры обработки нажатия на позиции в форме редактирования строки 
//2.1 Вызывается стр. фильтра; 2.2 ВВодится значение фильтр и кн. ок.; 2.3. Вызывается страница списка объектов;
// 2.4. Запрос на сервер с переданным значением фильтра и объектом метаданных; 2.5. Ответ - заполнение списка объектов
// 2.6. Выбор объекта; 2.7. RemovePage(-1); 2.8. Получить доп. сведения по объекту, в зависимости от от его типа
//2.9. перезаполнить объект по ссылке на страницу корректировки товара 
//2.10. выполнить сопутствующие операции, в зависимости от типа объекта (например, очистить хар-ки, единицы, серии и установить их по-умолчанию)
//2.11. PopAsync()
//3. Передача данных с фильтром, владельцем объекта и именем метаданных на сторону сервера
//3.1 Поиск подходящих строк на стороне сервера (с учетом владельца) и возврат списка значений обратно на устройство
//3.2 Обновление?
//4. Голосовой поиск
//поле штрихкода - ок

//При вводе новой строки документа обновлять сразу и штрихкоды по выбранному ключу товаров - ok

//Передача состава штрихкодов по товару обратно на сервер - ok

//Страницы, где применяется сканер штрихкодов: Page_GoodsList (наборка товара), 
//Page_GoodRedacting (добавление нового штрихкода для товара), 
//Page_EnterValue(поиск товара по штрихкоду)
//дополнить класс Good полями метаданных каждого элемента (для серии, хар-ки и единицы) - ok
//Сделать замену строки вместо добавления - ok
//Сообщение о не найденном товаре на странице Page_GoodRedacting

//ДОПОЛНИТЕЛЬНО
//реализовать поиск номенклатуры по штрихкоду из формы ввода значения - ok
//Заполнение настроек по ШК
//Проверка штрихкода на уникальность
//При сканировании неуникального по списку товаров текущего документа штрихкода - вывод списка позиций для уточнения выбора.


using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace ScanBee
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Document> lv_Documents = new ObservableCollection<Document>();
        public MainPage()
        {
            InitializeComponent();
            Resources["lv_DocNumber_col_width"] = "100";
            Resources["lv_DocData_col_width"] = "80";
            Resources["lv_DocValue0_col_width"] = "40";
            Resources["lv_DocValue1_col_width"] = "70";
            Resources["lv_DocValue2_col_width"] = "0";

            //lv_Documents = new ObservableCollection<Document> {
            //    new Document { Number = "0000000001", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000002", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000003", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000001", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000002", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000003", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000001", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000002", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000003", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000001", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000002", Date = "05.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //    new Document { Number = "0000000010", Date = "06.06.2017", Value0="Balakhna", Value1 = "111", Value2 = "Smth"},
            //};

            Documents.ItemsSource = lv_Documents;

            new CommonProcs().DefaultSettingsToPtopertyes();
            
            
            ToolbarItems.Add(new ToolbarItem("Button_Settings", "Settings.png", async () =>
            {
                if (((string)new CommonProcs().GetProperty("ext_SupervisorPassword")).Length == 0)
                {
                    Navigation.PushAsync(new Page_Settings(this));
                }
                else
                {
                    Navigation.PushAsync(new LoginPasswordRequest(this));
                }
            },ToolbarItemOrder.Default,1));
            
            ToolbarItems.Add(new ToolbarItem("Button_Refresh", "refresh_72_72.png",async ()=> {
                Device.BeginInvokeOnMainThread(UpdateList);
            },ToolbarItemOrder.Default,0));
            AppGlobals.refCurrentPageContext = this;

            AppGlobals.refMainPageContext = this;
        }
                
        private void Documents_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Navigation.PushAsync(new Page_GoodsList(this, (Document)((ListView)sender).SelectedItem));
                ((ListView)sender).SelectedItem = null;
            }
        }
        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(UpdateList);
            AppGlobals.refCurrentPageContext = this;
        }

        public async void UpdateList()
        {
            try
            {
                Exchange EE = new Exchange(this);
                bool result = await EE.GetDocumentsList();
            }
            catch (Exception e) // handle whatever exceptions you expect
            {
                //Handle exceptions
            }
        }
        
    }
    public class Document
    {
        public string Metadata { get; set; }
        public string UID { get; set; }
        public string Number { get; set; }
        public string Date { get; set; }
        public string Value0 { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }
}
