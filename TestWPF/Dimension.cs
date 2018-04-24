using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TestWPF
{
    public class Dimension : PropertyChangedBase
    {
        public string Name { get; set; }
        public bool Base { get; set; }
        public DataTable DataTable { get; set; }

        [NoMagic]
        public DataView GetView { get { return DataTable.AsDataView(); } }


        public Dimension(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
