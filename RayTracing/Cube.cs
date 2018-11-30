using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Cube
    {
        public Cube(Point3D p0, double dx, double dy, double dz)
        {
            P0 = p0;
        }

        public Point3D P0 { get; set; }

        public double DX { get; set; }
        public double DY { get; set; }
        public double DZ { get; set; }
    }
}
