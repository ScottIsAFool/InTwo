using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace InTwo.Views
{
    public partial class MainPage 
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string toClearBackStackOrNot;
            if (NavigationContext.QueryString.TryGetValue("clearbackstack", out toClearBackStackOrNot))
            {
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
        }
    }
}