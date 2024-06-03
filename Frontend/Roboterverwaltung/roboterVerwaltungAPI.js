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
    let robot = await response.json();
    return robot;
  } else {
    console.error("Failed to fetch Robot:", response.statusText);
    alert.apply("Fehler beim Ermitteln des Roboters");
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
    alert(`Publish Auf Topic ${mqttRequest.Topic} erfolgreich`);
  } else {
    const errorResponse = await response.json();
    alert(`Fehler beim publish-Test: ${errorResponse.Message}`);
  }
}

// Create a new robot
async function createRobot(robot) {
  try {
    const response = await fetch(`${endpoint}/Robot`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        currentUserId: robot.currentUserId,
        isConnected: robot.isConnected,
        isIngame: robot.isIngame,
        color: robot.color,
        name: robot.name,
        brokerAddress: robot.brokerAddress,
        brokerPort: robot.brokerPort,
        brokerTopic: robot.brokerTopic,
      }),
    });

    if (response.ok) {
      const result = await response.json();
      console.log("Roboter erfolgreich hinzugefügt:", result);
      alert("Roboter erfolgreich hinzugefügt!");
      await GetRobots(); // Refresh the list of robots
    } else {
      const error = await response.json();
      console.error("Fehler beim Hinzufügen des Roboters:", error.message);
      alert(`Fehler beim Hinzufügen des Roboters: ${error.message}`);
    }
  } catch (error) {
    console.error("Fehler:", error.message);
    alert(`Ein Fehler ist aufgetreten: ${error.message}`);
  }
}
