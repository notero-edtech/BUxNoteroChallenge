using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Threading.Unity
{
	public class MainThreadDispatcher : MonoBehaviour, IThreadRunner
	{
		public enum UpdateType
		{
			FixedUpdate,
			Update,
			LateUpdate,
		}

		public delegate void UpdateEvent();
		public static UpdateEvent unityFixedUpdate = null;
		public static UpdateEvent unityUpdate = null;
		public static UpdateEvent unityLateUpdate = null;

		/// <summary>
		/// The singleton instance of the Main Thread Manager
		/// </summary>
		private static MainThreadDispatcher _instance;
		public static MainThreadDispatcher Instance
		{
			get
			{
				if (_instance == null) Create();
				return _instance;
			}
		}

		/// <summary>
		/// This will create a main thread dispatcher if one is not already created
		/// </summary>
		public static void Create()
		{
			if (_instance != null)
				return;

			ThreadManagement.Initialize();

			if (!ReferenceEquals(_instance, null))
				return;

			new GameObject("Main Thread Dispatcher").AddComponent<MainThreadDispatcher>();
		}

		/// <summary>
		/// A dictionary of action queues for different updates.
		/// </summary>
		private static Dictionary<UpdateType, Queue<Action>> actionQueueDict = new Dictionary<UpdateType, Queue<Action>>();
		private static Dictionary<UpdateType, Queue<Action>> actionRunnerDict = new Dictionary<UpdateType, Queue<Action>>();

		// Setup the singleton in the Awake
		private void Awake()
		{
			// If an instance already exists then delete this copy
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}

			// Assign the static reference to this object
			_instance = this;

			// This object should move through scenes
			DontDestroyOnLoad(gameObject);
		}

		public void Execute(Action action)
		{
			Run(action);
		}

		/// <summary>
		/// Add a function to the list of functions to call on the main thread via the Update function
		/// </summary>
		/// <param name="action">The method that is to be run on the main thread</param>
		public static void Run(Action action, UpdateType updateType = UpdateType.FixedUpdate)
		{
			// Only create this object on the main thread
#if UNITY_WEBGL
			if (ReferenceEquals(Instance, null))
#else
			if (ReferenceEquals(Instance, null) && ThreadManagement.IsMainThread)
#endif
			{
				Create();
			}

			// Allocate new action queue by update type if there's no one exists.
			if (!actionQueueDict.ContainsKey(updateType))
			{
				actionQueueDict.Add(updateType, new Queue<Action>());

				// Since an action runner depends on the action queue, allocate new one here.
				actionRunnerDict.Add(updateType, new Queue<Action>());
			}

			Queue<Action> mainThreadActions = actionQueueDict[updateType];

			// Make sure to lock the mutex so that we don't override
			// other threads actions
			lock (mainThreadActions)
			{
				mainThreadActions.Enqueue(action);
			}
		}

		private void HandleActions(UpdateType updateType)
		{
			// Allocate new action queue by update type if there's no one exists.
			if (!actionQueueDict.ContainsKey(updateType))
			{
				actionQueueDict.Add(updateType, new Queue<Action>());

				// Since an action runner depends on the action queue, allocate new one here.
				actionRunnerDict.Add(updateType, new Queue<Action>());

			}
			Queue<Action> mainThreadActions = actionQueueDict[updateType];
			Queue<Action> mainThreadActionsRunner = actionRunnerDict[updateType];

			lock (mainThreadActions)
			{
				// Flush the list to unlock the thread as fast as possible
				if (mainThreadActions.Count > 0)
				{
					while (mainThreadActions.Count > 0)
						mainThreadActionsRunner.Enqueue(mainThreadActions.Dequeue());
				}
			}

			// If there are any functions in the list, then run
			// them all and then clear the list
			if (mainThreadActionsRunner.Count > 0)
			{
				while (mainThreadActionsRunner.Count > 0)
					mainThreadActionsRunner.Dequeue()();
			}
		}

		private void FixedUpdate()
		{
			HandleActions(UpdateType.FixedUpdate);

			if (unityFixedUpdate != null)
				unityFixedUpdate();
		}

		private void Update()
		{
			HandleActions(UpdateType.Update);

			if (unityUpdate != null)
				unityUpdate();
		}

		private void LateUpdate()
		{
			HandleActions(UpdateType.LateUpdate);

			if (unityLateUpdate != null)
				unityLateUpdate();
		}

#if WINDOWS_UWP
		public static async void ThreadSleep(int length)
#else
		public static void ThreadSleep(int length)
#endif
		{
#if WINDOWS_UWP
			await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(length));
#else
			System.Threading.Thread.Sleep(length);
#endif
		}
	}
}
