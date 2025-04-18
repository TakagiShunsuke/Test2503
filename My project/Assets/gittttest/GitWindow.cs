/*=====
<CodingRule.cs>	// スクリプト名
└作成者：takagi

＞内容
Git用のウィンドウ

＞更新履歴

__Y25
_M04
D
03:新チーム用にコードを刷新:takagi
=====*/

// 名前空間宣言
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// クラス定義
public class GitWindow : EditorWindow
{
	// 列挙定義
	public enum E_TAB	// ウィンドウタブ
	{
		E_TAB_,	
		E_TAB_HISTORY,	// ブランチ更新履歴
	};

	// 定数定義
	private const string TAB_NAME = "Git Panel";

	// 変数宣言
	private List<GitFileEntry> entries;
	private string commitMessage = "";
	private Vector2 scroll;
	private List<GitLogEntry> logEntries;

	[MenuItem("Tools/" + TAB_NAME)]
	public static void Open() => GetWindow<GitWindow>("Git");

	private void OnEnable() => RefreshAll();
	E_TAB toolbarInt = 0;



	Dictionary<E_TAB, string> TabInfo = new Dictionary<E_TAB, string>{
		{ E_TAB.E_TAB_, "" },
		{ E_TAB.E_TAB_HISTORY,  "Commit History" },	// 
	};

	string[] toolbarStrings = { "Toolbar1", "Changed File", "Toolbar3" };
	//Dictionary<string, delegate*<void>> toolbarStrings = new Dictionary<string, delegate*<void>>{
	//	{ "Toolbar1" }
	//	, { "Changed File" }
	//	, {"Toolbar3" }
	//};

	private void DisplayHistoryTab()
	{
		GUILayout.Label("📜 Commit History", EditorStyles.boldLabel);
		foreach (var log in logEntries)
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label($"{log.Hash.Substring(0, 7)} - {log.Message.Trim()}");
			GUILayout.Label($"Author: {log.Author}");
			GUILayout.Label($"Date: {log.Date}");
			GUILayout.EndVertical();
		}
	}

	private void OnGUI()
	{
		var temp = GUILayout.Toolbar((int)toolbarInt, TabInfo.Values.ToArray<string>());	// タブが表示される関数でもあるため戻り値を別領域に退避する
		if (Enum.IsDefined(typeof(E_TAB), temp))
		{
			toolbarInt = (E_TAB)temp;
		}
		else
		{
			Debug.LogError(TAB_NAME + "：タブに対応する列挙が存在しません");
		}
		switch (toolbarInt)
		{
			case 0:
				break;
			case E_TAB.E_TAB_HISTORY:
				DisplayHistoryTab();
				break;
			default:
				Debug.LogError(TAB_NAME + "：対応外のタブが選ばれました");
				break;
		}



		if (toolbarInt == 0)
		{



			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("🔄 Refresh")) RefreshAll();
			if (GUILayout.Button("⬇️ Pull")) GitCommand.Run("pull", out _, null);
			if (GUILayout.Button("⬆️ Push")) GitCommand.Run("push", out _, null);
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.Label("📂 Changed Files", EditorStyles.boldLabel);
			scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(200));

			foreach (var entry in entries)
			{
				GUILayout.BeginHorizontal();
				entry.IsStaged = GUILayout.Toggle(entry.IsStaged, entry.Path);
				GUILayout.Label($"[{entry.Status}]", GUILayout.Width(80));
				GUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();

			GUILayout.Space(10);
			GUILayout.Label("📝 Commit Message", EditorStyles.boldLabel);
			commitMessage = EditorGUILayout.TextField(commitMessage);

			EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(commitMessage));
			if (GUILayout.Button("Commit"))
			{
				foreach (var file in entries.Where(e => e.IsStaged))
					GitCommand.Run($"add \"{file.Path}\"", out _, null);

				GitCommand.Run($"commit -m \"{commitMessage}\"", out var err, null);
				if (!string.IsNullOrEmpty(err))
					Debug.LogError(err);
				commitMessage = "";
				RefreshAll();
			}
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(10);
		}

	}

	private void RefreshAll()
	{
		var status = GitCommand.Run("status --porcelain", out _, null);
		entries = GitStatusParser.Parse(status);
		var log = GitCommand.Run("log --oneline --decorate --pretty=medium -n 10", out _, null);
		logEntries = GitLogParser.Parse(log);
	}
}





//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//public class GitWindow : EditorWindow
//{
//    private List<GitFileEntry> entries;
//    private string commitMessage = "";
//    private Vector2 scroll;
//    private List<GitLogEntry> logEntries;

//    [MenuItem("Tools/Git Panel")]
//    public static void Open() => GetWindow<GitWindow>("Git");

//    private void OnEnable() => RefreshAll();

//    private void OnGUI()
//    {
//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("🔄 Refresh")) RefreshAll();
//        if (GUILayout.Button("⬇️ Pull")) GitCommand.Run("pull", out _, null);
//        if (GUILayout.Button("⬆️ Push")) GitCommand.Run("push", out _, null);
//        EditorGUILayout.EndHorizontal();

//        GUILayout.Space(10);
//        GUILayout.Label("📂 Changed Files", EditorStyles.boldLabel);
//        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(200));

//        foreach (var entry in entries)
//        {
//            GUILayout.BeginHorizontal();
//            entry.IsStaged = GUILayout.Toggle(entry.IsStaged, entry.Path);
//            GUILayout.Label($"[{entry.Status}]", GUILayout.Width(80));
//            GUILayout.EndHorizontal();
//        }

//        EditorGUILayout.EndScrollView();

//        GUILayout.Space(10);
//        GUILayout.Label("📝 Commit Message", EditorStyles.boldLabel);
//        commitMessage = EditorGUILayout.TextField(commitMessage);

//        EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(commitMessage));
//        if (GUILayout.Button("Commit"))
//        {
//            foreach (var file in entries.Where(e => e.IsStaged))
//                GitCommand.Run($"add \"{file.Path}\"", out _, null);

//            GitCommand.Run($"commit -m \"{commitMessage}\"", out var err, null);
//            if (!string.IsNullOrEmpty(err))
//                Debug.LogError(err);
//            commitMessage = "";
//            RefreshAll();
//        }
//        EditorGUI.EndDisabledGroup();

//        GUILayout.Space(10);
//        GUILayout.Label("📜 Commit History", EditorStyles.boldLabel);
//        foreach (var log in logEntries)
//        {
//            GUILayout.BeginVertical("box");
//            GUILayout.Label($"{log.Hash.Substring(0, 7)} - {log.Message.Trim()}");
//            GUILayout.Label($"Author: {log.Author}");
//            GUILayout.Label($"Date: {log.Date}");
//            GUILayout.EndVertical();
//        }
//    }

//    private void RefreshAll()
//    {
//        var status = GitCommand.Run("status --porcelain", out _, null);
//        entries = GitStatusParser.Parse(status);
//        var log = GitCommand.Run("log --oneline --decorate --pretty=medium -n 10", out _, null);
//        logEntries = GitLogParser.Parse(log);
//    }
//}
