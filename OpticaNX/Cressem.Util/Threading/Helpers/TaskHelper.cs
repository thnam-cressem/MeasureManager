// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cressem.Util.Threading
{
	/// <summary>
	/// Helper class for tasks.
	/// </summary>
	public static class TaskHelper
	{
		/// <summary>
		/// Runs all the specified actions in separate threads and waits for the to complete.
		/// </summary>
		/// <param name="actions">The actions to spawn in separate threads.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
		public static void RunAndWait(params Action[] actions)
		{
			Argument.IsNotNull("actions", actions);

			var list = actions.ToList();

			Parallel.Invoke(actions);
		}

		/// <summary>
		/// Processing tasks as they complete
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tasks"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// // O(n^2) algorithm
		/// // if the number of tasks is large (over 1000 tasks) here, this could result in non-negligible performance overheads.
		/// List<Task<T>> tasks = …;
		/// while(tasks.Count > 0) {
		///	var t = await Task.WhenAny(tasks);
		///	tasks.Remove(t);
		///	try { Process(await t); }
		///	catch(OperationCanceledException) {}
		///	catch(Exception exc) { Handle(exc); }
		/// }
		/// 
		/// // better solution
		/// List<Task<T>> tasks = …; 
		/// foreach(var bucket in Interleaved(tasks)) {
		///	var t = await bucket; 
		///	try { Process(await t); } 
		///	catch(OperationCanceledException) {}
		///	catch(Exception exc) { Handle(exc); }
		/// ]]>
		/// </example>
		/// <remarks>
		/// http://blogs.msdn.com/b/pfxteam/archive/2012/08/02/processing-tasks-as-they-complete.aspx
		/// </remarks>
		public static Task<Task<T>>[] Interleaved<T>(IEnumerable<Task<T>> tasks)
		{
			var inputTasks = tasks.ToList();

			var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
			var results = new Task<Task<T>>[buckets.Length];
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = new TaskCompletionSource<Task<T>>();
				results[i] = buckets[i].Task;
			}

			int nextTaskIndex = -1;
			Action<Task<T>> continuation = completed =>
			{
				var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
				bucket.TrySetResult(completed);
			};

			foreach (var inputTask in inputTasks)
				inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

			return results;
		}
	}
}
