using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    class Circle
    {
        private List<Fragment> Fragments;
        public double Radius { get; set; }
        private Point center;
        /// <summary>
        /// Designer constructor.
        /// </summary>

        public Circle(List<Fragment> Fragments, double radius, Point center)
        {
            this.Fragments = Fragments;
            this.Radius = radius;
            this.center = center;

        }
        /// <summary>
        /// do something....
        /// </summary>
        public List<Point> computeFragmentEndPoints ()
        {
            List<Point> pointsList = new List<Point>();
            List<long> seqLen = new List<long>();
            seqLen.Add(0);
            long fragLen = 0;
            long construct =0;
            if (Fragments.Count !=0)
            {
                foreach (Fragment i in Fragments)
                {
                    fragLen = i.Sequence.Count;
                    construct+=fragLen; 
                }
                
                seqLen.Add(fragLen);
            }
            foreach(long i in seqLen)
            {
                double alpha = ((double)i / (double)construct)* 2*Math.PI;
                double x= Radius * Math.Sin(alpha) + center.X;
                double y= Radius * Math.Cos(alpha) + center.Y;
                pointsList.Add(new Point((int)x,(int)y));
            }
            
            return pointsList;
        }
    }
}
