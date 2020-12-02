using System.Text;
using System.Windows.Forms;

namespace InputFilesTXT
{
    internal partial class SettingsForm_InputFilesTXT : Form
    {


        #region Get and Set Options

        public string TextFileDirectory { get; set; }
        public bool ScanSubfolders { get; set; }
        public string SelectedEncoding { get; set; }

       #endregion



        public SettingsForm_InputFilesTXT(string TextFileDirectory, bool ScanSubfolders, string SelectedEncoding)
        {
            InitializeComponent();

            foreach (var encoding in Encoding.GetEncodings())
            {
                EncodingDropdown.Items.Add(encoding.Name);
            }

            try
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(SelectedEncoding);
            }
            catch
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(Encoding.Default.BodyName);
            }

            IncludeSubfoldersCheckbox.Checked = ScanSubfolders;
            SelectedFolderTextbox.Text = TextFileDirectory;

        }






        private void SetFolderButton_Click(object sender, System.EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.Description = "Please choose the location of your .txt files to analyze";
                if (!string.IsNullOrWhiteSpace(SelectedFolderTextbox.Text)) dialog.SelectedPath = SelectedFolderTextbox.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolderTextbox.Text = dialog.SelectedPath.ToString();
                }
            }
        }


        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.SelectedEncoding = EncodingDropdown.SelectedItem.ToString();
            this.ScanSubfolders = IncludeSubfoldersCheckbox.Checked;
            this.TextFileDirectory = SelectedFolderTextbox.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
