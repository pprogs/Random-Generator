using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF
{
    [NoMagic]
    public class ModelView : PropertyChangedBase
    {
        ObservableCollection<Dimension> _dimensions;

        [Magic]
        public Dimension SelectedDimension { get; set; }

        public RelayCommand GenerateCommand { get { return new RelayCommand(this.DoGenerate); } }
        public RelayCommand RemoveCommand { get { return new RelayCommand(this.DoRemove); } }
        public RelayCommand AddCommand { get { return new RelayCommand(this.DoAdd); } }
        public ObservableCollection<Dimension> Dimensions { get { return _dimensions; } }

        [Magic]
        public int TotalLinesFrom { get; set; } = 100;
        [Magic]
        public int TotalLinesTo { get; set; } = 100;

        [Magic]
        public float FromAmount { get; set; } = 1500f;
        [Magic]
        public float ToAmount { get; set; } = 2000f;
        [Magic]
        public bool UseSimpleRandom { get; set; } = false;

        

        [Magic]
        public string SomeTest { get; set; } = "Collapsed";

        public ModelView()
        {
            _dimensions = new ObservableCollection<Dimension>();
            
        }

        public void DoGenerate(object o)
        {           
            var baseDim = Dimensions.FirstOrDefault( dim => { return dim.Base; });

            if (baseDim == null || baseDim.DataTable.Rows.Count == 0)
            {
                MessageBox.Show("There must be at least one base dim with lines");
                return;
            }

            int totalCols = Dimensions.Aggregate(0, (total, next) => total += next.DataTable.Columns.Count);

            object[,] exportLines = new object[100, totalCols + 1];
            int exportLine = 0;

            DataTable baseDimTable = baseDim.DataTable;

            IRandomValue val;

            if (UseSimpleRandom)
            {
                val = new SimpleRandomValue(FromAmount, ToAmount);
            }
            else
            {
                val = new RandomValue(FromAmount, ToAmount, baseDim.DataTable.Rows.Count);
            }

            Random rnd = new Random(DateTime.Now.Millisecond);

            using (ExcelWrapper ex = new ExcelWrapper())
            {
                ex.Create();

                var wb = ex.AddWorkbook();
                var sheet = wb.AddSheet();

                int line = 1;      
                bool firstLine = true;

                var dropToExcel = new Action(() =>
                {
                    if (exportLine == 0)
                        return;

                    var r1 = sheet.Cells[line, 1];
                    var r2 = sheet.Cells[line + exportLine - 1, totalCols + 1];

                    NetOffice.ExcelApi.Range rg = sheet.Range(r1, r2);
                    rg.set_Value(NetOffice.ExcelApi.Enums.XlRangeValueDataType.xlRangeValueDefault, exportLines);

                    line += exportLine;
                    exportLine = 0;
                });

                #region generation
                foreach(  DataRow baseRow in baseDimTable.Rows )
                {
                    int totalLines;

                    if (TotalLinesFrom == TotalLinesTo)
                        totalLines = TotalLinesFrom;
                    else
                        totalLines = rnd.Next(TotalLinesFrom, TotalLinesTo);

                    val.NextStep(totalLines);

                    for (int r = 0; r < totalLines; ++r)
                    {
                        int col = 0;

                        foreach (Dimension dim in Dimensions)
                        {
                            if (dim == baseDim)
                            {
                                for (int c = 0; c < baseDimTable.Columns.Count; c++)
                                {
                                    exportLines[exportLine, col++] = firstLine ? baseDimTable.Columns[c].ColumnName : baseRow[c];
                                }
                            }
                            else
                            {
                                int rows = dim.DataTable.Rows.Count;
                                int randRow = rnd.Next(0, rows);

                                DataRow dimRow = dim.DataTable.Rows[randRow];

                                for (int c = 0; c < dim.DataTable.Columns.Count; c++)
                                {
                                    exportLines[exportLine, col++] = firstLine ? dim.DataTable.Columns[c].ColumnName : dimRow[c];
                                }
                            }
                        }

                        //amounts

                        exportLines[exportLine, col++] = firstLine ? (object)"Amount" : (object)val.NextSubStep();

                        if (firstLine)
                        {
                            r--;
                            firstLine = false;
                        }

                        exportLine++;
                        if (exportLine == 100)
                            dropToExcel();
                    }
                }
                dropToExcel();

                #endregion


                ex.Show();
            }
        }
        public void DoRemove(object o)
        {
            if (SelectedDimension == null)
                return;
            
            Dimensions.Remove(SelectedDimension);
            SelectedDimension = null;
        }

        public async void DoAdd(object o)
        {
            Dimension dim = await BaseModel.GetNewDimension();

            if (dim != null)
                AddDimension(dim);
        }
        public void AddDimension(Dimension dim)
        {
            _dimensions.Add(dim);
        }
        public Dimension PasteToDimension()
        {
            string t = Clipboard.GetText(TextDataFormat.Text);

            if (string.IsNullOrWhiteSpace(t))
                return null;

            string[] lines = t.Split('\n');

            DataTable dt = new DataTable();        
            string name = null;

            foreach (string line in lines)
            {
                string[] cols = line.TrimEnd('\r').Split('\t');

                if (name == null)
                {
                    for (int c = 0; c < cols.Length; ++c)
                    {
                        if (c == 0)
                            name = cols[c];
                        else
                            name += "/" + cols[c];

                        dt.Columns.Add(cols[c]);
                    }
                    continue;
                }

                DataRow row = dt.NewRow();
                bool empty = true;

                for (int c = 0; c < cols.Length; ++c)
                {
                    row[c] = cols[c];
                    empty = empty && string.IsNullOrWhiteSpace(cols[c]);
                }

                if (!empty)
                    dt.Rows.Add(row);
            }

            Dimension dim = new Dimension(name);
            dim.DataTable = dt;

            return dim;
        }
    }
}
