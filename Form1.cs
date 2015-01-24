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
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        BackgroundWorker syncBackgroundWorker = new BackgroundWorker();


        public Form1()
        {
            InitializeComponent();            

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

        #region Event Methods

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
            if (result == DialogResult.OK)
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

        private void StartSyncButton_Click(object sender, EventArgs e)
        {
            List<Sync> syncs = repo.All<Sync>().ToList();

            foreach (Sync sync in syncs)
            {
                int numberOfThingsToCheck = CruiseDirectoriesAndGetCounts(sync.Source);

                syncBackgroundWorker = new BackgroundWorker();
                syncBackgroundWorker.DoWork += syncBackgroundWorker_DoWork;
                syncBackgroundWorker.ProgressChanged += syncBackgroundWorker_ProgressChanged;
                syncBackgroundWorker.WorkerReportsProgress = true;
                syncBackgroundWorker.RunWorkerCompleted += syncBackgroundWorker_RunWorkerCompleted;
                syncBackgroundWorker.RunWorkerAsync(new Tuple<string, string, int>(sync.Source.Trim(), sync.Destination.Trim(), numberOfThingsToCheck));
            }
        }

        private int CruiseDirectoriesAndGetCounts(string path)
        {
            int count = 0;
            foreach (var directory in Directory.GetDirectories(path))
            {
                count += CruiseDirectoriesAndGetCounts(directory);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                count++;
            }
            return count;
        }

        #endregion Event Methods

        #region Main Work Methods

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


        private int StartSync(string source, string sourceRoot, string destinationRoot, int totalNumberOfFiles)
        {
            int numberOfFilesProcessed = 0;
            foreach (var directory in Directory.GetDirectories(sourceRoot + source))
            {
                if (!Directory.Exists(directory.Replace(sourceRoot, destinationRoot)))
                {
                    FileOperations.Add(new FileOperation() { DestinationPath = directory.Replace(sourceRoot, destinationRoot), FileActionType = Enumerations.Enumeration.ActionType.CreateDirectory, SourcePath = string.Empty });
                    
                }
                numberOfFilesProcessed += StartSync(directory.Replace(sourceRoot, string.Empty), sourceRoot, destinationRoot, totalNumberOfFiles);
            }

            foreach (var file in Directory.GetFiles(sourceRoot + source))
            {
                if (!File.Exists(file.Replace(sourceRoot, destinationRoot)))
                {
                    FileOperations.Add(new FileOperation() { DestinationPath = file.Replace(sourceRoot, destinationRoot), FileActionType = Enumerations.Enumeration.ActionType.CopyFile, SourcePath = file });
                }
                else
                {
                    byte[] sourceFileHash, destinationFileHash;
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(file))
                        {
                            sourceFileHash = md5.ComputeHash(stream);
                        }
                    }

                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(file.Replace(sourceRoot, destinationRoot)))
                        {
                            destinationFileHash = md5.ComputeHash(stream);
                        }
                    }

                    bool fileChanged = false;

                    for (int i = 0; i < sourceFileHash.Length; i++)
                    {
                        if (sourceFileHash[i] != destinationFileHash[i])
                        {
                            fileChanged = true;
                            break;
                        }
                    }

                    if (fileChanged)
                    {
                        FileOperations.Add(new FileOperation() { DestinationPath = file.Replace(sourceRoot, destinationRoot), FileActionType = Enumerations.Enumeration.ActionType.Update, SourcePath = file });
                    }

                    numberOfFilesProcessed++;
                    syncBackgroundWorker.ReportProgress(0, new Tuple<string, int, int>("Checking " + file.Replace(sourceRoot, destinationRoot), numberOfFilesProcessed, totalNumberOfFiles));
                }
            }
            return numberOfFilesProcessed;
        }


        #endregion Main Work Methods

        #region File Operation Async Methods

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformFileOperations();
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<int, int> progressInfo = e.UserState as Tuple<int, int>;
            int currentOperationNumber = progressInfo.Item1;
            int totalOperations = progressInfo.Item2;

            progressBar.Value = (int)(Convert.ToDouble(currentOperationNumber) / Convert.ToDouble(totalOperations) * 100);

            statusLabel.Text = string.Format("Syncing {0} of {1}", currentOperationNumber, totalOperations);
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = "Sync Finished";
        }

        #endregion File Operation Async Methods

        #region Sync Operation Async Methods
        void syncBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<string, string, int> sourceAndDestinationPaths = e.Argument as Tuple<string, string, int>;


            StartSync(string.Empty, sourceAndDestinationPaths.Item1, sourceAndDestinationPaths.Item2, sourceAndDestinationPaths.Item3);
        }

        void syncBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<string, int, int> progressInfo = e.UserState as Tuple<string, int, int>;

            statusLabel.Text = progressInfo.Item2 + " of " + progressInfo.Item3 + " Checking " + progressInfo.Item1;

            progressBar.Value = (int)(Convert.ToDouble(progressInfo.Item2) / Convert.ToDouble(progressInfo.Item3));
        }

        void syncBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = "Starting Sync";

            backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        #endregion Sync Operation Async Methods
    }
}
