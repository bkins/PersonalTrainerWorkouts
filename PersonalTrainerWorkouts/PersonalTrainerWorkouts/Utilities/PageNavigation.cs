using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class PageNavigation
    {
        public static async Task NavigateTo(string nameOfPage)
        {
            await Navigate($"{nameOfPage}");
        }

        public static async Task NavigateTo(string nameOfPage
                                          , string nameOfParameter1
                                          , string valueOfParameter1)
        {
            try
            {
                await Navigate($"{nameOfPage}?{nameOfParameter1}={valueOfParameter1}");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Could not navigate to page: {nameOfPage}, because {e.Message}"
                               , Category.Error
                               , e);

                throw;
            }
        }

        public static async Task NavigateTo(string nameOfPage
                                          , string nameOfParameter1
                                          , string valueOfParameter1
                                          , string nameOfParameter2
                                          , string valueOfParameter2)
        {
            await Navigate($"{nameOfPage}?{nameOfParameter1}={valueOfParameter1}&{nameOfParameter2}={valueOfParameter2}");
        }

        private static async Task Navigate(string path)
        {
            Logger.WriteLine($"Navigating to: {path}."
                           , Category.Information);

            await Shell.Current.GoToAsync(path);
        }

        public static async Task NavigateBackwards()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
