using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace clustering
{
	public class PAM
	{
		private DataBase db = null;
		public DataBase DB
		{
			set { db = value; }
		}
			
		private List<Cluster> clusterList;

		public PAM()
		{
		}

		private List<Point> AssignRandomSeeds (int clusterNum)
		{
			clusterList = new List<Cluster>();
			var randomSeeds = RandomPoints(clusterNum);
			for(int i=0; i<clusterNum; ++i)
			{
				Point seed = randomSeeds[i];
				seed.inCluster = true;
				seed.isSeed = true;
				seed.clusterIdx = i;

				Cluster cluster = new Cluster();
				cluster.Add(seed);
				clusterList.Add(cluster);
			}

			return randomSeeds;
		}


		/// <summary>
		/// Returns a non-represntative object
		/// </summary>
		private Point RandomExcept (List<Point> repSet)
		{
			int limit = db.Count();
			Random r = new Random(DateTime.Now.Millisecond);

			int idx = r.Next(limit);
			while (repSet.Contains(db[idx]))
			{
				idx= r.Next(limit);
			}
			return db[idx];
		}

		// oldSeed = a represntative, newSeed = non-representative
		/// <summary> Returns total cost </summary>
//		private float TotalCost (List<Point> reps, Point oldSeed, Point newSeed)
//		{
//			float total = 0;
//			for (int j=0; j<db.Count(); ++j)
//			{
//				Point pj = db[j];
//
//				// newSeed in oldSeed's cluster.
//				if (oldSeed.clusterIdx == newSeed.clusterIdx)
//				{
//					// oldSeed is not the representative of cluster of pj
//					if (oldSeed.Equals(reps[pj.clusterIdx]) == false)
//					{
//						total += db.Distance(pj, newSeed) - db.Distance(pj, oldSeed);
//					}
//					else
//					{
//						total += 0;
//					}
//
//				}
//
//
//			}

//			if (oldSeed.clusterIdx == newSeed.clusterIdx)
//			{
//				
//			}
//		}

		public void Clustering(string inputFile, int clusterNum)
		{
			bool change = false;
			var reps = AssignRandomSeeds(clusterNum);	// representatives
			var distances = new float[clusterNum];

			do
			{
				// assign each remaining object to the nearest cluster.
				for (int i=0; i<db.Count(); ++i)
				{
					Point p = db[i];
					if (p.inCluster == false)
					{
						float minDistance = db.Distance(reps[0], p);
						int nearestCluster = 0;
						for(int j=1; j<clusterNum; ++j)
						{
							float distance = db.Distance(reps[j], p);
							if (minDistance > distance)
							{
								minDistance = distance;
								nearestCluster = j;
							}
						}
						clusterList[nearestCluster].Add(p);
						p.inCluster = true;
						p.clusterIdx = nearestCluster;
					}
				}
				// randomly select a nonrepresentative object, o(random);
				Point randPoint = RandomExcept(reps);
				for (int j=0; j<clusterNum; ++j)
				{
//					float totalCost = TotalCost(reps[j], randPoint);
				}
			} 
			while (change == false);
		}

		private List<Point> RandomPoints (int num)
		{
			var pointList = new List<Point>();
			int limit = db.Count();
			Random r = new Random(DateTime.Now.Millisecond);

			var indexList = new HashSet<int>();
			while (indexList.Count != num)
			{
				int rand = r.Next(limit);
				indexList.Add(rand);
				Console.Write(rand + ", ");
			}

			foreach (int index in indexList)
				pointList.Add(db[index]);

			return pointList;
		}

		/// <summary> Remove clusters with smallest # of points</summary>
		public void TrimClusters(int clusterNum)
		{
			while (clusterList.Count > clusterNum)
			{
				int min = clusterList[0].Count;
				int minIndex = 0;

				for (int i=0; i<clusterList.Count; ++i)
				{
					Cluster c = clusterList[i];
					if (min > c.Count)
					{
						min = c.Count;
						minIndex = i;
					}
				}

				clusterList.RemoveAt(minIndex);
			}
		}

		private void ExpandCluster (Point p, Cluster c, List<Point> neighbors)
		{

		}

		public void WriteOutput (string inputFile)
		{
			Console.WriteLine("# of clusters: " + clusterList.Count);

			char[] array = inputFile.ToArray();
			int inputNum = (int)Char.GetNumericValue(array[5]);

			for (int i=0; i<clusterList.Count; ++i)
			{
				var writer = new StreamWriter("output" + inputNum + "_cluster_" + i + ".txt");
				Cluster cluster = clusterList[i];
				foreach (Point p in cluster)
				{
					writer.WriteLine(p.ID.ToString());
				}
				writer.Close();
			}
		}
	}
}