using Application = Microsoft.Maui.Controls.Application;

namespace MauiBlazorApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
