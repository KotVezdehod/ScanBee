
using ScanBee;
using Android.Content;
using Android.Views;
using ScanBee.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ViewCell_GoodPage), typeof(localViewCellRenderer))]
[assembly: ExportRenderer(typeof(Grid_GoodPage), typeof(localGridRenderer))]
namespace ScanBee.Droid
{
    
    public class localViewCellRenderer : ViewCellRenderer
    {
        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);
        }

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            return base.GetCellCore(item, convertView, parent, context);
        }

    }
    public class localGridRenderer : ViewRenderer<Xamarin.Forms.Grid, Android.Widget.GridView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Grid> e)
        {
            if (e != null)
            {
                string a = ((Label)e.NewElement.Children[0]).Text;
                if (a.Length < 40)
                {
                    e.NewElement.RowDefinitions[0].Height = 60;
                }
                else if (a.Length < 60)
                {
                    e.NewElement.RowDefinitions[0].Height = 80;
                }
                else if (a.Length < 80)
                {
                    e.NewElement.RowDefinitions[0].Height = 100;
                }
                else
                {
                    e.NewElement.RowDefinitions[0].Height = 140;
                }
                
            }
            base.OnElementChanged(e);
        }
    }

}