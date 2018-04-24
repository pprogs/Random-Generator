using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Data;

namespace TestWPF
{
    public static class BaseModel
    {

        public static async Task<Dimension> GetNewDimension()
        {
            AddDimension d = new AddDimension();
            var vm = new AddDimensionViewModel();
            d.DataContext = vm;
          
            var result = d.ShowDialog();

            if (result ?? true)
            {          
                switch( vm.DimensionType)
                {
                    case DimensionType.Names:
                        return await GetNamesDimension(vm);
                    case DimensionType.Date:
                        return GetDateDimension(vm);
                    case DimensionType.Id:
                        return GetIdDimension(vm);
                }
            }

            return null;
        }

        static async Task<Dimension> GetNamesDimension(AddDimensionViewModel vm)
        {
            List<NameEntry> names = await GetRandomNames(vm.NamesCount);
            if (names == null || names.Count == 0)
                return null;

            Dimension dim = new Dimension(vm.DimensionName);

            DataTable dt = dim.DataTable = new DataTable();
            if (vm.Name) dt.Columns.Add("Имя");
            if (vm.SurName) dt.Columns.Add("Фамилия");
            if (vm.Gender) dt.Columns.Add("Пол");
            if (vm.FullName) dt.Columns.Add("Полное имя");
            if (vm.UID) dt.Columns.Add("UID");

            dt.BeginLoadData();

            foreach (var name in names)
            {
                var row = dt.NewRow();
                if (vm.Name) row["Имя"] = name.Name;
                if (vm.SurName) row["Фамилия"] = name.SurName;
                if (vm.Gender) row["Пол"] = name.Gender.ToString();
                if (vm.FullName) row["Полное имя"] = name.FullName;
                if (vm.UID) row["UID"] = System.Guid.NewGuid().ToString("N").ToUpper();

                dt.Rows.Add(row);
            }

            dt.EndLoadData();
      
            return dim;
        }

        static Dimension GetDateDimension(AddDimensionViewModel vm)
        {
            //set starting params
            DateTime from = vm.DateFrom;
            DateTime to = vm.DateTo;
            Func<DateTime, DateTime> IncDate = (ind) => ind.AddDays(1);

            Dimension dim = new Dimension(vm.DimensionName);
            DataTable dt = dim.DataTable = new DataTable();

            DateFields df = vm.DateFields;

            //add field to table for each selected field
            foreach (string fieldName in Enum.GetNames(typeof(DateFields)))
            {
                var field = (DateFields)Enum.Parse(typeof(DateFields), fieldName);
                if (df.HasFlag(field) && field != DateFields.None)
                    dt.Columns.Add(fieldName);
            }                

            //set start and end dates 
            switch (vm.DateLevel)
            {
                case DateDetailLevel.Month:
                    from = new DateTime(from.Year, from.Month, 1);
                    to = new DateTime(to.Year, to.Month, 1);
                    IncDate = (ind) => ind.AddMonths(1);                   
                    break;
                case DateDetailLevel.Year:
                    from = new DateTime(from.Year, 1, 1);
                    to = new DateTime(to.Year, 1, 1);
                    IncDate = (ind) => ind.AddYears(1);
                    break;
            }

            //create records and fill with values
            dt.BeginLoadData();
            while( from < to)
            {
                var row = dt.NewRow();

                if (df.HasFlag(DateFields.DateKey)) row["DateKey"] = from.ToShortDateString();
                if (df.HasFlag(DateFields.DayNum)) row["DayNum"] = from.Day;
                if (df.HasFlag(DateFields.MonthNum)) row["MonthNum"] = from.Month;
                if (df.HasFlag(DateFields.YearNum)) row["YearNum"] = from.Year;

                dt.Rows.Add(row);

                from = IncDate(from);
            }
            dt.EndLoadData();

            return dim;
        }

        static async Task<List<NameEntry>> GetRandomNames(int count = 10)
        {
            var serviceUri = new Uri("http://uinames.com/");
            var proxyUri = WebRequest.DefaultWebProxy?.GetProxy(serviceUri);
            var proxy = proxyUri != null ? new WebProxy(proxyUri) { UseDefaultCredentials = true } : null;

            HttpClientHandler handler = new HttpClientHandler() { Proxy = proxy };
            HttpClient client = new HttpClient(handler);
            
            client.BaseAddress = serviceUri;        
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage resp = await client.GetAsync($"api/?region=russia&amount={count}");           

                string respBody = await resp.Content.ReadAsStringAsync();

                return await Task.Run(new Func<List<NameEntry>>(() =>
                {
                    List<object> names = fastJSON.JSON.Parse(respBody) as List<object>;

                    if (names != null)
                    {
                        List<NameEntry> res = new List<NameEntry>(names.Count);

                        foreach (var name in names)
                        {
                            Dictionary<string, object> v = (name as Dictionary<string, object>);

                            var entry = new NameEntry()
                            {
                                Name = v["name"] as string,
                                SurName = v["surname"] as string,
                                Gender = (v["gender"] as string) == "male" ? Gender.Male : Gender.Female
                            };

                            res.Add(entry);
                        }

                        return res;
                    }

                    return null;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }

            return null;
        }

        static Dimension GetIdDimension(AddDimensionViewModel vm)
        {
            Dimension dim = new Dimension(vm.DimensionName);

            DataTable dt = dim.DataTable = new DataTable();
            if (vm.Name) dt.Columns.Add("id");
           

            dt.BeginLoadData();

            for( int from = vm.fromId; from < vm.toId; from++ )
            {
                var row = dt.NewRow();
                row["id"] = from;
                dt.Rows.Add(row);
            }        

            dt.EndLoadData();

            return dim;
        }
    }


    public struct NameEntry
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string FullName { get { return Name + " " + SurName; } }
        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
