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
using CourseChecker.Logging;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace CourseChecker.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private static bool IndividualChkBxUnCheckedFlagIntegrata { get; set; }
        private static bool IndividualChkBxUnCheckedFlagIDSIntegrata { get; set; }
        private static bool IndividualChkBxUnCheckedFlagTechData { get; set; }
        private static bool IndividualChkBxUnCheckedFlagIDSTechData { get; set; }
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Progressbar proBar;
        ChangeBook changeBook;
        private List<Kurse> listSelected;
        private List<Kurse> listSelectedChange;

        public MainWindow() {
            InitializeComponent();

            lstViewIntegrata.Visibility = Visibility.Collapsed;
            lstViewIDSIntegrata.Visibility = Visibility.Collapsed;
            lstViewTechData.Visibility = Visibility.Collapsed;
            lstViewIDSTechData.Visibility = Visibility.Collapsed;
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

                                                            lstViewIntegrata.Visibility = Visibility.Visible;
                                                            lstViewIDSIntegrata.Visibility = Visibility.Visible;
                                                            lstViewTechData.Visibility = Visibility.Visible;
                                                            lstViewIDSTechData.Visibility = Visibility.Visible;

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

            lstViewIntegrata.Visibility = Visibility.Collapsed;
            lstViewIDSIntegrata.Visibility = Visibility.Collapsed;
            lstViewTechData.Visibility = Visibility.Collapsed;
            lstViewIDSTechData.Visibility = Visibility.Collapsed;
            
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
                logger.Info("PDF erstellt!");
            }
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

            logger.Info("Kurse angepasst...");
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #region CheckBox Integrata
        private void chkBoxSelectAll_Checked(object sender, RoutedEventArgs e) {
            IndividualChkBxUnCheckedFlagIntegrata = false;

            foreach(Kurse kurs in lstViewIntegrata.ItemsSource) {
                kurs.IsSelected = true;
                lstViewIntegrata.SelectedItems.Add(kurs);
            }
        }

        private void chkBoxSelectAll_Unchecked(object sender, RoutedEventArgs e) {

            if (!IndividualChkBxUnCheckedFlagIntegrata) {
                foreach(Kurse kurs in lstViewIntegrata.ItemsSource) {
                    kurs.IsSelected = false;
                    lstViewIntegrata.SelectedItems.Remove(kurs);
                }
            }
        }

        private void chkBoxSelect_Unchecked(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIntegrata, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = false;

            IndividualChkBxUnCheckedFlagIntegrata = true;
            CheckBox headerChk = (CheckBox)((GridView)lstViewIntegrata.View).Columns[0].Header;
            headerChk.IsChecked = false;
        }

        private void chkBoxSelect_Checked(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIntegrata, e.OriginalSource as DependencyObject) as ListBoxItem;
            if(kurs != null) 
                kurs.IsSelected = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void lstViewIntegrata_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(e.AddedItems.Count > 0) {
                Kurse kurs = (Kurse)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIntegrata.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = true;
            } else {
                Kurse kurs = (Kurse)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIntegrata.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = false;
            }
        }

        #endregion CheckBox Integrata

        #region CheckBox IDSIntegrata
        private void chkBoxSelectAll_CheckedIDSInte(object sender, RoutedEventArgs e) {
            IndividualChkBxUnCheckedFlagIDSIntegrata = false;

            foreach (Kurse kurs in lstViewIDSIntegrata.ItemsSource) {
                kurs.IsSelected = true;
                lstViewIDSIntegrata.SelectedItems.Add(kurs);
            }
        }

        private void chkBoxSelectAll_UncheckedIDSInte(object sender, RoutedEventArgs e) {

            if (!IndividualChkBxUnCheckedFlagIDSIntegrata) {
                foreach (Kurse kurs in lstViewIDSIntegrata.ItemsSource) {
                    kurs.IsSelected = false;
                    lstViewIDSIntegrata.SelectedItems.Remove(kurs);
                }
            }
        }

        private void chkBoxSelect_UncheckedIDSInte(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIDSIntegrata, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = false;

            IndividualChkBxUnCheckedFlagIDSIntegrata = true;
            CheckBox headerChk = (CheckBox)((GridView)lstViewIDSIntegrata.View).Columns[0].Header;
            headerChk.IsChecked = false;
        }

        private void chkBoxSelect_CheckedIDSInte(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIDSIntegrata, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = true;
        }

        private void MenuItem_ClickIDSInte(object sender, RoutedEventArgs e) {

        }

        private void lstViewIDSIntegrata_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                Kurse kurs = (Kurse)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIDSIntegrata.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = true;
            } else {
                Kurse kurs = (Kurse)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIDSIntegrata.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = false;
            }
        }
        #endregion CheckBox IDSIntegrata

        #region CheckBox TechData
        private void chkBoxSelectAll_CheckedTechData(object sender, RoutedEventArgs e) {
            IndividualChkBxUnCheckedFlagTechData = false;

            foreach (Kurse kurs in lstViewTechData.ItemsSource) {
                kurs.IsSelected = true;
                lstViewTechData.SelectedItems.Add(kurs);
            }
        }

        private void chkBoxSelectAll_UncheckedTechData(object sender, RoutedEventArgs e) {

            if (!IndividualChkBxUnCheckedFlagTechData) {
                foreach (Kurse kurs in lstViewTechData.ItemsSource) {
                    kurs.IsSelected = false;
                    lstViewTechData.SelectedItems.Remove(kurs);
                }
            }
        }

        private void chkBoxSelect_UncheckedTechData(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewTechData, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = false;

            IndividualChkBxUnCheckedFlagTechData = true;
            CheckBox headerChk = (CheckBox)((GridView)lstViewTechData.View).Columns[0].Header;
            headerChk.IsChecked = false;
        }

        private void chkBoxSelect_CheckedTechdata(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewTechData, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = true;
        }

        private void MenuItem_ClickTechData(object sender, RoutedEventArgs e) {

        }

        private void lstViewTechData_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                Kurse kurs = (Kurse)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewTechData.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = true;
            } else {
                Kurse kurs = (Kurse)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewTechData.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = false;
            }
        }
        #endregion CheckBox TechData

        #region CheckBox IDSTechData
        private void chkBoxSelectAll_CheckedIDSTechData(object sender, RoutedEventArgs e) {
            IndividualChkBxUnCheckedFlagIDSTechData = false;

            foreach (Kurse kurs in lstViewIDSTechData.ItemsSource) {
                kurs.IsSelected = true;
                lstViewIDSTechData.SelectedItems.Add(kurs);
            }
        }

        private void chkBoxSelectAll_UncheckedIDSTechData(object sender, RoutedEventArgs e) {

            if (!IndividualChkBxUnCheckedFlagIDSTechData) {
                foreach (Kurse kurs in lstViewIDSTechData.ItemsSource) {
                    kurs.IsSelected = false;
                    lstViewIDSTechData.SelectedItems.Remove(kurs);
                }
            }
        }

        private void chkBoxSelect_UncheckedIDSTechData(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIDSTechData, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = false;

            IndividualChkBxUnCheckedFlagIDSTechData = true;
            CheckBox headerChk = (CheckBox)((GridView)lstViewIDSTechData.View).Columns[0].Header;
            headerChk.IsChecked = false;
        }

        private void chkBoxSelect_CheckedIDSTechdata(object sender, RoutedEventArgs e) {
            ListBoxItem kurs = ItemsControl.ContainerFromElement(lstViewIDSTechData, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (kurs != null)
                kurs.IsSelected = true;
        }

        private void MenuItem_ClickIDSTechData(object sender, RoutedEventArgs e) {

        }

        private void lstViewIDSTechData_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                Kurse kurs = (Kurse)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIDSTechData.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = true;
            } else {
                Kurse kurs = (Kurse)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lstViewIDSTechData.ItemContainerGenerator.ContainerFromItem(kurs);
                CheckBox chkBox = FindVisualChild<CheckBox>(lvi);
                if (chkBox != null)
                    chkBox.IsChecked = false;
            }
        }
        #endregion CheckBox IDSTechData

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
    }
}
