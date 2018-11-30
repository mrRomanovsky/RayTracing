using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6
{
    public partial class Form1 : Form
    {
        //Graphics g;
        Bitmap[] bmp;
        Polyhedron ph = null;
        public Form1()
        {
            InitializeComponent();
            //g = pictureBox1.CreateGraphics();
            bmp = new Bitmap[4];
            bmp[0] = new Bitmap("Iso.bmp");
            bmp[1] = new Bitmap("Oxy.bmp");
            bmp[2] = new Bitmap("Oxz.bmp");
            bmp[3] = new Bitmap("Oyz.bmp");
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        Cam CreateCam()
        {
            double pos_x = double.Parse(textBoxCameraX.Text);
            double pos_y = double.Parse(textBoxCameraY.Text);
            double pos_z = double.Parse(textBoxCameraZ.Text);

            double view_x = double.Parse(textBoxViewVectorX.Text);
            double view_y = double.Parse(textBoxViewVectorY.Text);
            double view_z = double.Parse(textBoxViewVectorZ.Text);

            double vert_angle = double.Parse(textBoxVerticalAngle.Text) / 180.0 * Math.PI;

            Vector pos = new Vector(pos_x, pos_y, pos_z);
            Vector view = new Vector(view_x, view_y, view_z);
            view = view.Normalize();

            // вектор вертикали без учета угла наклона
            Vector vert;
            if (Math.Abs(view_x) < 1e-6 && Math.Abs(view_y) < 1e-6)
                vert = new Vector(-1, 0, 0);
            else
                vert = new Vector(0, 0, 1);
            double scalar_prod_vert_view = view * vert;
            vert = (vert - (view * scalar_prod_vert_view)).Normalize();

            // вектор горизонтали без учета угла наклона
            Vector hor = vert[view].Normalize();

            // учет угла наклона вертикали
            vert = (vert * Math.Cos(vert_angle) + hor * Math.Sin(vert_angle)).Normalize();
            hor = vert[view].Normalize();

            return new Cam(pos, view, -hor, -vert);
        }

        

        private Color light_color(Color clr, double light)
        {
            Color res = Color.FromArgb(clr.A, clr);
            if (light < 0)
                res = Color.Black;
            else if (light <= 1)
            {
                int red = (int)(ambient.R + (res.R - ambient.R) * light);
                int green = (int)(ambient.G + (res.G - ambient.G) * light);
                int blue = (int)(ambient.B + (res.B - ambient.B) * light);
                res = Color.FromArgb(red, green, blue);
            }
            else
            {
                int red = (int)(res.R + (255 - res.R) * (light - 1));
                if (red > 255)
                    red = 255;
                int green = (int)(res.G + (255 - res.G) * (light - 1));
                if (green > 255)
                    green = 255;
                int blue = (int)(res.B + (255 - res.B) * (light - 1));
                if (blue > 255)
                    blue = 255;
                res = Color.FromArgb(red, green, blue);
            }
            return res;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private Dictionary<int, int> interpolate(int i0, int d0, int i1, int d1)
        {
            Dictionary<int, int> res = new Dictionary<int, int>();
            if (i0 == i1)
            {
                res.Add(i0, d0);
                return res;
            }

            double a = (d1 - d0) / (double)(i1 - i0);
            double d = d0;
            for (int i = i0; i <= i1; ++i)
            {
                res.Add(i, (int)Math.Round(d));
                d += a;
            }

            return res;
        }

        //получаем координаты окна обзора
        private List<Point3d> make_grid_corners(double dist)
        {
            var w = pictureBox1.Width / 2;
            var h = pictureBox1.Height / 2;
            Cam cam = CreateCam();

            Vector view = cam.View * dist;
            Point3d m0 = new Point3d(
                cam.Pos.X + view.X,
                cam.Pos.Y + view.Y,
                cam.Pos.Z + view.Z);

            Vector hor = cam.Hor * w;
            Vector vert = cam.Vert * h;
            Point3d p1 = new Point3d(
                m0.X - hor.X - vert.X,
                m0.Y - hor.Y - vert.Y,
                m0.Z - hor.Z - vert.Z);
            Point3d p2 = new Point3d(
                m0.X + hor.X - vert.X,
                m0.Y + hor.Y - vert.Y,
                m0.Z + hor.Z - vert.Z);
            Point3d p3 = new Point3d(
                m0.X - hor.X + vert.X,
                m0.Y - hor.Y + vert.Y,
                m0.Z - hor.Z + vert.Z);
            Point3d p4 = new Point3d(
                m0.X + hor.X + vert.X,
                m0.Y + hor.Y + vert.Y,
                m0.Z + hor.Z + vert.Z);

            List<Point3d> res = new List<Point3d>();
            res.Add(p1);
            res.Add(p2);
            res.Add(p3);
            res.Add(p4);
            return res;
        }

        List<Object> objects = new List<Object>();
        List<Sphere> spheres = new List<Sphere>();
        Dictionary<Object, Color> colors = new Dictionary<Object, Color>(); //цвета фигур
        Dictionary<Object, double> diffuse = new Dictionary<Object, double>(); //диффузное отражение фигур
        Dictionary<Object, double> reflect = new Dictionary<Object, double>(); //зеркальность фигур
        Dictionary<Object, double> trans = new Dictionary<Object, double>(); //прозрачность фигур
        Dictionary<Object, double> refract = new Dictionary<Object, double>(); //преломление фигур

        List<Point3d> lights = new List<Point3d>();
        Dictionary<Point3d, double> lights_power = new Dictionary<Point3d, double>();

        Color ambient = Color.Black;
        const double eps = 1e-6;
        const double amb_light = 0.3;

        //возвращает список сфер среди всех объектов
        private List<Sphere> find_spheres(List<Polyhedron> objects)
        {
            List<Sphere> spheres = new List<Sphere>();
            foreach (var obj in objects)
                spheres.Add(new Sphere(obj));
            return spheres;
        }

        //поиск пикселей, для которых требуется выполнить трассировку лучей
        private List<Point3d> find_rays()
        {
            var corners = make_grid_corners(pictureBox1.Width * Math.Sqrt(3) / 2); //углы окна обзора
            List<Point3d> points = new List<Point3d>();
            var w = pictureBox1.Width;
            var h = pictureBox1.Height;

            var step_h_x = (corners[2].X - corners[0].X) / h;
            var step_h_y = (corners[2].Y - corners[0].Y) / h;
            var step_h_z = (corners[2].Z - corners[0].Z) / h;

            var step_w_x = (corners[1].X - corners[0].X) / w;
            var step_w_y = (corners[1].Y - corners[0].Y) / w;
            var step_w_z = (corners[1].Z - corners[0].Z) / w;

            for (int i = 0; i < h; ++i)
            {
                var p = new Point3d(
                    corners[0].X + step_h_x * i,
                    corners[0].Y + step_h_y * i,
                    corners[0].Z + step_h_z * i);
                for (int j = 0; j < w; ++j)
                {
                    points.Add(new Point3d(
                        p.X + step_w_x * j, 
                        p.Y + step_w_y * j, 
                        p.Z + step_w_z * j));
                }
            }

            return points;
        }

        private bool are_crossed(Point3d cam_pos, Point3d ray_pos, Sphere s)
        {
            Vector n = new Vector(cam_pos, ray_pos);
            Vector v = new Vector(cam_pos, s.C);
            double dist = n[v].Norm() / n.Norm();
            return dist <= s.R;
        }

        //пересечение луча из камеры через окно обзора со сферой
        private bool FindIntersection(Point3d cam_pos, Point3d ray_pos, Sphere s, ref Point3d t)
        {
            Vector d = new Vector(
                ray_pos.X - cam_pos.X,
                ray_pos.Y - cam_pos.Y,
                ray_pos.Z - cam_pos.Z);
            Vector c = new Vector(
                cam_pos.X - s.C.X,
                cam_pos.Y - s.C.Y,
                cam_pos.Z - s.C.Z);

            double k1 = d * d,
                   k2 = 2 * (c * d),
                   k3 = (c * c) - s.R * s.R;
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

        //сложение цветов (поверхности, отражённого и прозрачного)
        private Color AddCols(Color diff, Color refl, Color trans, bool is_refl, bool is_trans)
        {
            int r = diff.R, g = diff.G, b = diff.B;
            if (is_refl)
            {
                r += refl.R;
                g += refl.G;
                b += refl.B;
            }
            if (is_trans)
            {
                r += trans.R;
                g += trans.G;
                b += trans.B;
            }
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;
            return Color.FromArgb(r, g, b);
        }

        //трассировка луча (если intense достаточно мал, останавливаемся)
        private Color RayTrace(Point3d start, Point3d p, double intense)
        {
            if (intense < 0.01)
                return Color.Black;

            double dist = double.MaxValue;
            Object obj = null;
            Point3d cross = new Point3d();
            foreach (var o in objects)
            {
                Point3d t = new Point3d();
                if (o.find_cross(start, p, ref t))
                {
                    double d1 = new Vector(start, t).Norm();
                    if (d1 < dist)
                    {
                        dist = d1;
                        obj = o;
                        cross = t;
                    }
                }
            }

            if (obj == null)
                return ambient;

            // диффузное освещение
            double ldiff = 0;
            Point3d tt = new Point3d();
            foreach (var l in lights)
            {
                bool flag = false;
                double trans_coeff = 1;
                foreach (var o in objects)
                {
                    //if (FindIntersection(cross, l, s, ref tt))
                    if (o.find_cross(cross, l, ref tt) && new Vector(tt, cross).Norm() < new Vector(l, cross).Norm())
                    {
                        if (trans[o] > 0)
                            trans_coeff *= trans[o];
                        else
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                    continue;
                //ll += lights_power[l];

                double kd = diffuse[obj];
                double l0 = lights_power[l];
                Vector N = obj.normal(cross);
                Vector L = new Vector(cross, l);
                if (N * L < 0)
                    N = -N;
                double cos = (N * L) / N.Norm() / L.Norm();
                ldiff += kd * cos * l0 * trans_coeff;
            }

            Color clr_diff = light_color(colors[obj], (ldiff + amb_light)* intense);

            // отражение
            Color clr_refl = Color.Black;
            if (reflect[obj] > 0)
            {
                Vector l = new Vector(start, p);
                l = l.Normalize();
                Vector n = obj.normal(cross);
                n = n.Normalize();
                if (n * l > 0)
                    n = -n;
                Vector r = l - 2 * n * (n * l);
                Point3d p_new = new Point3d(
                    cross.X + r.X,
                    cross.Y + r.Y,
                    cross.Z + r.Z);
                clr_refl = RayTrace(cross, p_new, intense * reflect[obj]);
            }

            //преломление
            Color clr_trans = Color.Black;
            if (trans[obj] > 0)
            {
                Vector l = new Vector(start, p);
                l = l.Normalize();
                Vector n = obj.normal(cross);
                n = n.Normalize();
                if (n * l > 0)
                    n = -n;
                double coef = 1 / refract[obj];
                double cos = Math.Sqrt(1 - coef * coef * (1 - (n * l) * (n * l)));
                Vector t = coef * l - (cos + coef * (n * l)) * n;
                Point3d p_new = new Point3d(
                    cross.X + t.X,
                    cross.Y + t.Y,
                    cross.Z + t.Z);
                //if (!FindIntersection(cross, p_new, sph, ref tt))

                for (int i = 0; i < 10; ++i)
                {
                    if (!obj.find_cross(cross, p_new, ref tt))
                        return clr_diff;

                    l = new Vector(cross, p_new);
                    l = l.Normalize();
                    n = obj.normal(tt);
                    n.Normalize();
                    if (n * l > 0)
                        n = -n;
                    coef = refract[obj];
                    cos = 1 - coef * coef * (1 - (n * l) * (n * l));
                    if (cos < 0)
                    {
                        if (i == 9)
                        {
                            cross = tt;
                            p_new = new Point3d(
                                cross.X + l.X,
                                cross.Y + l.Y,
                                cross.Z + l.Z);
                            break;
                        }
                        Vector r = l - 2 * n * (n * l);
                        cross = tt;
                        p_new = new Point3d(
                            cross.X + r.X,
                            cross.Y + r.Y,
                            cross.Z + r.Z);
                        continue;
                    }
                    cos = Math.Sqrt(cos);
                    t = coef * l - (cos + coef * (n * l)) * n;
                    p_new = new Point3d(
                        tt.X + t.X,
                        tt.Y + t.Y,
                        tt.Z + t.Z);
                    break;
                }

                clr_trans = RayTrace(tt, p_new, intense * trans[obj]);
            }

            return AddCols(clr_diff, clr_refl, clr_trans, reflect[obj] > 0, trans[obj] > 0);
        }

        //добавление в сцену фигур
        private void Init()
        {
            objects.Clear();
            colors.Clear();
            diffuse.Clear();
            reflect.Clear();
            trans.Clear();
            refract.Clear();

            // снеговик
            objects.Add(new Sphere(new Point3d(1, -2, -2), 1));
            colors.Add(objects.Last(), Color.White);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0);
            trans.Add(objects.Last(), 1);
            refract.Add(objects.Last(), 1.5);

            /*objects.Add(new Sphere(new Point3d(-2, 1, -2.5), 0.7));
            colors.Add(objects.Last(), Color.White);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0.8);
            refract.Add(objects.Last(), 1.5);*/

            objects.Add(new Poly(Polyhedron.CreateHexahedron(
                new Point3d(-1, 0, -5),
                new Point3d(0, -1, -5),
                new Point3d(0, 1, -5))));
            colors.Add(objects.Last(), Color.Orange);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            #region стены
            objects.Add(new Wall(
                new Point3d(-5, -5, -5), 
                new Point3d(-5, -5, 5), 
                new Point3d(-5, 5, 5), 
                new Point3d(-5, 5, -5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            objects.Add(new Wall(
                new Point3d(-5, 5, -5),
                new Point3d(-5, 5, 5),
                new Point3d(5, 5, 5),
                new Point3d(5, 5, -5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            objects.Add(new Wall(
                new Point3d(5, 5, -5),
                new Point3d(5, 5, 5),
                new Point3d(5, -5, 5),
                new Point3d(5, -5, -5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            objects.Add(new Wall(
                new Point3d(5, -5, -5),
                new Point3d(5, -5, 5),
                new Point3d(-5, -5, 5),
                new Point3d(-5, -5, -5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            objects.Add(new Wall(
               new Point3d(-5, -5, 5),
               new Point3d(5, -5, 5),
               new Point3d(5, 5, 5),
               new Point3d(-5, 5, 5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);

            objects.Add(new Wall(
               new Point3d(-5, -5, -5),
               new Point3d(5, -5, -5),
               new Point3d(5, 5, -5),
               new Point3d(-5, 5, -5)));
            colors.Add(objects.Last(), Color.DarkBlue);
            diffuse.Add(objects.Last(), 0.8);
            reflect.Add(objects.Last(), 0.2);
            trans.Add(objects.Last(), 0);
            refract.Add(objects.Last(), 1);
            #endregion

            lights.Clear();
            lights_power.Clear();

            //lights.Add(new Point3d(-4.9, -4.9, -4.9));
            //lights_power.Add(lights.Last(), 1);

            lights.Add(new Point3d(0, 3.9, 0));
            lights_power.Add(lights.Last(), 1);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            Init(); //добавление в сцену фигур

            var points = find_rays(); //пиксели, видимые через окно обзора

            var cam_pos = new Point3d(
                double.Parse(textBoxCameraX.Text), 
                double.Parse(textBoxCameraY.Text), 
                double.Parse(textBoxCameraZ.Text)); //позиция камеры

            //g.Clear(ambient);
            for (int i = 0; i < pictureBox1.Height; i += 1)
                for (int j = 0; j < pictureBox1.Width; j += 1)
                {
                    var color = RayTrace(cam_pos, points[i * pictureBox1.Width + j], 1);
                    ((Bitmap)pictureBox1.Image).SetPixel(j, i, color);
                }
            pictureBox1.Refresh();
        }
    }
}
