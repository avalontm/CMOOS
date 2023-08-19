using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moos.Framework
{
    public enum GridUnitType
    {
        Auto = 0,
        Pixel = 1,
        Star = 2
    }

    [TypeConverter(typeof(GridLengthConverter))]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public class GridLength
    {
        public GridUnitType GridUnitType { get; private set; }
        public bool IsAbsolute { get; private set; }
        public bool IsAuto { get; private set; }
        public bool IsStar { get; private set; }
        public int Value { get; set; }

        public GridLength(int value, GridUnitType unit)
        {
            this.GridUnitType = unit;
            this.Value = value;
            onSetProperties();
        }

        void onSetProperties()
        {
            switch (GridUnitType)
            {
                case GridUnitType.Auto:
                    IsAuto = true;
                    IsAbsolute = false;
                    IsStar = false;
                    break;
                case GridUnitType.Pixel:
                    IsAuto = false;
                    IsAbsolute = true;
                    IsStar = false;
                    break;
                case GridUnitType.Star:
                    IsAuto = false;
                    IsAbsolute = false;
                    IsStar = true;
                    break;
            }
        }
    }
}
