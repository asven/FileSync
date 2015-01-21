using FileSync.Common;
using SubSonic.DataProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Security.Cryptography;


namespace FileSync
{
    public partial class Form1 : Form
    {
        private SubSonic.Repository.SimpleRepository repo = new SubSonic.Repository.SimpleRepository("SYNC");
        private List<FileOperation> FileOperations = new List<FileOperation>();
        private string SourceRoot = string.Empty;
        private string DestinationRoot = string.Empty;
        BackgroundWorker backgroundWorker = new BackgroundWorker();


        public Form1()
        {
            InitializeComponent();


            //SWebRequest request = WebRequest.Create("https://www.googleapis.com/storage/v1");

            BindCurrentSyncsGrid();

            CurrentSyncsGridView.CellClick += CurrentSyncsGridView_CellClick;
        }


        private void BindCurrentSyncsGrid()
        {
            CurrentSyncsGridView.DataSource = repo.All<Sync>().ToList();

            if (!CurrentSyncsGridView.Columns.Contains("DeleteButton"))
            {
                DataGridViewButtonColumn col = new DataGridViewButtonColumn();
                col.UseColumnTextForButtonValue = true;
                col.Text = "Delete";
                col.Name = "DeleteButton";
                CurrentSyncsGridView.Columns.Add(col);
            }
        }

        private void CurrentSyncsGridView_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (CurrentSyncsGridView.Columns[e.ColumnIndex].Name == "DeleteButton")
            {
                var idToDelete = CurrentSyncsGridView.Rows.Cast<DataGridViewRow>().Where(r => r.Index == e.RowIndex).First().Cells["ID"].Value;
                repo.Delete<Sync>(idToDelete);
            }
            BindCurrentSyncsGrid();
        }

        private void SourceBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                SourceTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void DestinationBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                DestinationTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SaveSyncButton_Click(object sender, EventArgs e)
        {
            Sync newSync = new Sync();
            newSync.Source = SourceTextBox.Text;
            newSync.Destination = DestinationTextBox.Text;

            repo.Add(newSync);

            SourceTextBox.Text = string.Empty;
            DestinationTextBox.Text = string.Empty;

            BindCurrentSyncsGrid();
        }

        private void PerformFileOperations()
        {
            int i = 0;
            
            List<FileOperation> successfulOperations = new List<FileOperation>();

            foreach (FileOperation fileOperation in FileOperations.OrderBy(fo => fo.FileActionType.GetHashCode()))
            {
                i++;
                switch (fileOperation.FileActionType)
                {
                    case Enumerations.Enumeration.ActionType.CopyFile:
                        File.Copy(fileOperation.SourcePath, fileOperation.DestinationPath);
                        break;
                    case Enumerations.Enumeration.ActionType.CreateDirectory:
                        Directory.CreateDirectory(fileOperation.DestinationPath);
                        break;
                    case Enumerations.Enumeration.ActionType.Update:
                        File.Delete(fileOperation.DestinationPath);
                        File.Copy(fileOperation.SourcePath, fileOperation.DestinationPath);
                        break;
                }
                successfulOperations.Add(fileOperation);

                backgroundWorker.ReportProgress(0, new Tuple<int, int>(i, FileOperations.Count));
            }

            foreach (FileOperation successfulOperation in successfulOperations)
            {
                this.FileOperations.Remove(successfulOperation);
            }
        }

        private void StartSyncButton_Click(object sender, EventArgs e)
        {
            List<Sync> syncs = repo.All<Sync>().ToList();

            foreach (Sync sync in syncs)
            {
                SourceRoot = sync.Source.Trim();
                DestinationRoot = sync.Destination.Trim();

                StartSync(string.Empty);
            }


            statusLabel.Text = "Starting Sync";

            backgroundWorker = new BackgroundWorker();
            
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();   
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = "Sync Finished";
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<int, int> progressInfo = e.UserState as Tuple<int, int>;
            int currentOperationNumber = progressInfo.Item1;
            int totalOperations = progressInfo.Item2;

            progressBar.Value = (int)(Convert.ToDouble(currentOperationNumber) / Convert.ToDouble(totalOperations) * 100);

            statusLabel.Text = string.Format("Syncing {0} of {1}", currentOperationNumber, totalOperations);
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformFileOperations();
        }



        private void StartSync(string source)
        {
            foreach (var directory in Directory.GetDirectories(SourceRoot + source))
            {
                if(!Directory.Exists(directory.Replace(SourceRoot, DestinationRoot)))
                {
                    FileOperations.Add(new FileOperation() { DestinationPath = directory.Replace(SourceRoot, DestinationRoot), FileActionType = Enumerations.Enumeration.ActionType.CreateDirectory, SourcePath = string.Empty });
                }
                StartSync(directory.Replace(SourceRoot, string.Empty));
            }

            foreach (var file in Directory.GetFiles(SourceRoot + source))
            {
                if (!File.Exists(file.Replace(SourceRoot, DestinationRoot)))
                {
                    FileOperations.Add(new FileOperation() { DestinationPath = file.Replace(SourceRoot, DestinationRoot), FileActionType = Enumerations.Enumeration.ActionType.CopyFile, SourcePath = file });
                }
                else
                {
                    byte[] sourceFileHash, destinationFileHash;
                    using(var md5 = MD5.Create())
                    {
                        using(var stream = File.OpenRead(file))
                        {
                            sourceFileHash = md5.ComputeHash(stream);
                        }
                    }

                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(file.Replace(SourceRoot, DestinationRoot)))
                        {
                            destinationFileHash = md5.ComputeHash(stream);
                        }
                    }

                    bool fileChanged = false;

                    for (int i = 0; i < sourceFileHash.Length; i++)
                    {
                        if(sourceFileHash[i] != destinationFileHash[i])
                        {
                            fileChanged = true;
                            break;
                        }
                    }

                    if(fileChanged)
                    { 
                        FileOperations.Add(new FileOperation() { DestinationPath = file.Replace(SourceRoot, DestinationRoot), FileActionType = Enumerations.Enumeration.ActionType.Update, SourcePath = file });
                    }

                }
            }
        }

    }
}
