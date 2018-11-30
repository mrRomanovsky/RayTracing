using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Sphere
    {
        //specular - зеркальность
        public Sphere (Point3D centre, double radius, Color color, int specular = -1, double reflective = 0)
        {
            Centre = centre;
            Radius = radius;
            Color = color;
            Specular = specular;
            Reflective = reflective;
        }

        public Point3D Centre { get; set; }
        public double Radius { get; set; }
        public Color Color { get; set; }
        public int Specular { get; set; }
        public double Reflective { get; set; }
    }
}
