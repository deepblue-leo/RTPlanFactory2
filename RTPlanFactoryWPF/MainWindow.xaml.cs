using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择计划文件所在的目录";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TxtRpFilePath.Text = dialog.SelectedPath + "\\";
                _workflowImplementer.LoopOriginalFileSetFolder(this.TxtRpFilePath.Text.Trim(), ShowListOriginalPlanInfo);
            }

            //this.ListOriginalPlanInfo.Items.MoveCurrentTo(this.ListOriginalPlanInfo.Items[this.ListOriginalPlanInfo.Items.Count - 1]);
        }

        private void ShowListOriginalPlanInfo(DicomFileInfo info)
        {
            string showlog = string.Format("[{0}]:{1},{2}", info.SopType, info.OriginalFilePath, info.OrginalSopInfo.ToString());
            this.ListOriginalPlanInfo.Items.Add(showlog);
        }

        private void ShowListNewPlanInfo(DicomFileInfo info)
        {
            string showlog = string.Format("[{0}]:{1},{2}", info.SopType, info.NewFilePath, info.NewSopInfo.ToString());
            this.ListNewPlanInfo.Items.Add(showlog);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(this.TxtNewPlanNum.Text, out int newPlanCount))
            {
                MessageBox.Show("请输入正确的新计划数量");
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

            string newPatientName;
            string newPatientId;
            string newFileSetFolder;
            string newPlanLabel;

            switch (npp)
            {
                case CreateNewPlanPattern.SinglePatientMultiPlan:
                    newPatientName = GetNewPatientName();
                    newPatientId = GetNewPatientId();
                    
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        newFileSetFolder = string.Format("{0}\\{1}\\",dialog.SelectedPath, newPatientName);
                    }
                    else
                    {
                        return;
                    }

                    for (int i = 1; i <= newPlanCount; i++)
                    {
                        newFileSetFolder = string.Format("{0}{1}\\", newFileSetFolder, i);
                        newPlanLabel = GetNewPlanLabel(newPatientName, i);
                        _workflowImplementer.CreateNewRpSets(newFileSetFolder, newPatientName, newPatientId, newPlanLabel, ShowListNewPlanInfo);
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
                        newPatientName = GetNewPatientName();
                        newPatientId = GetNewPatientId();
                        newFileSetFolder = string.Format("{0}\\{1}\\{2}\\", newFileSetFolder, newPatientName, 1);
                        newPlanLabel = GetNewPlanLabel(newPatientName, 1);
                        _workflowImplementer.CreateNewRpSets(newFileSetFolder, newPatientName, newPatientId, newPlanLabel, ShowListNewPlanInfo);
                    }
                    break;
                default:
                    break;
            }
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


    }

    public enum CreateNewPlanPattern
    {
        SinglePatientMultiPlan,
        MultiPatientSinglePlan,
    }
}
