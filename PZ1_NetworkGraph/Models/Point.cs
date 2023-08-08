using System;

namespace Models
{
	public class Point
    {
        private double x;
        private double y;

        public Point()
        {

        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

		public override bool Equals(object obj)
		{
			Point point = (Point)obj;
			return (point.X == X) && (point.Y == Y);
		}

		public bool IsTooClose(Point point, double allowed)
		{
			if((Math.Abs(point.X - this.X) < allowed) && (Math.Abs(point.Y - this.Y) < allowed))
			{
				return true;
			}

			return false;
		}
	}
}
