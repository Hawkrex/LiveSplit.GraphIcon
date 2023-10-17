using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class GraphIconCompositeComponent : IComponent
    {
        protected GraphIconSettings Settings { get; set; }
        public ComponentRendererComponent InternalComponent { get; protected set; }

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public GraphIconCompositeComponent(LiveSplitState state)
        {
            Settings = new GraphIconSettings()
            {
                CurrentState = state
            };

            InternalComponent = new ComponentRendererComponent();

            var components = new List<IComponent>
            {
                new GraphIconSeparatorComponent(Settings) { LockToBottom = true },
                new GraphIconComponent(Settings),
                new GraphIconSeparatorComponent(Settings) { LockToBottom = false }
            };

            InternalComponent.VisibleComponents = components;
            state.ComparisonRenamed += StateComparisonRenamed;
        }

        private void StateComparisonRenamed(object sender, EventArgs e)
        {
            var args = (RenameEventArgs)e;
            if (Settings.Comparison == args.OldName)
            {
                Settings.Comparison = args.NewName;
                ((LiveSplitState)sender).Layout.HasChanged = true;
            }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public string ComponentName => "Graph Icon" + 
            (Settings.Comparison == "Current Comparison"
                ? string.Empty
                : $" ({CompositeComparisons.GetShortComparisonName(Settings.Comparison)})");

        public float HorizontalWidth => InternalComponent.HorizontalWidth;

        public float MinimumHeight => InternalComponent.MinimumHeight;

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public float VerticalHeight => InternalComponent.VerticalHeight;

        public float MinimumWidth => InternalComponent.MinimumWidth;

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (invalidator != null)
            {
                InternalComponent.Update(invalidator, state, width, height, mode);
            }
        }

        public void Dispose()
        {
            InternalComponent.Dispose();
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
