using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public static class Color
    {
        public static RgbColor PRIMARY_TEXT_COLOR = new RgbColor(23, 43, 76);
        public static RgbColor SECONDARY_TEXT_COLOR = new RgbColor(164, 174, 186);
        public static RgbColor SELECTED_ICON_COLOR = PRIMARY_TEXT_COLOR;
        public static RgbColor ICON_COLOR = SECONDARY_TEXT_COLOR;
        public static RgbColor BORDER_COLOR = new RgbColor(227, 228, 232);
        public static RgbColor TAB_BORDER_COLOR = new RgbColor(236, 237, 241);
        public static RgbColor UNKNOWN_SCHOOL_COLOR = new RgbColor(201, 29, 29);

        static readonly Dictionary<string, RgbColor> OutlineColors = new Dictionary<string, RgbColor>
        {
            { "BG", new RgbColor(254, 57, 94) },
            { "BS", new RgbColor(30, 154, 249) },
            { "BFS", new RgbColor(28, 200, 142) },
            { "HBFS", new RgbColor(113, 63, 192) }
        };
        static readonly Dictionary<string, Gradient> FillColors = new Dictionary<string, Gradient>
        {
            { "BG", new Gradient(new RgbColor(254, 150, 116), OutlineColors["BG"]) },
            { "BS", new Gradient(OutlineColors["BS"], new RgbColor(41, 182, 199)) },
            { "BFS", new Gradient(new RgbColor(27, 201, 182), OutlineColors["BFS"]) },
            { "HBFS", new Gradient(OutlineColors["HBFS"], new RgbColor(135, 70, 231)) }
        };

        public static RgbColor GetOutlineColor(string school)
        {
            if (!OutlineColors.ContainsKey(school))
            {
                return UNKNOWN_SCHOOL_COLOR;
            }
            return OutlineColors[school];
        }

        public static Gradient GetFillColor(string school)
        {
            if (!FillColors.ContainsKey(school))
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