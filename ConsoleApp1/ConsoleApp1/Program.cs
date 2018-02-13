using System;
using System.Collections.Generic;
using CourseChecker.Course;
using CourseChecker.WPF;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CourseChecker
{

    class Program
    {
        private readonly static List<String> listManualCheck = new List<string> {
                "TD-TS6S",
                "KM423G"
        };

        internal readonly static List<String> listExcludeForTechData = new List<string> {
                "OE98G",
                "OL4AG",
                "F258G",
                "F211G",
                "AS24G",
                "AS27G"
        };

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.Run(new MainWindow());


            //IDS ids = new IDS();
            //List<Kurse> idsAll = ids.GetCourse;
            //List<Kurse> idsIntegrata = ids.GetCourseIntegrata;
            //List<Kurse> idsTechData = ids.GetCourseTechData;
            //List<Kurse> integrata = (new Integrata()).GetCourse;
            //List<Kurse> techdata = (new Techdata(listExcludeForTechData)).GetCourse;
            ////Console.Clear();

            //RemoveMatches(integrata, idsIntegrata);
            //RemoveMatches(techdata, idsTechData);

            //PrintResult(integrata, "Integrata", "IDS");
            //PrintResult(idsIntegrata, "IDS", "Integrata");
            //PrintResult(techdata, "TechData", "IDS");
            //PrintResult(idsTechData, "IDS", "TechData");

            
        }


        //private static void RemoveMatches(List<Kurse> course, List<Kurse> idsCourse)
        //{
        //    List<Kurse> deleteCourse = new List<Kurse>();
        //    List<Kurse> deleteIDSCourse = new List<Kurse>();

        //    foreach (Kurse kurs in course) {
        //        foreach (Kurse kursIDS in idsCourse) {
        //            if (kurs.Contains(kursIDS)) {
        //                deleteCourse.Add(kurs);
        //                deleteIDSCourse.Add(kursIDS);
        //            } else if (kurs.ContainsForIDS(kursIDS)) {
        //                deleteIDSCourse.Add(kursIDS);
        //            }
        //        }
        //    }

        //    foreach (Kurse m in deleteCourse) {
        //        course.Remove(m);
        //    }

        //    foreach (Kurse m in deleteIDSCourse) {
        //        idsCourse.Remove(m);
        //    }
        //}

        //private static void PrintResult(List<Kurse> course, String strCourseName, String strTowardsCourseName)
        //{
        //    List<Kurse> listBuffer = new List<Kurse>();

        //    if (course.Count > 0) {
        //        int i = 1;
        //        int jBuffer = 1;
        //        Console.WriteLine("\nFolgende \"{0}\" Kurse sind nicht in \"{1}\" enthalten: \n", strCourseName, strTowardsCourseName);
        //        Console.WriteLine("{6,-3} |{0,-9} | {1,-82} | {2,-10} | {3,-10} | {4,-15} | {5,-9} | {7,-8} | {8}", "Kurs-Nr.", "Kurs Titel", "Beginn", "Ende", "Ort", "Preis", "", "Garantie", "Grund");
        //        Console.Write("-----------------------------------------------------------------------------------------------------------");
        //        Console.WriteLine("------------------------------------------------------------------------------");

        //        foreach (Kurse kurs in course) {
        //            if (!listManualCheck.Contains(kurs.StrKursNr)) {
        //                Console.WriteLine("{6,-3} |{0,-9} | {1,-82} | {2,-10} | {3,-10} | {4,-15} | {5,5} EUR | {7,-8} | {8}", kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn.Date.ToString("d"), kurs.DateEnde.Date.ToString("d"), kurs.StrOrt, kurs.IPreis, i, kurs.BoolGarantieTermin, kurs.StrReason);
        //                i++;
        //            } else {
        //                listBuffer.Add(kurs);
        //            }
        //        }

        //        if (listBuffer.Count > 0) {
        //            Console.Write("\n------------------------------------------------------------------------------- manuell Überprüfen -------");
        //            Console.WriteLine("------------------------------------------------------------------------------");
        //            foreach (Kurse kurs in listBuffer) {
        //                Console.WriteLine("{6,-3} |{0,-9} | {1,-82} | {2,-10} | {3,-10} | {4,-15} | {5,5} EUR | {7,-8} | {8}", kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn.Date.ToString("d"), kurs.DateEnde.Date.ToString("d"), kurs.StrOrt, kurs.IPreis, jBuffer, kurs.BoolGarantieTermin, kurs.StrReason);
        //                jBuffer++;
        //            }
        //        }

        //    } else {
        //        Console.WriteLine("\nFolgende \"{0}\" Kurse sind nicht in \"{1}\" enthalten: \n Keine gefunden!\n", strCourseName, strTowardsCourseName);
        //    }
        //    Console.WriteLine("\n\n");
        //}
    }
}

