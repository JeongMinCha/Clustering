using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace clustering
{
	public class AGNES
	{
		private List<Cluster> clusterList;

		private DataBase db = null;
		public DataBase DB
		{
			set {
				db = value;
			}
		}

		public AGNES ()
		{
		}

		public void Clustering (string inputFile, int clusterNum)
		{
			int clusterIndex = 0;
			var clusterOf = new int[db.Count()];		// cluster indexes of points
			int curCluster = db.Count();

			foreach (var pair in db.DistanceSortedList())
			{
				int p1 = pair.Key.id1;
				int p2 = pair.Key.id2;
				if (curCluster == clusterNum + 2)
					break;

				// If clusterOf[p1] == 0, p1 is not included into any cluster.
				if (clusterOf[p1] == 0 && clusterOf[p2] == 0)
				{
					++ clusterIndex;
					clusterOf[p1] = clusterIndex;
					clusterOf[p2] = clusterIndex;
				}
				else if (clusterOf[p1] != 0 && clusterOf[p2] == 0)
				{
					clusterOf[p2] = clusterOf[p1];
				}
				else if (clusterOf[p1] == 0 && clusterOf[p2] != 0)
				{
					clusterOf[p1] = clusterOf[p2];
				}
				else if (clusterOf[p1] != 0 && clusterOf[p2] != 0)
				{
					int clusterIndex1 = clusterOf[p1];
					int clusterIndex2 = clusterOf[p2];

					if (clusterIndex1 == clusterIndex2) {
						// no change
						continue;
					}

					int minIndex = (clusterIndex1 < clusterIndex2) ? clusterIndex1 : clusterIndex2;
					int maxIndex = (clusterIndex1 > clusterIndex2) ? clusterIndex1 : clusterIndex2;

					for (int i=0; i<clusterOf.Length; ++i)
					{
						if (clusterOf[i] == maxIndex)
						{
							clusterOf[i] = minIndex;
						}
					}
				}
				-- curCluster;
			}

			HashSet<int> hSet = new HashSet<int>();
			foreach (int idx in clusterOf)
			{
				hSet.Add(idx);
			}

			clusterList = new List<Cluster>();
			foreach (int clustIdx in hSet)
			{
				Cluster cluster = new Cluster();
				for (int i=0; i<clusterOf.Length; ++i)
				{
					if (clusterOf[i] == clustIdx) 
					{
						cluster.Add(db[i]);
					}
				}
				clusterList.Add(cluster);
			}

			WriteOutput(inputFile);
		}

		public void WriteOutput(string inputFile)
		{
			Console.WriteLine("# of clusters: " + clusterList.Count);

			char[] array = inputFile.ToCharArray();
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

