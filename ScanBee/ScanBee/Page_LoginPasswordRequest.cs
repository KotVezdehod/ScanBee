using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace ScanBee
{

    public class LoginPasswordRequest : ContentPage
    {
        Entry PasswordEntry;
        Button buttonOk;
        MainPage Context;
        public LoginPasswordRequest(MainPage Cont)
        {
            Context = Cont;
            PasswordEntry = new Entry
            {
                Placeholder = "Supervisor password",
                TextColor = Color.Gray,
                IsPassword = true,
                PlaceholderColor = Color.Gray
            };

            buttonOk = new Button { Text = "ok" };
            buttonOk.Clicked += OnButtonClicked;

            StackLayout Stack = new StackLayout { BackgroundColor = Color.Black };
            Stack.Children.Add(PasswordEntry);
            Stack.Children.Add(buttonOk);


            Content = Stack;
        }
        protected override void OnAppearing()
        {
            AppGlobals.refCurrentPageContext = this;
        }

        private void OnButtonClicked(object Sender, System.EventArgs e)
        {

            if ((string)new CommonProcs().GetProperty("ext_SupervisorPassword") == PasswordEntry.Text)
            {
                Navigation.InsertPageBefore(new Page_Settings(Context), this);
                Navigation.PopAsync();
            }
            else
            {
                this.DisplayAlert("", "Wrong password!", "Ok");
                Navigation.PopAsync();
            }
        }
    }
}
