using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class GraphIconComponent : IComponent
    {
        public string ComponentName => throw new NotImplementedException();

        public float HorizontalWidth => _settings.InAppGraphWidth;

        public float MinimumHeight => 20;

        public float VerticalHeight => _settings.InAppGraphHeight;

        public float MinimumWidth => 20;

        public float PaddingTop => 0f;

        public float PaddingBottom => 0f;

        public float PaddingLeft => 0f;

        public float PaddingRight => 0f;

        public IDictionary<string, Action> ContextMenuControls => throw new NotImplementedException();

        private List<TimeSpan?> _deltas;
        private TimeSpan _maxDelta;
        private TimeSpan _minDelta;
        private TimeSpan? _finalSplit;
        private bool _isLiveDeltaActive;

        private GraphicsCache _cache;

        private TimeSpan _graphEdgeValue;
        private const float _graphEdgeMin = 16;

        private GraphIconSettings _settings;
        private LiveSplitState _state;
        private bool _isFirstUpdate = true;

        public GraphIconComponent(GraphIconSettings settings)
        {
            _graphEdgeValue = new TimeSpan(0, 0, 0, 0, 200);
            _settings = settings;
            _cache = new GraphicsCache();
            _deltas = new List<TimeSpan?>();
            _finalSplit = TimeSpan.Zero;
            _maxDelta = TimeSpan.Zero;
            _minDelta = TimeSpan.Zero;

            settings.DedicatedWindowStateChanged += DedicatedWindowStateChanged;
        }

        public void Dispose()
        {
            if (_graphIconDedicatedWindow != null)
            {
                _graphIconDedicatedWindow.ResizeEnd -= GraphIconDedicatedWindow_ResizeEnd;
                _graphIconDedicatedWindow.Close();
                _graphIconDedicatedWindow = null;
            }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotImplementedException();
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            throw new NotImplementedException();
        }

        public void SetSettings(XmlNode settings)
        {
            throw new NotImplementedException();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (_isFirstUpdate)
            {
                _state = state;
                DedicatedWindowStateChanged(this, _settings.DedicatedWindow);
                _isFirstUpdate = false;
            }

            Calculate(state);

            _cache.Restart();
            _cache["FinalSplit"] = _finalSplit.ToString();
            _cache["IsLiveDeltaActive"] = _isLiveDeltaActive;
            _cache["DeltasCount"] = _deltas.Count;
            for (var ind = 0; ind < _deltas.Count; ind++)
            {
                _cache["Deltas" + ind] = _deltas[ind] == null ? "null" : _deltas[ind].ToString();
            }

            if (invalidator != null && _cache.HasChanged)
            {
                if (_graphIconDedicatedWindow != null)
                {
                    _graphIconDedicatedWindow.UpdateState(state);
                    _graphIconDedicatedWindow.Invalidate();
                }

                invalidator.Invalidate(0, 0, width, height);
            }
        }

        private void DedicatedWindowStateChanged(object sender, bool showDedicatedWindow)
        {
            if (showDedicatedWindow)
            {
                if (_graphIconDedicatedWindow == null)
                {
                    _graphIconDedicatedWindow = new GraphIconDedicatedWindow(this);
                    _graphIconDedicatedWindow.Width = _settings.DedicatedWindowWidth;
                    _graphIconDedicatedWindow.Height = _settings.DedicatedWindowHeight;
                    _graphIconDedicatedWindow.ResizeEnd += GraphIconDedicatedWindow_ResizeEnd;

                    _graphIconDedicatedWindow.Show();
                    _graphIconDedicatedWindow.UpdateState(_state);
                    _graphIconDedicatedWindow.Invalidate();
                }

                _settings.HideInAppGraph(true);
            }
            else
            {
                if (_graphIconDedicatedWindow != null)
                {
                    _graphIconDedicatedWindow.ResizeEnd -= GraphIconDedicatedWindow_ResizeEnd;
                    _graphIconDedicatedWindow.Close();
                    _graphIconDedicatedWindow = null;
                }

                _settings.HideInAppGraph(false);
            }
        }

        private void GraphIconDedicatedWindow_ResizeEnd(object sender, EventArgs e)
        {
            _settings.DedicatedWindowWidth = ((GraphIconDedicatedWindow)sender).Width;
            _settings.DedicatedWindowHeight = ((GraphIconDedicatedWindow)sender).Height;
        }

        private void CalculateMiddleAndGraphEdge(float height, TimeSpan totalDelta, out float graphEdge, out float graphHeight, out float middle)
        {
            graphEdge = 0;
            graphHeight = height / 2.0f;
            middle = graphHeight;
            if (totalDelta != TimeSpan.Zero)
            {
                graphEdge = (float)(_graphEdgeValue.TotalMilliseconds / (-totalDelta.TotalMilliseconds + _graphEdgeValue.TotalMilliseconds * 2) * (graphHeight * 2 - _graphEdgeMin * 2));
                graphEdge += _graphEdgeMin;
                middle = (float)(-(_maxDelta.TotalMilliseconds / totalDelta.TotalMilliseconds) * (graphHeight - graphEdge) * 2 + graphEdge);
            }
        }

        private void CalculateGridlines(LiveSplitState state, float width, TimeSpan totalDelta, float graphEdge, float graphHeight, out double gridValueX, out double gridValueY)
        {
            if (state.CurrentPhase != TimerPhase.NotRunning && _finalSplit > TimeSpan.Zero)
            {
                gridValueX = 1000;
                while (_finalSplit.Value.TotalMilliseconds / gridValueX > width / 20)
                {
                    gridValueX *= 6;
                }
                gridValueX = gridValueX / _finalSplit.Value.TotalMilliseconds * width;
            }
            else
            {
                gridValueX = -1;
            }

            if (state.CurrentPhase != TimerPhase.NotRunning && totalDelta < TimeSpan.Zero)
            {
                gridValueY = 1000;
                while ((-totalDelta.TotalMilliseconds) / gridValueY > (graphHeight - graphEdge) * 2 / 20)
                {
                    gridValueY *= 6;
                }
                gridValueY = gridValueY / (-totalDelta.TotalMilliseconds) * (graphHeight - graphEdge) * 2;
            }
            else
            {
                gridValueY = -1;
            }
        }

        private void CalculateRightSideCoordinates(LiveSplitState state, float width, TimeSpan TotalDelta, float graphEdge, float GraphHeight, ref float heightTwo, ref float widthTwo, int y)
        {
            if (y == _deltas.Count - 1 && _isLiveDeltaActive)
                widthTwo = width;
            else if (state.Run[y].SplitTime[state.CurrentTimingMethod] != null)
                widthTwo = (float)((state.Run[y].SplitTime[state.CurrentTimingMethod].Value.TotalMilliseconds / _finalSplit.Value.TotalMilliseconds) * (width));

            if (TotalDelta != TimeSpan.Zero)
                heightTwo = (float)((_deltas[y].Value.TotalMilliseconds - _maxDelta.TotalMilliseconds) / TotalDelta.TotalMilliseconds * (GraphHeight - graphEdge) * 2 + graphEdge);
            else
                heightTwo = GraphHeight;
        }

        private void CalculateLeftSideCoordinates(LiveSplitState state, float width, TimeSpan TotalDelta, float graphEdge, float GraphHeight, ref float heightOne, ref float widthOne, int y)
        {
            if (TotalDelta != TimeSpan.Zero)
                heightOne = (float)((_deltas[y].Value.TotalMilliseconds - _maxDelta.TotalMilliseconds) / TotalDelta.TotalMilliseconds)
                    * (GraphHeight - graphEdge) * 2 + graphEdge;
            else
                heightOne = GraphHeight;
            if (y != _deltas.Count - 1 && state.Run[y].SplitTime[state.CurrentTimingMethod] != null)
                widthOne = (float)((state.Run[y].SplitTime[state.CurrentTimingMethod].Value.TotalMilliseconds / _finalSplit.Value.TotalMilliseconds) * (width));
        }

        private void Calculate(LiveSplitState state)
        {
            var comparison = _settings.Comparison == "Current Comparison" ? state.CurrentComparison : _settings.Comparison;
            if (!state.Run.Comparisons.Contains(comparison))
                comparison = state.CurrentComparison;

            CalculateFinalSplit(state);
            CalculateDeltas(state, comparison);
            CheckLiveSegmentDelta(state, comparison);
        }

        private void CalculateFinalSplit(LiveSplitState state)
        {
            _finalSplit = TimeSpan.Zero;
            if (_settings.IsLiveGraph)
            {
                if (state.CurrentPhase != TimerPhase.NotRunning)
                    _finalSplit = state.CurrentTime[state.CurrentTimingMethod] ?? state.CurrentTime.RealTime;
            }
            else
            {
                foreach (var segment in state.Run)
                {
                    if (segment.SplitTime[state.CurrentTimingMethod] != null)
                        _finalSplit = segment.SplitTime[state.CurrentTimingMethod];
                }
            }
        }

        private void CalculateDeltas(LiveSplitState state, string comparison)
        {
            _deltas = new List<TimeSpan?>();
            _maxDelta = TimeSpan.Zero;
            _minDelta = TimeSpan.Zero;
            for (int x = 0; x < state.Run.Count; x++)
            {
                var time = state.Run[x].SplitTime[state.CurrentTimingMethod]
                        - state.Run[x].Comparisons[comparison][state.CurrentTimingMethod];
                if (time > _maxDelta)
                    _maxDelta = time.Value;
                if (time < _minDelta)
                    _minDelta = time.Value;
                _deltas.Add(time);
            }
        }

        private void CheckLiveSegmentDelta(LiveSplitState state, string comparison)
        {
            _isLiveDeltaActive = false;
            if (_settings.IsLiveGraph)
            {
                if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
                {
                    var bestSeg = LiveSplitStateHelper.CheckLiveDelta(state, true, comparison, state.CurrentTimingMethod);
                    var curSplit = state.Run[state.CurrentSplitIndex].Comparisons[comparison][state.CurrentTimingMethod];
                    var curTime = state.CurrentTime[state.CurrentTimingMethod];
                    if (bestSeg == null && curSplit != null && curTime - curSplit > _minDelta)
                    {
                        bestSeg = curTime - curSplit;
                    }
                    if (bestSeg != null)
                    {
                        if (bestSeg > _maxDelta)
                            _maxDelta = bestSeg.Value;
                        if (bestSeg < _minDelta)
                            _minDelta = bestSeg.Value;
                        _deltas.Add(bestSeg);
                        _isLiveDeltaActive = true;
                    }
                }
            }
        }

        private bool CheckBestSegment(LiveSplitState state, int splitNumber, TimingMethod method)
        {
            if (_settings.ShowBestSegments)
            {
                return LiveSplitStateHelper.CheckBestSegment(state, splitNumber, method);
            }
            return false;
        }
    }
}
