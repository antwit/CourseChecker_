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
            Task<IDS> taskIDS = Task<IDS>.Factory.StartNew(() => new IDS());
            Task<Integrata> taskIntegrata = Task<Integrata>.Factory.StartNew(() => new Integrata());
            Task<Techdata> taskTechData = Task<Techdata>.Factory.StartNew(d =>  new Techdata((List<string>) d) , Program.listExcludeForTechData);

            List<Kurse> idsAll = taskIDS.Result.GetCourse;
            List<Kurse> idsIntegrata = taskIDS.Result.GetCourseIntegrata;
            List<Kurse> idsTechData = taskIDS.Result.GetCourseTechData;
            List<Kurse> techdata = taskTechData.Result.GetCourse;
            List<Kurse> integrata = taskIntegrata.Result.GetCourse;
            

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
