using System.Collections.Generic;

public static class GitStatusParser
{
	public static List<GitFileEntry> Parse(string statusOutput)
	{
		var entries = new List<GitFileEntry>();
		foreach (string line in statusOutput.Split('\n'))
		{
			if (line.Length < 4) continue;
			var entry = new GitFileEntry
			{
				IsStaged = line[0] != ' ' && line[0] != '?',
				Status = ParseStatus(line),
				Path = line.Substring(3).Trim()
			};
			entries.Add(entry);
		}
		return entries;
	}

	private static GitFileStatus ParseStatus(string line)
	{
		return line switch
		{
			string s when s.StartsWith("??") => GitFileStatus.Untracked,
			string s when s.StartsWith(" M") => GitFileStatus.Modified,
			string s when s.StartsWith("A ") || s.StartsWith(" A") => GitFileStatus.Added,
			string s when s.StartsWith("D ") || s.StartsWith(" D") => GitFileStatus.Deleted,
			string s when s.StartsWith("R ") || s.StartsWith(" R") => GitFileStatus.Renamed,
			_ => GitFileStatus.Unknown
		};
	}
}