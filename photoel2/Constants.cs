using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace photoel2
{
    static class Constants
    {
        static Constants()
        {
            Filters = new Filter[]
            {
                new Filter("UV", Properties.Resources.singlefilter_uv, 180,  0.0040, Brushes.Purple),
                new Filter("purple", Properties.Resources.singlefilter_purple, 380,  0.0096, Brushes.MediumPurple),
                new Filter("violet", Properties.Resources.singlefilter_violet, 400,  0.01041, Brushes.Violet),
                new Filter("dark blue", Properties.Resources.singlefilter_blue, 450,  0.0108, Brushes.DarkBlue),
                new Filter("blue", Properties.Resources.singlefilter_cyan, 470,  0.01175, Brushes.Blue),
                new Filter("blue green", Properties.Resources.singlefilter_cyangreen, 500,  0.01194, Brushes.Turquoise),
                new Filter("green", Properties.Resources.singlefilter_green, 540,  0.01207, Brushes.Green),
                new Filter("green yellow", Properties.Resources.singlefilter_greenyellow, 570,  0.01201, Brushes.GreenYellow),
                new Filter("yellow", Properties.Resources.singlefilter_yellow, 590, 0.01191, Brushes.Yellow),
                new Filter("yellow orange", Properties.Resources.singlefilter_yelloworange, 600,  0.01184, Brushes.LightGoldenrodYellow),
                new Filter("orange", Properties.Resources.singlefilter_orange, 650,  0.01154, Brushes.Orange),
                new Filter("red", Properties.Resources.singlefilter_red, 700,  0.01094, Brushes.Red),
                new Filter("dark red", Properties.Resources.singlefilter_darkred, 750,  0.01053, Brushes.DarkRed),
            };
            DefaultFilter = Filters[8];

            Metals = new Metal[]
            {
                new Metal("aluminium", "Al", Properties.Resources.sym_aluminium, 2.9),
                new Metal("silver", "Ag", Properties.Resources.sym_silver, 4.73),
                new Metal("calcium", "Ca", Properties.Resources.sym_calcium, 2.15),
                new Metal("cobalt", "Co", Properties.Resources.sym_cobalt, 2.5),
                new Metal("copper", "Cu", Properties.Resources.sym_copper, 4.7),
                new Metal("lithium", "Li", Properties.Resources.sym_lithium, 2),
                new Metal("lead", "Pb", Properties.Resources.sym_lead, 4.14),
                new Metal("platinum", "Pt", Properties.Resources.sym_platinum, 6.35),
                new Metal("zinc", "Zn", Properties.Resources.sym_zinc, 4.31),
            };
            DefaultMetal = Metals[5];
        }

        public class Metal
        {
            public Metal(string n, string s, Image i, double w) { name = n;  image = i; symbol = s; work_function = w; }

            public readonly string name;
            public readonly string symbol;
            public readonly Image image;
            public readonly double work_function;
        }
        public class Filter
        {
            public Filter(String n, Image i, double w, double f, Brush b) { name = n;  image = i; wave_length = w; intensity_factor = f; brush = b;  }

            public readonly string name;
            public readonly Image image;
            public readonly double wave_length;
            public readonly double intensity_factor;
            public readonly Brush brush;
        }

        public static readonly double MinVoltage = -12.0;
        public static readonly double MaxVoltage =  12.0;

        public static readonly double MinDistance = 5.0;
        public static readonly double MaxDistance = 20.0;

        public static readonly double SourcePower = 1.0;    // Power that gets the light source in watts
        public static readonly double Efficiency = 0.005;   //Percentage of power transfered to lighth
        public static readonly double ElectrodeArea = 0.25;  //An electrode area in cm^2
        public static readonly double QFactor = 0.25;       //Constant for curve fitting


        public static readonly Filter[] Filters;
        public static readonly Filter DefaultFilter;
        public static readonly Metal[] Metals;
        public static readonly Metal DefaultMetal;
    }

}