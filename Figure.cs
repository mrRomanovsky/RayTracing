using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    interface IFigure
    {
        IFigure Clone();
        
        void Draw(Graphics g, Color c);
    }

    class Point3d : IFigure
    {
        public Point3d()
        {
            /* EMPTY */
        }
        
        public Point3d(double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

        public Point3d(Point3d a)
        {
            x = a.x;
            y = a.y;
            z = a.z;
        }

        public void Copy(Point3d a)
        {
            x = a.x;
            y = a.y;
            z = a.z;
        }

        public Point To2d()
        {
            int xp = (int)Math.Round(Point0.X + pixelsPerUnit * y);
            int yp = (int)Math.Round(Point0.Y - pixelsPerUnit * z);

            return new Point(xp, yp);
        }
        
        protected double x, y, z;

        public double X
        {
            get
            {
                return x;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
        }

        public double Z
        {
            get
            {
                return z;
            }
        }

        public IFigure Clone()
        {
            return new Point3d(this);
        }

        public void Draw(Graphics g, Color c) { /* EMPTY */ }
        
        static Point Point0 = new Point(200, 170);

        public static void DrawPoint0(Graphics g, Color c)
        {
            SolidBrush b = new SolidBrush(c);
            g.FillEllipse(b, Point0.X - 2, Point0.Y - 2, 5, 5);
        }

        int pixelsPerUnit = 40;
    }

    class Edge : IFigure
    {
        public Edge ()
        {
            /*empty*/
        }
        
        public Edge(Point3d a, Point3d b)
        {
            this.a = new Point3d(a); 
            this.b = new Point3d(b);
        }

        public Point3d First
        {
            get
            {
                return a;
            }
        }

        public Point3d Second
        {
            get
            {
                return b;
            }
        }

        public IFigure Clone()
        {
            return new Edge(a.Clone() as Point3d, b.Clone() as Point3d);
        }

        public void Draw(Graphics g, Color c)
        {
            Pen p = new Pen(c);
            g.DrawLine(p, a.To2d(), b.To2d());
            p.Dispose();
        }

        private Point3d a, b;
    }

    class Face : IFigure
    {
        private Face()
        {
            /*EMPTY*/
        }

        public Face(Point3d[] verteces)
        {
            edges = new Edge[verteces.Length];
            for (int i = 1; i < verteces.Length; i++)
            {
                edges[i - 1] = new Edge(verteces[i - 1], verteces[i]);
            }
            edges[verteces.Length - 1] = new Edge(verteces[verteces.Length - 1], verteces[0]);
        }

        private Edge[] edges;

        public Edge[] Edges
        {
            get
            {
                return edges;
            }
        }

        public byte Color;

        public static Face CreateTriangle(Point3d a, Point3d b, Point3d c)
        {
            Face Triangle = new Face();
            Triangle.edges = new Edge[3];
            
            Triangle.edges[0] = new Edge(a, b);
            Triangle.edges[1] = new Edge(b, c);
            Triangle.edges[2] = new Edge(c, a);

            return Triangle;
        }

        public static Face CreateSquare(Point3d a, Point3d b, Point3d c, Point3d d)
        {
            Face Square = new Face();
            Square.edges = new Edge[4];
            
            Square.edges[0] = new Edge(a, b);
            Square.edges[1] = new Edge(b, c);
            Square.edges[2] = new Edge(c, d);
            Square.edges[3] = new Edge(d, a);

            return Square;
        }
        
        public static Face CreatePentagon(Point3d a, Point3d b, Point3d c, Point3d d, Point3d e)
        {
            Face Pentagon = new Face();
            Pentagon.edges = new Edge[5];

            Pentagon.edges[0] = new Edge(a, b);
            Pentagon.edges[1] = new Edge(b, c);
            Pentagon.edges[2] = new Edge(c, d);
            Pentagon.edges[3] = new Edge(d, e);
            Pentagon.edges[4] = new Edge(e, a);

            return Pentagon;
        }
        
        public Point3d[] GetPoints()
        {
            Point3d[] res = new Point3d[edges.Length];
            for (int i = 0; i < res.Length; ++i)
            {
                res[i] = edges[i].First;
            }
            return res;
        }
        
        public IFigure Clone()
        {
            Face res = new Face();
            res.edges = new Edge[edges.Length];
            for (int i = 0; i < edges.Length; ++i)
            {
                res.edges[i] = edges[i].Clone() as Edge;
            }
            res.Color = Color;
            return res;
        }

        public void Draw(Graphics g, Color c)
        {
            foreach (var x in Edges)
            {
                x.Draw(g, c);
            }
        }
    }

    class Polyhedron : IFigure
    {
        private Polyhedron()
        {
            /* EMPTY */
        }

        public Polyhedron(Face[] _faces, Point3d _center)
        {
            faces = new Face[_faces.Length];
            for (int i = 0; i < faces.Length; ++i)
            {
                faces[i] = _faces[i].Clone() as Face;
            }
            center = _center.Clone() as Point3d;
        }

        private Face[] faces;

        private Point3d center;

        public Face[] Faces
        {
            get 
            {
                return faces;
            }    
        }

        public Point3d Center
        {
            get
            {
                return center;
            }
        }

        private bool colorized = false;

        public bool Colorized
        {
            get
            {
                return colorized;
            }
        }

        public static Polyhedron CreateTetrahedron(Point3d a, Point3d b, Point3d q)
        {
            Point3d c = GetThirdVertexForTriangle(a, b, q);
            Vector m = new Vector(a, b);
            Vector n = new Vector(a, c);
            double sideLen = m.Norm();
            double hLen = sideLen * Math.Sqrt(6) / 3.0;
            Vector h = m[n].Normalize() * hLen;
            Vector r = h + ((m + n) * (1.0 / 3.0));
            Point3d d = new Point3d(a.X + r.X, a.Y + r.Y, a.Z + r.Z);

            Polyhedron Tetrahedron = new Polyhedron();
            Tetrahedron.faces = new Face[4];
            Tetrahedron.faces[0] = Face.CreateTriangle(a, b, c);
            Tetrahedron.faces[1] = Face.CreateTriangle(a, b, d);
            Tetrahedron.faces[2] = Face.CreateTriangle(a, c, d);
            Tetrahedron.faces[3] = Face.CreateTriangle(b, c, d);

            double cx = (a.X + b.X + c.X + d.X) / 4;
            double cy = (a.Y + b.Y + c.Y + d.Y) / 4;
            double cz = (a.Z + b.Z + c.Z + d.Z) / 4;
            Tetrahedron.center = new Point3d(cx, cy, cz);

            return Tetrahedron;
        }

        public static Polyhedron CreateHexahedron(Point3d a, Point3d b, Point3d q)
        {
            Point3d[] cd = GetVertecesForSquare(a, b, q);
            Point3d c = cd[0];
            Point3d d = cd[1];
            Vector m = new Vector(a, b);
            Vector n = new Vector(a, c);
            double sideLen = m.Norm();            
            Vector h = m[n].Normalize() * sideLen;
            Point3d a1 = new Point3d(a.X + h.X, a.Y + h.Y, a.Z + h.Z);
            Point3d b1 = new Point3d(b.X + h.X, b.Y + h.Y, b.Z + h.Z);
            Point3d c1 = new Point3d(c.X + h.X, c.Y + h.Y, c.Z + h.Z);
            Point3d d1 = new Point3d(d.X + h.X, d.Y + h.Y, d.Z + h.Z);

            Polyhedron Hexahedron = new Polyhedron();
            Hexahedron.faces = new Face[6];
            Hexahedron.faces[0] = Face.CreateSquare(a, b, c, d);
            Hexahedron.faces[1] = Face.CreateSquare(a, b, b1, a1);
            Hexahedron.faces[2] = Face.CreateSquare(b, c, c1, b1);
            Hexahedron.faces[3] = Face.CreateSquare(c, d, d1, c1);
            Hexahedron.faces[4] = Face.CreateSquare(a, d, d1, a1);
            Hexahedron.faces[5] = Face.CreateSquare(a1, b1, c1, d1);

            double cx = (a.X + c1.X) / 2;
            double cy = (a.Y + c1.Y) / 2;
            double cz = (a.Z + c1.Z) / 2;
            Hexahedron.center = new Point3d(cx, cy, cz);

            return Hexahedron;
        }

        public static Polyhedron CreateOktahedron(Point3d a, Point3d b, Point3d q)
        {
            Point3d[] cd = GetVertecesForSquare(a, b, q);
            Point3d c = cd[0];
            Point3d d = cd[1];
            Vector m = new Vector(a, b);
            Vector n = new Vector(a, d);
            double sideLen = m.Norm();
            double hLen = sideLen / Math.Sqrt(2);
            Vector h = m[n].Normalize() * hLen;
            Vector r = h + ((m + n) * 0.5);
            Point3d top = new Point3d(a.X + r.X, a.Y + r.Y, a.Z + r.Z);
            Point3d bottom = new Point3d(c.X - r.X, c.Y - r.Y, c.Z - r.Z);
            
            Polyhedron Oktahedron = new Polyhedron();
            Oktahedron.faces = new Face[8];
            Oktahedron.faces[0] = Face.CreateTriangle(a, b, top);
            Oktahedron.faces[1] = Face.CreateTriangle(b, c, top);
            Oktahedron.faces[2] = Face.CreateTriangle(c, d, top);
            Oktahedron.faces[3] = Face.CreateTriangle(d, a, top);
            Oktahedron.faces[4] = Face.CreateTriangle(a, b, bottom);
            Oktahedron.faces[5] = Face.CreateTriangle(b, c, bottom);
            Oktahedron.faces[6] = Face.CreateTriangle(c, d, bottom);
            Oktahedron.faces[7] = Face.CreateTriangle(d, a, bottom);

            double cx = (a.X + c.X) / 2;
            double cy = (a.Y + c.Y) / 2;
            double cz = (a.Z + c.Z) / 2;
            Oktahedron.center = new Point3d(cx, cy, cz);

            return Oktahedron;
        }

        public static Polyhedron CreateIcosahedron()
        {
            Polyhedron Icosahedron = new Polyhedron();
            Point3d[] verts = new Point3d[12];

            // верх и низ
            verts[0] = new Point3d(0, 0, 1);
            verts[1] = new Point3d(0, 0, -1);

            double h = Math.Sqrt(5) / 5;
            double R = h / Math.Sin(Math.PI / 5);

            // верхний круг
            double angle = 0;
            for (int i = 2; i < 7; ++i)
            {
                verts[i] = new Point3d(R * Math.Sin(angle), R * Math.Cos(angle), h);
                angle += 72 * Math.PI / 180;
            }

            // нижний круг
            angle = 36 * Math.PI / 180;
            for (int i = 7; i < 12; ++i)
            {
                verts[i] = new Point3d(R * Math.Sin(angle), R * Math.Cos(angle), -h);
                angle += 72 * Math.PI / 180;
            }

            Icosahedron.faces = new Face[20];
            for (int i = 0; i < 4; ++i)
            {
                Icosahedron.faces[2 * i] = Face.CreateTriangle(verts[0], verts[2 + i], verts[3 + i]);
                Icosahedron.faces[2 * i + 1] = Face.CreateTriangle(verts[1], verts[7 + i], verts[8 + i]);
            }
            Icosahedron.faces[8] = Face.CreateTriangle(verts[0], verts[2], verts[6]);
            Icosahedron.faces[9] = Face.CreateTriangle(verts[1], verts[7], verts[11]);

            for (int i = 0; i < 4; ++i)
            {
                Icosahedron.faces[10 + 2 * i] = Face.CreateTriangle(verts[2 + i], verts[3 + i], verts[7 + i]);
                Icosahedron.faces[11 + 2 * i] = Face.CreateTriangle(verts[7 + i], verts[8 + i], verts[3 + i]);
            }
            Icosahedron.faces[18] = Face.CreateTriangle(verts[6], verts[2], verts[11]);
            Icosahedron.faces[19] = Face.CreateTriangle(verts[11], verts[7], verts[2]);

            Icosahedron.center = new Point3d(0, 0, 0);
            return Icosahedron;
        }

        public static Polyhedron CreateDodecahedron()
        {
            Polyhedron Icosahedron = CreateIcosahedron();
            Point3d[] verts = new Point3d[20];
            for (int i = 0; i < 20; ++i)
            {
                double x = (Icosahedron.faces[i].Edges[0].First.X + Icosahedron.faces[i].Edges[1].First.X + Icosahedron.faces[i].Edges[2].First.X) / 3;
                double y = (Icosahedron.faces[i].Edges[0].First.Y + Icosahedron.faces[i].Edges[1].First.Y + Icosahedron.faces[i].Edges[2].First.Y) / 3;
                double z = (Icosahedron.faces[i].Edges[0].First.Z + Icosahedron.faces[i].Edges[1].First.Z + Icosahedron.faces[i].Edges[2].First.Z) / 3;
                verts[i] = new Point3d(x, y, z);
            }

            Polyhedron Dodecahedron = new Polyhedron();
            Dodecahedron.faces = new Face[12];
            Dodecahedron.faces[0] = Face.CreatePentagon(verts[0], verts[2], verts[4], verts[6], verts[8]);
            Dodecahedron.faces[1] = Face.CreatePentagon(verts[1], verts[3], verts[5], verts[7], verts[9]);

            for (int i = 0; i < 4; ++i)
            {
                Dodecahedron.faces[2 + 2 * i] = Face.CreatePentagon(verts[2 * i + 2], verts[2 * i], verts[10 + 2 * i], verts[11 + 2 * i], verts[12 + 2 * i]);
                Dodecahedron.faces[3 + 2 * i] = Face.CreatePentagon(verts[2 * i + 3], verts[2 * i + 1], verts[11 + 2 * i], verts[12 + 2 * i], verts[13 + 2 * i]);
            }

            Dodecahedron.faces[10] = Face.CreatePentagon(verts[0], verts[8], verts[18], verts[19], verts[10]);
            Dodecahedron.faces[11] = Face.CreatePentagon(verts[9], verts[1], verts[11], verts[10], verts[19]);

            Dodecahedron.center = new Point3d(0, 0, 0);

            return Dodecahedron;
        }

        private static Point3d GetThirdVertexForTriangle(Point3d a, Point3d b, Point3d q)
        {
            Vector m = new Vector(a, b);
            Vector n = new Vector(a, q);
            double sideLen = m.Norm();
            double hLen = sideLen * Math.Sqrt(3) / 2.0;

            double coeff = (m * n) / (m * m);
            Vector h = (n - (coeff * m)).Normalize() * hLen;
            Vector r = (0.5 * m) + h;
            return new Point3d(a.X + r.X, a.Y + r.Y, a.Z + r.Z);
        }

        private static Point3d[] GetVertecesForSquare(Point3d a, Point3d b, Point3d q)
        {
            Vector m = new Vector(a, b);
            Vector n = new Vector(a, q);
            double sideLen = m.Norm();

            double coeff = (m * n) / (m * m);
            Vector h = (n - (coeff * m)).Normalize() * sideLen;
            Vector r = m + h;
            Point3d c = new Point3d(a.X + r.X, a.Y + r.Y, a.Z + r.Z);
            Point3d d = new Point3d(a.X + h.X, a.Y + h.Y, a.Z + h.Z);
            return new Point3d[2] { c, d };
        }

        public void Colorize(Random R)
        {
            foreach (var x in faces)
            {
                x.Color = (byte)R.Next(8);
            }
            colorized = true;
        }

        public IFigure Clone()
        {
            Polyhedron res = new Polyhedron();
            res.faces = new Face[faces.Length];
            for (int i = 0; i < faces.Length; ++i)
            {
                res.faces[i] = faces[i].Clone() as Face;
            }
            res.center = center.Clone() as Point3d;
            return res;
        }

        public void Draw(Graphics g, Color c)
        {
            foreach (var x in Faces)
            {
                x.Draw(g, c);
            }
        }
    }

    class Vector
    {
        public Vector()
        {
            /*empty*/
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Vector (Point3d a)
        {
            this.x = a.X;
            this.y = a.Y;
            this.z = a.Z;
        }

        public Vector(Point3d a, Point3d b)
        {
            this.x = b.X - a.X;
            this.y = b.Y - a.Y;
            this.z = b.Z - a.Z;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.x, -a.y, -a.z);
        }

        public static Vector operator *(double a, Vector b)
        {
            return new Vector(a * b.x, a * b.y, a * b.z);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.x * b, a.y * b, a.z * b);
        }

        public static double operator *(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public double Norm()
        {
            return Math.Sqrt(this * this);
        }
        
        public Vector Normalize()
        {
            return this * (1.0 / this.Norm());
        }

        public Vector this[Vector a]
        {
            get
            {
                return new Vector(this.y * a.z - a.y * this.z, this.z * a.x - a.z * this.x, this.x * a.y - a.x * this.y);
            }
        }

        private double x, y, z;

        public double X
        {
            get
            {
                return x;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
        }

        public double Z
        {
            get
            {
                return z;
            }
        }
    }

    class Cam
    {
        private Vector pos;
        private Vector view;
        private Vector hor;
        private Vector vert;

        public Cam(Vector pos, Vector view, Vector hor, Vector vert)
        {
            this.pos = pos;
            this.view = view;
            this.hor = hor;
            this.vert = vert;
        }

        /// <summary>
        /// NB: oi, oj, ok should be orthonormal and be a rigth three!!!
        /// </summary>
        public void SetCamByPoints(Point3d o, Point3d i, Point3d j, Point3d k)
        {
            pos = new Vector(o);
            view = new Vector(o, i);
            hor = new Vector(o, j);
            vert = new Vector(o, k);
        }

        public Vector Pos
        {
            get
            {
                return pos;
            }
        }

        public Vector View
        {
            get
            {
                return view;
            }
        }

        public Vector Hor
        {
            get
            {
                return hor;
            }
        }

        public Vector Vert
        {
            get
            {
                return vert;
            }
        }
    }

    class Zbuffer
    {
        public Zbuffer(int w, int h)
        {
            depth = new double[w, h];
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    depth[i, j] = double.MinValue;
                }
            }
            bmp = new Bitmap(w, h);
        }

        public void ProcessFace(Face face)
        {
            byte COLOR = face.Color;

            Point3d[] points = face.GetPoints();
            int pointsNum = points.Length;

            Point[] pixelPoints = Array.ConvertAll(points, new Converter<Point3d, Point>(x => x.To2d()));
            int indLeft = 0, indRight = 0;
            for (int i = 0; i < pointsNum; ++i)
            {
                if (pixelPoints[indLeft].X > pixelPoints[i].X)
                {
                    indLeft = i;
                }
                if (pixelPoints[indRight].X < pixelPoints[i].X)
                {
                    indRight = i;
                }
            }

            int[] topIndices = new int[(indRight > indLeft) ? (indRight - indLeft + 1) : (indRight - indLeft + pointsNum + 1)];
            int[] bottomIndices = new int[pointsNum + 2 - topIndices.Length];
            topIndices[0] = indLeft;
            bottomIndices[0] = indLeft;
            for (int i = 1; i < topIndices.Length; ++i)
            {
                if (topIndices[i - 1] == pointsNum - 1)
                    topIndices[i] = 0;
                else
                    topIndices[i] = topIndices[i - 1] + 1;
            }
            for (int i = 1; i < bottomIndices.Length; ++i)
            {
                if (bottomIndices[i - 1] == 0)
                    bottomIndices[i] = pointsNum - 1;
                else
                    bottomIndices[i] = bottomIndices[i - 1] - 1;
            }

            double cTop = (double)(pixelPoints[topIndices[1]].Y - pixelPoints[topIndices[0]].Y) /
                          (double)(pixelPoints[topIndices[1]].X - pixelPoints[topIndices[0]].X);
            double cBottom = (double)(pixelPoints[bottomIndices[1]].Y - pixelPoints[bottomIndices[0]].Y) /
                             (double)(pixelPoints[bottomIndices[1]].X - pixelPoints[bottomIndices[0]].X);
            if (cTop > cBottom)
            {
                var tmp = topIndices;
                topIndices = bottomIndices;
                bottomIndices = tmp;
            }

            int topCnt = 1, bottomCnt = 1, currCol = pixelPoints[indLeft].X;
            while (topCnt < topIndices.Length && bottomCnt < bottomIndices.Length)
            {
                double topRatio = 0, bottomRatio = 0;
                if ((pixelPoints[topIndices[topCnt]].X - pixelPoints[topIndices[topCnt - 1]].X) == 0)
                {
                    topRatio = 1;
                }
                else
                {
                    topRatio = (double)(currCol - pixelPoints[topIndices[topCnt - 1]].X) /
                               (double)(pixelPoints[topIndices[topCnt]].X - pixelPoints[topIndices[topCnt - 1]].X);
                }
                if ((pixelPoints[bottomIndices[bottomCnt]].X - pixelPoints[bottomIndices[bottomCnt - 1]].X) == 0)
                {
                    bottomRatio = 1;
                }
                else
                {
                    bottomRatio = (double)(currCol - pixelPoints[bottomIndices[bottomCnt - 1]].X) /
                                  (double)(pixelPoints[bottomIndices[bottomCnt]].X - pixelPoints[bottomIndices[bottomCnt - 1]].X);
                }

                int topLimit = (int)(topRatio * pixelPoints[topIndices[topCnt]].Y + (1 - topRatio) * pixelPoints[topIndices[topCnt - 1]].Y);
                int bottomLimit = (int)(bottomRatio * pixelPoints[bottomIndices[bottomCnt]].Y + (1 - bottomRatio) * pixelPoints[bottomIndices[bottomCnt - 1]].Y);

                double topDepth = topRatio * points[topIndices[topCnt]].X + (1 - topRatio) * points[topIndices[topCnt - 1]].X;
                double bottomDepth = bottomRatio * points[bottomIndices[bottomCnt]].X + (1 - bottomRatio) * points[bottomIndices[bottomCnt - 1]].X;

                for (int i = topLimit; i <= bottomLimit; ++i)
                {
                    double ratio = 0;
                    if ((topLimit - bottomLimit) == 0)
                    {
                        ratio = 0.5;
                    }
                    else
                    {
                        ratio = (double)(i - bottomLimit) / (double)(topLimit - bottomLimit);
                    }
                    double pDepth = ratio * topDepth + (1 - ratio) * bottomDepth;
                    if (pDepth > depth[currCol, i])
                    {
                        depth[currCol, i] = pDepth;
                        bmp.SetPixel(currCol, i, palette[COLOR]);
                    }
                }

                ++currCol;
                if (pixelPoints[topIndices[topCnt]].X < currCol)
                    ++topCnt;
                if (pixelPoints[bottomIndices[bottomCnt]].X < currCol)
                    ++bottomCnt;
            }


        }
        
        public Bitmap GetImage()
        {
            return bmp;
        }

        private double[,] depth;
        private Bitmap bmp;

        private static Color[] palette = new Color[8] { Color.Gray, Color.Black, Color.Red, Color.Yellow, Color.Green, Color.DarkBlue, Color.Cyan, Color.Magenta };
    }

    class Object
    {
        virtual public bool find_cross(Point3d cam_pos, Point3d ray_pos, ref Point3d t) { return true; }
        virtual public Vector normal(Point3d p) { return new Vector(); }

        protected const double eps = 1e-6;
    }

    class Sphere : Object
    {
        Point3d center;
        double radius;

        public Point3d C
        {
            get
            {
                return center;
            }
        }

        public double R
        {
            get
            {
                return radius;
            }
        }

        public Sphere(Point3d c, double r)
        {
            this.center = c;
            this.radius = r;
        }

        public Sphere(Polyhedron ph)
        {
            var p = ph.Faces[0].Edges[0].First;
            var dist = new Vector(ph.Center, p).Norm();
            foreach (var f in ph.Faces)
                foreach (var e in f.Edges)
                {
                    var pp = e.First;
                    var ddist = new Vector(ph.Center, pp).Norm();
                    if (ddist > dist)
                    {
                        dist = ddist;
                        p = pp;
                    }
                }
            
            this.center = ph.Center;
            this.radius = dist;
        }

        public override bool find_cross(Point3d cam_pos, Point3d ray_pos, ref Point3d t)
        {
            Vector d = new Vector(
                ray_pos.X - cam_pos.X,
                ray_pos.Y - cam_pos.Y,
                ray_pos.Z - cam_pos.Z);
            Vector c = new Vector(
                cam_pos.X - this.C.X,
                cam_pos.Y - this.C.Y,
                cam_pos.Z - this.C.Z);

            double k1 = d * d,
                   k2 = 2 * (c * d),
                   k3 = (c * c) - this.R * this.R;
            double D = k2 * k2 - 4 * k1 * k3;
            if (D < 0)
                return false;

            double x1 = (-k2 + Math.Sqrt(D)) / (2 * k1);
            double x2 = (-k2 - Math.Sqrt(D)) / (2 * k1);
            double x = 0;
            if (x1 < eps && x2 < eps)
                return false;
            else if (x1 < eps)
                x = x2;
            else if (x2 < eps)
                x = x1;
            else
                x = x1 < x2 ? x1 : x2;

            t = new Point3d(
                cam_pos.X + d.X * x,
                cam_pos.Y + d.Y * x,
                cam_pos.Z + d.Z * x);
            return true;
        }

        public override Vector normal(Point3d p)
        {
            return new Vector(this.C, p);
        }
    }

    class Wall : Object
    {
        Point3d[] points;
        double square;

        public Wall(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            Vector v1 = new Vector(p1, p2);
            Vector v2 = new Vector(p1, p3);
            Vector n = v1[v2];
            double d = -(n.X * p1.X + n.Y * p1.Y + n.Z * p1.Z);
            if (Math.Abs(n.X * p4.X + n.Y * p4.Y + n.Z * p4.Z + d) > eps)
                throw new Exception("bad wall");
            points = new Point3d[4];
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
            points[3] = p4;
            square = triangle_square(p1, p2, p3) + triangle_square(p1, p3, p4);
        }

        private double triangle_square(Point3d A, Point3d B, Point3d C)
        {
            double a = new Vector(B, C).Norm();
            double b = new Vector(A, C).Norm();
            double c = new Vector(A, B).Norm();
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public override bool find_cross(Point3d cam_pos, Point3d ray_pos, ref Point3d t)
        {
            Vector n = this.normal(null);
            double d = -(n.X * points[0].X + n.Y * points[0].Y + n.Z * points[0].Z);

            Vector v = new Vector(cam_pos, ray_pos);
            Vector u = new Vector(cam_pos);

            double denum = n * v;
            if (Math.Abs(denum) < eps)
                return false;
            double num = n * u + d;
            double tp = -num / denum;
            if (tp < eps)
                return false;
            t = new Point3d(
                v.X * tp + u.X,
                v.Y * tp + u.Y,
                v.Z * tp + u.Z);

            /*Vector vt = new Vector(this.center, t);
            Point3d ut = this.center;

            for (int i = 0; i < 4; ++i)
            {
                Vector v_ = new Vector(points[i], points[(i + 1) % 4]);
                Point3d u_ = points[i];
                if (Math.Abs(Math.Abs(vt * v_ / vt.Norm() / v_.Norm()) - 1) > eps)
                {
                    double t_ = (-vt.Y * (u_.X - ut.X) + vt.X * (u_.Y - ut.Y)) / (-v_.X * vt.Y + vt.X * v_.Y);
                    double s_ = (-v_.Y * (u_.X - ut.X) + v_.X * (u_.Y - ut.Y)) / (-v_.X * vt.Y + vt.X * v_.Y);
                    if (0 <= t_ && t_ <= 1 && 0 <= s_ && s_ <= 1)
                        return false;
                }
            }*/
            double square = 0;
            for (int i = 0; i < 4; ++i)
                square += triangle_square(points[i], points[(i + 1) % 4], t);
            if (Math.Abs(this.square - square) > eps)
                return false;

            return true;
        }

        public override Vector normal(Point3d p)
        {
            Vector v1 = new Vector(points[0], points[1]);
            Vector v2 = new Vector(points[0], points[2]);
            return v1[v2];
        }
    }

    class Poly : Object
    {
        Polyhedron ph;
        Sphere sph;
        Dictionary<Face, double> squares = new Dictionary<Face, double>();

        public Poly(Polyhedron p)
        {
            ph = p.Clone() as Polyhedron;
            sph = new Sphere(ph);
            squares.Clear();
            foreach (var face in ph.Faces)
            {
                double s = 0;
                int cnt = face.Edges.Count();
                for (int i = 1; i < cnt - 1; ++i)
                    s += triangle_square(face.Edges[0].First, face.Edges[i].First, face.Edges[i + 1].First);
                squares.Add(face, s);
            }
        }

        private double triangle_square(Point3d A, Point3d B, Point3d C)
        {
            double a = new Vector(B, C).Norm();
            double b = new Vector(A, C).Norm();
            double c = new Vector(A, B).Norm();
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public override bool find_cross(Point3d cam_pos, Point3d ray_pos, ref Point3d t)
        {
            if (!sph.find_cross(cam_pos, ray_pos, ref t))
                return false;

            double dist = double.MaxValue;
            Point3d res = new Point3d();
            bool flag = false;

            foreach (var face in ph.Faces)
            {
                var p1 = face.Edges[0].First;
                var p2 = face.Edges[1].First;
                var p3 = face.Edges[2].First;
                Vector v1 = new Vector(p1, p2);
                Vector v2 = new Vector(p1, p3);
                Vector n = v1[v2];
                double d = -(n.X * p1.X + n.Y * p1.Y + n.Z * p1.Z);

                Vector v = new Vector(cam_pos, ray_pos);
                Vector u = new Vector(cam_pos);

                double denum = n * v;
                if (Math.Abs(denum) < eps)
                    continue;
                double num = n * u + d;
                double tp = -num / denum;
                if (tp < eps)
                    continue;
                t = new Point3d(
                    v.X * tp + u.X,
                    v.Y * tp + u.Y,
                    v.Z * tp + u.Z);

                double square = 0;
                int cnt = face.Edges.Count();
                for (int i = 0; i < cnt; ++i)
                    square += triangle_square(face.Edges[i].First, face.Edges[(i + 1) % cnt].First, t);
                if (Math.Abs(squares[face] - square) > eps)
                    continue;
                var dist_t = new Vector(cam_pos, t).Norm();
                if (dist_t < dist)
                {
                    res = t.Clone() as Point3d;
                    dist = dist_t;
                    flag = true;
                }
            }

            t = res.Clone() as Point3d;
            return flag;
        }

        public override Vector normal(Point3d p)
        {
            foreach (var face in ph.Faces)
            {
                double square = 0;
                int cnt = face.Edges.Count();
                for (int i = 0; i < cnt; ++i)
                    square += triangle_square(face.Edges[i].First, face.Edges[(i + 1) % cnt].First, p);
                if (Math.Abs(squares[face] - square) > eps)
                    continue;

                var p1 = face.Edges[0].First;
                var p2 = face.Edges[1].First;
                var p3 = face.Edges[2].First;
                Vector v1 = new Vector(p1, p2);
                Vector v2 = new Vector(p1, p3);
                return v1[v2];
            }
            return new Vector();
        }
    }
}
