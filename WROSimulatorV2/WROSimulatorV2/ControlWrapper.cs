using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class ControlWrapper
    {
        public Control Control { get; set; }
        public object Info { get; set; }
        public event EventHandler<InfoEventArgs> ValueChanged;
        public ControlWrapper(Control control, object info)
        {
            Control = control;
            Info = info;
        }
        public void Control_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(new InfoEventArgs(Info));
        }
        protected virtual void OnValueChanged(InfoEventArgs args)
        {
            EventHandler<InfoEventArgs> handler = ValueChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

    }
    public class InfoEventArgs : EventArgs
    {
        public object Info { get; set; }
        public InfoEventArgs(object info)
        {
            Info = info;
        }
    }
}
