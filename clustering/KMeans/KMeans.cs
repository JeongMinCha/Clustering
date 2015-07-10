using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace clustering
{
	public class KMeans
	{
		private int k = 0;
		public int K
		{
			get { return k; }
			set {
				if (value > 0)
					k = value;
				else
					k = 0;
			}
		}

		private DataBase db = null;
		public DataBase DB
		{
			set { db = value; }
		}

		private int[] clusterIdx;
		private List<Cluster> clusterList;
		private List<Point> centroidList;

		public KMeans ()
		{
			
		}

		private Point RandomPoint (int clusterId)
		{
			var list = new List<int>();
			for (int i=0; i<clusterIdx.Length; ++i)
			{
				int idx = clusterIdx[i];
				if (idx == clusterId)
				{
					list.Add(i);
				}
			}
			Console.WriteLine(clusterId + " - #: " + list.Count);

			return RandomPoint(list);
		}

		private Point RandomPoint (List<int> list)
		{
			Random r = new Random(DateTime.Now.Millisecond);
			int idx = r.Next(list.Count);
			int id = list[idx];
			return db[id];
		}

		public void Clustering (string inputFile, int clusterNum)
		{
			float score = 0;
			float maxScore = 0;
			int experiment = 10;

			// 10 experiments to get max score.
			for (int i=0; i<experiment; ++i)
			{
				db.Initialize();
				SmallClustering(inputFile, clusterNum);
				score = Globals.ClusteringScore(inputFile);
				RemoveOutput(inputFile);
				if (maxScore < score)
					maxScore = score;
			}

			while(true)
			{
				db.Initialize();
				SmallClustering(inputFile, clusterNum);
				score = Globals.ClusteringScore(inputFile);
				if (score >= maxScore)
				{
					return;
				}
				RemoveOutput(inputFile);
			}
		}

		public void RemoveOutput (string inputFile)
		{
			char[] array = inputFile.ToArray();
			int inputNum = (int)Char.GetNumericValue(array[5]);

			for (int i=0; i<clusterList.Count; ++i)
			{
				File.Delete("output" + inputNum + "_cluster_" + i + ".txt");
			}
		}

		public void SmallClustering(string inputFile, int clusterNum)
		{
			bool change = true;

			// arbitrarily choose k objects from D as the initial cluster centers
			centroidList = RandomPoints(clusterNum);
			for (int i=0; i<clusterNum; ++i)
			{
				Point seed = centroidList[i];
				seed.inCluster = true;
				seed.clusterIdx = i+1;
			}

			do
			{
				change = false;
				// (re)assign each object to the cluster to which the object is the 
				// most similar, based on the mean value of the objects in the cluster;
				int length = db.Count();
				for (int i=0; i<length; ++i)
				{
					Point p = db[i];
					float minDistance = Point.Distance(centroidList[0], p);
					int nearestCentroid = 1;
					for(int j=1; j<clusterNum; ++j)
					{
						float distance = Point.Distance(centroidList[j], p);
						if (minDistance > distance)
						{
							minDistance = distance;
							nearestCentroid = j+1;
						}
					}
//					p.inCluster = true;
					if (nearestCentroid != p.clusterIdx)
					{
						p.clusterIdx = nearestCentroid;
						change = true;
					}
				}
				// update the cluster means, that is, calculate the mean value of 
				// the objects for each cluster
				clusterList = UpdateMeans();
			}
			while (change == true);

			WriteOutput(inputFile);
		}

		private List<Cluster> UpdateMeans()
		{
			int clusterNum = centroidList.Count;
			var tClusterList = new List<Cluster>();
			for (int i=0; i<clusterNum; ++i)
				tClusterList.Add(new Cluster());

			int length = db.Count();
			for (int i=0; i<length; ++i)
			{
				Point p = db[i];
				tClusterList[p.clusterIdx-1].Add(p);
			}

			for (int i=0; i<clusterNum; ++i)
			{
				centroidList[i] = tClusterList[i].Centorid();
			}

			return tClusterList;
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
			}

			foreach (int index in indexList)
			{
				pointList.Add(db[index]);
			}

			return pointList;
		}

		private void RandomPartition (int clusterNum)
		{
			Random r = new Random(DateTime.Now.Millisecond);
			for (int i=0; i<db.Count(); ++i)
			{
				clusterIdx[i] = r.Next(clusterNum);
			}
		}


		public void WriteOutput (string inputFile)
		{
//			Console.WriteLine("# of clusters: " + clusterList.Count);

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