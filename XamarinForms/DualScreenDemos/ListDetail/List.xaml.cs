using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DualScreenDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class List : CollectionView
    {
        public List()
        {
            InitializeComponent();
            ItemsSource = Enumerable.Range(1, 100)
                .Select(x => new ListDetailsItem(x))
                .ToList();
        }
    }
}