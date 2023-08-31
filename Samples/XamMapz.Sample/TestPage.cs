using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace XamMapz.Sample
{
    public class TestPage : ContentPage
    {
        XamMapz.Map _map;

        public TestPage()
        {
            _map = new XamMapz.Map();
            this.Padding = new Thickness(5);
            Content = _map;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _map.Center = new Position(46.83, 16.83);
        }
    }
}
