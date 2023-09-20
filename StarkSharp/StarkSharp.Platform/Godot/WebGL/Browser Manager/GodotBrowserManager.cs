using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using GodotArray = Godot.Collections.Array;
using StarkSharp.Components.Task;

namespace StarkSharp.Platforms.Godot.WebGL
{
	public class GodotBrowserManager : Node
	{
		public static GodotBrowserManager Instance;

		public static List<int> waitingAsyncTasks = new List<int>();
		public static Dictionary<int, Action<int, Action<string>, Action<string>>> actionList = new Dictionary<int, Action<int, Action<string>, Action<string>>>();
		public static Dictionary<int, Action<string>> successCallbacks = new Dictionary<int, Action<string>>();
		public static Dictionary<int, Action<string>> failCallbacks = new Dictionary<int, Action<string>>();

		public Timer timer;

		public override void _Ready() {
			Instance = this;
			JavaScript.Eval(BrowserScript.SetGlobalDictionary(), true);
			StartTimer();
		}

		public static void ConnectWallet(string walletType, int id)
		=> JavaScript.Eval(BrowserScript.ConnectWallet(walletType, id));
		
		public static void SendTransaction(string walletType, int id, string contractAddress, string entryPoint, string callData)
		=> JavaScript.Eval(BrowserScript.SendTransaction(walletType, id, contractAddress, entryPoint, callData));
		
		public static string GetAccountInformation()
		=> (string) JavaScript.Eval(BrowserScript.GetAccountInformation());
		
		public static bool CheckWalletConnection() 
		=> (bool) JavaScript.Eval(BrowserScript.CheckWalletConnection());
		
		public static void DebugMessage(string message)
		=> JavaScript.Eval(BrowserScript.DebugMessage(message));

		public static bool CheckMessage(int id)
		=> (bool) JavaScript.Eval(BrowserScript.CheckMessage(id));

		public static string GetMessage(int id)
		=> (string) JavaScript.Eval(BrowserScript.GetMessage(id));
		
		public void StartTimer()
		{
			timer = new Timer();
			AddChild(timer);
			timer.WaitTime = 0.5f;
			timer.Connect("timeout", this, nameof(OnTimerTimeout));
			timer.Start();
		}
		
		public void UpdateTimer(Func<bool> predicate, int id, Action<string> successCallback, Action<string> failCallback, Action<int, Action<string>, Action<string>> action)
		{
			actionList[id] = action;
			successCallbacks[id] = successCallback;
			failCallbacks[id] = failCallback;
			waitingAsyncTasks.Add(id);
		}
		
		private void OnTimerTimeout()
		{
			List<int> completedTasks = new List<int>();
			List<int> safeWaitingAsyncTasks = waitingAsyncTasks.ToList();

			foreach(int id in safeWaitingAsyncTasks)
			{
				string message = GetMessage(id);
				if(message != "")
				{
					ConnectorTask.RecieveMessage(message);
					OnTaskCompleted(id);
					completedTasks.Add(id);
				}
			}
		}
		
		public void OnTaskCompleted(int id)
		{
			actionList[id](id, successCallbacks[id], failCallbacks[id]);
		}
	}
}