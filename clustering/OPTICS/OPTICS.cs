using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace clustering
{
	public class OPTICS
	{

		#region Member Variables
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

		private List<Cluster> clusterList;
		private List<Point> orderedList;
		#endregion

		#region Constructor
		public OPTICS ()
		{
			db = null;
			epsilon = 0;
			minPts = 0;
		}
		#endregion

		public float AverageRDistance()
		{
			float total = 0;
			int count = 0;
			foreach (Point p in orderedList)
			{
				if (p.ReachDistance.Equals(Globals.Undefined) == false)
				{
					total += p.ReachDistance;
					++ count;
				}
			}

			return total / count;
		}

		public void WriteOutput(string inputFile)
		{
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

		public void Clustering (string inputFile, int clusterNum)
		{
			Epsilon = 5.00F;
			MinPts = 5;
			DoOptics();
			float distanceThreshold = AverageRDistance();
			int modelClusterNum;
			while (true)
			{
				modelClusterNum = SmallClustering(distanceThreshold);
				if (modelClusterNum <= clusterNum)
					break;
				distanceThreshold += 0.001F;
			}
			WriteOutput(inputFile);
		}

		public int SmallClustering (float distanceThreshold)
		{
			clusterList = new List<Cluster>();
			bool upState = true;

			foreach (Point p in orderedList)
			{
				if (p.ReachDistance.Equals(Globals.Undefined))
				{
					continue;
				}
				else if (p.ReachDistance < distanceThreshold)
				{
					if (upState == true) 
					{
						upState = false;
						clusterList.Add(new Cluster());
					}
					clusterList[clusterList.Count-1].Add(p);
				}
				else if (p.ReachDistance > distanceThreshold)
				{
					if (upState == false)
					{
						upState = true;
					}	
				}
			}
			clusterList.TrimExcess();
			return clusterList.Count;
		}

		public void DoOptics ()
		{
			orderedList = new List<Point>();	// orderedList open
			for (int i=0, length=db.Count(); i<length; ++i)
			{
				if (db[i].Processed == false)
				{
					ExpandClusterOrder(db[i]);
				}
			}
			orderedList.TrimExcess();			// orderedList close
		}	// END OF Clustering

		private void ExpandClusterOrder (Point p)
		{
			var neighbors = db.Neighbors(p, Epsilon);
			p.Processed = true;
			p.ReachDistance = Globals.Undefined;		// UNDEFINED
			p.SetCoreDistance(neighbors, Epsilon, MinPts);
			orderedList.Add(p);

			// core distance of pt is not undefined. (core object)
			if (p.CoreDistance.Equals(Globals.Undefined) == false)
			{
				var orderSeeds = new OrderSeeds();	// empty priority queue
				orderSeeds.update(neighbors, p);

				while (orderSeeds.Empty() == false)
				{
					var curPt = orderSeeds.Next();
					var newNeighbors = db.Neighbors(curPt, Epsilon);
					curPt.Processed = true;
					curPt.SetCoreDistance(newNeighbors, Epsilon, MinPts);
					orderedList.Add(curPt);

					// core distance of current point is not undefined. (core object)
					if (curPt.CoreDistance.Equals(Globals.Undefined) == false)
						orderSeeds.update(newNeighbors, curPt);
				}
			}
		}	// END OF ExpandClusterOrder

		public void PrintOutput()
		{
			var writer = new StreamWriter("./result.txt");
			foreach (Point p in orderedList)
			{
				writer.WriteLine(p.ToString() + ", r-dist: " + p.ReachDistance);
			}
			writer.Close();
		}
	}
}

