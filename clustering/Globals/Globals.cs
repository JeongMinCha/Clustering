using System;
using System.Diagnostics;

namespace clustering
{
	public static class Globals
	{
		public static float Undefined = -1;

		public static float ClusteringScore (String inputFile)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "PA3.exe";
			psi.Arguments = inputFile.Replace(".txt", null) + " >> log.log";
			psi.RedirectStandardOutput = true;
			psi.UseShellExecute = false;

			Process proc = Process.Start(psi);
			proc.WaitForExit();

			string txt = proc.StandardOutput.ReadToEnd().Replace("점", null);
			return Convert.ToSingle(txt);
		}
	}
}

