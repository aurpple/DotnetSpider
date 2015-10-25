function $(str) {
	return page.GetHtml().Css(str).ToString();
}
function xpath(str) {
	return page.GetHtml().XPath(str).ToString();
}
function urls(str) {
	links = page.GetHtml().Links().Regex(str).GetAll();
	page.AddTargetRequests(links);
}