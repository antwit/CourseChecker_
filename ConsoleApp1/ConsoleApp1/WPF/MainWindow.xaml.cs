using CourseChecker.Course;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using CourseChecker.PDF;
using System;
using System.Collections;
using Microsoft.Win32;

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
        private List<Kurse> listSelected;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnStart(object sender, RoutedEventArgs e)
        {
            Progressbar pb =  new Progressbar();
            pb.Width = 200;
            pb.Height = 60;
            pb.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pb.Show();
            await GetItemsAsync();
            pb.Close();

            lstViewIntegrata.ItemsSource = integrata;
            lstViewIDSIntegrata.ItemsSource = idsIntegrata;
            lstViewTechData.ItemsSource = techdata;
            lstViewIDSTechData.ItemsSource = idsTechData;
        }

        private void BtnPDF(object sender, RoutedEventArgs e)
        {
            IList tmp = lstViewIDSIntegrata.SelectedItems;
            IList tmp2 = lstViewIntegrata.SelectedItems;
            IList tmp3 = lstViewIDSTechData.SelectedItems;
            IList tmp4 = lstViewTechData.SelectedItems;

            listSelected = new List<Kurse>();
            //Scheiß C#
            AddListSelected(tmp);
            AddListSelected(tmp2);
            AddListSelected(tmp3);
            AddListSelected(tmp4);

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.DefaultExt = ".pdf";
            if(dlg.ShowDialog() == true)
            {
                new CreatePDF(listSelected, dlg.FileName);
            }
        }

        private void AddListSelected(IList tmp)
        {
            foreach (object tmpo in tmp)
            {
                this.listSelected.Add((Kurse)tmpo);
            }
        }

        private Task GetItemsAsync()
        {
            Task task = Task.Run(() =>
            {
                Task<IDS> taskIDS = Task<IDS>.Factory.StartNew(() => new IDS());
                Task<Integrata> taskIntegrata = Task<Integrata>.Factory.StartNew(() => new Integrata());
                Task<Techdata> taskTechData = Task<Techdata>.Factory.StartNew(d => new Techdata((List<string>)d), Program.listExcludeForTechData);
                Task.WaitAll(new Task[] { taskIDS, taskIntegrata, taskTechData });

                idsAll = taskIDS.Result.GetCourse;
                idsIntegrata = taskIDS.Result.GetCourseIntegrata;
                idsTechData = taskIDS.Result.GetCourseTechData;
                techdata = taskTechData.Result.GetCourse;
                integrata = taskIntegrata.Result.GetCourse;

                CourseProvider.RemoveMatches(integrata, idsIntegrata);
                CourseProvider.RemoveMatches(techdata, idsTechData);
            });
            return task;
        }
    }
}
