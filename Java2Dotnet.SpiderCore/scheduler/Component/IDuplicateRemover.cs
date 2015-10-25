using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Scheduler.Component
{
	/// <summary>
	/// Remove duplicate requests.
	/// </summary>
	public interface IDuplicateRemover
	{
		/// <summary>
		/// Check whether the request is duplicate.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		bool IsDuplicate(Request request, ITask task);

		/// <summary>
		/// Reset duplicate check.
		/// </summary>
		/// <param name="task"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		void ResetDuplicateCheck(ITask task);

		/// <summary>
		/// Get TotalRequestsCount for monitor.
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		int GetTotalRequestsCount(ITask task);
	}
}
