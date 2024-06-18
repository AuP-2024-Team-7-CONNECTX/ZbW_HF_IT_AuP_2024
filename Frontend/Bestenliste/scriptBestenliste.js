function goToMainMenu() {
  window.location.href = "../Hauptmenu/hauptmenu.html";
}

async function fetchGames() {
  try {
    const response = await fetch(`${endpoint}/Game`, {
      method: "GET",
      mode: "cors",
    });

    if (response.ok) {
      const games = await response.json();
      console.log("Fetched games:", games);
      await displayGames(games);
    } else {
      console.error("Failed to fetch games:", response.statusText);
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
}

async function displayGames(games) {
  const bestenlisteElement = document.getElementById("bestenliste");
  bestenlisteElement.innerHTML = ""; // Clear existing entries

  // Process games and calculate winner points
  const processedGames = [];
  for (const game of games) {
    if (game.state === 0) {
      const user1 = await getUserByIdOrNull(game.user1Id);
      const user2 = await getUserByIdOrNull(game.user2Id);
      const robot1 = await getRobotById(game.robot1Id);
      const robot2 = await getRobotById(game.robot2Id);

      let winnerName = "";
      let winnerPoints = 0;

      if (game.winnerUser) {
        winnerName = game.winnerUser.name;
        winnerPoints =
          game.totalPointsPlayerOne > game.totalPointsPlayerTwo
            ? game.totalPointsPlayerOne
            : game.totalPointsPlayerTwo;
      } else if (game.winnerRobot) {
        winnerName = game.winnerRobot.name;
        winnerPoints =
          game.totalPointsUserOne > game.totalPointsUserTwo
            ? game.totalPointsUserOne
            : game.totalPointsUserTwo;
      }

      processedGames.push({
        user1Name: user1?.name ?? "Unknown User",
        user2Name: user2?.name ?? "Unknown User",
        robot1Name: robot1?.name ?? "Unknown Robot",
        robot2Name: robot2?.name ?? "Unknown Robot",
        winnerName,
        winnerPoints,
      });
    }
  }

  // Sort games by winnerPoints in descending order
  processedGames.sort((a, b) => b.winnerPoints - a.winnerPoints);

  // Display sorted games with ranking
  for (let i = 0; i < processedGames.length; i++) {
    const game = processedGames[i];
    const listItem = document.createElement("li");
    listItem.innerHTML = `<span class="ranking">${i + 1}</span><br> ${
      game.user1Name
    } & ${game.robot1Name} vs.<br>${game.user2Name} & ${
      game.robot2Name
    }<br>Winner: ${game.winnerName} with ${
      game.winnerPoints
    } points (Remaining turns left)`;
    bestenlisteElement.appendChild(listItem);
  }
}

async function getUserByIdOrNull(userId) {
  if (!userId) return null;
  const response = await fetch(`${endpoint}/User/${userId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const user = await response.json();
    return user;
  } else {
    console.error("Failed to fetch user:", response.statusText);
    return null;
  }
}

async function getRobotById(robotId) {
  const response = await fetch(`${endpoint}/Robot/${robotId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const robot = await response.json();
    return robot;
  } else {
    console.error("Failed to fetch Robot:", response.statusText);
    alert("Fehler beim Ermitteln des Roboters");
    return null;
  }
}

// Fetch and display games on page load
document.addEventListener("DOMContentLoaded", fetchGames);
