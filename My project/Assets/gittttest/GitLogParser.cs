using System.Collections.Generic;

public class GitLogEntry
{
	public string Hash;
	public string Author;
	public string Date;
	public string Message;
}

public static class GitLogParser
{
	public static List<GitLogEntry> Parse(string logOutput)
	{
		var logs = new List<GitLogEntry>();
		var lines = logOutput.Split('\n');
		GitLogEntry current = null;

		foreach (var line in lines)
		{
			if (line.StartsWith("commit "))
			{
				if (current != null) logs.Add(current);
				current = new GitLogEntry { Hash = line.Substring(7).Trim() };
			}
			else if (line.StartsWith("Author:"))
				current.Author = line.Substring(7).Trim();
			else if (line.StartsWith("Date:"))
				current.Date = line.Substring(5).Trim();
			else if (!string.IsNullOrWhiteSpace(line))
				current.Message += line.Trim() + " ";
		}

		if (current != null) logs.Add(current);
		return logs;
	}
}
