using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace clustering
{
	public class DistanceMatrix
	{
		#region Member Variables
		/// <summary> The number of rows. </summary>
		private int numOfRows = 0;
		public int NumOfRows
		{
			get {
				return numOfRows;
			}
			set {
				numOfRows = value;
				for (int i=0; i<numOfRows; ++i)
				{
					float[] floats = new float[i];
					distanceList.Add(floats);
				}
				distanceList.TrimExcess();
			}
		}

		/// <summary> list of array of distances. It's like a biased matrix. </summary>
		List<float[]> distanceList = null;

		List<KeyValuePair<PointPair, float>> sortedDistList;
		#endregion

		/// <summary> Returns distance between two points with given IDs. </summary>
		public float this [int id1, int id2]
		{
			get {
				int a = id1, b = id2;
				if (id1 == id2)
					return 0;
				else if (id1 < id2) {
					a = id2; b = id1;
				}
				return distanceList[a][b];
			}
		}

		#region Constructor
		public DistanceMatrix ()
		{
			distanceList = new List<float[]>();
		}
		#endregion

		public void ConstructMatrix (DataBase db)
		{
			for (int i=0; i<numOfRows; ++i) {
				for (int j=0; j<i; ++j) {
					if (i != j) {
						distanceList[i][j] = Point.Distance(db[i], db[j]);
					}
				}
			}
		}

		public void ConstructSortedDictionary (DataBase db)
		{
			var sortedDistDict = new Dictionary<PointPair, float>();
			for (int i=0; i<NumOfRows; ++i)
			{
				for (int j=0; j<i; ++j)
				{
					if (i != j)
						sortedDistDict.Add(new PointPair(i, j), Point.Distance(db[i], db[j]));
				}
			}

			sortedDistList = sortedDistDict.ToList();
			sortedDistList.Sort((firstPair, nextPair) => 
			{
				return firstPair.Value.CompareTo(nextPair.Value);
			});
		}

		public List<KeyValuePair<PointPair, float>> SortedDistList()
		{
			return sortedDistList;
		}
	}
}