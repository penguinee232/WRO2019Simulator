using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public enum TestEnum
    {
        One,
        Two,
        Three
    }
    public enum Motors
    {
        LeftDrive,
        RightDrive,
        Attachment1,
        Attachment2,
        Other
    }
    public enum MoveByMillisMode
    {
        LeftDriveMode,
        RightDiveMode,
        AverageMode
    }
    public enum Operatiors
    {
        Equals,
        LessThan,
        GreaterThan,
        NotEqual,
        LessThanEqual,
        GreaterThanEqual
    }
}
