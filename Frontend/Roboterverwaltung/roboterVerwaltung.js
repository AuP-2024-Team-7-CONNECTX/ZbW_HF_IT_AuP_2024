document.addEventListener("DOMContentLoaded", async () => {
  await GetRobots();
  document
    .getElementById("edit-robot-form")
    .addEventListener("submit", saveRobot);
  document
    .getElementById("create-robot-button")
    .addEventListener("click", showCreateModal);
});

// Fetch and display robots
async function GetRobots() {
  try {
    const response = await fetch(`${endpoint}/Robot`, {
      method: "GET",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (response.ok) {
      const robots = await response.json();
      console.log("Fetched robots:", robots);
      await displayRobots(robots); // Hier kannst du eine Funktion aufrufen, um die Roboter anzuzeigen
    } else {
      const error = await response.json();
      console.error("Failed to fetch robots:", error.message);
      alert(`Failed to fetch robots: ${error.message}`);
    }
  } catch (error) {
    console.error("Error:", error.message);
    alert(`An error occurred: ${error.message}`);
  }
}

async function displayRobots(robots) {
  const robotsContainer = document.getElementById("robots-container");
  robotsContainer.innerHTML = ""; // Leere den Container

  for (const robot of robots) {
    const robotElement = document.createElement("div");
    robotElement.className = "robot";
    robotElement.id = `robot-${robot.id}`;

    let user = await getUserByIdOrNull(robot.currentUserId);
    let userMail = user ? user.email : "Kein User verbunden";

    let robotInfo = `
      <h3>${robot.name}</h3>
      <p>Aktueller Benutzer: ${userMail}</p>
      <p>Im Spiel: ${robot.isIngame}</p>
      <p>Verbunden: ${robot.isConnected}</p>
      <p>Broker Address: ${robot.brokerAddress}</p>
      <p>Broker Port: ${robot.brokerPort}</p>
      <p>Broker Topic: ${robot.brokerTopic}</p>
      <button class="action-button" onclick="editRobot('${robot.id}', '${robot.name}', '${robot.brokerAddress}', ${robot.brokerPort}, '${robot.brokerTopic}')">Bearbeiten</button>
    `;

    if (!robot.isConnected) {
      robotInfo += `
        <button class="action-button" onclick='ConnectToMqtt(${JSON.stringify(
          robot
        )})'>MQTT - Verbinden</button>
      `;
    }

    robotElement.innerHTML = robotInfo;
    let localStorageUser = JSON.parse(localStorage.getItem("user"));

    if (robot.isConnected && robot.currentUserId === localStorageUser.id) {
      if (!robotElement.querySelector(".connected-info")) {
        const connectedInfo = document.createElement("p");
        connectedInfo.innerText = "Verbunden";
        connectedInfo.style.color = "green";
        connectedInfo.style.fontWeight = "bold";
        connectedInfo.classList.add("connected-info");

        const publishButton = document.createElement("button");
        publishButton.innerText = "Publish Test";
        publishButton.onclick = () => publishTest(robot);
        publishButton.classList.add("publish-button");

        const subscribeButton = document.createElement("button");
        subscribeButton.innerText = "Subscribe Test";
        subscribeButton.onclick = () => subscribeTest(robot);
        subscribeButton.classList.add("subscribe-button");

        robotElement.appendChild(connectedInfo);
        robotElement.appendChild(publishButton);
        robotElement.appendChild(subscribeButton);
        robotElement.classList.add("connected");
      }
    }

    robotsContainer.appendChild(robotElement);
  }
}

// Show create robot modal
function showCreateModal() {
  document.getElementById("edit-robot-id").value = "";
  document.getElementById("edit-robot-name").value = "";
  document.getElementById("edit-robot-address").value = "";
  document.getElementById("edit-robot-port").value = "";
  document.getElementById("edit-robot-topic").value = "";

  document.getElementById("modal-title").innerText = "Neuen Roboter erstellen";
  document.getElementById("save-button").innerText = "Erstellen";
  document.getElementById("save-button").onclick = saveNewRobot;

  document.getElementById("edit-robot-modal").style.display = "block";
}

// Save new robot
async function saveNewRobot(event) {
  event.preventDefault();

  const name = document.getElementById("edit-robot-name").value;
  const brokerAddress = document.getElementById("edit-robot-address").value;
  const brokerPort = document.getElementById("edit-robot-port").value;
  const brokerTopic = document.getElementById("edit-robot-topic").value;

  const robot = {
    currentUserId: null,
    isConnected: false,
    isIngame: false,
    color: null,
    name,
    brokerAddress,
    brokerPort,
    brokerTopic,
  };

  await createRobot(robot);
  closeEditModal();
}

// Edit robot functionality
function editRobot(id, name, address, port, topic) {
  document.getElementById("edit-robot-id").value = id;
  document.getElementById("edit-robot-name").value = name;
  document.getElementById("edit-robot-address").value = address;
  document.getElementById("edit-robot-port").value = port;
  document.getElementById("edit-robot-topic").value = topic;

  document.getElementById("modal-title").innerText = "Roboter bearbeiten";
  document.getElementById("save-button").innerText = "Speichern";
  document.getElementById("save-button").onclick = saveRobot;

  document.getElementById("edit-robot-modal").style.display = "block";
}

function closeEditModal() {
  document.getElementById("edit-robot-modal").style.display = "none";
}

async function saveRobot(event) {
  event.preventDefault();

  const id = document.getElementById("edit-robot-id").value;
  let robot = await getRobotById(id);
  const name = document.getElementById("edit-robot-name").value;
  const brokerAddress = document.getElementById("edit-robot-address").value;
  const brokerPort = document.getElementById("edit-robot-port").value;
  const brokerTopic = document.getElementById("edit-robot-topic").value;

  robot.name = name;
  robot.brokerAddress = brokerAddress;
  robot.brokerPort = brokerPort;
  robot.brokerTopic = brokerTopic;
  closeEditModal();
  await UpdateRobot(robot);
}

// Dummy function for getUserByIdOrNull for the purpose of this example
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

// Dummy function for getUserByIdOrNull for the purpose of this example
async function getRobotById(robotId) {
  const response = await fetch(`${endpoint}/Robot/${robotId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    let robot = await response.json();
    return robot;
  } else {
    console.error("Failed to fetch Robot:", response.statusText);
    alert.apply("Fehler beim Ermitteln des Roboters");
  }
}

// Function for MQTT connection
async function ConnectToMqtt(robot) {
  // Check if there is already a connected robot and disconnect it
  const connectedRobot = document.querySelector(".robot.connected");
  if (connectedRobot) {
    let localStorageRobot = JSON.parse(localStorage.getItem("robot"));
    if (localStorageRobot !== null) {
      localStorageRobot.currentUserId = null;
      localStorageRobot.isConnected = false;
      await DisconnectFromMqtt(localStorageRobot);
      await UpdateRobot(localStorageRobot);
    }

    connectedRobot.classList.remove("connected");
    connectedRobot.querySelectorAll(".action-button").forEach((button) => {
      button.style.backgroundColor = "#d32f2f"; // Reset button color
    });
    connectedRobot.querySelector("p.connected-info")?.remove();
    connectedRobot.querySelector("button.publish-button")?.remove();
    connectedRobot.querySelector("button.subscribe-button")?.remove();
  }

  let mqttRequest = {
    BrokerAddress: robot.brokerAddress,
    Port: String(robot.brokerPort),
    Topic: robot.brokerTopic,
  };

  try {
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
      localStorage.setItem("robot", JSON.stringify(robot));
      alert(`Verbindung mit MQTT-Broker konnte hergestellt werden!`);
      let localStorageUser = JSON.parse(localStorage.getItem("user"));
      robot.currentUserId = localStorageUser.id;
      robot.isConnected = true;
      await UpdateRobot(robot);
      // Update robot element to show connection status and new buttons
      const robotElement = document.getElementById(`robot-${robot.id}`);
      if (!robotElement.querySelector(".connected-info")) {
        const connectedInfo = document.createElement("p");
        connectedInfo.innerText = "Verbunden";
        connectedInfo.style.color = "green";
        connectedInfo.style.fontWeight = "bold";
        connectedInfo.classList.add("connected-info");

        const publishButton = document.createElement("button");
        publishButton.innerText = "Publish Test";
        publishButton.onclick = () => publishTest(robot);
        publishButton.classList.add("publish-button");

        const subscribeButton = document.createElement("button");
        subscribeButton.innerText = "Subscribe Test";
        subscribeButton.onclick = () => subscribeTest(robot);
        subscribeButton.classList.add("subscribe-button");

        robotElement.appendChild(connectedInfo);
        robotElement.appendChild(publishButton);
        robotElement.appendChild(subscribeButton);
        robotElement.classList.add("connected");
      }

      // Entfernen Sie den "MQTT - Verbinden" Button
      const connectButton = robotElement.querySelector(
        "button[onclick*='ConnectToMqtt']"
      );
      if (connectButton) {
        connectButton.remove();
      }
    } else {
      const errorResponse = await response.json();
      alert(`Fehler beim Verbinden mit Mqtt-Broker: ${errorResponse.message}`);
    }
  } catch (error) {
    console.error("Fehler beim Verbinden mit Mqtt-Broker:", error.message);
    alert(`Ein Fehler ist aufgetreten: ${error.Message}`);
  }
}

async function DisconnectFromMqtt(robot) {
  let mqttRequest = {
    BrokerAddress: robot.brokerAddress,
    Port: String(robot.brokerPort),
    Topic: robot.brokerTopic,
  };

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

async function UpdateRobot(robot) {
  let robotRequest = {
    currentUserId: robot.currentUserId,
    isConnected: robot.isConnected,
    isIngame: robot.isIngame,
    color: robot.color,
    name: robot.name,
    brokerAddress: robot.brokerAddress,
    brokerPort: String(robot.brokerPort),
    brokerTopic: robot.brokerTopic,
  };

  try {
    const response = await fetch(`${endpoint}/Robot/${robot.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(robotRequest),
    });

    if (response.ok) {
      await GetRobots(); // Refresh the list of robots
    } else {
      const error = await response.json();
      console.error("Fehler beim Aktualisieren des Roboters:", error.Message);
      alert(`Fehler beim Aktualisieren des Roboters: ${error.Message}`);
    }
  } catch (error) {
    console.error("Fehler:", error.Message);
    alert(`Ein Fehler ist aufgetreten: ${error.Message}`);
  }
}

// Dummy publish test function
function publishTest(robot) {
  console.log(`Publish test for robot ID: ${robot.id}`);
}

// Dummy subscribe test function
function subscribeTest(robot) {
  console.log(`Subscribe test for robot ID: ${robot.id}`);
}

// Optionally, add an event listener to a button to fetch robots when clicked
document
  .getElementById("fetch-robots-button")
  .addEventListener("click", async () => {
    await GetRobots();
  });
