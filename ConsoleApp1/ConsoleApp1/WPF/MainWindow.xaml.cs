using CourseChecker.Course;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using CourseChecker;

namespace CourseChecker.WPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RemoveMatches(List<Kurse> course, List<Kurse> idsCourse)
        {
            List<Kurse> deleteCourse = new List<Kurse>();
            List<Kurse> deleteIDSCourse = new List<Kurse>();

            foreach (Kurse kurs in course)
            {
                foreach (Kurse kursIDS in idsCourse)
                {
                    if (kurs.Contains(kursIDS))
                    {
                        deleteCourse.Add(kurs);
                        deleteIDSCourse.Add(kursIDS);
                    }
                    else if (kurs.ContainsForIDS(kursIDS))
                    {
                        deleteIDSCourse.Add(kursIDS);
                    }
                }
            }

            foreach (Kurse m in deleteCourse)
            {
                course.Remove(m);
            }

            foreach (Kurse m in deleteIDSCourse)
            {
                idsCourse.Remove(m);
            }
        }

        private void BtnStart(object sender, RoutedEventArgs e)
        {
            IDS ids = new IDS();
            List<Kurse> idsAll = ids.GetCourse;
            List<Kurse> idsIntegrata = ids.GetCourseIntegrata;
            List<Kurse> idsTechData = ids.GetCourseTechData;
            
            List<Kurse> integrata = (new Integrata()).GetCourse;
            List<Kurse> techdata = (new Techdata(Program.listExcludeForTechData)).GetCourse;

            RemoveMatches(integrata, idsIntegrata);
            RemoveMatches(techdata, idsTechData);

            lstViewIntegrata.ItemsSource = integrata;
            lstViewIDSIntegrata.ItemsSource = idsIntegrata;
            lstViewTechData.ItemsSource = techdata;
            lstViewIDSTechData.ItemsSource = idsTechData;
        }

        private void BtnPDF(object sender, RoutedEventArgs e)
        {

        }
    }
}
