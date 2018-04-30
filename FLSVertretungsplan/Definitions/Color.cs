using System;
using System.Collections.Generic;
using System.Globalization;

namespace FLSVertretungsplan
{
    public static class Color
    {
        public static RgbColor PRIMARY_TEXT_COLOR = new RgbColor("172B4C");
        public static RgbColor SECONDARY_TEXT_COLOR = new RgbColor("A4AEBA");
        public static RgbColor SELECTED_ICON_COLOR = PRIMARY_TEXT_COLOR;
        public static RgbColor ICON_COLOR = SECONDARY_TEXT_COLOR;
        public static RgbColor BORDER_COLOR = new RgbColor("E3E4E8");
        public static RgbColor TAB_BORDER_COLOR = new RgbColor("ECEDF1");
        public static RgbColor UNKNOWN_SCHOOL_COLOR = new RgbColor("C91D1D");

        static readonly Dictionary<string, RgbColor> OutlineColors = new Dictionary<string, RgbColor>
        {
            { "BG", new RgbColor("8ADE5F") },
            { "BS", new RgbColor("F9D423") },
            { "BFS", new RgbColor("FE9674") },
            { "HBFS", new RgbColor("29B6C7") },
            { "InteA", new RgbColor("1CECD6") }
        };
        static readonly Dictionary<string, Gradient> FillColors = new Dictionary<string, Gradient>
        {
            { "BG", new Gradient(new RgbColor("DDF855"), new RgbColor("8ADE5F")) },
            { "BS", new Gradient(new RgbColor("F9D423"), new RgbColor("F88200")) },
            { "BFS", new Gradient(new RgbColor("FE9674"), new RgbColor("FE395E")) },
            { "HBFS", new Gradient(new RgbColor("45E3F6"), new RgbColor("1E8BF9")) },
            { "InteA", new Gradient(new RgbColor("1CECD6"), new RgbColor("09D778")) }
        };

        public static RgbColor GetOutlineColor(string school)
        {
            if (school == null || !OutlineColors.ContainsKey(school))
            {
                return UNKNOWN_SCHOOL_COLOR;
            }
            return OutlineColors[school];
        }

        public static Gradient GetFillColor(string school)
        {
            if (school == null || !FillColors.ContainsKey(school))
            {
                return new Gradient(UNKNOWN_SCHOOL_COLOR, UNKNOWN_SCHOOL_COLOR);
            }
            return FillColors[school];
        }
    }

    public class RgbColor
    {
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        public RgbColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public RgbColor(string hex)
        {
            Red = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            Green = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            Blue = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        }
    }

    public class Gradient
    {
        public readonly RgbColor Start;
        public readonly RgbColor End;

        public Gradient(RgbColor start, RgbColor end)
        {
            Start = start;
            End = end;
        }
    }
}