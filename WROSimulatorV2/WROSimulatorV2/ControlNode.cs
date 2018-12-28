using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{

    public class ControlNode
    {
        public ControlNode Parent { get; set; }
        public LabeledControl Control { get; set; }
        public List<ControlNode> Children { get; set; }
        public ControlNode(LabeledControl control, ControlNode parent)
        {
            control.ControlNode = this;
            Control = control;
            Children = new List<ControlNode>();
            Parent = parent;
        }
        public void ReLocateChildren(int spaceAmount)
        {
            if (Control.Control.GetType() == typeof(Panel) || Control.Control.GetType() == typeof(LabeledControl))
            {
                Size panelSize = new Size(0, 0);
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Control.Location = new Point(Children[i].Control.Location.X, panelSize.Height);
                    int padding = 0;
                    if (i + 1 < Children.Count)
                    {
                        padding = spaceAmount;
                    }
                    panelSize = new Size(Math.Max(panelSize.Width, Children[i].Control.Width), panelSize.Height + Children[i].Control.Height + padding);
                }
                Control.Control.Size = panelSize;
            }
            Control.SetSize();
            if (Parent != null)
            {
                Parent.ReLocateChildren(spaceAmount);
            }
        }
    }
}
