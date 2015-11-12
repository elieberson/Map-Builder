using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEASL.Core.Rendering;
using DEASL.Core.Mathematics;
using System.Drawing;
using System.Windows.Forms;

namespace NewMapEditor
{
    class ClickTool : IRenderTool
    {
        public Cursor currentCursor = Cursors.Cross;

        public event EventHandler<ClickedEventArgs> clicked;

        public void OnMouseDown(SubRenderer r, System.Windows.Forms.MouseEventArgs e)
        {
            if (clicked != null)
            {
                PointF p = r.WorldTransform.AbsoluteScreenToWorld(e.Location);
                clicked(this, new ClickedEventArgs(new Vector2(p.X, p.Y)));
            }
        }

        public Cursor Cursor
        {
            get { return currentCursor; }
        }

        #region not used
        public void OnMouseMove(SubRenderer r, MouseEventArgs eventArgs)
        {
        }

        public void OnMouseUp(SubRenderer r, MouseEventArgs eventArgs)
        {
        }

        public void OnMouseDoubleClick(SubRenderer r, MouseEventArgs eventArgs)
        {
        }

        public void OnMouseEnter(SubRenderer r, EventArgs eventArgs)
        {
        }

        public void OnMouseLeave(SubRenderer r, EventArgs eventArgs)
        {
        }

        public void OnMouseWheel(SubRenderer r, MouseEventArgs eventArgs)
        {
        }

        public void OnKeyDown(SubRenderer r, KeyEventArgs eventArgs)
        {
        }

        public void OnKeyUp(SubRenderer r, KeyEventArgs eventArgs)
        {
        }

        public void ClearMouseState()
        {
        }

        public void Draw(SubRenderer renderer)
        {
        }
        #endregion
    }

    public class ClickedEventArgs : EventArgs
    {
        readonly Vector2 V;

        public Vector2 Location
        {
            get { return V; }
        }

        public ClickedEventArgs(Vector2 Location)
        {
            V = Location;
        }
    }
}
