﻿using CourseChecker.Course;
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
using CourseChecker.Logging;

namespace CourseChecker.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Progressbar proBar;
        ChangeBook changeBook;
        private List<Kurse> listSelected;
        private List<Kurse> listSelectedChange;

        public MainWindow() {
            InitializeComponent();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e) {
            Task<IDS> taskIDS = Task<IDS>.Factory.StartNew(() => new IDS());
            Task<Integrata> taskIntegrata = Task<Integrata>.Factory.StartNew(() => new Integrata());
            Task<Techdata> taskTechData = Task<Techdata>.Factory.StartNew(d => new Techdata((List<string>)d), Program.listExcludeForTechData);
            Task.WaitAll(new Task[] { taskIDS, taskIntegrata, taskTechData });

            List<Kurse> idsAll = taskIDS.Result.GetCourse;
            List<Kurse> idsIntegrata = taskIDS.Result.GetCourseIntegrata;
            List<Kurse> idsTechData = taskIDS.Result.GetCourseTechData;
            List<Kurse> techdata = taskTechData.Result.GetCourse;
            List<Kurse> integrata = taskIntegrata.Result.GetCourse;

            CourseProvider.RemoveMatches(integrata, idsIntegrata);
            CourseProvider.RemoveMatches(techdata, idsTechData);

            lstViewIntegrata.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                        (ThreadStart)delegate () {
                                                            lstViewIntegrata.ItemsSource = integrata;
                                                            lstViewIDSIntegrata.ItemsSource = idsIntegrata;
                                                            lstViewTechData.ItemsSource = techdata;
                                                            lstViewIDSTechData.ItemsSource = idsTechData;

                                                            if (integrata.Count < 1)
                                                                lstViewIntegrata.Visibility = Visibility.Collapsed;
                                                            if (idsIntegrata.Count < 1)
                                                                lstViewIDSIntegrata.Visibility = Visibility.Collapsed;
                                                            if (techdata.Count < 1)
                                                                lstViewTechData.Visibility = Visibility.Collapsed;
                                                            if (idsTechData.Count < 1)
                                                                lstViewIDSTechData.Visibility = Visibility.Collapsed;
                                                        });
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            proBar.prgBar.Value = e.ProgressPercentage;
        }

        private void Bw_RunworkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            proBar.Close();
            this.btnStart.IsEnabled = true;
            this.btnPDF.IsEnabled = true;
            this.btnBuchungen.IsEnabled = true;

            logger.Info("Suche fertiggestellt!");
        }

        private void BtnStart(object sender, RoutedEventArgs e) {
            logger.Info("Suche gestartet...");
            proBar = new Progressbar();
            Program.boolIDS = Program.boolIntegrata = Program.boolTechData = false;
            Program.iCounter = 0;
            Program.iNumberOfCourses = 0;

            lstViewLogs.ItemsSource = null;
            lstViewIntegrata.ItemsSource = null;
            lstViewIDSIntegrata.ItemsSource = null;
            lstViewTechData.ItemsSource = null;
            lstViewIDSTechData.ItemsSource = null;

            if (!Program.bw.WorkerReportsProgress) {
                Program.bw.WorkerReportsProgress = true;
                Program.bw.DoWork += Bw_DoWork;
                Program.bw.ProgressChanged += Bw_ProgressChanged;
                Program.bw.RunWorkerCompleted += Bw_RunworkerCompleted;
            }

            if (!Program.bw.IsBusy) {
                Program.bw.RunWorkerAsync();
                this.btnStart.IsEnabled = false;
                this.btnPDF.IsEnabled = false;
                this.btnBuchungen.IsEnabled = false;
            }

            lstViewLogs.ItemsSource = Logger.GetLogging;
            proBar.Show();
        }

        private void BtnPDF(object sender, RoutedEventArgs e) {
            IList tmp = lstViewIDSIntegrata.SelectedItems;
            IList tmp2 = lstViewIntegrata.SelectedItems;
            IList tmp3 = lstViewIDSTechData.SelectedItems;
            IList tmp4 = lstViewTechData.SelectedItems;

            listSelected = new List<Kurse>();
            //Scheiß C#
            AddToListSelected(tmp, this.listSelected);
            AddToListSelected(tmp2, this.listSelected);
            AddToListSelected(tmp3, this.listSelected);
            AddToListSelected(tmp4, this.listSelected);

            SaveFileDialog dlg = new SaveFileDialog() {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".pdf"
            };

            if (dlg.ShowDialog() == true) {
                new CreatePDF(listSelected, dlg.FileName);
            }
            logger.Info("PDF erstellt!");
        }

        private void AddToListSelected(IList tmp, List<Kurse> list) {
            foreach (object tmpo in tmp) {
                list.Add((Kurse)tmpo);
            }
        }

        private void BtnBuchungen(object sender, RoutedEventArgs e) {
            changeBook = new ChangeBook();

            IList tmp = lstViewIDSIntegrata.SelectedItems;
            IList tmp2 = lstViewIntegrata.SelectedItems;
            IList tmp3 = lstViewIDSTechData.SelectedItems;
            IList tmp4 = lstViewTechData.SelectedItems;

            listSelectedChange = new List<Kurse>();
            //Scheiß C#
            AddToListSelected(tmp, this.listSelectedChange);
            AddToListSelected(tmp2, this.listSelectedChange);
            AddToListSelected(tmp3, this.listSelectedChange);
            AddToListSelected(tmp4, this.listSelectedChange);

            changeBook.dataGridCourses.DataContext = listSelectedChange;
            changeBook.ShowDialog();
        }
    }
}
