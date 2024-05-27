document.addEventListener("DOMContentLoaded", async function () {
  var user = JSON.parse(localStorage.getItem("user"));
  var userElement = document.getElementById("user-info");

  if (user) {
    if (userElement) {
      userElement.textContent = `Angemeldet als: ${user.name}`;
    }
    await setLocalStorageRobot(user);
  } else {
    // Umleiten zur Login-Seite, wenn kein Benutzer angemeldet ist
    window.location.href = "../Login/login.html";
  }
});

document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  localStorage.removeItem("robot");
  window.location.href = "../Login/login.html";
});

async function setLocalStorageRobot(user) {
  try {
    const response = await fetch(`${endpoint}/Robot`, {
      // Setze hier deinen API-Endpunkt ein
      method: "GET",
      mode: "cors",
    });

    if (response.ok) {
      const robots = await response.json();
      console.log("Fetched robots:", robots);

      const robot = robots.find((robot) => robot.currentUserId === user.id);

      if (robot) {
        localStorage.setItem("robot", JSON.stringify(robot));
      }
    } else {
      console.error("Failed to fetch robots:", response.statusText);
    }
  } catch (error) {
    console.error("Error fetching robots:", error);
  }
}
