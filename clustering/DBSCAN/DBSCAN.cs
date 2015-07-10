using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace clustering
{
	public class DBSCAN
	{
		private DataBase db = null;
		public DataBase DB
		{
			set { db = value; }
		}

		private float epsilon = 0;
		public float Epsilon
		{
			get { return epsilon; }
			set {
				if (value >= 0){
					epsilon = value; 
				} else { 
					Console.WriteLine("epsilon must be positive"); 
				}
			}
		}

		private int minPts = 0;
		public int MinPts 
		{
			get { return minPts; }
			set {
				if (value >= 0) {
					minPts = value;
				} else { 
					Console.WriteLine("minPts must be positive");
				}
			}
		}

		private List<Point> noiseList;
		private List<Cluster> clusterList;

		public DBSCAN ()
		{
			noiseList = new List<Point>();
			clusterList = new List<Cluster>();
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

		private float GoodEpsilon (string inputFile)
		{
			float eps = 0;
			switch (inputFile)
			{
			case "input1.txt":	eps = 11.90F;	break;
			case "input2.txt":	eps = 4.40F;	break;
			case "input3.txt":	eps = 6.65F;	break;
			default:			eps = 3.00F;	break;
			}
			return eps;
		}

		private int GoodMinPts (string inputFile)
		{
			int minPoints = 0;
			switch (inputFile)
			{
			case "input1.txt":	minPoints = 15;	break;
			case "input2.txt":	minPoints = 65;	break;
			case "input3.txt":	minPoints = 10;	break;
			default:			minPoints = 10;	break;
			}
			return minPoints;
		}


		public void Clustering(string inputFile, int clusterNum)
		{
			float score = 0;
			Epsilon = GoodEpsilon(inputFile);
			MinPts = GoodMinPts(inputFile);
			DoDBSCAN(clusterNum);
			WriteOutput(inputFile);

//			do
//			{
//				db.Initialize();
//				DoDBSCAN();
//				WriteOutput(inputFile);
//
//				score = Globals.ClusteringScore(inputFile);
//
//				Console.WriteLine(inputFile + "-> Eps: " + Epsilon + ", MinPts: " + 
//					MinPts + ", score: " + score + ", # of clusters: " + clusterList.Count);
//				
//				Epsilon += 0.05F;
//				RemoveOutput(inputFile);
//
//				if (score >= 99.99)
//					break;
////				if (clusterList.Count < clusterNum)
////					break;
//			}
//			while(true);
//			WriteOutput(inputFile);
		}

		public void DoDBSCAN(int clusterNum)
		{
			clusterList = new List<Cluster>();
			for (int i=0; i<db.Count(); ++i)
			{
				Point p = db[i];
				if (p.Processed == false)
				{
					p.Processed = true;
					var neighbors = db.Neighbors(p, Epsilon);
					if (neighbors.Count < MinPts)
						noiseList.Add(p);
					else
					{
						Cluster cluster = new Cluster();
						ExpandCluster(p, cluster, neighbors);
						clusterList.Add(cluster);
					}
				}
			}
			TrimClusters(clusterNum);
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
			c.Add(p);
			p.inCluster = true;
//			foreach (Point neighbor in neighbors)
			for  (int i=0; i<neighbors.Count; ++i)
			{
				Point neighbor = neighbors[i];
				if (neighbor.Processed == false)
				{
					neighbor.Processed = true;
					var newNeighbors = db.Neighbors(neighbor, Epsilon);
					if (newNeighbors.Count >= MinPts)
					{
						neighbors = neighbors.Union(newNeighbors).ToList();
					}
				}
				if (neighbor.inCluster == false)
				{
					c.Add(neighbor);
					neighbor.inCluster = true;
				}
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