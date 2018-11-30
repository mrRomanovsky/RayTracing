using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracing
{
    public partial class Form1 : Form
    {

        private int distanceToViewPort = 1;
        private int ViewPortSize { get { return 1; } }
        private Point3D Camera = new Point3D(0, 0, 0);

        List<Sphere> spheres = new List<Sphere>();
        Color BackgroundColor = Color.Black;//Color.White;
        List<Light> lights = new List<Light>();
        Cube room = new Cube(new Point3D(-10, -10, -10), 10, 10, 10);

        /*
         * sphere {
    center = (0, -1, 3)
    radius = 1
    color = (255, 0, 0)  # Красный
    specular = 500  # Блестящий
    reflective = 0.2  # Немного отражающий
}
sphere {
    center = (-2, 1, 3)
    radius = 1
    color = (0, 0, 255)  # Синий
    specular = 500  # Блестящий
    reflective = 0.3  # Немного более отражающий
}
sphere {
    center = (2, 1, 3)
    radius = 1
    color = (0, 255, 0)  # Зелёный
    specular = 10  # Немного блестящий
    reflective = 0.4  # Ещё более отражающий
}
         * */
        public Form1()
        {
            InitializeComponent();
            spheres.Add(new Sphere(new Point3D(0, 0, 4), 1, Color.FromArgb(255, 0, 0), 500, 0.2));
            spheres.Add(new Sphere(new Point3D(-1, 1, 6), 1, Color.FromArgb(0, 0, 255), 500, 0.3));
            spheres.Add(new Sphere(new Point3D(1, 1, 6), 1, Color.FromArgb(0, 255, 0), 10, 0.4));

            lights.Add(new Light(LightType.Ambient, 0.2));
            lights.Add(new Light(LightType.Point, 0.6, new Point3D(2, 1, 0)));
            lights.Add(new Light(LightType.Directional, 0.2));
            lights[2].Direction = new Point3D(1, 4, 4);
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private PointF GetRealCoords(PointF canvasCoords) =>
            new PointF(canvasCoords.X + pictureBox1.Width / 2, pictureBox1.Height / 2 - canvasCoords.Y - 1);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void renderSceneButton_Click(object sender, EventArgs e)
        {
            var col = IntersectRaySphere(new Point3D(0, 0, 0), new Point3D(-7, 35, 1), new Sphere(new Point3D(0, 50, 3), 10, Color.Red));
            int picWidth = pictureBox1.Width;
            int picHeight = pictureBox1.Height;
            for (int x = -picWidth / 2; x < picWidth / 2; ++x)
                for (int y = -picHeight / 2; y < picHeight / 2; ++y)
                {
                    if (x == -7 && y == 35)
                        Console.WriteLine("Howdy!");
                    var D = CanvasToViewport(x, y);
                    var color = TraceRay(Camera, D, 1, double.MaxValue, 2);
                    if (color != Color.White && (Math.Abs(x) > 10  || Math.Abs(y - 50) > 10))
                        Console.WriteLine("Howdy!");
                    PutCanvasPixel(x, y, color);
                }
           // pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private Point3D CanvasToViewport(double x, double y) =>
            new Point3D(x * 1.0 * ViewPortSize / pictureBox1.Width, y * 1.0 * ViewPortSize / pictureBox1.Height, distanceToViewPort);

        private void PutCanvasPixel(double x, double y, Color c)
        {
            var coords = GetRealCoords(new PointF((float)x, (float)y));
            using (var g = Graphics.FromImage(pictureBox1.Image))
                g.DrawRectangle(new Pen(c), coords.X, coords.Y, 1, 1);
            //((Bitmap)pictureBox1.Image).SetPixel((int)coords.X, (int)coords.Y, c);
        }

            //TODO: ambient - составной цвет (из R, G, B), а не просто яркость
        //P - точка поверхности; N - нормаль к поверхности в этой точке
        //V - вектор от объекта к камер, s - зеркальность
        private double ComputeLighting(Point3D P, Point3D N, Point3D V, int s)
        {
            double i = 0; //intensity
            Point3D L;
            double t_max;
            foreach (var light in lights)
            {
                if (light.Type == LightType.Ambient)
                    i += light.Intensity;
                else
                {
                    if (light.Type == LightType.Point)
                    {
                        L = light.Position - P;
                        t_max = 1;
                    }
                    else
                    {
                        L = light.Direction;
                        t_max = double.MaxValue;
                    }

                    //проверка тени
                    var shadowSphereAndT = ClosestIntersection(P, L, 0.001, t_max);
                    if (shadowSphereAndT.Item2 != double.MaxValue)
                        continue;

                    //диффузность
                    var scalarProdNL = N * L;
                    if (scalarProdNL > 0)
                        i += light.Intensity * scalarProdNL / (N.Length * L.Length);

                    //зеркальность
                    if (s != -1)
                    {
                        var R = 2 * (N * N * L) - L;
                        var scalarProdRV = R * V;
                        if (scalarProdRV > 0)
                            i += light.Intensity * Math.Pow(scalarProdRV / (R.Length * V.Length), s);
                    }
                }
            }
            return i;
        }

        //отражение луче относительно нормали
        //R - луч, N - нормаль
        private Point3D ReflectRay(Point3D R, Point3D N) => 
            2 * (N * N * R) - R;

         //depth - максимальная глубина рекурсии (для отражения)
        private Color TraceRay(Point3D O, Point3D D, double t_min, double t_max, int depth)
        {
            var closestTAndSphere = ClosestIntersection(O, D, t_min, t_max);
            var closest_t = closestTAndSphere.Item2;
            var closestSphere = closestTAndSphere.Item1;
            if (closest_t == double.MaxValue)











                return BackgroundColor;
            var P = O + closest_t * D; // пересечение
            var N = P - closestSphere.Centre; // нормаль в точке пересечения
            N = N / N.Length;
            var c = closestSphere.Color;
            var intensity = ComputeLighting(P, N, -1 * D, closestSphere.Specular);
            var local_color = Color.FromArgb(
                Math.Min((int)(c.R * intensity), 255),
                Math.Min((int)(c.G * intensity), 255),
                Math.Min((int)(c.B * intensity), 255)); //closestSphere.Color * ComputeLighting(P, N);*/

            var r = closestSphere.Reflective;
            if (depth <= 0 || r <= 0) //мы достигли глубины рекурсии или объект не отражающий
                return local_color;

            var R = ReflectRay(-1 * D, N); //отражённый луч
            var reflectedColor = TraceRay(P, R, 0.001, double.MaxValue, depth - 1);

            //localColor * (1 - r) + reflectedColor * r
            return Color.FromArgb(
                Math.Min((int)(local_color.R * (1 - r) + reflectedColor.R * r), 255),
                Math.Min((int)(local_color.G * (1 - r) + reflectedColor.G * r), 255),
                Math.Min((int)(local_color.B * (1 - r) + reflectedColor.B * r), 255));
        }

        private Tuple<Sphere, double> ClosestIntersection(Point3D O, Point3D D, double t_min, double t_max)
        {
            double closest_t = double.MaxValue;
            Sphere closestSphere = new Sphere(new Point3D(-1, -1, -1), -1, BackgroundColor);
            bool closestSphereFound = false;
            foreach (var sphere in spheres)
            {
                var sphereIntersections = IntersectRaySphere(O, D, sphere);
                var t1 = sphereIntersections.Item1;
                var t2 = sphereIntersections.Item2;
                if (t1 >= t_min && t1 < t_max && t1 < closest_t)
                {
                    closest_t = t1;
                    closestSphere = sphere;
                    closestSphereFound = true;
                }
                if (t2 >= t_min && t2 < t_max && t2 < closest_t)
                {
                    closest_t = t2;
                    closestSphere = sphere;
                    closestSphereFound = true;
                }
            }
            return new Tuple<Sphere, double>(closestSphere, closest_t);
        }

        private Tuple<double, double> IntersectRaySphere(Point3D O, Point3D D, Sphere sphere)
        {
            var C = sphere.Centre;
            var r = sphere.Radius;
            Point3D oc = O - C;
            var k1 = D * D;
            var k2 = 2 * (oc * D);
            var k3 = oc * oc - r * r;
            var discriminant = k2 * k2 - 4 * k1 * k3;
            if (discriminant < 0)
                return new Tuple<double, double>(double.MinValue, double.MinValue);

            var t1 = (-k2 + Math.Sqrt(discriminant)) / (2 * k1);
            var t2 = (-k2 - Math.Sqrt(discriminant)) / (2 * k1);
            return new Tuple<double, double>(t1, t2);
        }
    }
}
