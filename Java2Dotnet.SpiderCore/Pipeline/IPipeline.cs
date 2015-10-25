namespace Java2Dotnet.Spider.Core.Pipeline
{
	/// <summary>
	/// Pipeline is the persistent and offline process part of crawler. 
	/// The interface Pipeline can be implemented to customize ways of persistent.
	/// </summary>
	public interface IPipeline
	{
		/// <summary>
		/// Process extracted results.
		/// </summary>
		/// <param name="resultItems"></param>
		/// <param name="task"></param>
		void Process(ResultItems resultItems, ITask task);
	}
}