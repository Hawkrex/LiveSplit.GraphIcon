using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class GraphIconFactory : IComponentFactory
    {
        public string ComponentName => "Graph Icon";

        public string Description => "Shows a graph of the current run in relation to a comparison with icons from the splits.";

        public ComponentCategory Category => ComponentCategory.Media;

        public IComponent Create(LiveSplitState state) => new GraphIconCompositeComponent(state);

        public string UpdateName => ComponentName;

        public string XMLURL => "";

        public string UpdateURL => "";

        public Version Version => Version.Parse("1.0.0");
    }
}
