public enum GitFileStatus
{
	Modified, Added, Deleted, Renamed, Untracked, Unknown
}

public class GitFileEntry
{
	public string Path;
	public GitFileStatus Status;
	public bool IsStaged;
}
