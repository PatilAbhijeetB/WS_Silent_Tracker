using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Service.ThreadHandler
{
	public class ThreadExecutorService
	{
		public int ThreadCount => threads.Length;
		public EventHandler<Exception> OnException;

		private ManualResetEvent waiter;
		private Thread[] threads;
		private SortedSet<Tuple<DateTime, Action>> queue;

		public ThreadExecutorService(int threadCount)
		{
			waiter = new ManualResetEvent(false);
			queue = new SortedSet<Tuple<DateTime, Action>>();
			OnException += (o, e) => { };
			threads = Enumerable.Range(0, threadCount).Select(i => new Thread(RunLoop)).ToArray();
			foreach (var thread in threads)
			{
				thread.Start();
			}
		}

		private void RunLoop()
		{
			while (true)
			{
				TimeSpan sleepingTime = TimeSpan.MaxValue;
				bool needToSleep = true;
				Action task = null;

				try
				{
					lock (waiter)
					{
						if (queue.Any())
						{
							if (queue.First().Item1 <= DateTime.Now)
							{
								task = queue.First().Item2;
								queue.Remove(queue.First());
								needToSleep = false;
							}
							else
							{
								sleepingTime = queue.First().Item1 - DateTime.Now;
							}
						}
					}

					if (needToSleep)
					{
						waiter.WaitOne((int)sleepingTime.TotalMilliseconds);
					}
					else
					{
						task();
					}
				}
				catch (Exception e)
				{
					OnException(task, e);
				}
			}
		}
	}
}
