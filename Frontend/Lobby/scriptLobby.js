document.addEventListener("DOMContentLoaded", () => {
  const localStorageUser = JSON.parse(localStorage.getItem("user"));
  const localStorageRobot = JSON.parse(localStorage.getItem("robot"));

  // Zeige die Roboterinformationen in "robot-info-lobby"
  function displayRobotInfo(robot) {
    const robotInfoLobby = document.getElementById("robot-info-lobby-own");
    robotInfoLobby.innerHTML = ""; // Clear any existing content

    const robotId = document.createElement("div");
    robotId.classList.add("robot-id");
    robotId.textContent = `ID: ${robot.id}`;

    const robotName = document.createElement("div");
    robotName.classList.add("robot-name");
    robotName.textContent = `Name: ${robot.name}`;

    const robotStatus = document.createElement("div");
    robotStatus.classList.add("robot-status");
    robotStatus.textContent = `Status: ${
      robot.isConnected ? "Verbunden" : "Nicht verbunden"
    }`;

    const robotBrokerAddress = document.createElement("div");
    robotBrokerAddress.classList.add("robot-broker-address");
    robotBrokerAddress.textContent = `Broker-Adresse: ${robot.brokerAddress}`;

    const robotBrokerPort = document.createElement("div");
    robotBrokerPort.classList.add("robot-broker-port");
    robotBrokerPort.textContent = `Broker-Port: ${robot.brokerPort}`;

    robotInfoLobby.appendChild(robotId);
    robotInfoLobby.appendChild(robotName);
    robotInfoLobby.appendChild(robotStatus);
    robotInfoLobby.appendChild(robotBrokerAddress);
    robotInfoLobby.appendChild(robotBrokerPort);
  }

  async function displayRobots(robots) {
    const robotInfoLobby = document.getElementById(
      "robot-info-lobby-opponents"
    );
    robotInfoLobby.innerHTML = ""; // Clear any existing content

    for (const robot of robots) {
      const robotDiv = document.createElement("div");
      robotDiv.classList.add("robot-info");

      // Apply border to robotDiv
      robotDiv.style.border = "0.3cm solid red";
      robotDiv.style.padding = "10px"; // Optional: add some padding inside the div for better appearance

      const robotName = document.createElement("div");
      robotName.classList.add("name");
      robotName.textContent = `Name: ${robot.name}`;

      const robotStatus = document.createElement("div");
      robotStatus.classList.add("status");
      robotStatus.textContent = `Status: ${
        robot.isConnected ? "Verbunden" : "Nicht verbunden"
      }`;

      robotStatus.style.marginBottom = "15px"; // Add margin-bottom for spacing

      robotDiv.appendChild(robotName);
      robotDiv.appendChild(robotStatus);

      const user = await fetchUserForRobot(robot);

      if (user) {
        const userDiv = document.createElement("div");

        userDiv.classList.add("user-info-forRobot");
        userDiv.textContent = `Benutzer: ${user.name}`;

        userDiv.style.marginBottom = "30px";

        robotDiv.appendChild(userDiv);
      }

      // Create buttons
      const testConnectionButton = document.createElement("button");
      testConnectionButton.textContent = "Verbindung testen";
      testConnectionButton.onclick = () => {
        // Add your onClick event logic here
        console.log(`Verbindung testen for robot: ${robot.name}`);
      };

      const sendGameRequestButton = document.createElement("button");
      sendGameRequestButton.textContent = "Spielanfrage senden";
      sendGameRequestButton.onclick = () => {
        // Add your onClick event logic here
        console.log(`Spielanfrage senden for robot: ${robot.name}`);
      };

      // Create a div to hold the buttons and add some margin-left
      const buttonDiv = document.createElement("div");
      buttonDiv.style.marginTop = "10px";

      buttonDiv.appendChild(testConnectionButton);
      buttonDiv.appendChild(sendGameRequestButton);

      // Append the buttonDiv to the robotDiv
      robotDiv.appendChild(buttonDiv);

      robotInfoLobby.appendChild(robotDiv);
    }
  }

  async function fetchRobots() {
    try {
      const response = await fetch(`${endpoint}/Robot`, {
        method: "GET",
        mode: "cors",
      });

      if (response.ok) {
        const robots = await response.json();

        console.log("Fetched robots:", robots);
        const filteredRobots = robots.filter(
          (robot) =>
            robot.isIngame === false &&
            robot.currentUserId !== null &&
            robot.id !== localStorageRobot.id
        );
        displayRobots(filteredRobots);
      } else {
        console.error("Failed to fetch robots:", response.statusText);
      }
    } catch (error) {
      console.error("Error:", error.message);
    }
  }

  async function fetchUserForRobot(robot) {
    try {
      const response = await fetch(`${endpoint}/User/${robot.currentUserId}`, {
        method: "GET",
        mode: "cors",
      });

      if (response.ok) {
        const user = await response.json();
        console.log("Fetched user:", user);
        return user;
      } else {
        console.error("Failed to fetch user:", response.statusText);
      }
    } catch (error) {
      console.error("Error:", error.message);
    }
  }

  // Display the current user's robot
  if (localStorageRobot) {
    displayRobotInfo(localStorageRobot);
  }

  fetchRobots();
});

async function ConnectToMqtt(robot) {
  const response = await fetch(`${endpoint}/User`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(userRequest),
  });

  if (response.ok) {
    alert(
      "Registrierung erfolgreich! Sie erhalten ein Mail zur Registrierung!"
    );

    await sendRegistrationEmail(email);

    // Aufruf der Funktion zum Senden der Registrierungs-E-Mail
  } else {
    const error = await response.json();
    alert("Fehler bei der Registrierung: " + error.message);
  }
}

function goToMainMenu() {
  window.location.href = "../Hauptmenu/hauptmenu.html";
}
