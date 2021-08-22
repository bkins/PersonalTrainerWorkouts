using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class PageNavigation
    {
        public static async Task NavigateTo(string page)
        {
            await Navigate($"{page}");
        }
        
        public static async Task NavigateTo(string   page
                                          , string nameOfParameter1
                                          , string valueOfParameter1)
        {
            await Navigate($"{page}?{nameOfParameter1}={valueOfParameter1}");
        }

        public static async Task NavigateTo(string   page
                                          , string nameOfParameter1
                                          , string valueOfParameter1
                                          , string nameOfParameter2
                                          , string valueOfParameter2)
        {
            await Navigate($"{page}?{nameOfParameter1}={valueOfParameter1}&{nameOfParameter2}={valueOfParameter2}");
         
        }

        private static async Task Navigate(string path)
        {
            //App.AsyncDatabase.FillModels();
            await Shell.Current.GoToAsync(path);
        }
        public static async Task NavigateBackwards()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}