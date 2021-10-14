using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using RTPlanFactoryLib.Implementor;
using RTPlanFactoryLib.Model;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RTPlanFactoryWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {        
        private CreateNewRpWorkflowImplementer _workflowImplementer = new CreateNewRpWorkflowImplementer();
        public MainWindow()
        {
            InitializeComponent();
        }                

        private void BtnSelectRpFile_Click(object sender, RoutedEventArgs e)
        {
            ListOriginalPlanInfo.Items.Clear();

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择计划文件所在的目录";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                
                ListNewPlanInfo.Items.Clear();

                this.TxtRpFilePath.Text = dialog.SelectedPath + "\\";
                _workflowImplementer.LoopOriginalFileSetFolder(this.TxtRpFilePath.Text.Trim(), ShowListOriginalPlanInfo);                
            }            
        }

        private async void ShowListOriginalPlanInfo(DicomFileInfo info)
        {            
            await Task.Run(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (info.SopType == RTPlanFactoryLib.Utility.EnumSopType.RT_PLAN)
                    {
                        this.TxtMachineName.Text = ((RpInfo)(info.OrginalSopInfo)).TreatmentMachineNames[0];
                    }
                    
                    string showlog = string.Format("[{0}]:{1},{2}", info.SopType, info.OriginalFilePath, info.OrginalSopInfo.ToString());
                    this.ListOriginalPlanInfo.Items.Add(showlog);

                    this.ListOriginalPlanInfo.Items.MoveCurrentToLast();
                    //this.ListNewPlanInfo.Focus();
                    this.ListOriginalPlanInfo.SelectedItem = this.ListOriginalPlanInfo.Items.CurrentItem;
                    this.ListOriginalPlanInfo.ScrollIntoView(this.ListOriginalPlanInfo.SelectedItem);
                }));
            });
        }

        private async void ShowListNewPlanInfo(DicomFileInfo info)
        {            
            await Task.Run(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    string showlog = string.Format("[{0}]:{1},{2}", info.SopType, info.NewFilePath, info.NewSopInfo.ToString());
                    this.ListNewPlanInfo.Items.Add(showlog);
                    this.ListNewPlanInfo.Items.MoveCurrentToLast();
                    //this.ListNewPlanInfo.Focus();
                    this.ListNewPlanInfo.SelectedItem = this.ListNewPlanInfo.Items.CurrentItem;
                    this.ListNewPlanInfo.ScrollIntoView(this.ListNewPlanInfo.SelectedItem);
                }));
            });
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {            
            this.ListNewPlanInfo.Items.Clear();

            if (!int.TryParse(this.TxtNewPlanNum.Text, out int newPlanCount))
            {
                MessageBox.Show("请输入正确的新计划数量");
                this.BtnStart.IsEnabled = true;
                return;
            }

            string newMachineName = this.TxtMachineName.Text.Trim();

            if (string.IsNullOrEmpty(newMachineName))
            {
                MessageBox.Show("请输入正确的设备名称");
                this.BtnStart.IsEnabled = true;
                return;
            }            


            CreateNewPlanPattern npp = CreateNewPlanPattern.SinglePatientMultiPlan;

            if (this.radioMultiPatientSinglePlan.IsChecked.Value)
            {
                npp = CreateNewPlanPattern.MultiPatientSinglePlan;
            }
            else if (this.radioSinglePatientMultiPlan.IsChecked.Value)
            {
                npp = CreateNewPlanPattern.SinglePatientMultiPlan;
            }

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择新计划文件要保存的目录";

            string newPatientName = GetNewPatientName();
            string newPatientId = GetNewPatientId();
            string newFileSetFolder;
            string newPlanLabel;
            string newStudyInstanceUid = GetNewStudyInstanceUid();

            switch (npp)
            {
                case CreateNewPlanPattern.SinglePatientMultiPlan:                                        
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.BtnStart.IsEnabled = false;
                        newFileSetFolder = string.Format("{0}\\{1}\\",dialog.SelectedPath, newPatientName);
                    }
                    else
                    {
                        this.BtnStart.IsEnabled = true;
                        return;
                    }

                    for (int i = 1; i <= newPlanCount; i++)
                    {
                        //还原路径
                        newFileSetFolder = string.Format("{0}\\{1}\\", dialog.SelectedPath, newPatientName);

                        newFileSetFolder = string.Format("{0}{1}\\", newFileSetFolder, i);
                        newPlanLabel = GetNewPlanLabel(newPatientName, i);
                        _workflowImplementer.CreateNewRpSets(newFileSetFolder, newPatientName, newPatientId, newPlanLabel, newMachineName, newStudyInstanceUid, ShowListNewPlanInfo);
                    }                    
                    break;
                case CreateNewPlanPattern.MultiPatientSinglePlan:
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        newFileSetFolder = dialog.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                    
                    for (int i = 1; i <= newPlanCount; i++)
                    {
                        //还原路径
                        newFileSetFolder = dialog.SelectedPath;                        
                        newFileSetFolder = string.Format("{0}\\{1}\\{2}\\", newFileSetFolder, newPatientName, 1);
                        newPlanLabel = GetNewPlanLabel(newPatientName, 1);
                        _workflowImplementer.CreateNewRpSets(newFileSetFolder, newPatientName, newPatientId, newPlanLabel, newMachineName, newStudyInstanceUid, ShowListNewPlanInfo);
                    }
                    break;
                default:
                    break;
            }

            this.BtnStart.IsEnabled = true;
        }

        private string GetNewPatientName()
        {
            string firstName = null;
            string lastName = DateTime.Now.ToString("HHmmssfff");
            string fullName = lastName;

            if (!string.IsNullOrEmpty(this.TxtFirstName.Text))
            {
                firstName = this.TxtFirstName.Text.Trim();
                fullName = string.Format("{0}^{1}", firstName, lastName);
            }

            return fullName;
        }

        private string GetNewPatientId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private string GetNewPlanLabel(string patientName,int planNumber)
        {
            string newPlanLable = this.TxtPlanLabel.Text.Trim();
            //[PatientName]_FactoryPlan[Number]
            newPlanLable.Replace("[PatientName]", patientName);
            newPlanLable.Replace("[Number]", planNumber.ToString());

            return newPlanLable;
        }

        private string GetNewStudyInstanceUid()
        {
            return DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid());
        }


    }

    public enum CreateNewPlanPattern
    {
        SinglePatientMultiPlan,
        MultiPatientSinglePlan,
    }
}
