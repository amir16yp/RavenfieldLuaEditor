using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Text;

namespace RavenfieldLuaEditor
{
    public partial class Form1 : Form
    {
        private AssetsManager manager;
        private BundleFileInstance bundleInstance;
        private Dictionary<int, AssetsFileInstance> assetsFileInstances = new Dictionary<int, AssetsFileInstance>();
        private List<TextAssetInfo> textAssets = new List<TextAssetInfo>();
        private TextAssetInfo currentAsset;
        private bool bundleModified = false;
        private bool contentModified = false;
        private string bundlePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Handle command-line arguments
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                OpenBundle(args[1]);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Unity Bundle Files|*.unity3d;*.assets;*.bundle;*.rfc|All Files|*.*";
                dialog.Title = "Open Unity Bundle File";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    OpenBundle(dialog.FileName);
                }
            }
        }

        private void OpenBundle(string path)
        {
            try
            {
                // Reset UI
                txtContent.Clear();
                lstTextAssets.Items.Clear();
                bundleModified = false;
                contentModified = false;
                currentAsset = null;

                // Update status
                SetStatus("Loading bundle...");

                // Clean up previous resources
                if (manager != null)
                {
                    CleanupResources();
                }

                // Initialize new manager
                manager = new AssetsManager();
                bundlePath = path;

                // Load bundle
                bundleInstance = manager.LoadBundleFile(path, true);

                // Check if we need to load class database for type information
                bool needClassDatabase = false;

                // Try to check first assets file to see if type tree is enabled
                try
                {
                    var firstAsset = manager.LoadAssetsFileFromBundle(bundleInstance, 0, false);
                    if (firstAsset != null && firstAsset.file != null &&
                        firstAsset.file.Metadata != null && !firstAsset.file.Metadata.TypeTreeEnabled)
                    {
                        needClassDatabase = true;
                    }
                }
                catch
                {
                    // If we can't check, assume we might need it
                    needClassDatabase = true;
                }

                if (needClassDatabase)
                {
                    SetStatus("Loading class database...");
                    // Try to load class database from common locations
                    string[] possiblePaths = {
                        "classdata.tpk",
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "classdata.tpk"),
                        Path.Combine(Path.GetDirectoryName(path), "classdata.tpk")
                    };

                    bool loaded = false;
                    foreach (string classPath in possiblePaths)
                    {
                        if (File.Exists(classPath))
                        {
                            try
                            {
                                manager.LoadClassPackage(classPath);
                                SetStatus($"Loaded class DB from {Path.GetFileName(classPath)}");
                                loaded = true;
                                break;
                            }
                            catch
                            {
                                // Continue trying other paths
                            }
                        }
                    }

                    if (!loaded)
                    {
                        SetStatus("Warning: No class database found");
                    }
                }

                // Load all assets files and find TextAssets
                LoadAllAssetsFiles();
                FindAllTextAssets();

                // Populate the list box
                PopulateTextAssetsList();

                // Update UI state
                btnRefresh.Enabled = true;
                btnSave.Enabled = false; // No changes yet
                btnImportFile.Enabled = true; // Enable import button

                // Update window title
                Text = $"Ravenfield Lua Editor - {Path.GetFileName(path)}";
                SetStatus($"Loaded {textAssets.Count} text assets");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening bundle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Error opening bundle");
            }
        }

        private void LoadAllAssetsFiles()
        {
            try
            {
                string[] fileNames = bundleInstance.file.GetAllFileNames().ToArray();
                SetStatus($"Bundle contains {fileNames.Length} files");

                assetsFileInstances.Clear();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    try
                    {
                        string fileName = fileNames[i];
                        SetStatus($"Loading: {fileName}");

                        // Try to load each file as an assets file
                        AssetsFileInstance instance = manager.LoadAssetsFileFromBundle(bundleInstance, i, false);

                        if (instance != null && instance.file != null)
                        {
                            assetsFileInstances[i] = instance;
                        }
                    }
                    catch
                    {
                        // Skip files that can't be loaded
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files from bundle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FindAllTextAssets()
        {
            textAssets.Clear();

            foreach (var entry in assetsFileInstances)
            {
                try
                {
                    int fileIndex = entry.Key;
                    AssetsFileInstance instance = entry.Value;

                    if (instance?.file == null) continue;

                    // Get all TextAssets from the file
                    var fileTextAssets = instance.file.GetAssetsOfType(AssetClassID.TextAsset);
                    if (fileTextAssets == null) continue;

                    SetStatus($"Found {fileTextAssets.Count} TextAssets in file {fileIndex}");

                    foreach (var info in fileTextAssets)
                    {
                        try
                        {
                            if (info == null) continue;

                            var baseField = manager.GetBaseField(instance, info);
                            if (baseField == null) continue;

                            var nameField = baseField["m_Name"];
                            var scriptField = baseField["m_Script"];

                            if (nameField == null || scriptField == null) continue;

                            var name = nameField.AsString;
                            var script = scriptField.AsByteArray;

                            var textAssetInfo = new TextAssetInfo
                            {
                                FileIndex = fileIndex,
                                AssetInfo = info,
                                Name = name ?? "[Unnamed]",
                                Content = script ?? new byte[0],
                                AssetInstance = instance
                            };

                            textAssets.Add(textAssetInfo);
                        }
                        catch
                        {
                            // Skip assets that can't be processed
                        }
                    }
                }
                catch
                {
                    // Skip files that cause errors
                }
            }
        }

        private void PopulateTextAssetsList()
        {
            lstTextAssets.Items.Clear();

            // Sort TextAssets alphabetically by name
            var sortedAssets = textAssets.OrderBy(a => a.Name).ToList();
            textAssets = sortedAssets;

            foreach (var asset in textAssets)
            {
                lstTextAssets.Items.Add(asset.Name);
            }

            // Select the first item if available
            if (lstTextAssets.Items.Count > 0)
            {
                lstTextAssets.SelectedIndex = 0;
            }
        }

        private void lstTextAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if there are unsaved changes before switching
            if (contentModified && currentAsset != null)
            {
                var result = MessageBox.Show(
                    $"Save changes to {currentAsset.Name}?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    // Restore previous selection
                    lstTextAssets.SelectedIndexChanged -= lstTextAssets_SelectedIndexChanged;
                    lstTextAssets.SelectedIndex = textAssets.IndexOf(currentAsset);
                    lstTextAssets.SelectedIndexChanged += lstTextAssets_SelectedIndexChanged;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    // Save changes
                    SaveCurrentAsset();
                }
            }

            // Load selected TextAsset
            int selectedIndex = lstTextAssets.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < textAssets.Count)
            {
                contentModified = false;
                currentAsset = textAssets[selectedIndex];
                string content = GetTextAssetContent(currentAsset);

                // Update UI
                txtContent.Text = content;
                lblCurrentFile.Text = $"{currentAsset.Name} (File: {currentAsset.FileIndex}, Size: {currentAsset.Content.Length} bytes)";
                btnImportFile.Enabled = true; // Enable import button when an asset is selected
            }
            else
            {
                currentAsset = null;
                txtContent.Clear();
                lblCurrentFile.Text = "No file selected";
                btnImportFile.Enabled = false; // Disable import button when no asset is selected
            }
        }

        private string GetTextAssetContent(TextAssetInfo asset)
        {
            // Try to interpret as UTF-8 text
            try
            {
                return Encoding.UTF8.GetString(asset.Content);
            }
            catch
            {
                // If it fails, return a hex representation
                StringBuilder sb = new StringBuilder();
                foreach (byte b in asset.Content)
                {
                    sb.Append(b.ToString("X2") + " ");
                }
                return sb.ToString();
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            if (currentAsset != null)
            {
                contentModified = true;
                btnSave.Enabled = true;
            }
        }

        private void SaveCurrentAsset()
        {
            if (currentAsset == null || !contentModified) return;

            try
            {
                // Convert text content to bytes
                byte[] newContent = Encoding.UTF8.GetBytes(txtContent.Text);

                // Update the asset data
                UpdateTextAsset(currentAsset, newContent);

                // Update UI state
                contentModified = false;
                bundleModified = true;
                SetStatus($"Updated: {currentAsset.Name}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving asset: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTextAsset(TextAssetInfo asset, byte[] newContent)
        {
            // Get the base field for the asset
            var baseField = manager.GetBaseField(asset.AssetInstance, asset.AssetInfo);

            // Update the content
            baseField["m_Script"].AsByteArray = newContent;

            // Update the asset in the assets file
            asset.AssetInfo.SetNewData(baseField);

            // Update our local copy
            asset.Content = newContent;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Save the current asset if modified
            if (contentModified && currentAsset != null)
            {
                SaveCurrentAsset();
            }

            if (!bundleModified)
            {
                MessageBox.Show("No changes to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                string extension = Path.GetExtension(bundlePath);
                dialog.Filter = $"Unity Bundle Files|*{extension}|All Files|*.*";
                dialog.Title = "Save Bundle As";
                dialog.FileName = Path.GetFileNameWithoutExtension(bundlePath) + "_modified" + extension;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveBundle(dialog.FileName);
                }
            }
        }

        private void SaveBundle(string outputPath)
        {
            try
            {
                SetStatus("Saving bundle...");

                // For each modified assets file, update the bundle's directory info
                foreach (var entry in assetsFileInstances)
                {
                    int fileIndex = entry.Key;
                    AssetsFileInstance assetFileInst = entry.Value;

                    // Set the updated assets file data to the bundle
                    bundleInstance.file.BlockAndDirInfo.DirectoryInfos[fileIndex].SetNewData(assetFileInst.file);
                }

                // First write the uncompressed bundle
                string tempUncompressedPath = Path.GetTempFileName();
                using (AssetsFileWriter writer = new AssetsFileWriter(tempUncompressedPath))
                {
                    bundleInstance.file.Write(writer);
                }

                SetStatus("Compressing bundle...");

                // Now compress the bundle using LZ4 compression
                var uncompressedBundle = new AssetBundleFile();
                uncompressedBundle.Read(new AssetsFileReader(File.OpenRead(tempUncompressedPath)));

                using (AssetsFileWriter writer = new AssetsFileWriter(outputPath))
                {
                    uncompressedBundle.Pack(writer, AssetBundleCompressionType.LZ4);
                }

                uncompressedBundle.Close();

                // Delete the temporary uncompressed file
                try
                {
                    File.Delete(tempUncompressedPath);
                }
                catch
                {
                    // Ignore errors when deleting temp file
                }

                bundleModified = false;
                btnSave.Enabled = false;
                SetStatus($"Saved to: {Path.GetFileName(outputPath)}");

                MessageBox.Show($"Bundle saved successfully to:\n{outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bundle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Error saving bundle");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Check for unsaved changes
            if (contentModified && currentAsset != null)
            {
                var result = MessageBox.Show(
                    $"Save changes to {currentAsset.Name}?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveCurrentAsset();
                }
            }

            // Reopen the current bundle
            if (bundlePath != null)
            {
                OpenBundle(bundlePath);
            }
        }

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            if (currentAsset == null)
            {
                MessageBox.Show("Please select a TextAsset to replace first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Text Files|*.txt;*.lua;*.cs;*.js;*.json;*.xml|All Files|*.*";
                dialog.Title = "Import File Content";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read the file content
                        byte[] fileContent = File.ReadAllBytes(dialog.FileName);

                        // Update the TextAsset with the new content
                        UpdateTextAsset(currentAsset, fileContent);

                        // Update the text in the editor
                        txtContent.Text = GetTextAssetContent(currentAsset);

                        // Update UI state
                        bundleModified = true;
                        contentModified = false;
                        btnSave.Enabled = true;

                        SetStatus($"Imported file: {Path.GetFileName(dialog.FileName)} to {currentAsset.Name}");
                        MessageBox.Show($"Successfully imported '{Path.GetFileName(dialog.FileName)}' into TextAsset '{currentAsset.Name}'.",
                            "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SetStatus(string message)
        {
            // Update status label
            lblStatus.Text = message;
            Application.DoEvents();
        }

        private void CleanupResources()
        {
            // Clean up AssetsManager and related resources
            if (manager != null)
            {
                foreach (var instance in assetsFileInstances.Values)
                {
                    try
                    {
                        instance?.file?.Close();
                    }
                    catch
                    {
                        // Ignore errors during cleanup
                    }
                }

                assetsFileInstances.Clear();

                try
                {
                    bundleInstance?.file?.Close();
                }
                catch
                {
                    // Ignore errors during cleanup
                }

                bundleInstance = null;
                manager = null;
            }
        }
    }

    class TextAssetInfo
    {
        public int FileIndex { get; set; }
        public AssetFileInfo AssetInfo { get; set; }
        public AssetsFileInstance AssetInstance { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}