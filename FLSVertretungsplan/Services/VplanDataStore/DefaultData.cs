using System;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public static class DefaultData
    {
        public static Collection<string> Schools = new Collection<string> {
            "BFS",
            "BG",
            "BS",
            "HBFS",
            "InteA"
        };

        public static Collection<SchoolClass> SchoolClasses = new Collection<SchoolClass>
        {
            new SchoolClass("BFS", "1082"),
            new SchoolClass("BFS", "1181"),
            new SchoolClass("BG", "11 REFILAS"),
            new SchoolClass("BG", "11/1 W"),
            new SchoolClass("BG", "11/2 W"),
            new SchoolClass("BG", "11/3 W"),
            new SchoolClass("BG", "11/5 W/Bili"),
            new SchoolClass("BG", "11/6 DV"),
            new SchoolClass("BG", "11/7 DV"),
            new SchoolClass("BG", "11/8 CT"),
            new SchoolClass("BG", "12"),
            new SchoolClass("BG", "13"),
            new SchoolClass("BS", "1021"),
            new SchoolClass("BS", "1031"),
            new SchoolClass("BS", "1035"),
            new SchoolClass("BS", "1037"),
            new SchoolClass("BS", "1038"),
            new SchoolClass("BS", "1051"),
            new SchoolClass("BS", "1061"),
            new SchoolClass("BS", "1071"),
            new SchoolClass("BS", "1137"),
            new SchoolClass("BS", "1141"),
            new SchoolClass("BS", "1151"),
            new SchoolClass("BS", "1162"),
            new SchoolClass("BS", "1163"),
            new SchoolClass("BS", "1171"),
            new SchoolClass("BS", "1222"),
            new SchoolClass("BS", "1225"),
            new SchoolClass("BS", "1245"),
            new SchoolClass("BS", "1251"),
            new SchoolClass("BS", "1271"),
            new SchoolClass("BS", "InteA 3"),
            new SchoolClass("HBFS", "1191"),
            new SchoolClass("HBFS", "1193"),
            new SchoolClass("HBFS", "1291"),
            new SchoolClass("InteA", "InteA 1"),
            new SchoolClass("InteA", "InteA 2"),
            new SchoolClass("InteA", "InteA 3")
        };
    }
}
