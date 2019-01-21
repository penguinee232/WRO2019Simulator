using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public static class Pid
    {
        public static float GetPidMotorChange(float error, float previousError, ref float intergral, PidValues pidValues)
        {
            intergral += error;
            if (error == 0)
            {
                intergral = 0;
            }
            if (error > 20)
            {
                intergral = 0;
            }
            float deriviative = error - previousError;
            previousError = error;

            return error * pidValues.Kp + intergral * pidValues.Ki + deriviative * pidValues.Kd;
        }
        public struct PidValues
        {
            public float Kp;
            public float Ki;
            public float Kd;
            public PidValues(float kp, float ki, float kd)
            {
                Kp = kp;
                Ki = ki;
                Kd = kd;
            }
        }
    }
}
