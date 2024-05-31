document.addEventListener("DOMContentLoaded", () => {
  const localStorageUser = JSON.parse(localStorage.getItem("user"));
  const localStorageRobot = JSON.parse(localStorage.getItem("robot"));

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

    const robotBrokerTopic = document.createElement("div");
    robotBrokerTopic.classList.add("robot-broker-topic");
    robotBrokerTopic.textContent = `Broker-Topic: ${robot.brokerTopic}`;

    robotInfoLobby.appendChild(robotId);
    robotInfoLobby.appendChild(robotName);
    robotInfoLobby.appendChild(robotStatus);
    robotInfoLobby.appendChild(robotBrokerAddress);
    robotInfoLobby.appendChild(robotBrokerPort);
    robotInfoLobby.appendChild(robotBrokerTopic);
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

      const robotBrokerAddress = document.createElement("div");
      robotBrokerAddress.classList.add("robot-broker-address");
      robotBrokerAddress.textContent = `Broker-Adresse: ${robot.brokerAddress}`;

      const robotBrokerPort = document.createElement("div");
      robotBrokerPort.classList.add("robot-broker-port");
      robotBrokerPort.textContent = `Broker-Port: ${robot.brokerPort}`;

      const robotBrokerTopic = document.createElement("div");
      robotBrokerTopic.classList.add("robot-broker-topic");
      robotBrokerTopic.textContent = `Broker-Topic: ${robot.brokerTopic}`;

      robotDiv.appendChild(robotName);
      robotDiv.appendChild(robotStatus);
      robotDiv.appendChild(robotBrokerAddress);
      robotDiv.appendChild(robotBrokerPort);
      robotDiv.appendChild(robotBrokerTopic);

      const user = await fetchUserForRobot(robot);

      if (user) {
        const userDiv = document.createElement("div");
        userDiv.classList.add("user-info-forRobot");
        userDiv.textContent = `Benutzer: ${user.name}`;
        userDiv.style.marginBottom = "30px";

        robotDiv.appendChild(userDiv);
      }

      // Erstelle den "Verbindung testen"-Button
      const testConnectionButton = document.createElement("button");
      testConnectionButton.className = "mqtt-button full-width";
      testConnectionButton.textContent = "Verbindung testen";
      testConnectionButton.onclick = () => {
        ConnectToMqttWithRobot(robot, testConnectionButton);
      };

      // Create a div to hold the buttons and add some margin-top
      const buttonDiv = document.createElement("div");
      buttonDiv.style.marginTop = "10px";

      buttonDiv.appendChild(testConnectionButton);

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
        return user;
      } else {
        console.error("Failed to fetch user:", response.statusText);
      }
    } catch (error) {
      console.error("Error:", error.message);
    }
  }

  if (localStorageRobot) {
    displayRobotInfo(localStorageRobot);
  }

  fetchRobots();
});

async function ConnectToMqttWithRobot(robot, button) {
  let mqttRequest = {
    BrokerAddress: robot.brokerAddress,
    Port: String(robot.brokerPort),
    Topic: robot.brokerTopic,
  };

  const response = await fetch(`${endpoint}/MqttTest/MqttConnectToBroker`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(mqttRequest),
  });

  let infoText = button.parentNode.querySelector(".info-text");

  if (!infoText) {
    infoText = document.createElement("span");
    infoText.classList.add("info-text");
    infoText.style.marginLeft = "10px";
    infoText.style.padding = "10px"; // Padding hinzufügen
    button.parentNode.appendChild(infoText);
  }

  if (response.ok) {
    infoText.textContent = "Verbindung zum Mqtt-Client möglich";
    infoText.style.color = "green";

    // Entferne vorhandene Buttons, falls sie existieren
    removeExistingButtons(button.parentNode);

    // Erstelle den Button-Div
    const buttonDiv = document.createElement("div");
    buttonDiv.className = "button-group"; // Klasse für Styling

    // Erstelle den "Spielanfrage senden"-Button, wenn die Verbindung erfolgreich ist
    const sendGameRequestButton = document.createElement("button");
    sendGameRequestButton.className = "mqtt-button";
    sendGameRequestButton.textContent = "Spielanfrage senden";
    sendGameRequestButton.onclick = () => {
      sendGameRequest(robot.currentUserId);
    };
    buttonDiv.appendChild(sendGameRequestButton);

    // Erstelle den "Test - publish auf Topic"-Button, wenn die Verbindung erfolgreich ist
    const testPublishButton = document.createElement("button");
    testPublishButton.className = "mqtt-button";
    testPublishButton.textContent =
      "MQTT Test-publish mit '5' auf Roboter-Topic";
    testPublishButton.onclick = async () => {
      console.log(`Test - publish auf Topic for robot: ${robot.name}`);

      await ConnectToMqtt(mqttRequest);
      await PublishToMqttTopic(mqttRequest);
      await DisconnectFromMqtt(mqttRequest);
    };
    buttonDiv.appendChild(testPublishButton);

    button.parentNode.appendChild(buttonDiv);

    await DisconnectFromMqtt(mqttRequest);
  } else {
    infoText.textContent = "Verbindung zum Mqtt-Client nicht möglich";
    infoText.style.color = "red";
  }
}

function removeExistingButtons(parent) {
  const existingButtonGroup = parent.querySelector(".button-group");
  if (existingButtonGroup) {
    existingButtonGroup.remove();
  }
}

async function DisconnectFromMqtt(mqttRequest) {
  const response = await fetch(`${endpoint}/MqttTest/MqttDisconnect`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(mqttRequest),
  });

  if (response.ok) {
    const jsonResponse = await response.json();
    console.log(jsonResponse.Message);
  } else {
    const errorResponse = await response.json();
    alert(
      `Fehler beim Trennen der Verbindung zu Mqtt-Broker: ${errorResponse.Message}`
    );
  }
}

async function PublishToMqttTopic(mqttRequest) {
  const response = await fetch(`${endpoint}/MqttTest/MqttPublish`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(mqttRequest),
  });

  if (response.ok) {
    const jsonResponse = await response.json();
    console.log(jsonResponse.Message);
  } else {
    const errorResponse = await response.json();
    alert(`Fehler beim publish-Test: ${errorResponse.Message}`);
  }
}

async function ConnectToMqtt(mqttRequest) {
  const response = await fetch(`${endpoint}/MqttTest/MqttConnectToBroker`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(mqttRequest),
  });

  if (response.ok) {
    const jsonResponse = await response.json();
    console.log(jsonResponse.Message);
  } else {
    const errorResponse = await response.json();
    alert(`Fehler beim Verbinden mit Mqtt-Broker: ${errorResponse.Message}`);
  }
}

function goToMainMenu() {
  window.location.href = "../Hauptmenu/hauptmenu.html";
}
async function sendGameRequest(receiverId) {
  const localStorageUser = JSON.parse(localStorage.getItem("user"));
  const request = {
    SenderId: localStorageUser.id,
    ReceiverId: receiverId,
  };

  const response = await fetch(`${endpoint}/GameRequest/SendRequest`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
    body: JSON.stringify(request),
  });

  const responseData = await response.json();

  if (response.ok && responseData.success) {
    alert(responseData.message);
  } else {
    alert("Fehler beim Senden der Spielanfrage.");
  }
}
function displayGameAcceptButton(senderId, senderEmail) {
  const invitationsList = document.getElementById("invitations-list");
  const invitationDiv = document.createElement("div");
  invitationDiv.className = "invitation";

  const message = document.createElement("p");
  message.textContent = `Spieleinladung von ${senderEmail}`;

  const acceptButton = document.createElement("button");
  acceptButton.id = "accept-game-button";
  acceptButton.innerHTML = "Spiel annehmen";
  acceptButton.onclick = () => acceptGameRequest(senderId);

  invitationDiv.appendChild(message);
  invitationDiv.appendChild(acceptButton);
  invitationsList.appendChild(invitationDiv);
}

async function checkForGameRequest() {
  const localStorageUser = JSON.parse(localStorage.getItem("user"));
  const response = await fetch(
    `${endpoint}/GameRequest/CheckRequest/${localStorageUser.id}`,
    {
      method: "GET",
      mode: "cors",
    }
  );

  if (response.ok) {
    const data = await response.json();
    if (data.success) {
      const senderEmail = await fetchUserEmailById(data.senderId); // Funktion zum Abrufen der E-Mail des Senders
      displayGameAcceptButton(data.senderId, senderEmail);
    } else {
      console.log(data.message);
    }
  } else {
    const errorData = await response.json();
    console.error("Error:", errorData.message);
  }
}

async function fetchUserEmailById(userId) {
  const response = await fetch(`${endpoint}/User/${userId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const user = await response.json();
    return user.email;
  } else {
    console.error("Failed to fetch user email:", response.statusText);
    return "unbekannt";
  }
}

async function acceptGameRequest(senderId) {
  // Logik zum Akzeptieren der Spielanfrage
  console.log(`Spielanfrage von ${senderId} angenommen`);
}

// Wiederholtes Überprüfen auf eingehende Spielanfragen
setInterval(checkForGameRequest, 5000); // Überprüfe alle 5 Sekunden
