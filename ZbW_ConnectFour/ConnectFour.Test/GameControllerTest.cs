using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Drawing;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Tests
{
	[TestClass]
	public class GameControllerTest
	{

		[TestMethod]
		public async Task DeleteGame_DeletesExistingGame()
		{

			// Finde die erste leere Zeile in der angegebenen Spalte
			for (int zeile = 0; zeile < 6; zeile++)
			{
				var count = zeile;
			}



		}
	}
}
