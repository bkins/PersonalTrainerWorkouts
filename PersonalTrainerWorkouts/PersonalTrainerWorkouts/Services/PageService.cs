using System.Threading.Tasks;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Services
{
    public class PageService : IPageService
    {
        public async Task DisplayAlert(string title
                                     , string message
                                     , string ok)
        {
            await MainPage.DisplayAlert(title
                                      , message
                                      , ok);
        }

        public async Task<bool> DisplayAlert(string title
                                           , string message
                                           , string ok
                                           , string cancel)
        {
            return await MainPage.DisplayAlert(title
                                             , message
                                             , ok
                                             , cancel);
        }

        public async Task PushAsync(Page page)
        {
            await MainPage.Navigation.PushAsync(page);
        }

        public async Task<Page> PopAsync()
        {
            return await MainPage.Navigation.PopAsync();
        }

        private static Page MainPage => Application.Current.MainPage;
    }
}
