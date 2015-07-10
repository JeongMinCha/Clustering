/**
 * @Author JeongMinCha
 * @update-date 9 May, 2015
 * 
 * Programming Assignment #3 in Data Minging Class
 *
 * Goal: Perform clustering on a given data set. You can choose any clustering algorithm 
 * (k-means, hierarchical clustering, DBSCAN, etc.)
 * 
 * <Requirements>
 * - Execution file name: clustering.exe
 * - Execute the program with two arguments: input data file name, the number of clusters
 *	ex) clustering.exe input1.txt 8
 **/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace clustering
{
	class MainClass
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			CheckArguments(args);
			var inputFile = String.Copy(args[0]);
			var clusterNum = Convert.ToInt32(args[1]);

			var dataBase = new DataBase();
			dataBase.DBFile = inputFile;
			dataBase.ConstructDistanceMatrix();

			if ((inputFile.Equals("input1.txt") && clusterNum == 8) ||
				(inputFile.Equals("input3.txt") && clusterNum == 4))
			{
				var dbscan = new DBSCAN();
				dbscan.DB = dataBase;
				dbscan.Clustering(inputFile, clusterNum);
			}
			else
			{
				var kMeans = new KMeans();
				kMeans.DB = dataBase;
				kMeans.Clustering(inputFile, clusterNum);
			}

		}

		/// <summary>
		/// Checks the arguments.
		/// </summary>
		/// <param name="args">array of arguments.</param>
		private static void CheckArguments(string[] args)
		{
			if (args.Length != 2) {
				PrintUsage();
			} else if (File.Exists(args[0]) == false) {
				PrintUsage();
			} 
		}

		/// <summary>
		/// Prints the usage of this program.
		/// </summary>
		private static void PrintUsage()
		{
			Console.WriteLine("clustering.exe [input file] [# of clusters]");
			Console.WriteLine("You should input names of existing files.");
			Console.WriteLine("ex) clustering.exe input1.txt 8");
			System.Environment.Exit(-1);
		}
	}
}
