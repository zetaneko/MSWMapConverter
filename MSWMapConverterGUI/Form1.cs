using MSWMapConverterLib;
using System.Configuration;
using System.Text.Json;
using System.Xml.Linq;

namespace MSWMapConverterGUI
{
    public partial class Form1 : Form
    {
        private MapConverter converter;
        private const string ConfigFilePath = "config.json";
        private Dictionary<string, Dictionary<string, string>> regionToMapsDict = new Dictionary<string, Dictionary<string, string>>();

        public Form1()
        {
            InitializeComponent();
            converter = new MapConverter();
            LoadConfiguration();
            InitializeRegions();
        }

        private void InitializeRegions()
        {
            LoadRegionsFromXml();

            regionCB.Items.Clear();
            foreach (var regionName in regionToMapsDict.Keys)
            {
                regionCB.Items.Add(regionName);
            }

            if (regionCB.Items.Count > 0)
            {
                regionCB.SelectedIndex = 0;
            }
        }

        private void LoadRegionsFromXml()
        {
            string xmlFilePath = Path.Combine(WZFolderPathTB.Text, "String.wz", "Map.img.xml");

            if (!File.Exists(xmlFilePath))
            {

                logDetails.AppendText("Map.img.xml not found. Please specify WZ folder path with correct contents." + Environment.NewLine);
                logDetails.SelectionStart = logDetails.Text.Length;
                logDetails.ScrollToCaret();

                return;
            }

            regionToMapsDict.Clear();
            XDocument doc = XDocument.Load(xmlFilePath);
            var regionElements = doc.Descendants("imgdir").First().Elements("imgdir");

            foreach (var regionElement in regionElements)
            {
                var regionName = regionElement.Attribute("name")?.Value;
                if (!string.IsNullOrEmpty(regionName))
                {
                    var mapsDict = new Dictionary<string, string>();
                    var mapElements = regionElement.Elements("imgdir");
                    foreach (var mapElement in mapElements)
                    {
                        var idAttribute = mapElement.Attribute("name");
                        if (idAttribute != null)
                        {
                            string mapId = idAttribute.Value;
                            var mapNameElement = mapElement.Descendants("string")
                                .FirstOrDefault(e => e.Attribute("name")?.Value == "mapName");

                            if (mapNameElement != null)
                            {
                                string mapName = mapNameElement.Attribute("value")?.Value;
                                mapsDict[mapId] = mapName ?? mapId;
                            }
                        }
                    }
                    regionToMapsDict[regionName] = mapsDict;
                }
            }
        }

        private void mswPathBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Show the dialog and check if the user clicked OK
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string requiredSubfolder = "collisiongroupset";

                    // Check if the required subfolder exists in the selected directory
                    if (Directory.Exists(Path.Combine(selectedPath, requiredSubfolder)))
                    {
                        logDetails.AppendText("MSW Folder contents validated." + Environment.NewLine);
                        logDetails.SelectionStart = logDetails.Text.Length;
                        logDetails.ScrollToCaret();
                        mswPathTB.Text = selectedPath; // Set the TextBox text to the full path of the folder
                        SaveConfiguration();
                    }
                    else
                    {
                        // Display an error dialog if the subfolder is not found
                        MessageBox.Show(
                            "Please select the folder where the MapleStory Worlds extracted mod contents are.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void WZFolderPathBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Show the dialog and check if the user clicked OK
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string requiredSubfolder = "Map.wz";
                    string requiredSubfolder2 = "String.wz";

                    // Check if the required subfolder exists in the selected directory
                    if (Directory.Exists(Path.Combine(selectedPath, requiredSubfolder)) && Directory.Exists(Path.Combine(selectedPath, requiredSubfolder2)))
                    {
                        logDetails.AppendText("WZ Folder contents validated." + Environment.NewLine);
                        logDetails.SelectionStart = logDetails.Text.Length;
                        logDetails.ScrollToCaret();
                        WZFolderPathTB.Text = selectedPath; // Set the TextBox text to the full path of the folder
                        SaveConfiguration();
                        InitializeRegions();
                    }
                    else
                    {
                        // Display an error dialog if the subfolder is not found
                        MessageBox.Show(
                            "Please select the folder where the MapleStory extracted WZ contents are, containing atleast 'Map.wz' and 'String.wz'.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveConfiguration()
        {
            Configuration config = new Configuration { SelectedMSWPath = mswPathTB.Text, SelectedWZPath = WZFolderPathTB.Text };
            string json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigFilePath, json);
        }

        public void LoadConfiguration()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    Configuration config = JsonSerializer.Deserialize<Configuration>(json);

                    if (config != null && Directory.Exists(config.SelectedMSWPath))
                    {
                        mswPathTB.Text = config.SelectedMSWPath;
                    }

                    if (config != null && Directory.Exists(config.SelectedWZPath))
                    {
                        WZFolderPathTB.Text = config.SelectedWZPath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WZFolderPathSet(object sender, EventArgs e)
        {
            // Re-populate the list of regions and maps
            var regions = converter.GetListsOfRegionIds(WZFolderPathTB.Text);
            foreach (var region in regions)
            {
                regionCB.Items.Add(region);
            }

            regionCB.SelectedIndex = 0;
        }

        private void RegionSelected(object sender, EventArgs e)
        {
            UpdateMapListBox();
        }

        private void MapSelected(object sender, EventArgs e)
        {
            convertBtn.Enabled = true;
        }

        private async void convertBtn_Click(object sender, EventArgs e)
        {
            int totalItems = MapLB.SelectedItems.Count;
            progressBar1.Maximum = totalItems;
            progressBar1.Value = 0;

            // Disable UI elements for the duration of the conversion
            convertBtn.Enabled = false;
            WZFolderPathBtn.Enabled = false;
            mswPathBtn.Enabled = false;
            regionCB.Enabled = false;
            MapLB.Enabled = false;

            var i = 0;

            foreach (ListBoxItem selectedItem in MapLB.SelectedItems)
            {
                i++;
                conversionStatusLabel.Text = $"Converting {i} / {MapLB.SelectedItems.Count}: {selectedItem.DisplayName}";
                int regionCode = GetFirstDigit(selectedItem.Id);  // Use the first character of mapId as regionCode
                                                                  // Run the conversion in a separate thread
                await Task.Run(() =>
                {
                    try
                    {
                        // Perform the conversion
                        converter.Convert(regionCode, selectedItem.Id, WZFolderPathTB.Text, mswPathTB.Text);
                    }
                    catch (Exception ex)
                    {
                        // Log error to console for debugging
                        Console.WriteLine(ex.ToString());

                        // Append error to RichTextBox and ensure scrolling
                        logDetails.Invoke((MethodInvoker)(() =>
                        {
                            logDetails.AppendText(ex.ToString() + Environment.NewLine);
                            logDetails.SelectionStart = logDetails.Text.Length;
                            logDetails.ScrollToCaret();
                        }));
                    }
                });

                // Update progress
                progressBar1.Value += 1;
            }

            conversionStatusLabel.Text = "Conversion Complete!";
            logDetails.AppendText("All selected items have been converted." + Environment.NewLine);
            logDetails.SelectionStart = logDetails.Text.Length;
            logDetails.ScrollToCaret();

            // Re-enable UI elements after conversion is complete
            convertBtn.Enabled = true;
            WZFolderPathBtn.Enabled = true;
            mswPathBtn.Enabled = true;
            regionCB.Enabled = true;
            MapLB.Enabled = true;
        }

        private int GetFirstDigit(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("The input string cannot be null or empty.", nameof(input));
            }

            char firstChar = input[0];

            // Ensure the first character is a digit and convert it to an integer
            if (char.IsDigit(firstChar))
            {
                return firstChar - '0'; // Convert the character to its integer representation
            }
            else
            {
                throw new FormatException("The first character is not a digit.");
            }
        }

        private void UpdateMapListBox()
        {
            MapLB.Items.Clear();

            if (regionCB.SelectedItem is string selectedRegion && regionToMapsDict.TryGetValue(selectedRegion, out var maps))
            {
                foreach (var kvp in maps)
                {
                    MapLB.Items.Add(new ListBoxItem { Id = kvp.Key, DisplayName = kvp.Value });
                }

                MapLB.DisplayMember = "DisplayName";
            }
        }

    }

    public class Configuration
    {
        public string SelectedMSWPath { get; set; }
        public string SelectedWZPath { get; set; }
    }

    public class ListBoxItem
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class ComboBoxItem
    {
        public int Index { get; set; }
        public string DisplayName { get; set; }

        public override string ToString() => DisplayName;
    }
}