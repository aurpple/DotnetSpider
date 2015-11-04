using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Scheduler 
{
	/// <summary>
	/// The scheduler whose requests can be counted for monitor.
	/// </summary>
	public interface IMonitorableScheduler : IScheduler
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		int GetLeftRequestsCount(ITask task);

		[MethodImpl(MethodImplOptions.Synchronized)]
		int GetTotalRequestsCount(ITask task);
	}
}