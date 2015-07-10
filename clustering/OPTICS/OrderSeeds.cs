using System;
using System.Collections;
using System.Collections.Generic;

namespace clustering
{
	public class OrderSeeds : PriorityQueue<Point>
	{
		#region Constructor
		public OrderSeeds () : base()
		{
		}
		#endregion

		public bool Empty()
		{
			return (base.Count() == 0);
		}

		public Point Next()
		{
			return Dequeue();
		}

		/// <summary> Update the specified neighbors and centerPoint. </summary>
		public void update (List<Point> neighbors, Point centerPoint)
		{
			float coreDist = centerPoint.CoreDistance;

			foreach (Point neighborPt in neighbors)
			{
				if (neighborPt.Processed == false)	// Not Processed.
				{
					// new reachable distance = max (cDist, centerPt.Distance(neighborPt))
					float newReachDist = Point.Distance(neighborPt, centerPoint);
					if (newReachDist < coreDist) {
						newReachDist = coreDist;
					}

					// If reachability distance is UNDEFINED
					if (neighborPt.ReachDistance.Equals(Globals.Undefined) == true)
					{
						Insert(neighborPt, newReachDist);
						neighborPt.ReachDistance = newReachDist;
					}
					// If already neighborPt is in OrderSeeds, check improvement
					else
					{
						if (newReachDist < neighborPt.ReachDistance)
						{
							UpdatePriority(neighborPt, newReachDist);
							neighborPt.ReachDistance = newReachDist;
						}
					}
				}
 			}
		}	// END of OrderSeeds::update
	}
}

