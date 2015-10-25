using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Scheduler 
{
	/// <summary>
	/// Scheduler is the part of url management. 
	/// You can implement interface Scheduler to do:
	/// manage urls to fetch
	/// remove duplicate urls
	/// </summary>
	public interface IScheduler
	{
		void Init(ITask task);

		/// <summary>
		/// Add a url to fetch
		/// </summary>
		/// <param name="request"></param>
		/// <param name="task"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		void Push(Request request, ITask task);

		/// <summary>
		/// Get an url to crawl
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		Request Poll(ITask task);
	}
}
