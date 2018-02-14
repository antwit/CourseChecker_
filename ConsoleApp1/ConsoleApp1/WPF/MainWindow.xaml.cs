using CourseChecker.Course;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using CourseChecker.PDF;

namespace CourseChecker.WPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Kurse> idsIntegrata;
        private List<Kurse> idsTechData;
        private List<Kurse> techdata;
        private List<Kurse> integrata;
        private List<Kurse> idsAll;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnStart(object sender, RoutedEventArgs e)
        {
            await GetItemsAsync();

            lstViewIntegrata.ItemsSource = integrata;
            lstViewIDSIntegrata.ItemsSource = idsIntegrata;
            lstViewTechData.ItemsSource = techdata;
            lstViewIDSTechData.ItemsSource = idsTechData;
        }

        private void BtnPDF(object sender, RoutedEventArgs e)
        {
            new CreatePDF();
        }

        private Task GetItemsAsync()
        {
            Task task = Task.Run(() =>
            {
                Task<IDS> taskIDS = Task<IDS>.Factory.StartNew(() => new IDS());
                //Task<Integrata> taskIntegrata = Task<Integrata>.Factory.StartNew(() => new Integrata());
                //Task<Techdata> taskTechData = Task<Techdata>.Factory.StartNew(d => new Techdata((List<string>)d), Program.listExcludeForTechData);
                //Task.WaitAll(new Task[] { taskIDS, taskIntegrata, taskTechData });
                
                idsAll = taskIDS.Result.GetCourse;
                idsIntegrata = taskIDS.Result.GetCourseIntegrata;
                idsTechData = taskIDS.Result.GetCourseTechData;
                //techdata = taskTechData.Result.GetCourse;
                //integrata = taskIntegrata.Result.GetCourse;

                //CourseProvider.RemoveMatches(integrata, idsIntegrata);
                //CourseProvider.RemoveMatches(techdata, idsTechData);

            });
            return task;
        }
    }
}
