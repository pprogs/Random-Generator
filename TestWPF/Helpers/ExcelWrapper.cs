using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCore = NetOffice.Core;
using NExcel = NetOffice.ExcelApi;
using NOffice = NetOffice.OfficeApi;
using NEnums = NetOffice.OfficeApi.Enums;
using NExcelE = NetOffice.ExcelApi.Enums;

namespace TestWPF
{
    public class ExcelWrapper : IDisposable
    {
        NExcel.Application _appl = null;
        bool _disposed = false;

        public NExcel.Application Appl { get { return _appl; } }


        ~ExcelWrapper()
        {
            InternalClose();
        }

        public void Create(bool visible = false)
        {
            if (_disposed)
                throw new ObjectDisposedException("ExcelWrapper");

            _appl = new NExcel.Application();
            if (!visible)
            {
                _appl.Visible = false;
                _appl.DisplayAlerts = false;
                _appl.ScreenUpdating = false;
            }
        }

        public void Show()
        {
            if (_disposed)
                throw new ObjectDisposedException("ExcelWrapper");

            if (_appl == null)
                return;

            _appl.ScreenUpdating = true;
            _appl.DisplayAlerts = true;
            _appl.Visible = true;
        }

        public void Close()
        {
            if (_disposed)
                throw new ObjectDisposedException("ExcelWrapper");

            Dispose();
        }

        public NExcel.Workbook AddWorkbook()
        {
            if (_disposed)
                throw new ObjectDisposedException("ExcelWrapper");
            if (_appl == null)
                return null;

            return _appl.Workbooks.Add();           
        }

        void InternalClose()
        {
            if (_appl != null)
            {
                if (!_appl.Visible)
                {
                    try { _appl.Quit(); }
                    catch { }
                }

                _appl.Dispose(true);
                _appl = null;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            InternalClose();

            GC.SuppressFinalize(this);
        }          
    }

    public static class ExcelExt
    {
        public static NetOffice.ExcelApi.Worksheet AddSheet(this NetOffice.ExcelApi.Sheets sheets)
        {
            return (NExcel.Worksheet)sheets.Add();
        }

        public static NetOffice.ExcelApi.Worksheet AddSheet(this NetOffice.ExcelApi.Workbook book)
        {
            return book.Worksheets.AddSheet();
        }
    }
}
