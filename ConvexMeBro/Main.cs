using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvexMeBro
{
    public partial class Main : Form
    {
        System.Drawing.Graphics g;
        Pen pen;
        public Main()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            pen = new Pen(Color.Red, 2);
            txtInput.Text = "100,100/10,100/25,25/200,100/200,200/100,150/50,200";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            string[] first = txtInput.Text.Split('/');

            List<PointF> shape = convert(first);

            if (shape == null) return;

            if (isConvex(shape)) DrawLines(shape, Color.Blue);
            else
            {
                pen.Width = 4;
                DrawLines(shape, Color.Red);
                pen.Width = 2;
                List<List<PointF>> convex = MakeConvex(shape);

                foreach (List<PointF> points in convex)
                {
                    DrawLines(points, Color.Black);
                }
            }
        }

        private bool isConvex(List<PointF> shape)
        {
            bool set = false;
            bool positive = false;
            for(int i = 0; i < shape.Count; i++)
            {
                PointF p1 = shape[i];
                PointF p2 = shape[(i + 1) % shape.Count];
                PointF p3 = shape[(i + 2) % shape.Count];

                PointF d1 = new PointF(p2.X - p1.X, p2.Y - p1.Y);
                PointF d2 = new PointF(p3.X - p2.X, p3.Y - p2.Y);
                
                float cross = Helper.CrossProduct(d1, d2);

                if (!set)
                {
                    if (cross != 0) set = true;
                    positive = (cross > 0);
                }
                else if (positive != (cross > 0) && cross != 0) return false;
            }

            return true;
        }

        private List<List<PointF>> MakeConvex(List<PointF> shape)
        {
            List<List<PointF>> output = new List<List<PointF>>();

            List<PointF> main = new List<PointF>(shape);

            bool go = true;
            while (go && main.Count > 3)
            {
                go = false;

                bool set = false;
                bool positive = false;
                for (int i = 0; i < main.Count; i++)
                {
                    PointF p1 = main[i];
                    PointF p2 = main[(i + 1) % main.Count];
                    PointF p3 = main[(i + 2) % main.Count];

                    PointF d1 = new PointF(p2.X - p1.X, p2.Y - p1.Y);
                    PointF d2 = new PointF(p3.X - p2.X, p3.Y - p2.Y);

                    float cross = Helper.CrossProduct(d1, d2);

                    if (!set)
                    {
                        if (cross != 0) set = true;
                        positive = (cross > 0);
                    }
                    else if (positive != (cross > 0) && cross != 0)
                    {
                        output.Add(new List<PointF>() { p2, p3, main[(i + 3) % main.Count] });
                        main.Remove(p3);

                        go = true;
                        break;
                    }
                }
            }

            output.Add(main);

            return output;
        }


        private List<PointF> convert(string[] text)
        {
            List<PointF> points = new List<PointF>();

            for (int i = 0; i < text.Length; i++)
            {
                string[] split = text[i].Split(',');
                float x;
                float y;
                if (!float.TryParse(split[0], out x)) return null;
                if (!float.TryParse(split[1], out y)) return null;

                points.Add(new PointF(x, y));
            }

            return points;
        }

        private void DrawLines(List<PointF> points, Color color)
        {
            pen.Color = color;
            for (int i = 0; i < points.Count; i++)
            {
                PointF p1 = points[i];
                PointF p2 = points[(i + 1) % points.Count];
                g.DrawLine(pen, p1, p2);
            }
        }
    }
}
