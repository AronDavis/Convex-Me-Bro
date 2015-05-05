using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConvexMeBro
{
    static class Helper
    {
        public static float CrossProduct(PointF p1, PointF p2)
        {
            return (p1.X * p2.Y) - (p1.Y * p2.X);
        }
    }
}
