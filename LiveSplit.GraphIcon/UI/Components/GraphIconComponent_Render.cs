using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public partial class GraphIconComponent : IComponent
    {
        private GraphIconDedicatedWindow _graphIconDedicatedWindow;

        public void DrawGeneral(Graphics g, LiveSplitState state, float width, float height)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var oldMatrix = g.Transform;
            if (_settings.FlipGraph)
            {
                g.ScaleTransform(1, -1);
                g.TranslateTransform(0, -height);
            }
            DrawUnflipped(g, state, width, height);
            g.Transform = oldMatrix;
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (_settings.InAppGraphHeight != 0)
                DrawGeneral(g, state, width, VerticalHeight);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            if (_settings.InAppGraphWidth != 0)
                DrawGeneral(g, state, HorizontalWidth, height);
        }

        private void DrawUnflipped(Graphics g, LiveSplitState state, float width, float height)
        {
            var comparison = _settings.Comparison == "Current Comparison" ? state.CurrentComparison : _settings.Comparison;
            if (!state.Run.Comparisons.Contains(comparison))
                comparison = state.CurrentComparison;

            TimeSpan totalDelta = _minDelta - _maxDelta;

            float graphEdge, graphHeight, middle;
            CalculateMiddleAndGraphEdge(height, totalDelta, out graphEdge, out graphHeight, out middle);

            var brush = new SolidBrush(_settings.MainGraphColor);
            DrawGreenAndRedGraphPortions(g, width, graphHeight, middle, brush);

            double gridValueX, gridValueY;
            CalculateGridlines(state, width, totalDelta, graphEdge, graphHeight, out gridValueX, out gridValueY);

            var defaultLinePen = new Pen(_settings.GridlinesColor, 2.0f);
            var middleLinePen = new Pen(_settings.MiddleGridlineColor, 2.0f);
            DrawGridlines(g, width, graphHeight, middle, gridValueX, gridValueY, defaultLinePen, middleLinePen);

            try
            {
                DrawGraphIcon(g, state, width, comparison, totalDelta, graphEdge, graphHeight, middle, brush, defaultLinePen);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void DrawGreenAndRedGraphPortions(Graphics g, float width, float GraphHeight, float Middle, SolidBrush brush)
        {
            brush.Color = _settings.BehindGraphColor;
            g.FillRectangle(brush, 0, 0, width, Middle);
            brush.Color = _settings.AheadGraphColor;
            g.FillRectangle(brush, 0, Middle, width, GraphHeight * 2 - Middle);
        }

        private static void DrawGridlines(Graphics g, float width, float GraphHeight, float Middle, double gridValueX, double gridValueY, Pen defaultLinePen, Pen middleLinePen)
        {
            // Vertical lines
            if (gridValueX > 0)
            {
                for (double x = gridValueX; x < width; x += gridValueX)
                {
                    g.DrawLine(defaultLinePen,
                        (float)x, 0,
                        (float)x, GraphHeight * 2);
                }
            }

            // Above middle line
            for (float y = Middle - 1; y > 0; y -= (float)gridValueY)
            {
                g.DrawLine(defaultLinePen,
                    0, y,
                    width, y);

                if (gridValueY < 0)
                    break;
            }

            // Below middle line
            for (float y = Middle; y <= GraphHeight * 2; y += (float)gridValueY)
            {
                g.DrawLine(defaultLinePen,
                    0, y,
                    width, y);

                if (gridValueY < 0)
                    break;
            }

            // Middle line
            g.DrawLine(middleLinePen,
                    0, Middle,
                    width, Middle);
        }

        private void DrawGraphIcon(Graphics g, LiveSplitState state, float width, string comparison, TimeSpan TotalDelta, float graphEdge, float graphHeight, float middle, SolidBrush brush, Pen pen)
        {
            pen.Width = 1.75f;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            var iconList = new List<Tuple<PointF, Image>>();
            if (_deltas.Count > 0)
            {
                float heightOne = graphHeight;
                if (TotalDelta != TimeSpan.Zero)
                    heightOne = (float)(((-_maxDelta.TotalMilliseconds) / TotalDelta.TotalMilliseconds)
                        * (graphHeight - graphEdge) * 2 + graphEdge);
                float heightTwo = 0;
                float widthOne = 0;
                float widthTwo = 0;
                int y = 0;

                var pointArray = new List<PointF>();
                pointArray.Add(new PointF(0, middle));
                iconList.Add(new Tuple<PointF, Image>(new PointF(0, heightOne), null));

                while (y < _deltas.Count)
                {
                    while (_deltas[y] == null && y < _deltas.Count - 1)
                    {
                        y++;
                    }

                    if (_deltas[y] != null)
                    {
                        CalculateRightSideCoordinates(state, width, TotalDelta, graphEdge, graphHeight, ref heightTwo, ref widthTwo, y);
                        DrawFillBeneathGraph(g, TotalDelta, middle, brush, heightOne, heightTwo, widthOne, widthTwo, y, pointArray);
                        AddGraphNode(g, state, iconList, heightTwo, widthTwo, y);
                        CalculateLeftSideCoordinates(state, width, TotalDelta, graphEdge, graphHeight, ref heightOne, ref widthOne, y);
                    }
                    else
                    {
                        DrawFinalPolygon(g, middle, brush, pointArray);
                    }

                    y++;
                }

                DrawIconsAndLines(g, state, width, comparison, pen, brush, iconList);
            }
        }

        private void DrawFillBeneathGraph(Graphics g, TimeSpan TotalDelta, float Middle, SolidBrush brush, float heightOne, float heightTwo, float widthOne, float widthTwo, int y, List<PointF> pointArray)
        {
            if ((heightTwo - Middle) / (heightOne - Middle) > 0)
            {
                AddFillOneSide(g, Middle, brush, heightOne, heightTwo, widthOne, widthTwo, y, pointArray);
            }
            else
            {
                float ratio = (heightOne - Middle) / (heightOne - heightTwo);
                if (float.IsNaN(ratio))
                {
                    ratio = 0.0f;
                }
                AddFillFirstHalf(g, TotalDelta, Middle, brush, heightOne, widthOne, widthTwo, y, pointArray, ratio);
                AddFillSecondHalf(g, TotalDelta, Middle, brush, heightTwo, widthOne, widthTwo, y, pointArray, ratio);
            }

            if (y == _deltas.Count - 1)
            {
                DrawFinalPolygon(g, Middle, brush, pointArray);
            }
        }

        // Adds to the point array the fill under the graph if the current portion of the graph is either completely ahead or completely behind
        private void AddFillOneSide(Graphics g, float Middle, SolidBrush brush, float heightOne, float heightTwo, float widthOne, float widthTwo, int y, List<PointF> pointArray)
        {
            if (y == _deltas.Count - 1 && _isLiveDeltaActive)
            {
                brush.Color = heightTwo > Middle ? _settings.PartialFillColorAhead : _settings.PartialFillColorBehind;
                g.FillPolygon(brush, new PointF[]
                {
                     new PointF(widthOne, Middle),
                     new PointF(widthOne, heightOne),
                     new PointF(widthTwo, heightTwo),
                     new PointF(widthTwo, Middle)
                });
            }
            else
            {
                pointArray.Add(new PointF(widthTwo, heightTwo));
            }
        }

        // Adds to the point array the first portion of the fill if the graph goes from ahead to behind or vice versa
        private void AddFillFirstHalf(Graphics g, TimeSpan TotalDelta, float Middle, SolidBrush brush, float heightOne, float widthOne, float widthTwo, int y, List<PointF> pointArray, float ratio)
        {
            if (y == _deltas.Count - 1 && _isLiveDeltaActive)
            {
                brush.Color = heightOne > Middle ? _settings.PartialFillColorAhead : _settings.PartialFillColorBehind;
                if (TotalDelta != TimeSpan.Zero)
                {
                    g.FillPolygon(brush, new PointF[]
                    {
                        new PointF(widthOne, Middle),
                        new PointF(widthOne, heightOne),
                        new PointF(widthOne+(widthTwo-widthOne)*ratio, Middle)
                    });
                }
            }
            else
            {
                pointArray.Add(new PointF(widthOne + (widthTwo - widthOne) * ratio, Middle));
                brush.Color = heightOne > Middle ? _settings.CompleteFillColorAhead : _settings.CompleteFillColorBehind;
                g.FillPolygon(brush, pointArray.ToArray());
                brush.Color = heightOne > Middle ? _settings.CompleteFillColorAhead : _settings.CompleteFillColorBehind;
            }
        }

        // Adds to the point array the second portion of the fill if the graph goes from ahead to behind or vice versa
        private void AddFillSecondHalf(Graphics g, TimeSpan TotalDelta, float Middle, SolidBrush brush, float heightTwo, float widthOne, float widthTwo, int y, List<PointF> pointArray, float ratio)
        {
            if (y == _deltas.Count - 1 && _isLiveDeltaActive)
            {
                brush.Color = heightTwo > Middle ? _settings.PartialFillColorAhead : _settings.PartialFillColorBehind;
                if (TotalDelta != TimeSpan.Zero)
                {
                    g.FillPolygon(brush, new PointF[]
                    {
                        new PointF(widthOne+(widthTwo-widthOne)*ratio, Middle),
                        new PointF(widthTwo, heightTwo),
                        new PointF(widthTwo, Middle)
                    });
                }
            }
            else
            {
                brush.Color = heightTwo > Middle ? _settings.CompleteFillColorAhead : _settings.CompleteFillColorBehind;
                pointArray.Clear();
                pointArray.Add(new PointF(widthOne + (widthTwo - widthOne) * ratio, Middle));
                pointArray.Add(new PointF(widthTwo, heightTwo));
            }
        }

        private void DrawFinalPolygon(Graphics g, float Middle, SolidBrush brush, List<PointF> pointArray)
        {
            pointArray.Add(new PointF(pointArray.Last().X, Middle));
            if (pointArray.Count > 1)
            {
                brush.Color = pointArray[pointArray.Count - 2].Y > Middle ? _settings.CompleteFillColorAhead : _settings.CompleteFillColorBehind;
                g.FillPolygon(brush, pointArray.ToArray());
            }
        }

        private void AddGraphNode(Graphics g, LiveSplitState state, List<Tuple<PointF, Image>> circleList, float heightTwo, float widthTwo, int y)
        {
            if (y < state.Run.Count)
                circleList.Add(new Tuple<PointF, Image>(new PointF(widthTwo, heightTwo), state.Run[y].Icon));
            else
                circleList.Add(new Tuple<PointF, Image>(new PointF(widthTwo, heightTwo), null));
        }

        private void DrawIconsAndLines(Graphics g, LiveSplitState state, float width, string comparison, Pen pen, SolidBrush brush, List<Tuple<PointF, Image>> iconList)
        {
            int i = _deltas.Count - 1;

            iconList.Reverse();
            var previousCircle = iconList.FirstOrDefault();
            if (previousCircle != null)
                iconList.RemoveAt(0);

            foreach (var circle in iconList)
            {
                while (_deltas[i] == null)
                    i--;

                pen.Color = brush.Color = _settings.MainGraphColor;
                var finalDelta = previousCircle.Item1.X == width && _isLiveDeltaActive;
                if (!finalDelta && CheckBestSegment(state, i, state.CurrentTimingMethod))
                    pen.Color = brush.Color = _settings.GraphBestSegmentColor;

                DrawLineShadowed(g, pen, previousCircle.Item1.X, previousCircle.Item1.Y, circle.Item1.X, circle.Item1.Y, _settings.FlipGraph);
                if (!finalDelta)
                {
                    if (previousCircle.Item2 != null)
                        DrawIcon(g, pen, previousCircle.Item2, previousCircle.Item1.X - 2.5f, previousCircle.Item1.Y - 2.5f);
                    else
                        DrawEllipseShadowed(g, brush, previousCircle.Item1.X - 2.5f, previousCircle.Item1.Y - 2.5f, 5, 5, _settings.FlipGraph);
                }

                previousCircle = circle;
                i--;
            }
        }

        private void DrawLineShadowed(Graphics g, Pen pen, float x1, float y1, float x2, float y2, bool flipShadow)
        {
            var shadowPen = (Pen)pen.Clone();
            shadowPen.Color = _settings.ShadowsColor;
            if (!flipShadow)
            {
                g.DrawLine(shadowPen, x1 + 1, y1 + 1, x2 + 1, y2 + 1);
                g.DrawLine(shadowPen, x1 + 1, y1 + 2, x2 + 1, y2 + 2);
                g.DrawLine(shadowPen, x1 + 1, y1 + 3, x2 + 1, y2 + 3);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
            else
            {
                g.DrawLine(shadowPen, x1 + 1, y1 - 1, x2 + 1, y2 - 1);
                g.DrawLine(shadowPen, x1 + 1, y1 - 2, x2 + 1, y2 - 2);
                g.DrawLine(shadowPen, x1 + 1, y1 - 3, x2 + 1, y2 - 3);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private void DrawIcon(Graphics g, Pen pen, Image icon, float x, float y)
        {
            if (icon != null)
            {
                if(_settings.FlipGraph)
                {
                    g.DrawImage(icon, x - _settings.IconSize / 2, y + _settings.IconSize / 2, _settings.IconSize, -_settings.IconSize);
                }
                else
                {
                    g.DrawImage(icon, x - _settings.IconSize / 2, y - _settings.IconSize / 2, _settings.IconSize, _settings.IconSize);
                }               
            }
        }

        private void DrawEllipseShadowed(Graphics g, Brush brush, float x, float y, float width, float height, bool flipShadow)
        {
            var shadowBrush = new SolidBrush(_settings.ShadowsColor);

            if (!flipShadow)
            {
                g.FillEllipse(shadowBrush, x + 1, y + 1, width, height);
                g.FillEllipse(shadowBrush, x + 1, y + 2, width, height);
                g.FillEllipse(shadowBrush, x + 1, y + 3, width, height);
                g.FillEllipse(brush, x, y, width, height);
            }
            else
            {
                g.FillEllipse(shadowBrush, x + 1, y - 1, width, height);
                g.FillEllipse(shadowBrush, x + 1, y - 2, width, height);
                g.FillEllipse(shadowBrush, x + 1, y - 3, width, height);
                g.FillEllipse(brush, x, y, width, height);
            }
        }
    }
}
