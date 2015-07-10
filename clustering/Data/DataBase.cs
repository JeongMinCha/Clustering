using System;
using System.Collections.Generic;
using System.Text;


namespace clustering
{
	public class DataBase
	{
		#region Meber Variables
		/// <summary> list of points stored in database. </summary>
		private List<Point> list = null;

		/// <summary> name of db file (input file) </summary>
		private string dbFile = null;
		public string DBFile
		{	
			get { return dbFile; }
			set {
				dbFile = value;
				ConstructDataBase();
			}
		}

		/// <summary> 
		/// The data structure storing distances between
		/// all points stored in this database instance. 
		/// </summary>
		private DistanceMatrix distance;
		#endregion

		/// <summary> Returns point stored at given index. </summary>
		/// <param name="index">Index.</param>
		public Point this [int index]
		{
			get { return list[index]; }
		}
		
		#region Constructor
		public DataBase ()
		{
			list = new List<Point>();
			distance = new DistanceMatrix();
		}
		#endregion

		public void ConstructDistanceMatrix ()
		{
			distance.NumOfRows = list.Count;
			distance.ConstructMatrix(this);
		}

		public float AverageDistance ()
		{
			float total = 0;
			int count = 0;

			for (int i=0; i<list.Count; ++i)
			{
				for (int j=0; j<i; ++j)
				{
					total += Distance(i, j);
					++count;
				}
			}

			return total / count;
		}

		public List<KeyValuePair<PointPair, float>> DistanceSortedList ()
		{
			distance.ConstructSortedDictionary(this);
			return distance.SortedDistList();
		}

		public float Distance (Point pt1, Point pt2)
		{
			return distance [pt1.ID, pt2.ID];
		}

		public float Distance (int id1, int id2)
		{
			return distance [id1, id2];
		}

		public int Count()
		{
			return list.Count;
		}

		/// <summary>
		/// Read input file and construct the contents of database from the file.
		/// </summary>
		private void ConstructDataBase ()
		{
			string[] lines = System.IO.File.ReadAllLines(dbFile);
			foreach (string line in lines)
			{
				string[] words = line.Split('\t');
				if (words.Length == 3) {
					int id = Convert.ToInt32(words[0]);
					float x = Convert.ToSingle(words[1]);
					float y = Convert.ToSingle(words[2]);

					Point pt = new Point(id, x, y);
					list.Add(pt);
				} else {
					Console.WriteLine("The format of the input file is wrong.");
					System.Environment.Exit(-1);
				}
			}
			list.TrimExcess();
		}

		public void Initialize ()
		{
			list = new List<Point>();
			ConstructDataBase();
		}

		/// <summary>
		/// list of neighbors 
		/// </summary>
		/// <param name="pt">Point.</param>
		/// <param name="epsilon">Epsilon.</param>
		public List<Point> Neighbors (Point pt, float epsilon)
		{
			var neighbors = new List<Point>();
			for (int i=0, length=list.Count; i<length; ++i)
			{
				if (Distance(list[i], pt) <= epsilon)
					neighbors.Add(list[i]);
			}
			return neighbors;
		}

		public List<Point> Neighbors (int id, float epsilon)
		{
			var neighbors = new List<Point>();
			for (int i=0, length=list.Count; i<length; ++i)
			{
				if (Distance(list[i], list[id]) <= epsilon)
					neighbors.Add(list[i]);
			}
			return neighbors;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
//			sb.Append("# of points is ").Append(list.Count.ToString()).AppendLine();
			foreach (Point pt in list)
			{
				sb.Append(pt.ToString()).AppendLine();
			}
			return sb.ToString();
		}
	}
}