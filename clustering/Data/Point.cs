using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace clustering
{
	public class Point : IIdenticable, IComparable<Point>, IPriorityEntity<Point>
	{
		#region IPriorityEntity implementation
//		private float priority = 0;
		public float Priority {
			get {
				return reachDistance;
			}
			set {
				reachDistance = value;
			}
		}

		#endregion

		#region Member Variables
		private int id = 0;
		/// <summary> x-axis coordinate value </summary>
		public int ID { get { return id; } }

		private float x = 0;
		/// <summary> x-axis coordinate value </summary>
		public float X { get { return x; } }

		private float y = 0;
		/// <summary> y-axis coordinate value </summary>
		public float Y { get { return y; } }

		public int clusterIdx;

		/// <summary> 
		/// Says if this point is a representative of any cluster. 
		/// </summary>
		public bool isSeed;

		/// <summary> Says if this point is in any cluster. </summary>
		public Boolean inCluster;
			
		/// <summary> Says if this point is processed or not. </summary>
		public Boolean Processed;

		/// <summary> The reachability distance of this point. </summary>
		private float reachDistance = 0;
		public float ReachDistance
		{
			get { return reachDistance; }
			set {
				reachDistance = value;
			}
		}

		/// <summary> The core distance of this point. </summary>
		private float coreDistance = 0;
		public float CoreDistance { get { return coreDistance; } }
		#endregion

		#region Constructor
		public Point (int ID, float X, float Y)
		{
			id = ID;
			x = X;
			y = Y;

			ReachDistance = Globals.Undefined;
			coreDistance = Globals.Undefined;
		}
		#endregion

		public void SetCoreDistance (List<Point> neighbors, float epsilon, int minPts)
		{
			if (neighbors.Count < minPts)
			{
				coreDistance = Globals.Undefined;
				return;
			}

			var distanceSet = new SortedSet<float>();
			foreach (Point neighbor in neighbors)
			{
				distanceSet.Add(Distance(neighbor));
			}
			// set minPts-th least distance 
			coreDistance = distanceSet.ElementAt(minPts-1);
		}
			
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(ID.ToString()).Append(", ");
			sb.Append(X.ToString()).Append(", ");
			sb.Append(Y.ToString());
			return sb.ToString();
		}

		public float Distance (Point pt)
		{
			if (this.ID == pt.ID)
				return 0;

			float diff = 0;
			diff += (this.X-pt.X)*(this.X-pt.X);
			diff += (this.Y-pt.Y)*(this.Y-pt.Y);
			return (float) Math.Sqrt(diff);
		}

		public static float Distance (Point pt1, Point pt2)
		{
			if (pt1.ID == pt2.ID)
				return 0;

			float diff = 0;
			diff += (pt1.X-pt2.X)*(pt1.X-pt2.X);
			diff += (pt1.Y-pt2.Y)*(pt1.Y-pt2.Y);
			return (float) Math.Sqrt(diff);
		}

		#region IComparable implementation
		int IComparable<Point>.CompareTo (Point other)
		{
			if (this.Priority > other.Priority)
				return 1;
			else if (this.Priority < other.Priority)
				return -1;
			else
				return 0;
		}
		#endregion
	}
}