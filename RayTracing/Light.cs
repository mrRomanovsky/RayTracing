using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public enum LightType { Ambient, Point, Directional};

    public class Light
    {
        public Light(LightType type, double intensity)
        {
            Type = type;
            Intensity = intensity;
        }

        public Light(LightType type, double intensity, Point3D position)
        {
            Type = type;
            Intensity = intensity;
            Position = position;
        }

        public LightType Type { get; set; }
        public double Intensity { get; set; }
        public Point3D Position { get; set; }
        public Point3D Direction { get; set; }
    }
}
