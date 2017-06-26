using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Learning
{
    [Serializable]
    public class DriveSnapShot
    {
        public float Tourgue;
        public float SteeringAngle;

        public float Speed;
        public float SensorLeft;
        public float SensorFront;
        public float SensorRight;
    }
}
