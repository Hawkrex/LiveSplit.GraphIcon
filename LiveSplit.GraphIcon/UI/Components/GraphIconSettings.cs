using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class GraphIconSettings : UserControl, ICloneable
    {
        public float InAppGraphWidth { get; set; }
        public float InAppGraphWidthScaled { get { return InAppGraphWidth / 10; } set { InAppGraphWidth = value * 10; } }
        public float InAppGraphHeight { get; set; }
        public float InAppGraphHeightScaled { get { return InAppGraphHeight / 5; } set { InAppGraphHeight = value * 5; } }

        public int DedicatedWindowWidth { get; set; }
        public int DedicatedWindowHeight { get; set; }

        public string Comparison { get; set; }
        public int IconSize { get; set; }
        public Color BehindGraphColor { get; set; }
        public Color AheadGraphColor { get; set; }
        public Color GridlinesColor { get; set; }
        public Color MiddleGridlineColor { get; set; }
        public Color PartialFillColorBehind { get; set; }
        public Color CompleteFillColorBehind { get; set; }
        public Color PartialFillColorAhead { get; set; }
        public Color CompleteFillColorAhead { get; set; }
        public Color MainGraphColor { get; set; }
        public Color ShadowsColor { get; set; }
        public Color GraphSeparatorsColor { get; set; }
        public Color GraphBestSegmentColor { get; set; }

        public bool IsLiveGraph { get; set; }
        public bool FlipGraph { get; set; }
        public bool ShowBestSegments { get; set; }
        public bool DedicatedWindow { get; set; }

        public LiveSplitState CurrentState { get; set; }
        public LayoutMode Mode { get; set; }

        public event EventHandler<bool> DedicatedWindowStateChanged;

        private float _inAppGraphHeightMemory;

        public GraphIconSettings()
        {
            InitializeComponent();
            InAppGraphWidth = 120;
            InAppGraphHeight = 180;
            _inAppGraphHeightMemory = InAppGraphHeight;
            Comparison = "Current Comparison";
            IconSize = 16;
            BehindGraphColor = Color.FromArgb(115, 40, 40);
            AheadGraphColor = Color.FromArgb(40, 115, 52);
            GridlinesColor = Color.FromArgb(0x50, 0x0, 0x0, 0x0);
            MiddleGridlineColor = Color.FromArgb(0x50, 0x0, 0x0, 0x0);
            PartialFillColorBehind = Color.FromArgb(25, 255, 255, 255);
            CompleteFillColorBehind = Color.FromArgb(50, 255, 255, 255);
            PartialFillColorAhead = Color.FromArgb(25, 255, 255, 255);
            CompleteFillColorAhead = Color.FromArgb(50, 255, 255, 255);
            MainGraphColor = Color.White;
            ShadowsColor = Color.FromArgb(0x38, 0x0, 0x0, 0x0);
            GraphSeparatorsColor = Color.White;
            GraphBestSegmentColor = Color.FromArgb(216, 175, 31);
            IsLiveGraph = true;
            FlipGraph = false;
            ShowBestSegments = false;
            DedicatedWindow = false;

            cmbComparison.DataBindings.Add("SelectedItem", this, nameof(Comparison), false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.SelectedIndexChanged += cmbComparison_SelectedIndexChanged;
            nudIconSize.DataBindings.Add("Value", this, nameof(IconSize), false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindColor.DataBindings.Add("BackColor", this, nameof(BehindGraphColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadColor.DataBindings.Add("BackColor", this, nameof(AheadGraphColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnGridlinesColor.DataBindings.Add("BackColor", this, nameof(GridlinesColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnMiddleGridlineColor.DataBindings.Add("BackColor", this, nameof(MiddleGridlineColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnPartialColorBehind.DataBindings.Add("BackColor", this, nameof(PartialFillColorBehind), false, DataSourceUpdateMode.OnPropertyChanged);
            btnCompleteColorBehind.DataBindings.Add("BackColor", this, nameof(CompleteFillColorBehind), false, DataSourceUpdateMode.OnPropertyChanged);
            btnPartialColorAhead.DataBindings.Add("BackColor", this, nameof(PartialFillColorAhead), false, DataSourceUpdateMode.OnPropertyChanged);
            btnCompleteColorAhead.DataBindings.Add("BackColor", this, nameof(CompleteFillColorAhead), false, DataSourceUpdateMode.OnPropertyChanged);
            btnGraphColor.DataBindings.Add("BackColor", this, nameof(MainGraphColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnShadowsColor.DataBindings.Add("BackColor", this, nameof(ShadowsColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnSeparatorsColor.DataBindings.Add("BackColor", this, nameof(GraphSeparatorsColor), false, DataSourceUpdateMode.OnPropertyChanged);
            btnBestSegmentColor.DataBindings.Add("BackColor", this, nameof(GraphBestSegmentColor), false, DataSourceUpdateMode.OnPropertyChanged);
            chkLiveGraph.DataBindings.Add("Checked", this, nameof(IsLiveGraph), false, DataSourceUpdateMode.OnPropertyChanged);
            chkFlipGraph.DataBindings.Add("Checked", this, nameof(FlipGraph), false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowBestSegments.DataBindings.Add("Checked", this, nameof(ShowBestSegments), false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowBestSegments.CheckedChanged += chkShowBestSegments_CheckedChanged;
            chkDedicatedWindow.DataBindings.Add("Checked", this, nameof(DedicatedWindow), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public object Clone()
        {
            return new GraphIconSettings()
            {
                InAppGraphWidth = InAppGraphWidth,
                InAppGraphHeight = InAppGraphHeight,
                _inAppGraphHeightMemory = InAppGraphHeight,
                DedicatedWindowWidth = DedicatedWindowWidth,
                DedicatedWindowHeight = DedicatedWindowHeight,
                Comparison = Comparison,
                IconSize = IconSize,
                BehindGraphColor = BehindGraphColor,
                AheadGraphColor = AheadGraphColor,
                GridlinesColor = GridlinesColor,
                MiddleGridlineColor = MiddleGridlineColor,
                PartialFillColorBehind = PartialFillColorBehind,
                CompleteFillColorBehind = CompleteFillColorBehind,
                PartialFillColorAhead = PartialFillColorAhead,
                CompleteFillColorAhead = CompleteFillColorAhead,
                MainGraphColor = MainGraphColor,
                ShadowsColor = ShadowsColor,
                GraphSeparatorsColor = GraphSeparatorsColor,
                GraphBestSegmentColor = GraphBestSegmentColor,
                IsLiveGraph = IsLiveGraph,
                FlipGraph = FlipGraph,
                ShowBestSegments = ShowBestSegments,
                DedicatedWindow = DedicatedWindow
            };
        }

        void chkShowBestSegments_CheckedChanged(object sender, EventArgs e)
        {
            btnBestSegmentColor.Enabled = lblBestSegmentColor.Enabled = chkShowBestSegments.Checked;
        }
        void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison = cmbComparison.SelectedItem.ToString();
        }
        void GraphSettings_Load(object sender, EventArgs e)
        {
            cmbComparison.Items.Clear();
            cmbComparison.Items.Add("Current Comparison");
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);

            if (Mode == LayoutMode.Vertical)
            {
                trkInAppGraphHeight.DataBindings.Clear();
                InAppGraphHeightScaled = Math.Min(Math.Max(trkInAppGraphHeight.Minimum, InAppGraphHeightScaled), trkInAppGraphHeight.Maximum);
                trkInAppGraphHeight.DataBindings.Add("Value", this, nameof(InAppGraphHeightScaled), false, DataSourceUpdateMode.OnPropertyChanged);
                heightLabel.Text = "Height:";
            }
            else
            {
                trkInAppGraphHeight.DataBindings.Clear();
                InAppGraphHeightScaled = Math.Min(Math.Max(trkInAppGraphHeight.Minimum, InAppGraphHeightScaled), trkInAppGraphHeight.Maximum);
                trkInAppGraphHeight.DataBindings.Add("Value", this, nameof(InAppGraphWidthScaled), false, DataSourceUpdateMode.OnPropertyChanged);
                heightLabel.Text = "Width:";
            }
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);
            InAppGraphWidth = SettingsHelper.ParseFloat(element[nameof(InAppGraphWidth)]);
            InAppGraphHeight = SettingsHelper.ParseFloat(element[nameof(InAppGraphHeight)]);
            _inAppGraphHeightMemory = InAppGraphHeight;
            DedicatedWindowWidth = SettingsHelper.ParseInt(element[nameof(DedicatedWindowWidth)]);
            DedicatedWindowHeight = SettingsHelper.ParseInt(element[nameof(DedicatedWindowHeight)]);
            Comparison = SettingsHelper.ParseString(element[nameof(Comparison)], "Current Comparison");
            IconSize = SettingsHelper.ParseInt(element[nameof(IconSize)]);
            BehindGraphColor = SettingsHelper.ParseColor(element[nameof(BehindGraphColor)]);
            AheadGraphColor = SettingsHelper.ParseColor(element[nameof(AheadGraphColor)]);
            GridlinesColor = SettingsHelper.ParseColor(element[nameof(GridlinesColor)]);
            MiddleGridlineColor = SettingsHelper.ParseColor(element[nameof(MiddleGridlineColor)]);

            if (version >= new Version(1, 2))
            {
                PartialFillColorBehind = SettingsHelper.ParseColor(element[nameof(PartialFillColorBehind)]);
                CompleteFillColorBehind = SettingsHelper.ParseColor(element[nameof(CompleteFillColorBehind)]);
                PartialFillColorAhead = SettingsHelper.ParseColor(element[nameof(PartialFillColorAhead)]);
                CompleteFillColorAhead = SettingsHelper.ParseColor(element[nameof(CompleteFillColorAhead)]);
            }
            else
            {
                PartialFillColorAhead = SettingsHelper.ParseColor(element["PartialFillColor"]);
                PartialFillColorBehind = SettingsHelper.ParseColor(element["PartialFillColor"]);
                CompleteFillColorAhead = SettingsHelper.ParseColor(element["CompleteFillColor"]);
                CompleteFillColorBehind = SettingsHelper.ParseColor(element["CompleteFillColor"]);
            }

            MainGraphColor = SettingsHelper.ParseColor(element[nameof(MainGraphColor)]);
            ShadowsColor = SettingsHelper.ParseColor(element[nameof(ShadowsColor)]);
            GraphSeparatorsColor = SettingsHelper.ParseColor(element[nameof(GraphSeparatorsColor)]);
            GraphBestSegmentColor = SettingsHelper.ParseColor(element[nameof(GraphBestSegmentColor)], Color.Gold);
            IsLiveGraph = SettingsHelper.ParseBool(element[nameof(IsLiveGraph)]);
            FlipGraph = SettingsHelper.ParseBool(element[nameof(FlipGraph)], false);
            ShowBestSegments = SettingsHelper.ParseBool(element[nameof(ShowBestSegments)], false);
            DedicatedWindow = SettingsHelper.ParseBool(element[nameof(DedicatedWindow)], false);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.5") ^
            SettingsHelper.CreateSetting(document, parent, nameof(InAppGraphWidth), InAppGraphWidth) ^
            SettingsHelper.CreateSetting(document, parent, nameof(InAppGraphHeight), InAppGraphHeight) ^
            SettingsHelper.CreateSetting(document, parent, nameof(DedicatedWindowWidth), DedicatedWindowWidth) ^
            SettingsHelper.CreateSetting(document, parent, nameof(DedicatedWindowHeight), DedicatedWindowHeight) ^
            SettingsHelper.CreateSetting(document, parent, nameof(Comparison), Comparison) ^
            SettingsHelper.CreateSetting(document, parent, nameof(IconSize), IconSize) ^
            SettingsHelper.CreateSetting(document, parent, nameof(BehindGraphColor), BehindGraphColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(AheadGraphColor), AheadGraphColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(GridlinesColor), GridlinesColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(MiddleGridlineColor), MiddleGridlineColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(PartialFillColorBehind), PartialFillColorBehind) ^
            SettingsHelper.CreateSetting(document, parent, nameof(CompleteFillColorBehind), CompleteFillColorBehind) ^
            SettingsHelper.CreateSetting(document, parent, nameof(PartialFillColorAhead), PartialFillColorAhead) ^
            SettingsHelper.CreateSetting(document, parent, nameof(CompleteFillColorAhead), CompleteFillColorAhead) ^
            SettingsHelper.CreateSetting(document, parent, nameof(MainGraphColor), MainGraphColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(ShadowsColor), ShadowsColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(GraphSeparatorsColor), GraphSeparatorsColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(GraphBestSegmentColor), GraphBestSegmentColor) ^
            SettingsHelper.CreateSetting(document, parent, nameof(IsLiveGraph), IsLiveGraph) ^
            SettingsHelper.CreateSetting(document, parent, nameof(FlipGraph), FlipGraph) ^
            SettingsHelper.CreateSetting(document, parent, nameof(ShowBestSegments), ShowBestSegments) ^
            SettingsHelper.CreateSetting(document, parent, nameof(DedicatedWindow), DedicatedWindow);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void chkDedicatedWindow_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                trkInAppGraphHeight.Enabled = false;
            else
                trkInAppGraphHeight.Enabled = true;

            DedicatedWindowStateChanged?.Invoke(sender, ((CheckBox)sender).Checked);
        }

        public void HideInAppGraph(bool hideInAppGraph)
        {
            if (hideInAppGraph)
            {
                _inAppGraphHeightMemory = InAppGraphHeight;
                InAppGraphHeight = 0;
            }
            else
            {
                InAppGraphHeight = _inAppGraphHeightMemory;
            }
        }
    }
}
