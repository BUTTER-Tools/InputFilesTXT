using PluginContracts;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using OutputHelperLib;
using System.Linq;

namespace InputFilesTXT
{
    public class InputFilesTXT : InputPlugin
    {

        public string[] InputType { get; } = { ".txt Files" };
        public string OutputType { get; } = "String";

        public bool KeepStreamOpen { get; } = false;
        public StreamReader InputStream { get; set; }
        public string IncomingTextLocation { get; set; } = "";
        private bool ScanSubfolders = false;
        public string SelectedEncoding { get; set; } = "utf-8";
        public bool InheritHeader { get; } = false;

        public Dictionary<int, string> OutputHeaderData { get; set; } = new Dictionary<int, string>(){
                                                                                            {0, "Text"}
                                                                                        };

        public int TextCount { get; set; }

        #region IPlugin Details and Info

        public string PluginName { get; } = "Load .txt Files from Folder";
        public string PluginType { get; } = "Load File(s)";
        public string PluginVersion { get; } = "1.0.4";
        public string PluginAuthor { get; } = "Ryan L. Boyd (ryan@ryanboyd.io)";
        public string PluginDescription { get; } = "This plugin will read texts from .txt files contained within a folder. This plugin should always be at the top level of your Analysis Pipeline. For example:" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "\tLoad .txt Files from Folder" + Environment.NewLine +
            "\t |" + Environment.NewLine +
            "\t |-- Tokenize Texts" + Environment.NewLine +
            "\t |" + Environment.NewLine +
            "\t |-- etc." + Environment.NewLine;
        public bool TopLevel { get; } = true;
        public string PluginTutorial { get; } = "https://youtu.be/k2hBFC4QX5M";

        public Icon GetPluginIcon
        {
            get
            {
                return Properties.Resources.icon;
            }
        }

        #endregion

        #region Settings and ChangeSettings() Method

        public void ChangeSettings()
        {



            using (var form = new SettingsForm_InputFilesTXT(IncomingTextLocation, ScanSubfolders, SelectedEncoding))
            {


                form.Icon = Properties.Resources.icon;

                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedEncoding = form.SelectedEncoding;
                    IncomingTextLocation = form.TextFileDirectory;
                    ScanSubfolders = form.ScanSubfolders;
                }
            }



        }
        #endregion

        //sets GetTextList with the files to be analyzed
        public Payload RunPlugin(Payload Incoming)
        {

            Payload pData = new Payload();
            pData.FileID = Incoming.FileID;
            pData.SegmentID = Incoming.SegmentID;


            try
            {
                List<object> FileContents = new List<object>();
                pData.FileID = Path.GetFileName((string)Incoming.ObjectList[0]);
                pData.StringList.Add(File.ReadAllText(Incoming.ObjectList[0].ToString(), Encoding.GetEncoding(SelectedEncoding)).Trim());
                pData.SegmentNumber.Add(1);

                if (ScanSubfolders) pData.SegmentID.Add(Path.GetDirectoryName((string)Incoming.ObjectList[0]).Remove(0,IncomingTextLocation.Length));


            }
            catch
            {
                pData.StringList = new List<string>();
                pData.SegmentNumber.Add(1);
            }

            return (pData);


        }



        //public string GetFileIdentifier(object TextInputItem)
        //{
        //    //since this plugin only generates filepaths, we just need
        //    //to strip it down to the filenames
        //    return (Path.GetFileName((string)TextInputItem));
        //}

        public IEnumerable TextEnumeration()
        {
            //for this plugin, all that we're really doing is setting the IEnumerable full of the text files
            SearchOption FolderDepth = new SearchOption();

            if (ScanSubfolders)
            {
                FolderDepth = SearchOption.AllDirectories;
            }
            else
            {
                FolderDepth = SearchOption.TopDirectoryOnly;
            }

            if (!string.IsNullOrEmpty(IncomingTextLocation))
            {
                return (Directory.EnumerateFiles(IncomingTextLocation, "*.txt", FolderDepth));
            }
            else
            {
                return (Enumerable.Empty<string>());
            }
            
        }



        //for input streams, we use the Initialize() method to tally up the number of items to be analyzed
        public void Initialize()
        {
            TextCount = 0;

            SearchOption FolderDepth = new SearchOption();
            if (ScanSubfolders)
            {
                FolderDepth = SearchOption.AllDirectories;
            }
            else
            {
                FolderDepth = SearchOption.TopDirectoryOnly;
            }

            var files = Directory.EnumerateFiles(IncomingTextLocation, "*.txt", FolderDepth);

            

            foreach (string filecount in files)
            {
                TextCount++;
            }

        }




        public bool InspectSettings()
        {
            if (string.IsNullOrEmpty(IncomingTextLocation))
            {
                return false;
            }
            else if (!Directory.Exists(IncomingTextLocation))
            {
                MessageBox.Show("Your selected directory does not appear to exist anymore. Has it been deleted/moved?", "Cannot Find Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }

        }

        public Payload FinishUp(Payload Input)
        {
            return (Input);
        }





        public Dictionary<string, string> ExportSettings(bool suppressWarnings)
        {
            Dictionary<string, string> Settings = new Dictionary<string, string>();

            Settings.Add("IncomingTextLocation", IncomingTextLocation);
            Settings.Add("ScanSubfolders", ScanSubfolders.ToString());
            Settings.Add("SelectedEncoding", SelectedEncoding);

            return (Settings);
        }

        public void ImportSettings(Dictionary<string, string> Settings)
        {
            IncomingTextLocation = Settings["IncomingTextLocation"];
            Boolean.Parse(Settings["ScanSubfolders"]);
            SelectedEncoding = Settings["SelectedEncoding"];
            ScanSubfolders = Boolean.Parse(Settings["ScanSubfolders"]);

        }



    }

}
