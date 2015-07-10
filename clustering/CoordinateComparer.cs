using System;
using System.Collections.Generic;

namespace clustering
{
	public class CoordinateComparer : IComparer<Point>
	{
		public CoordinateComparer ()
		{
			
		}

		#region IComparer implementation

		int IComparer<Point>.Compare (Point obj1, Point obj2)
		{
			if (obj1.X.Equals(obj2.X))
				return (int)(obj1.Y - obj2.Y);
			else
				return (int)(obj1.X - obj2.X);
		}

		#endregion
	}
}

