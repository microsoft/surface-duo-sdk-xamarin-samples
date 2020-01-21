using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Duo.Forms.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoPage : DuoPage
    {
        public TwoPage()
        {
            InitializeComponent();
            cv.ItemsSource =
                Enumerable.Range(0, 1000)
                    .Select(i => $"Page {i}")
                    .ToList();
        }
    }
}