using LiveSplit.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public partial class GraphIconDedicatedWindow : Form
    {
        private GraphIconComponent _graphIcon;
        private LiveSplitState _state;

        public GraphIconDedicatedWindow(GraphIconComponent graphIcon)
        {
            _graphIcon = graphIcon;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();
        }

        private void GraphIconDedicatedWindow_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var clip = e.Graphics.Clip;
                e.Graphics.Clip = new Region();
                if (e.Graphics != null && _state != null)
                    _graphIcon.DrawGeneral(e.Graphics, _state, ClientSize.Width, ClientSize.Height);
            }
            catch (Exception)
            {
                Invalidate();
            }
        }

        public void UpdateState(LiveSplitState state)
        {
            _state = state;
        }
    }
}
