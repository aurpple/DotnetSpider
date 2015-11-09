using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Core.Test
{
    [TestClass]
    public class ResultItemsCollectorsPipelineTest
    {
        ResultItemsCollectorPipeline ResultItemsCollectorPipeline = new ResultItemsCollectorPipeline();

        [TestMethod]
        public void TestCollectorPipeline()
        {
            ResultItems item = new ResultItems();
            ResultItems resultItems = new ResultItems();
            resultItems.Put("a", "a");
            resultItems.Put("b", "b");
            resultItems.Put("c", "c");
            ResultItemsCollectorPipeline.Process(resultItems, null);
            Assert.IsTrue(ResultItemsCollectorPipeline.GetCollected().Count == 3);
        }
    }
}
