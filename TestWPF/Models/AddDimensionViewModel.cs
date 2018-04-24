
using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TestWPF
{

    public enum DateDetailLevel
    {
        Year,
        Quarter,
        Month,
        Week,
        Day
    }

    public enum DimensionType
    {
        Date,
        Names,
        Id,
        Value
    }

    [Flags]
    public enum DateFields : byte
    {
        None = 0,
        DateKey = 1,
        YearNum = 2,
        QuarterNum = 4,
        MonthNum = 8,
        MonthName = 16,
        WeekNum = 32,
        WeekDay = 64,
        DayNum = 128
    }

    [Magic]
    public class AddDimensionViewModel : PropertyChangedBase
    {
        public static AddDimensionViewModel Instance { get { return new AddDimensionViewModel();} }

        DimensionType _dimensionType = DimensionType.Id;
        public DimensionType DimensionType
        {
            get { return _dimensionType; }
            set
            {
                OnDimensionTypeChange(_dimensionType, value);
                _dimensionType = value;                
            }
        }

        public string DimensionName { get; set; } = "New Dimension Name";

        private void OnDimensionTypeChange(DimensionType oldDim, DimensionType newDim)
        {
            _dimensionType = newDim;
        }

        #region Date dimension

        public DateTime DateFrom { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime DateTo { get; set; } = DateTime.Now;
        public DateDetailLevel DateLevel { get; set; } = DateDetailLevel.Day;
        public DateFields DateFields { get; set; } = DateFields.MonthNum | DateFields.YearNum;

        public RelayCommand DoDateFieldSelectCommand {  get { return _doDateFieldSelect; } }

        void DoDateFieldSelect(object parameter)
        {
            DateFields p = (DateFields)Enum.Parse(typeof(DateFields), (string)parameter);
            DateFields = (DateFields ^ p);
        }

        RelayCommand _doDateFieldSelect;

        #endregion

        #region Names

        public int NamesCount { get; set; } = 100;
        public bool Name { get; set; } = true;
        public bool SurName { get; set; } = true;
        public bool Gender { get; set; } = false;
        public bool FullName { get; set; } = false;
        public bool UID { get; set; } = false;

        #endregion

        #region ids

        public int fromId { get; set; } = 0;
        public int toId { get; set; } = 100;


        #endregion

        public AddDimensionViewModel()
        {
            _doDateFieldSelect = new RelayCommand(DoDateFieldSelect);
        }
       
    }
}
