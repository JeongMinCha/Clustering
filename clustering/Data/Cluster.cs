using System;
using System.Collections.Generic;
using System.Text;

namespace clustering
{
	public class Cluster : List<Point>
	{
		HashSet<int> ids;
//		List<Point> points;

		public Cluster ()
		{
			ids = new HashSet<int>();
//			points = new List<Point>();
		}

		public void Add (Point p)
		{
			base.Add(p);
			ids.Add(p.ID);
		}

		public bool Contains (int id)
		{
			return ids.Contains(id);
		}

		public Point Centorid ()
		{
			float totalX = 0;
			float totalY = 0;
			int count = 0;

			foreach (Point p in this)
			{
				totalX += p.X;
				totalY += p.Y;
				++ count;
			}
//			Console.WriteLine(points.Count);
//			Console.WriteLine("centroid total x: " + totalX + ", y:" + totalY);

			float _x = totalX / (float)count;
			float _y = totalY / (float)count;
			Point point = new Point(-1, _x, _y);
			return point;
		}
	}
}

