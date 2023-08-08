using System;

namespace Models
{
	public class CoordinateScaleConverter
	{
		public double XMinValue { get; private set; } = 9999999999;

		public double XMaxValue { get; private set; } = -9999999999;

		public double YMinValue { get; private set; } = 9999999999;

		public double YMaxValue { get; private set; } = -9999999999;

		public double ScaleFactor { get; private set; }

		public double FixedTransition { get; set; } = 5; //20

		public double XResizeFactor { get; private set; } = 1;

		public double YResizeFactor { get; private set; } = 1;

		public void CalculateMinMax(Point point)
		{
			if (point.X > XMaxValue)
			{
				XMaxValue = point.X;
			}
			else if (point.X < XMinValue)
			{
				XMinValue = point.X;
			}

			if (point.Y > YMaxValue)
			{
				YMaxValue = point.Y;
			}
			else if (point.Y < YMinValue)
			{
				YMinValue = point.Y;
			}
		}

		public void CalculateScaleFactorAndProportion(double width, double height)
		{
			double XDifference = Math.Abs(XMaxValue - XMinValue);
			double YDifference = Math.Abs(YMaxValue - YMinValue);

			double difference = XDifference > YDifference ? XDifference : YDifference;
			double resolution = XDifference > YDifference ? width : height;

			ScaleFactor = resolution / difference;

			if (resolution == width)
			{
				if ((XDifference / YDifference) < (width / height))
				{
					YResizeFactor = (XDifference / YDifference) / (width / height);
					XResizeFactor = 1;
				}
			}
			else
			{
				if ((YDifference / XDifference) < (height / width))
				{
					XResizeFactor = (YDifference / XDifference) / (height / width);
					YResizeFactor = 1;
				}
			}
		}

		public void DoScalePoint(Point point)
		{
			point.X = Math.Ceiling((point.X - XMinValue) * ScaleFactor * XResizeFactor);// + FixedTransition);
			point.Y = Math.Ceiling((point.Y - YMinValue) * ScaleFactor * YResizeFactor);// + FixedTransition);
			
			if(300 / 2 > point.Y)//1240/2
			{
				point.Y += (300 / 2 - point.Y) * 2;//1240/2
			}
			else
			{
				point.Y -= (point.Y - 300 / 2) * 2;//1240/2
			}

			
		}
	}
}
