using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

        public static Point3D operator -(Point3D p1, Point3D p2) =>
            new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);

        public static Point3D operator +(Point3D p1, Point3D p2) =>
            new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);

        //scalar product of vectors from 0 to p1 and from 0 to p2
        public static double operator *(Point3D p1, Point3D p2) => p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;

        public static Point3D operator *(double d, Point3D p) => new Point3D(d * p.X, d * p.Y, d * p.Z);

        public static Point3D operator /(Point3D p, double d) => new Point3D(p.X / d, p.Y / d, p.Z / d);
    }
}
