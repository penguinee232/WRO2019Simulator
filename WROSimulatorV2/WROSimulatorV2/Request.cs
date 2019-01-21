using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public abstract class Request
    {
        public Motors Motor { get; set; }
        public int Power { get; set; }
        public abstract void InitRequest(Robot robot);
        public abstract bool UpdateRequest(Robot robot, long elapsedMillis);//returns true to contrinue, false to stop
    }
}
