using CourseChecker.Course;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using CourseChecker.PDF;
using System;
using System.Collections;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

namespace CourseChecker.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        Progressbar proBar;
        private List<Kurse> listSelected;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Task<IDS> taskIDS = Task<IDS>.Factory.StartNew(() => new IDS());
            Task<Integrata> taskIntegrata = Task<Integrata>.Factory.StartNew(() => new Integrata());
            Task<Techdata> taskTechData = Task<Techdata>.Factory.StartNew(d => new Techdata((List<string>)d), Program.listExcludeForTechData);
            Task.WaitAll(new Task[] { taskIDS, taskIntegrata, taskTechData });
            Task.WaitAll(new Task[] { taskIDS, taskIntegrata });

            List<Kurse> idsAll = taskIDS.Result.GetCourse;
            List<Kurse> idsIntegrata = taskIDS.Result.GetCourseIntegrata;
            List<Kurse> idsTechData = taskIDS.Result.GetCourseTechData;
            List<Kurse> techdata = taskTechData.Result.GetCourse;
            List<Kurse> integrata = taskIntegrata.Result.GetCourse;

            CourseProvider.RemoveMatches(integrata, idsIntegrata);
            CourseProvider.RemoveMatches(techdata, idsTechData);

            lstViewIntegrata.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                        (ThreadStart)delegate ()
                                                        {
                                                            lstViewIntegrata.ItemsSource = integrata;
                                                            lstViewIDSIntegrata.ItemsSource = idsIntegrata;
                                                            lstViewTechData.ItemsSource = techdata;
                                                            lstViewIDSTechData.ItemsSource = idsTechData;
                                                        });
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            proBar.prgBar.Value = e.ProgressPercentage;
        }

        private void Bw_RunworkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            proBar.Close();
        }

        private void BtnStart(object sender, RoutedEventArgs e)
        {
            proBar = new Progressbar();
            Program.boolIDS = Program.boolIntegrata = Program.boolTechData = false;
            Program.bw.WorkerReportsProgress = true;
            Program.bw.DoWork += Bw_DoWork;
            Program.bw.ProgressChanged += Bw_ProgressChanged;
            Program.bw.RunWorkerCompleted += Bw_RunworkerCompleted;
            Program.bw.RunWorkerAsync();

            proBar.Show();
        }

        private void BtnPDF(object sender, RoutedEventArgs e)
        {
            IList tmp = lstViewIDSIntegrata.SelectedItems;
            IList tmp2 = lstViewIntegrata.SelectedItems;
            IList tmp3 = lstViewIDSTechData.SelectedItems;
            IList tmp4 = lstViewTechData.SelectedItems;

            listSelected = new List<Kurse>();
            //Scheiß C#
            AddToListSelected(tmp);
            AddToListSelected(tmp2);
            AddToListSelected(tmp3);
            AddToListSelected(tmp4);

            SaveFileDialog dlg = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                new CreatePDF(listSelected, dlg.FileName);
            }
        }

        private void AddToListSelected(IList tmp)
        {
            foreach (object tmpo in tmp)
            {
                this.listSelected.Add((Kurse)tmpo);
            }
        }
    }
}
