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
    //ASSUMES THAT ALL POINTS ARE CLOCKWISE
    public partial class Main : Form
    {
        System.Drawing.Graphics g;
        Pen pen;
        public Main()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            pen = new Pen(Color.Red, 2);
            txtInput.Text = "100,100/10,100/25,25/200,100/200,200/100,150/50,200/0,150/0,120";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            string[] first = txtInput.Text.Split('/');

            List<PointF> shape = convert(first);

            if (shape == null) return;

            if (isClockWiseConvex(shape)) DrawLines(shape, Color.Blue);
            else
            {
                pen.Width = 4;
                DrawLines(shape, Color.Red);
                pen.Width = 2;
                List<List<PointF>> convex = MakeConvex(shape);

                if (hasDuplicates(convex)) MessageBox.Show("DUPLICATES FOUND");

                foreach (List<PointF> points in convex)
                {
                    DrawLines(points, Color.Black);
                }
            }
        }

        //convert text input into a list of PointF
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


        //assumes points are listing in clockwise order
        private bool isClockWiseConvex(List<PointF> shape)
        {
            for(int i = 0; i < shape.Count; i++)
            {
                PointF p1 = shape[i];
                PointF p2 = shape[(i + 1) % shape.Count];
                PointF p3 = shape[(i + 2) % shape.Count];

                PointF d1 = new PointF(p2.X - p1.X, p2.Y - p1.Y);
                PointF d2 = new PointF(p3.X - p2.X, p3.Y - p2.Y);
                
                float cross = Helper.CrossProduct(d1, d2);

                //if we turn counter-clockwise
                if (cross < 0) return false;
            }

            return true;
        }

        private List<List<PointF>> MakeConvex(List<PointF> shape)
        {
            List<List<PointF>> output = new List<List<PointF>>();

            List<PointF> main = new List<PointF>(shape);

            int last = 0;

            for (int i = 0; i < shape.Count; i++)
            {
                PointF p1 = shape[i];
                PointF p2 = shape[(i + 1) % shape.Count];
                PointF p3 = shape[(i + 2) % shape.Count];

                PointF d1 = new PointF(p2.X - p1.X, p2.Y - p1.Y);
                PointF d2 = new PointF(p3.X - p2.X, p3.Y - p2.Y);

                float cross = Helper.CrossProduct(d1, d2);

                if (cross < 0) //if it turns counter-clockwise
                {
                    List<PointF> points;
                    if(i - last > 3)
                    {
                        points = new List<PointF>(shape.GetRange(last, i - last));

                        //Only consider points that are within our shape (CC shapes will be outside)
                        if (isClockWiseConvex(points))
                        {
                            //add the points we missed (they should all have clockwise turns)
                            output.Add(points);

                            //remove the excess from main (we don't include start and end points because they're needed)
                            for (int j = 1; j < points.Count - 1; j++)
                            {
                                if (main.Contains(points[j])) main.Remove(points[j]);
                            }
                        }
                        
                    }

                    points = new List<PointF>() { shape[(i - 1 + shape.Count) % shape.Count], p1, p2 };
                    //Only consider points that are within our shape (CC shapes will be outside)
                    if (isClockWiseConvex(points))
                    {
                        //add the "ear"
                        output.Add(points);

                        //remove the trouble point from main
                        if (main.Contains(p1)) main.Remove(p1);

                        //mark last so we can later add the points that weren't troublesome
                        last = i;
                    }
                }
            }

            //only add main if it is a shape
            if (main.Count >= 3)
            {
                output.Add(main);
            }

            //make sure all of the shapes we generated were convex
            for (int i = output.Count - 1; i >= 0; i--)
            {
                //if the shape is not convex
                if (!isClockWiseConvex(output[i]))
                {
                    //break it apart into convex shapes and add those shapes to the list
                    output.AddRange(MakeConvex(output[i]));

                    //remove the concave shape
                    output.RemoveAt(i);
                }
            }

            return output;
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

        //check if there are any lists with the same content
        private bool hasDuplicates(List<List<PointF>> mainList)
        {
            for(int i = 0; i < mainList.Count; i++)
            {
                for (int j = i+1; j < mainList.Count; j++)
                {
                    if (areDuplicates(mainList[i], mainList[j])) return true;
                }
            }

            return false;
        }

        //assumes no duplicate points in a single list
        private bool areDuplicates(List<PointF> list1, List<PointF> list2)
        {
            if (list1.Count != list2.Count) return false;

            //go through list1
            for(int i = 0; i < list1.Count; i++)
            {
                bool foundMatch = false;

                //go through list2
                for (int j = 0; j < list2.Count; j++)
                {
                    //if the X's and Y's are the same
                    if(list1[i].X == list2[j].X && list1[i].Y == list2[j].Y)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                //didn't find a match, can't be duplicates
                if (!foundMatch) return false;
            }

            return true;
        }
    }
}
