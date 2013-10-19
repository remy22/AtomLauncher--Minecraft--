﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace AtomLauncher
{
    class atomDownloading
    {

        static WebClient webConnect;// Our WebClient that will be doing the downloading for us
        static Stopwatch sw = new Stopwatch();// The stopwatch which we will be using to calculate the download speed
        static bool downloadBusy = false;
        static bool cancelDownload = false;
        static double downloadAmmount = 0;
        static double timeAmmount = 0;
        static double totalBytes = 0;
        static double bytesRecievedTotal = 0;
        static double bytesRecieved = 0;
        static string downloadingFile = "NoFile";

        static public Dictionary<int, string[]> testDictionary = new Dictionary<int, string[]>{
            {0, new string[] { "http://trinaryatom.com/_web_downloads/Test/test0.zip", @"Folder\test0.zip" }},
            {1, new string[] { "http://trinaryatom.com/_web_downloads/Test/test1.zip", @"Folder\test1.zip" }},
            {2, new string[] { "http://trinaryatom.com/_web_downloads/Test/test2.zip", @"Folder\test2.zip" }},
            {3, new string[] { "http://trinaryatom.com/_web_downloads/Test/test3.zip", @"Folder\test3.zip" }},
            {4, new string[] { "http://trinaryatom.com/_web_downloads/Test/test4.zip", @"Folder\test4.zip" }},
            {5, new string[] { "http://trinaryatom.com/_web_downloads/Test/test5.zip", @"Folder\test5.zip" }}
        };

        /// <summary>
        /// Input multiple file to download. Does not stop until error or completed.
        /// </summary>
        /// <param name="urlFilePath">A Dictonary of files that contain the list of files.
        /// [Doesnt matter number, { conatining file url, file name, folder if any}]</param>
        /// <param name="filePATH"></param>
        internal static void Multi(Dictionary<int,string[]> urlFilePath, string filePATH)
        {
            cancelDownload = false;
            downloadAmmount = 0;
            timeAmmount = 0;
            totalBytes = 0;
            bytesRecievedTotal = 0;
            bytesRecieved = 0;
            downloadingFile = "NoFile";
            atomLauncher.atomLaunch.formText("formLabelStatus", "Checking Remote Files");
            atomLauncher.atomLaunch.formText("formLabelDLFile", "");
            atomLauncher.atomLaunch.formText("formLabelDLSpeed", "");
            atomLauncher.atomLaunch.formText("formLabelFileMB", "");
            atomLauncher.atomLaunch.formText("formLabelTotalMB", "");
            atomLauncher.atomLaunch.barValues(0, 0);
            foreach (KeyValuePair<int, string[]> entry in urlFilePath)
            {
                if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Checking Remote Files");
                int contentLength = 0;
                atomLauncher.atomLaunch.formText("formLabelDLFile", (entry.Value[0]).TruncateDots(60));
                try
                {
                    System.Net.WebRequest req = System.Net.HttpWebRequest.Create(entry.Value[0]);
                    req.Method = "HEAD";
                    using (System.Net.WebResponse resp = req.GetResponse())
                    {
                        int.TryParse(resp.Headers.Get("Content-Length"), out contentLength);
                    }
                }
                catch (Exception ex)
                {
                    cancelDownload = true;
                    throw new System.Exception("Error: " + ex.Message);
                }
                totalBytes = totalBytes + contentLength;
            }
            atomLauncher.atomLaunch.formText("formLabelStatus", "Downloading Files");
            foreach (KeyValuePair<int, string[]> entry in urlFilePath)
            {
                if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Downloading Files");
                downloadBusy = true;
                try
                {
                    download(entry.Value[0], filePATH + @"\" + entry.Value[1]); // Start downloading the file
                }
                catch (Exception ex)
                {
                    cancelDownload = true;
                    throw new System.Exception("Error: " + ex.Message);
                }
                while (downloadBusy) // Wait for Complete File
                {
                    Thread.Sleep(100);
                    if (atomLauncher.cancelPressed || cancelDownload)
                    {
                        break;
                    }
                }
            }
            atomLauncher.atomLaunch.formText("formLabelDLFile", "");
            atomLauncher.atomLaunch.formText("formLabelDLSpeed", "");
            atomLauncher.atomLaunch.formText("formLabelFileMB", "");
            atomLauncher.atomLaunch.formText("formLabelTotalMB", "");
            atomLauncher.atomLaunch.barValues(100, 100);
            if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Downloading Files");
        }

        /// <summary>
        /// Input a single file to download. Does not stop until error or completed.
        /// </summary>
        /// <param name="urlFilePATH">Example: "http://www.webaddress.com/file.zip" </param>
        /// <param name="filePATH">Example: "C:\LOCATION\file.zip" </param>
        internal static void Single(string urlFilePATH, string filePATH)
        {
            cancelDownload = false;
            downloadAmmount = 0;
            timeAmmount = 0;
            totalBytes = 0;
            bytesRecievedTotal = 0;
            bytesRecieved = 0;
            downloadingFile = "NoFile";
            atomLauncher.atomLaunch.formText("formLabelStatus", "Checking Remote File");
            atomLauncher.atomLaunch.formText("formLabelDLFile", urlFilePATH.TruncateDots(60));
            atomLauncher.atomLaunch.formText("formLabelDLSpeed", "");
            atomLauncher.atomLaunch.formText("formLabelFileMB", "");
            atomLauncher.atomLaunch.formText("formLabelTotalMB", "");
            atomLauncher.atomLaunch.barValues(0, 0);
                /////////////////////
            if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Checking Remote File");
                /////////////////////
            int contentLength = 0;
            try
            {
                System.Net.WebRequest req = System.Net.HttpWebRequest.Create(urlFilePATH);
                req.Method = "HEAD";
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    int.TryParse(resp.Headers.Get("Content-Length"), out contentLength);
                }
                totalBytes = contentLength;
            }
            catch (Exception ex)
            {
                cancelDownload = true;
                throw new System.Exception("Error: " + ex.Message);
            }
                /////////////////////
            if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Downloading File");
                /////////////////////
            atomLauncher.atomLaunch.formText("formLabelStatus", "Downloading File");
            downloadBusy = true;
            try
            {
                download(urlFilePATH, filePATH); // Start downloading the file
            }
            catch (Exception ex)
            {
                cancelDownload = true;
                throw new System.Exception("Error: " + ex.Message);
            }
                /////////////////////
            if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Downloading File");
                /////////////////////
            while (downloadBusy) // Wait for Complete File
            {
                Thread.Sleep(100);
                if (atomLauncher.cancelPressed || cancelDownload)
                {
                    break;
                }
            }
            atomLauncher.atomLaunch.formText("formLabelDLFile", "");
            atomLauncher.atomLaunch.formText("formLabelDLSpeed", "");
            atomLauncher.atomLaunch.formText("formLabelFileMB", "");
            atomLauncher.atomLaunch.formText("formLabelTotalMB", "");
            atomLauncher.atomLaunch.barValues(100, 100);
                /////////////////////
            if (atomLauncher.cancelPressed || cancelDownload) throw new System.Exception("Downloading File");
        }

        /// <summary>
        /// The base method used to start individual downloads.
        /// </summary>
        /// <param name="urlFilePATH">Example: "http://www.webaddress.com/file.zip" </param>
        /// <param name="filePATH">Example: "C:\LOCATION\file.zip" </param>
        internal static void download(string urlFilePATH, string filePATH)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePATH));
            downloadingFile = filePATH;
            sw.Reset();
            if (atomLauncher.cancelPressed || cancelDownload) return;
            using (webConnect = new WebClient())
            {
                webConnect.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progress);
                webConnect.DownloadFileCompleted += new AsyncCompletedEventHandler(completed);
                try
                {
                    Uri URL = new Uri(urlFilePATH);
                    sw.Start();
                    webConnect.DownloadFileAsync(URL, downloadingFile);
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// The method used by the AsyncDownload to show progress.
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Download Progress Changed Event Args - e</param>
        internal static void progress(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                if (atomLauncher.cancelPressed || cancelDownload)
                {
                    webConnect.CancelAsync();
                }
                else
                {
                    if (totalBytes == 0)
                    {
                        totalBytes = 1;
                    }
                    bytesRecieved = e.BytesReceived;
                    double compileReceivedBytes = bytesRecievedTotal + bytesRecieved;
                    if (e.ProgressPercentage != 100)
                    {
                        atomLauncher.atomLaunch.barValues(0, 0);
                        downloadAmmount = Convert.ToDouble(e.BytesReceived) - downloadAmmount; //Refine this. To more accuratly show appropriot speed. Espesially in low bandwidth situations.
                        timeAmmount = sw.Elapsed.TotalSeconds - timeAmmount;
                        atomLauncher.atomLaunch.formText("formLabelDLFile", downloadingFile.TruncateDots(60));
                        if (atomLauncher.atomLaunch.formLabelDLSpeed.Text != (downloadAmmount / 1024 / timeAmmount).ToString("0.00"))
                        {
                            atomLauncher.atomLaunch.formText("formLabelDLSpeed", (downloadAmmount / 1024 / timeAmmount).ToString("0.00") + " kb/s");
                        }
                        if (atomLauncher.atomLaunch.formBarTop.Value != e.ProgressPercentage)
                        {
                            atomLauncher.atomLaunch.barValues(e.ProgressPercentage, Convert.ToInt32(compileReceivedBytes * 100 / totalBytes));
                            atomLauncher.atomLaunch.formText("formLabelFileMB", (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0.00") + " Mb's " + "/ " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0.00") + " Mb's");
                            atomLauncher.atomLaunch.formText("formLabelTotalMB", (Convert.ToDouble(compileReceivedBytes) / 1024 / 1024).ToString("0.00") + " Mb's " + "/ " + (Convert.ToDouble(totalBytes) / 1024 / 1024).ToString("0.00") + " Mb's");
                        }
                    }
                    else
                    {
                        atomLauncher.atomLaunch.barValues(100, Convert.ToInt32(compileReceivedBytes * 100 / totalBytes));
                        atomLauncher.atomLaunch.formText("formLabelFileMB", (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0.00") + " Mb's " + "/ " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0.00") + " Mb's");
                    }
                }
            }
            catch (Exception ex)
            {
                cancelDownload = true;
                MessageBox.Show("Error: " + ex.Message + " : Please Report this. (Error - atomDownloading.progress)");
            }
        }
        /// <summary>
        /// The method used by the AsyncDownload when completed.
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Async Completed Event Args - e</param>
        internal static void completed(object sender, AsyncCompletedEventArgs e)
        {
            bytesRecievedTotal = bytesRecievedTotal + bytesRecieved;
            sw.Reset();
            if (e.Cancelled == true)
            {
                atomFileData.queueDelete(downloadingFile);
            }
            else if (e.Error != null)
            {
                cancelDownload = true;
                atomFileData.queueDelete(downloadingFile);
                MessageBox.Show("Error: " + e.Error.Message + " : " + e.Error.InnerException.Message + " Please Report this. (Error - atomDownloading.completed)");
            }
            else
            {
                downloadBusy = false;
                downloadingFile = "NoFile";
            }
        }
    }
}