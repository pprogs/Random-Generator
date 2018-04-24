using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ModelView _model;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _model = new ModelView();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command as RoutedUICommand;

            if (command.Name == "Paste")
            {
                Dimension dim = _model.PasteToDimension();
                if (dim != null)
                {
                    _model.AddDimension(dim);
                    _model.SelectedDimension = dim;                   
                }

            }
        }


        void PasteFromClipboard()
        {

            string t = Clipboard.GetText(TextDataFormat.Text);

            if (string.IsNullOrWhiteSpace(t))
                return;

            string[] lines = t.Split('\n');

            DataTable dt = new DataTable();
            bool firstRow = true;

            foreach (string line in lines )
            {
                string[] cols = line.TrimEnd('\r').Split('\t');

                if (firstRow)
                {
                    for (int c = 0; c < cols.Length; ++c)
                    {
                        dt.Columns.Add(cols[c]);
                    }

                    firstRow = false;
                    continue;
                }
                               
                DataRow row = dt.NewRow();

                for ( int c = 0; c < cols.Length; ++c)
                {
                    row[c] = cols[c];
                }

                dt.Rows.Add(row);
            
            }

            dataGrid.ItemsSource = dt.AsDataView();

            MessageBox.Show( $"Lines count {lines.Length}");
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            PasteFromClipboard();
        }
    }
}
