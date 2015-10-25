using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Selectable text.
	/// </summary>
	public interface ISelectable
	{
		/// <summary>
		/// Select list with xpath
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		ISelectable XPath(string xpath);

		/// <summary>
		/// Select list with css selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectable Css(string selector);

		/// <summary>
		/// Select list with css selector
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		ISelectable Css(string selector, string attrName);

		/// <summary>
		/// Select smart content with ReadAbility algorithm
		/// </summary>
		/// <returns></returns>
		ISelectable SmartContent();

		/// <summary>
		/// Select all links
		/// </summary>
		/// <returns></returns>
		ISelectable Links();

		/// <summary>
		/// Select list with regex, default group is group 1
		/// </summary>
		/// <param name="regex"></param>
		/// <returns></returns>
		ISelectable Regex(string regex);

		/// <summary>
		/// Select list with regex
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		ISelectable Regex(string regex, int group);

		/// <summary>
		/// Replace with regex
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		ISelectable Replace(string regex, string replacement);

		/// <summary>
		/// Single string result
		/// </summary>
		/// <returns></returns>
		string ToString();

		/// <summary>
		/// Single string result
		/// </summary>
		string Value { get; }

		/// <summary>
		/// If result exist for select
		/// </summary>
		/// <returns></returns>
		bool Exist();

		/// <summary>
		/// Multi string result
		/// </summary>
		/// <returns></returns>
		IList<string> GetAll();

		/// <summary>
		/// Extract by JSON Path expression
		/// </summary>
		/// <param name="jsonPath"></param>
		/// <returns></returns>
		ISelectable JsonPath(string jsonPath);

		/// <summary>
		/// Extract by custom selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectable Select(ISelector selector);

		/// <summary>
		/// Extract by custom selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectable SelectList(ISelector selector);

		/// <summary>
		/// Get all nodes
		/// </summary>
		/// <returns></returns>
		IList<ISelectable> Nodes();
	}
}
