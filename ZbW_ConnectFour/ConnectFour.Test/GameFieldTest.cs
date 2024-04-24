using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ConnectFour.Tests
{
	[TestClass]
	public class GameFieldTest
	{
		
		[TestMethod]
		public void SetUp()
		{
			string jsonInput = @"{
            ""1"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""2"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""3"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""4"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""5"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""6"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""7"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0}
        }"
;

			var dic = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, int>>>(jsonInput);
		}
	}
}
