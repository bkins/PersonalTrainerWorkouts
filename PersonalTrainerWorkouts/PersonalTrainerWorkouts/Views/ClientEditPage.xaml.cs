using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ClientId)
                 , nameof(ClientId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientEditPage : ContentPage, IQueryAttributable
    {
        public string          ClientId  { get; set; }
        public ClientViewModel ViewModel { get; set; }

        public ClientEditPage()
        {
            InitializeComponent();

            ViewModel = new ClientViewModel();
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                ClientId  = HttpUtility.UrlDecode(query[nameof(ClientId)]);
                ViewModel = new ClientViewModel(ClientId);

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed initiate {nameof(ClientEditPage)}."
                               , Category.Error
                               , e);
            }
        }
        private void LoadData()
        {
            ClientNameEntry.Text      = ViewModel.Client.DisplayName;
            ClientMainPhoneEntry.Text = ViewModel.Client.MainNumber;
        }

        private void SaveButton_OnClicked(object    sender
                                        , EventArgs e)
        {
            //ViewModel.Client.DisplayName = ClientNameEntry.Text;
            //TODO: Update properties unique to the Client class, not in Contact
            ViewModel.Save();
            
            PageNavigation.NavigateBackwards();
        }

        private void LinkContactToolbarItem_OnClicked(object    sender
                                                    , EventArgs e)
        {
            ViewModel.LinkContactToClient();
        }
    }
}