using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BillPage : Page
    {
        public BillPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var args = SerializationHelper.Deserialize<Bill>(e.Parameter as string);
                try
                {
                    NameTextBlock.Text = args.UserInfo.Name;
                    AddressTextBlock.Text = args.UserInfo.Address;
                    PhoneTextBlock.Text = args.UserInfo.Phone;
                    EmailTextBlock.Text = args.UserInfo.Email;
                    DeliveryDateTextBlock.Text = args.PlanDate.Date.ToString(CultureInfo.InvariantCulture);
                    TotalTextBlock.Text = args.Total.ToString(CultureInfo.InvariantCulture);
                    AccessoryListView.ItemsSource = args.ReturnBuyingDetail;

                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException || ex is AggregateException)
                    {
                        //NoConnectionGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
