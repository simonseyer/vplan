﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class MockVplanDataStore: IVplanDataStore
    {
        string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><vplan xmlns=\"http://www.fls-wiesbaden.de/schema/vplan\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" lastupdate=\"1512064926\" xsi:schemalocation=\"http://www.fls-wiesbaden.de/schema/vplan.xsd\"> <school name=\"BG\"><class name=\"11/2 W\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Marcel</firstname> <lastname>Metz</lastname> <shortcut>METZ</shortcut></tutor><subject> <name>Powi</name> <shortcut>POWI</shortcut></subject><room>C02</room> </original> <substitute><type>1026</type><room>203SB GH</room><attribute /><info /> </substitute></lesson> </date></class><class name=\"11/3 W\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Jörg</firstname> <lastname>Sundermann</lastname> <shortcut>SUND</shortcut></tutor><subject> <name>Wirtschaft</name> <shortcut>WIRT</shortcut></subject><room>202</room> </original> <substitute><type>1028</type><tutor> <firstname>Kathrin</firstname> <lastname>Lappe</lastname> <shortcut>LAPP</shortcut></tutor><room /><attribute /><info /> </substitute></lesson> </date></class><class name=\"11/4 W\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>8.-9.</hours><tutor> <firstname>Lea</firstname> <lastname>Kissinger</lastname> <shortcut>KISS</shortcut></tutor><subject> <name>Mathematik</name> <shortcut>MATH</shortcut></subject><room>A208 SB</room> </original> <substitute><type>1028</type><tutor> <firstname>Sarah</firstname> <lastname>Kupfer</lastname> <shortcut>KUPF</shortcut></tutor><room /><attribute /><info /> </substitute></lesson> </date></class><class name=\"12\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Doris</firstname> <lastname>Müller</lastname> <shortcut>MUEL</shortcut></tutor><subject> <name>Religion katholisch</name> <shortcut>KREL</shortcut></subject><room>A212</room> </original> <substitute><type>1028</type><tutor> <firstname>Petra</firstname> <lastname>Schmidt 2</lastname> <shortcut>SCHP</shortcut></tutor><room /><attribute /><info /> </substitute></lesson> </date> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>6.</hours><tutor> <firstname>Florian</firstname> <lastname>Becker</lastname> <shortcut>BECK</shortcut></tutor><subject> <name>Englisch GK</name> <shortcut>ENGK</shortcut></subject><room>A306SB</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class> </school> <school name=\"BS\"><class name=\"InteA 1\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>5.-6.</hours><tutor> <firstname>Lea</firstname> <lastname>Kissinger</lastname> <shortcut>KISS</shortcut></tutor><subject> <name>Mathematik</name> <shortcut>MATH</shortcut></subject><room>C02</room> </original> <substitute><type>1028</type><tutor> <firstname>Martina</firstname> <lastname>Krämer</lastname> <shortcut>KRÄM</shortcut></tutor><room /><attribute /><info /> </substitute></lesson> </date> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>1.-2.</hours><tutor> <firstname>Martina</firstname> <lastname>Herz</lastname> <shortcut>HERZ</shortcut></tutor><subject> <shortcut>DaZ</shortcut></subject><room>C02</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Karin</firstname> <lastname>Ködderitzsch</lastname> <shortcut>KÖDD</shortcut></tutor><subject> <name>Englisch</name> <shortcut>ENGL</shortcut></subject><room>203SB GH</room> </original> <substitute><type>1028</type><tutor> <firstname>Martina</firstname> <lastname>Herz</lastname> <shortcut>HERZ</shortcut></tutor><room /><attribute /><info /> </substitute></lesson><lesson> <original><hours>5.-6.</hours><tutor> <firstname>Martina</firstname> <lastname>Herz</lastname> <shortcut>HERZ</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>C02</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class><class name=\"InteA 3\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Martina</firstname> <lastname>Herz</lastname> <shortcut>HERZ</shortcut></tutor><subject> <shortcut>DaZ</shortcut></subject><room>C04</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class><class name=\"InteA 2\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>1.-2.</hours><tutor> <firstname>Marina</firstname> <lastname>Dobroschke</lastname> <shortcut>DOBR</shortcut></tutor><subject> <shortcut>DaZ</shortcut></subject><room>C03</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson><lesson> <original><hours>3.-6.</hours><tutor> <firstname>Anne</firstname> <lastname>Klar</lastname> <shortcut>KLAR</shortcut></tutor><subject> <shortcut>DaZ</shortcut></subject><room>C03</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class><class name=\"1171\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>2.</hours><tutor> <firstname>Florian</firstname> <lastname>Becker</lastname> <shortcut>BECK</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>A310</room> </original> <substitute><type>1028</type><tutor> <firstname>Martina</firstname> <lastname>Ecker-Link</lastname> <shortcut>ECKL</shortcut></tutor><room /><attribute /><info /> </substitute></lesson><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Florian</firstname> <lastname>Becker</lastname> <shortcut>BECK</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>A310</room> </original> <substitute><type>1028</type><tutor> <firstname>Marianne</firstname> <lastname>Löffler</lastname> <shortcut>LÖFF</shortcut></tutor><room /><attribute /><info /> </substitute></lesson><lesson> <original><hours>5.</hours><tutor> <firstname>Marianne</firstname> <lastname>Löffler</lastname> <shortcut>LÖFF</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>A310</room> </original> <substitute><type>1036</type><tutor> <firstname>Katja</firstname> <lastname>Hell-Berlin</lastname> <shortcut>HELL</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room /><attribute>anstatt Di, 05.12. 8. Std.</attribute><info /> </substitute></lesson><lesson> <original><hours>6.</hours><tutor> <firstname>Marianne</firstname> <lastname>Löffler</lastname> <shortcut>LÖFF</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>A310</room> </original> <substitute><type>1024</type><room /><attribute>Selbstorganisiertes Lernen</attribute><info>Klasse frei</info> </substitute></lesson><lesson> <original><hours>8.</hours><tutor> <firstname>Katja</firstname> <lastname>Hell-Berlin</lastname> <shortcut>HELL</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>A310</room> </original> <substitute><type>1024</type><room /><attribute>verschoben auf Di, 05.12. 5. Std.</attribute><info /> </substitute></lesson> </date></class><class name=\"1223\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>3.-4.</hours><tutor> <firstname>Norbert</firstname> <lastname>Pieper</lastname> <shortcut>PIEP</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>406</room> </original> <substitute><type>1030</type><tutor> <firstname>Annette</firstname> <lastname>Pulch</lastname> <shortcut>PULC</shortcut></tutor><room>C13</room><attribute /><info>Arbeitsauftrag Dr. Pieper</info> </substitute></lesson><lesson> <original><hours>5.-6.</hours><tutor> <firstname>Norbert</firstname> <lastname>Pieper</lastname> <shortcut>PIEP</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>406</room> </original> <substitute><type>1024</type><room /><attribute>In die Betriebe</attribute><info>Klasse frei</info> </substitute></lesson> </date></class><class name=\"1245\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>1.-2.</hours><tutor> <firstname>Norbert</firstname> <lastname>Pieper</lastname> <shortcut>PIEP</shortcut></tutor><subject> <shortcut>BBUN</shortcut></subject><room>406</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class><class name=\"1271\"> <date value=\"Dienstag, 05.12.2017\" timestamp=\"1512428400\"><lesson> <original><hours>5.</hours><tutor> <firstname>Florian</firstname> <lastname>Becker</lastname> <shortcut>BECK</shortcut></tutor><subject> <name>Englisch</name> <shortcut>ENGL</shortcut></subject><room>C13</room> </original> <substitute><type>1036</type><tutor> <firstname>Swantje</firstname> <lastname>Feuerhake</lastname> <shortcut>FEUE</shortcut></tutor><subject> <name>Englisch</name> <shortcut>ENGL</shortcut></subject><room /><attribute>anstatt Di, 05.12. 8. Std.</attribute><info /> </substitute></lesson><lesson> <original><hours>8.</hours><tutor> <firstname>Swantje</firstname> <lastname>Feuerhake</lastname> <shortcut>FEUE</shortcut></tutor><subject> <name>Englisch</name> <shortcut>ENGL</shortcut></subject><room>A110SB</room> </original> <substitute><type>1024</type><room /><attribute>verschoben auf Di, 05.12. 5. Std.</attribute><info /> </substitute></lesson> </date></class> </school> <school name=\"HBFS\"><class name=\"1191\"> <date value=\"Montag, 04.12.2017\" timestamp=\"1512342000\"><lesson> <original><hours>7.-8.</hours><tutor> <firstname>Norbert</firstname> <lastname>Pieper</lastname> <shortcut>PIEP</shortcut></tutor><subject> <name>Powi</name> <shortcut>POWI</shortcut></subject><room>406</room> </original> <substitute><type>1024</type><room /><attribute /><info>Klasse frei</info> </substitute></lesson> </date></class> </school></vplan>";
        Property<Vplan> vplan;
        Property<Vplan> bookmarkedVplan;
        ObservableCollection<SchoolBookmark> bookmarkedSchools;
        ObservableCollection<SchoolClassBookmark> bookmarkedClasses;


        public MockVplanDataStore()
        {
            vplan = new Property<Vplan>
            {
                Value = VplanParser.Parse(xml)
            };
            bookmarkedVplan = new Property<Vplan>
            {
                Value = new Vplan
                {
                    LastUpdate = DateTime.Now,
                    Changes = new List<Change>()
                }
            };
            bookmarkedSchools = new ObservableCollection<SchoolBookmark>();
            bookmarkedClasses = new ObservableCollection<SchoolClassBookmark>();
        }

        public void BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark)
        {
            
        }

        public void BookmarkSchool(string schoolName, bool bookmark)
        {
            
        }

        Property<Vplan> GetBookmarkdVplan()
        {
            return bookmarkedVplan;
        }

        public Property<Vplan> GetBookmarkedVplan()
        {
            return bookmarkedVplan;
        }

        public ObservableCollection<SchoolBookmark> GetSchoolBookmarks()
        {
            return bookmarkedSchools;
        }

        public ObservableCollection<SchoolClassBookmark> GetSchoolClassBookmarks()
        {
            return bookmarkedClasses;
        }

        public async Task Refresh()
        {
            
        }

        public Property<bool> GetIsRefreshing()
        {
            return new Property<bool>();
        }

        public Property<Vplan> GetVplan()
        {
            return vplan;
        }
    }
}
