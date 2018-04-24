using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF
{
    public class GenerateModelView : PropertyChangedBase
    {

        public ObservableCollection<AmountEntry> Amounts { get; set; }
        public AmountEntry SelectedAmount { get; set; }




        public GenerateModelView()
        {
            Amounts = new ObservableCollection<AmountEntry>();
        }

    }


    public class AmountEntry : PropertyChangedBase
    {
        public string Name { get; set; }
        public float From { get; set; }
        public float To { get; set; }
        public float Spread { get; set; }
    }
}
