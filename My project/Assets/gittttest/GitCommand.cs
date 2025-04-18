using System.Diagnostics;
using UnityEngine;

public static class GitCommand
{
	public static string Run(string args, out string err, string workingDir = null)
	{
		var psi = new ProcessStartInfo
		{
			FileName = "git",
			Arguments = args,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = workingDir ?? Application.dataPath + "/.."
		};

		using var process = Process.Start(psi);
		string output = process.StandardOutput.ReadToEnd();
		err = process.StandardError.ReadToEnd();
		process.WaitForExit();
		return output;
	}
}