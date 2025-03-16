using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class GitCommitPushUnifiedWrapper : EditorWindow
{
	string workingDirectory = "";
	string commitMessage = "Your commit message";
	string filesToAdd = "";

	// ここで Git の絶対パスを指定
	string gitExecutablePath = @"C:\Program Files\Git\bin\git.exe"; // インストール先によっては変更必要！

	[MenuItem("Tools/Git Commit & Push Unified")]
	public static void ShowWindow()
	{
		GetWindow<GitCommitPushUnifiedWrapper>("Git Commit & Push");
	}

	void OnGUI()
	{
		GUILayout.Label("Git Commit & Push Settings", EditorStyles.boldLabel);

		// 作業ディレクトリ
		GUILayout.Label("Working Directory:");
		workingDirectory = EditorGUILayout.TextField("Working Directory", string.IsNullOrEmpty(workingDirectory) ? Directory.GetParent(Directory.GetCurrentDirectory()).FullName : workingDirectory);

		// コミットメッセージ
		GUILayout.Label("Commit Message:");
		commitMessage = GUILayout.TextField(commitMessage);

		// コミット対象ファイル (空欄で全ファイル)
		GUILayout.Label("Files to Add (Leave blank for all):");
		filesToAdd = GUILayout.TextField(filesToAdd);

		GUILayout.Space(10);

		// ボタン
		if (GUILayout.Button("Commit Only"))
		{
			if (ValidateDirectory()) ExecuteGitCommit(workingDirectory, commitMessage, filesToAdd);
		}

		if (GUILayout.Button("Push Only"))
		{
			if (ValidateDirectory()) ExecuteGitPush(workingDirectory);
		}

		if (GUILayout.Button("Commit & Push"))
		{
			if (ValidateDirectory())
			{
				ExecuteGitCommit(workingDirectory, commitMessage, filesToAdd);
				ExecuteGitPush(workingDirectory);
			}
		}
	}

	bool ValidateDirectory()
	{
		if (string.IsNullOrEmpty(workingDirectory) || !Directory.Exists(workingDirectory))
		{
			UnityEngine.Debug.LogError("Invalid working directory.");
			return false;
		}
		return true;
	}

	void ExecuteGitCommit(string workingDir, string message, string files)
	{
		if (string.IsNullOrEmpty(files))
		{
			// 全ファイル追加
			RunGitCommand(workingDir, "add .", "Staging all changes");
		}
		else
		{
			// 指定ファイルのみ追加
			RunGitCommand(workingDir, $"add {files}", $"Staging {files}");
		}

		RunGitCommand(workingDir, $"commit -m \"{message}\"", "Committing changes");
		UnityEngine.Debug.Log("✅ Commit 完了");
	}

	void ExecuteGitPush(string workingDir)
	{
		RunGitCommand(workingDir, "push", "Pushing to remote");
		UnityEngine.Debug.Log("✅ Push 完了");
	}

	void RunGitCommand(string workingDir, string arguments, string actionDescription)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo(gitExecutablePath, arguments);
		startInfo.WorkingDirectory = workingDir;
		startInfo.RedirectStandardOutput = true;
		startInfo.RedirectStandardError = true;
		startInfo.UseShellExecute = false;
		startInfo.CreateNoWindow = true;

		using (Process process = new Process())
		{
			process.StartInfo = startInfo;
			process.Start();

			string output = process.StandardOutput.ReadToEnd();
			string error = process.StandardError.ReadToEnd();

			process.WaitForExit();

			UnityEngine.Debug.Log($"--- {actionDescription} ---");
			UnityEngine.Debug.Log(output);

			if (!string.IsNullOrEmpty(error))
			{
				UnityEngine.Debug.LogError(error);
			}
		}
	}
}



//using UnityEngine;
//using UnityEditor;
//using System.Diagnostics;
//using System.IO;

//public class GitCommandWrapper : EditorWindow
//{
//    private string gitArguments = "push";
//    private string workingDirectory = "";

//    [MenuItem("Tools/Git Command Wrapper")]
//    public static void ShowWindow()
//    {
//        GetWindow<GitCommandWrapper>("Git Command Wrapper");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Git Command Wrapper", EditorStyles.boldLabel);

//        // 作業ディレクトリ。通常はUnityプロジェクトのルートディレクトリ。
//        workingDirectory = EditorGUILayout.TextField("Working Directory", string.IsNullOrEmpty(workingDirectory) ? Directory.GetParent(Directory.GetCurrentDirectory()).FullName : workingDirectory);

//        // Gitに渡す引数（例："commit -m \"Update\"" や "push"）
//        gitArguments = EditorGUILayout.TextField("Git Arguments", gitArguments);

//        if (GUILayout.Button("Execute Git Command"))
//        {
//            ExecuteGitCommand(gitArguments, workingDirectory);
//        }
//    }

//    private void ExecuteGitCommand(string arguments, string workingDir)
//    {
//        try
//        {
//            ProcessStartInfo startInfo = new ProcessStartInfo();
//            startInfo.FileName = "git";  // システムのPATHにgitが含まれている必要があります
//            startInfo.Arguments = arguments;
//            startInfo.WorkingDirectory = workingDir;
//            startInfo.RedirectStandardOutput = true;
//            startInfo.RedirectStandardError = true;
//            startInfo.UseShellExecute = false;
//            startInfo.CreateNoWindow = true;

//            Process process = new Process();
//            process.StartInfo = startInfo;
//            process.Start();

//            // 結果を取得してUnityのコンソールに出力
//            string output = process.StandardOutput.ReadToEnd();
//            string error = process.StandardError.ReadToEnd();
//            process.WaitForExit();

//            if (!string.IsNullOrEmpty(output))
//                UnityEngine.Debug.Log("Git Output:\n" + output);
//            if (!string.IsNullOrEmpty(error))
//                UnityEngine.Debug.LogError("Git Error:\n" + error);
//        }
//        catch (System.Exception ex)
//        {
//            UnityEngine.Debug.LogError("Git Command Exception:\n" + ex.Message);
//        }
//    }
//}