using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CourseChecker.Course;

namespace CourseChecker
{

    class Program
    {
        static void Main(string[] args)
        {
            List<String> listExcludeForTechData = new List<string> {
                "OE98G",
                "OL4AG",
                "F258G",
                "F211G",
                "AS24G",
                "AS27G"
            };

            IDS ids = new IDS();
            List<Kurse> idsAll = ids.GetCourse;
            List<Kurse> idsIntegrata = ids.GetCourseIntegrata;
            List<Kurse> idsTechData = ids.GetCourseTechData;
            List<Kurse> integrata = (new Integrata()).GetCourse;
            List<Kurse> techdata = (new Techdata(listExcludeForTechData)).GetCourse;
            Console.Clear();

            RemoveMatches(integrata, idsIntegrata);
            RemoveMatches(techdata, idsTechData);

            PrintResult(integrata, "Integrata", "IDS");
            PrintResult(idsIntegrata, "IDS", "Integrata");
            PrintResult(techdata, "TechData", "IDS");
            PrintResult(idsTechData, "IDS", "TechData");

            Console.ReadKey();
        }

        private static void RemoveMatches(List<Kurse> course, List<Kurse> idsCourse)
        {
            List<Kurse> deleteCourse = new List<Kurse>();
            List<Kurse> deleteIDSCourse = new List<Kurse>();

            foreach (Kurse kurs in course) {
                foreach (Kurse kursIDS in idsCourse) {
                    if (kurs.Contains(kursIDS)) {
                        deleteCourse.Add(kurs);
                        deleteIDSCourse.Add(kursIDS);
                    }
                    //else if (kurs.ContainsForIDS(kursIDS)) {
                    //    deleteIDSCourse.Add(kursIDS);
                    //}
                }
            }

            foreach (Kurse m in deleteCourse) {
                course.Remove(m);
            }

            foreach (Kurse m in deleteIDSCourse) {
                idsCourse.Remove(m);
            }
        }

        private static void PrintResult(List<Kurse> course, String strCourseName, String strTowardsCourseName)
        {
            if (course.Count > 0) {
                int i = 1;
                Console.WriteLine("\nFolgende \"{0}\" Kurse sind nicht in \"{1}\" enthalten: \n", strCourseName, strTowardsCourseName);
                foreach (Kurse kurs in course) {
                    Console.WriteLine("{6,-3} |{0,-7} | {1,-82} | {2,-20} | {3,-20} | {4,-15} | {5,-8} | {7}", kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn, kurs.DateEnde, kurs.StrOrt, kurs.IPreis, i, kurs.StrReason);
                    i++;
                }
            } else {
                Console.WriteLine("\nFolgende \"{0}\" Kurse sind nicht in \"{1}\" enthalten: \n Keine gefunden!\n", strCourseName, strTowardsCourseName);
            }
        }
    }



    
    

    

    

   
}

