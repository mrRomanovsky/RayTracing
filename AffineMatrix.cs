using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    class AffineMatrix
    {
        private AffineMatrix()
        {
            matrix = new double[4, 4];
            for (int i = 0; i < 4; ++i)
            {
                matrix[i, i] = 1;
            }
        }

        public static AffineMatrix CreateTranslationMatrix(double x, double y, double z) 
        {
            AffineMatrix result = new AffineMatrix();
            result[0, 3] = x;
            result[1, 3] = y;
            result[2, 3] = z;
            return result;
        }

        public static AffineMatrix CreateXaxisRotationMatrix(double cos_phi, double sin_phi)
        {
            AffineMatrix result = new AffineMatrix();
            result[1, 1] = cos_phi;
            result[1, 2] = -sin_phi;
            result[2, 1] = sin_phi;
            result[2, 2] = cos_phi;
            return result;
        }

        public static AffineMatrix CreateXaxisRotationMatrix(double phi)
        {
            double cos_phi = Math.Cos(phi);
            double sin_phi = Math.Sin(phi);
            AffineMatrix result = new AffineMatrix();
            result[1, 1] = cos_phi;
            result[1, 2] = -sin_phi;
            result[2, 1] = sin_phi;
            result[2, 2] = cos_phi;
            return result;
        }

        public static AffineMatrix CreateYaxisRotationMatrix(double cos_phi, double sin_phi)
        {
            AffineMatrix result = new AffineMatrix();
            result[1, 1] = cos_phi;
            result[3, 1] = -sin_phi;
            result[1, 3] = sin_phi;
            result[3, 3] = cos_phi;
            return result;
        }

        public static AffineMatrix CreateYaxisRotationMatrix(double phi)
        {
            double cos_phi = Math.Cos(phi);
            double sin_phi = Math.Sin(phi);
            AffineMatrix result = new AffineMatrix();
            result[1, 1] = cos_phi;
            result[3, 1] = -sin_phi;
            result[1, 3] = sin_phi;
            result[3, 3] = cos_phi;
            return result;
        }
        
        public static AffineMatrix CreateZaxisRotationMatrix(double cos_phi, double sin_phi)
        {
            AffineMatrix result = new AffineMatrix();
            result[0, 0] = cos_phi;
            result[0, 1] = -sin_phi;
            result[1, 0] = sin_phi;
            result[1, 1] = cos_phi;
            return result;
        }

        public static AffineMatrix CreateZaxisRotationMatrix(double phi)
        {
            double cos_phi = Math.Cos(phi);
            double sin_phi = Math.Sin(phi);
            AffineMatrix result = new AffineMatrix();
            result[0, 0] = cos_phi;
            result[0, 1] = -sin_phi;
            result[1, 0] = sin_phi;
            result[1, 1] = cos_phi;
            return result;
        }

        public static AffineMatrix CreateRotationMatrix(Point3d a, Point3d b, double phi)
        {
            double m = b.X - a.X;
            double n = b.Y - a.Y;
            double p = b.Z - a.Z;
            double s = Math.Sqrt(m * m + n * n);
            double d = Math.Sqrt(m * m + n * n + p * p);

            AffineMatrix m0;
            if (Math.Abs(s) < 1e-6)
            {
                AffineMatrix m1 = AffineMatrix.CreateTranslationMatrix(-a.X, -a.Y, -a.Z);
                AffineMatrix m4 = AffineMatrix.CreateZaxisRotationMatrix(phi);
                AffineMatrix m7 = AffineMatrix.CreateTranslationMatrix(a.X, a.Y, a.Z);
                m0 = m7 * m4 * m1;

            }
            else
            {
                AffineMatrix m1 = AffineMatrix.CreateTranslationMatrix(-a.X, -a.Y, -a.Z);
                AffineMatrix m2 = AffineMatrix.CreateZaxisRotationMatrix(n / s, m / s);
                AffineMatrix m3 = AffineMatrix.CreateXaxisRotationMatrix(p / d, s / d);
                AffineMatrix m4 = AffineMatrix.CreateZaxisRotationMatrix(phi);
                AffineMatrix m5 = AffineMatrix.CreateXaxisRotationMatrix(p / d, -s / d);
                AffineMatrix m6 = AffineMatrix.CreateZaxisRotationMatrix(n / s, -m / s);
                AffineMatrix m7 = AffineMatrix.CreateTranslationMatrix(a.X, a.Y, a.Z);
                m0 = m7 * m6 * m5 * m4 * m3 * m2 * m1;
            }
            return m0;
        }

        public static AffineMatrix CreateScaleMatrix(double mx, double my, double mz)
        {
            AffineMatrix result = new AffineMatrix();
            result[0, 0] = mx;
            result[1, 1] = my;
            result[2, 2] = mz;
            return result;
        }

        public static AffineMatrix CreateReflectionMatrix(char axis)
        {
            AffineMatrix result = new AffineMatrix();
            switch (axis)
            {
                case 'x':
                case 'X':
                    result[0, 0] = -1;
                    break;
                case 'y':
                case 'Y':
                    result[1, 1] = -1;
                    break;
                case 'z':
                case 'Z':
                    result[2, 2] = -1;
                    break;
                default:
                    throw new Exception("Wrong char in reflection!");
            }
            return result;
        }

        public static AffineMatrix CreateIdentityMatrix()
        {
            return new AffineMatrix();
        }

        public static AffineMatrix CreateCentralProjectionMatrix(double a)
        {
            AffineMatrix result = new AffineMatrix();
            result[0, 0] = 0;
            result[3, 0] = -1.0 / a;
            return result;
        }

        public static AffineMatrix CreateIsometricProjectionMatrix()
        {
            AffineMatrix result = new AffineMatrix();
            result[1, 0] = -Math.Sqrt(3) / 2.0;
            result[1, 1] = -result[1, 0];
            result[2, 0] = result[2, 1] = -1.0 / 2.0;
            result[2, 2] = 1;
            result[0, 0] = 0;
            return result;
        }

        public static AffineMatrix CreateOrtographicProjectionMatrix(char axis)
        {
            AffineMatrix result = new AffineMatrix();
            switch (axis)
            {
                case 'x':
                case 'X':
                    result[0, 0] = 0;
                    break;
                case 'y':
                case 'Y':
                    result[0, 0] = 0;
                    result[1, 1] = 0;
                    result[2, 2] = 0;
                    result[1, 2] = 1;
                    result[2, 0] = 1;
                    break;
                case 'z':
                case 'Z':
                    result[0, 0] = 0;
                    result[1, 1] = 0;
                    result[2, 2] = 0;
                    result[1, 0] = 1;
                    result[2, 1] = 1;
                    break;
                default:
                    throw new Exception("Wrong char in ortographic projection!");
            }
            return result;
        }

        public static AffineMatrix CreateViewMatrix(Vector pos, Vector view, Vector hor, Vector vert)
        {
            AffineMatrix res = new AffineMatrix();
            res[0, 0] = vert.X;
            res[0, 1] = vert.Y;
            res[0, 2] = vert.Z;
            res[1, 0] = hor.X;
            res[1, 1] = hor.Y;
            res[1, 2] = hor.Z;
            res[2, 0] = view.X;
            res[2, 1] = view.Y;
            res[2, 2] = view.Z;
            res[3, 0] = res[3, 1] = res[3, 2] = 0;
            res[0, 3] = -(pos * vert);
            res[1, 3] = -(pos * hor);
            res[2, 3] = -(pos * view);
            res[3, 3] = 1;
            return res;
        }

        public static AffineMatrix operator *(AffineMatrix a, AffineMatrix b)
        {
            AffineMatrix result = new AffineMatrix();
            for (int i = 0; i < 4; ++i)
                result[i, i] = 0;
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    for (int k = 0; k < 4; ++k)
                        result[i, j] += a[i, k] * b[k, j];
            return result;
        }

        public static Point3d operator *(AffineMatrix m, Point3d vec)
        {
            double[] v = new double[4] { vec.X, vec.Y, vec.Z, 1 };
            double[] res = new double[4];
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    res[i] += m[i, j] * v[j];
            return new Point3d(res[0] / res[3], res[1] / res[3], res[2] / res[3]);
        }

        private double this[int i, int j]
        {
            get
            {
                return matrix[i, j];
            }
            set
            {
                matrix[i, j] = value;
            }
        }

        private double[,] matrix;
    }

    class Affine
    {
        public static void Execute(IFigure obj, AffineMatrix m)
        {
            if (obj is Polyhedron)
            {
                foreach (var x in (obj as Polyhedron).Faces)
                {
                    Execute(x, m);
                }
                Execute((obj as Polyhedron).Center, m);
            }
            else if (obj is Face)
            {
                foreach (var x in (obj as Face).Edges)
                {
                    Execute(x, m);
                }
            }
            else if (obj is Edge)
            {
                Execute((obj as Edge).First, m);
                Execute((obj as Edge).Second, m);
            }
            else if (obj is Point3d)
            {
                (obj as Point3d).Copy(m * (obj as Point3d));
            }
            else
            {
                throw new Exception("Unknown type");
            }
        }

        public static void Execute(Cam cam, AffineMatrix m)
        {
            Point3d o = m * new Point3d(cam.Pos.X, cam.Pos.Y, cam.Pos.Z);
            Point3d oX = m * new Point3d(cam.Pos.X + cam.View.X, cam.Pos.Y + cam.View.Y, cam.Pos.Z + cam.View.Z);
            Point3d oY = m * new Point3d(cam.Pos.X + cam.Hor.X,  cam.Pos.Y + cam.Hor.Y,  cam.Pos.Z + cam.Hor.Z);
            Point3d oZ = m * new Point3d(cam.Pos.X + cam.Vert.X, cam.Pos.Y + cam.Vert.Y, cam.Pos.Z + cam.Vert.Z);
            cam.SetCamByPoints(o, oX, oY, oZ);
        }

        public static void Translate(Polyhedron ph, Point3d a)
        {
            AffineMatrix m = AffineMatrix.CreateTranslationMatrix(a.X, a.Y, a.Z);
            Execute(ph, m);
        }

        public static void ScaleOverCenter(Polyhedron ph, double a) 
        {
            Point3d p = ph.Center.Clone() as Point3d;
            AffineMatrix m1 = AffineMatrix.CreateTranslationMatrix(-p.X, -p.Y, -p.Z);
            AffineMatrix m2 = AffineMatrix.CreateScaleMatrix(a, a, a);
            AffineMatrix m3 = AffineMatrix.CreateTranslationMatrix(p.X, p.Y, p.Z);
            AffineMatrix m = m3 * m2 * m1;
            Execute(ph, m);
        }

        public static void Reflect(Polyhedron ph, char axis)
        {
            AffineMatrix m = AffineMatrix.CreateReflectionMatrix(axis);
            Execute(ph, m);
        }

        public static void RotateOverCenter(Polyhedron ph, char axis, double phi) 
        {
            Point3d p = ph.Center.Clone() as Point3d;
            AffineMatrix m1 = AffineMatrix.CreateTranslationMatrix(-p.X, -p.Y, -p.Z);
            AffineMatrix m2;
            switch (axis)
            {
                case 'x':
                case 'X':
                    m2 = AffineMatrix.CreateXaxisRotationMatrix(phi);
                    break;
                case 'y':
                case 'Y':
                    m2 = AffineMatrix.CreateYaxisRotationMatrix(phi);
                    break;
                case 'z':
                case 'Z':
                    m2 = AffineMatrix.CreateZaxisRotationMatrix(phi);
                    break;
                default:
                    throw new Exception("Wrong char in rotation over center!");
            }
            AffineMatrix m3 = AffineMatrix.CreateTranslationMatrix(p.X, p.Y, p.Z);
            AffineMatrix m = m3 * m2 * m1;
            Execute(ph, m);
        }

        public static void RotateOverStreight(Polyhedron ph, Point3d a, Point3d b, double phi)
        {
            AffineMatrix m = AffineMatrix.CreateRotationMatrix(a, b, phi);
            Execute(ph, m);
        }

        public static void CentralProjection(Polyhedron ph, double c)
        {
            AffineMatrix m = AffineMatrix.CreateCentralProjectionMatrix(c);
            Execute(ph, m);
        }

        public static void IsometricProjection(Polyhedron ph)
        {
            AffineMatrix m = AffineMatrix.CreateIsometricProjectionMatrix();
            Execute(ph, m);
        }

        public static void OrtographicProjection(Polyhedron ph, char axis)
        {
            AffineMatrix m = AffineMatrix.CreateOrtographicProjectionMatrix(axis);
            Execute(ph, m);
        }

        public static void MakeView(Polyhedron ph, Cam cam)
        {
            AffineMatrix m = AffineMatrix.CreateViewMatrix(cam.Pos, cam.View, cam.Hor, cam.Vert);
            Execute(ph, m);
        }

        public static void RotateCam(Cam cam, double dist, double phi)
        {
            Vector toFixedPoint = cam.Pos - (cam.View * dist);
            Vector RotDirection = toFixedPoint + cam.Vert;
            Point3d fixedPoint = new Point3d(toFixedPoint.X, toFixedPoint.Y, toFixedPoint.Z);
            Point3d rotDir = new Point3d(RotDirection.X, RotDirection.Y, RotDirection.Z);

            AffineMatrix m = AffineMatrix.CreateRotationMatrix(fixedPoint, rotDir, phi);
            Execute(cam, m);
        }
    }
}
