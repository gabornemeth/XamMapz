using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XamMapz.Sample
{
    public class TestPage : ContentPage
    {
        public TestPage()
        {
            this.Padding = new Thickness(5);
            Content = new XamMapz.Map();
        }
    }
}
