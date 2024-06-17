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

  for (const game of games) {
    if (game.state === 0) {
      // Only list games with status 0
      const user1 = await getUserByIdOrNull(game.users[0].id);
      const user2 = await getUserByIdOrNull(game.users[1].id);
      const robot1 = await getRobotById(game.robots[0].id);
      const robot2 = await getRobotById(game.robots[1].id);

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

      const listItem = document.createElement("li");
      listItem.innerHTML = `${user1?.name ?? "Unknown User"} & ${
        robot1?.name ?? "Unknown Robot"
      } vs.<br>${user2?.name ?? "Unknown User"} & ${
        robot2?.name ?? "Unknown Robot"
      }<br>Winner: ${winnerName} with ${winnerPoints} points (Remaining turns left)`;
      bestenlisteElement.appendChild(listItem);
    }
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
