// Copyright (C)
// Author: Dylan Muller

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Test
{
    public class Hack : MonoBehaviour
{
	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	public KeyCode toggleKey = KeyCode.BackQuote;

	List<Log> logs = new List<Log>();
	Vector2 scrollPosition;
	bool show;
	bool speedhack;
	bool salt;
	bool collision;

	bool collapse;
	Vector3 position = new Vector3(0,0,0);

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	const int margin = 20;

	Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
	GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	void OnEnable ()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	void OnDisable ()
	{
		Application.RegisterLogCallback(null);
	}

	void Update ()
	{
		if (Input.GetKeyDown(toggleKey)) {
			show = !show;	
		}

		if(Input.GetKeyDown(KeyCode.F2)){
			Debug.Log("Toggle [Speedhack]");
			speedhack =  !speedhack;
			if(speedhack == true){
				Time.timeScale = 3;
			}else{
				Time.timeScale = 1;
			}
		}

		if(Input.GetKeyDown(KeyCode.F3)){
			Debug.Log("Toggle [NoCollision]");
			collision =  !collision;
			GameObject obj = GameObject.Find("Player Machine Root");
			Rigidbody rb = obj.GetComponent<Rigidbody>();
			if(collision == true){
				rb.detectCollisions = false;
			}else{
				rb.detectCollisions = true;
			}
		}

		if(Input.GetKeyDown(KeyCode.F1)){
			salt = !salt;
			Debug.Log("Toggle [Salt Mode]");
			GameObject obj = GameObject.Find("Player Machine Root");
            position = obj.transform.position;

				foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
			{
				if(gameObj.name == "centerGameObject")
				{
					GameObject parent = gameObj.transform.parent.gameObject;
					if(parent.name != "Player Machine Root"){
						MonoBehaviour[] comp = parent.GetComponents<MonoBehaviour>();
						foreach (MonoBehaviour c in comp){
							c.enabled = !salt;
					
						}
						Vector3 myposition = position;
						parent.transform.position = myposition;
					   
					}
					
				}
			}
		}	
			
	}

	void OnGUI ()
	{
		if (!show) {
			return;
		}

		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	void ConsoleWindow (int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		
			for (int i = 0; i < logs.Count; i++) {
				var log = logs[i];

				if (collapse) {
					var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

					if (messageSameAsPrevious) {
						continue;
					}
				}

				GUI.contentColor = logTypeColors[log.type];
				GUILayout.Label(log.message);
			}

		GUILayout.EndScrollView();

		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();


		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();
		GUI.DragWindow(titleBarRect);
	}

	void HandleLog (string message, string stackTrace, LogType type)
	{
		logs.Add(new Log() {
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}
}


    public static class Test
    {
        private static GameObject loader;
        public static void Load()
        {
            loader = new GameObject();
            loader.AddComponent<Hack>();
            UnityEngine.Object.DontDestroyOnLoad(loader);
        }
    }
}
