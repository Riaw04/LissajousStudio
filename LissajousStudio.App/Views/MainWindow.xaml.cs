using System.Windows;
using LissajousStudio.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LissajousStudio.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var services = (IServiceProvider)Application.Current.Resources["Services"];
        DataContext = services.GetRequiredService<MainViewModel>();
    }
}
