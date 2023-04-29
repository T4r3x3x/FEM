﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{   
    internal class Point
    {
        public double x,y;
        public Point (double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        internal void Print()
        {
            Console.WriteLine(x + " " +  y);
        }


        public static Point operator *(Point point, double a)
        {
            Point result;

            result = new Point(point.x*a, point.y*a);

            return result;
        }
    }
}
