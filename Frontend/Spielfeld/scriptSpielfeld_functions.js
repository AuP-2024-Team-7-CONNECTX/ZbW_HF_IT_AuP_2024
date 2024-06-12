async function CreateNewGame(gameRequest) {
  try {
    const response = await fetch(`${endpoint}/Game`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
      body: JSON.stringify(gameRequest),
    });

    if (response.ok) {
      const result = await response.json();
      console.log(result);
      return result;
    } else {
      const error = await response.json();

      alert("Fehler beim Erstellen des Spiels: " + error.Message);
    }
  } catch (error) {
    console.error("Fehler beim Erstellen des Spiels: ", error.message);

    alert("Ein Fehler ist aufgetreten: " + error.message);
  }
}

async function UpdateGame(gameRequest) {
  try {
    let game = await getCurrentGame();

    const response = await fetch(`${endpoint}/Game/${game.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
      body: JSON.stringify(gameRequest),
    });

    if (response.ok) {
      const result = await response.json();
      console.log(result);
      return result;
    } else {
      const error = await response.json();

      alert("Fehler beim Aktualisieren des Spiels: " + error.Message);
    }
  } catch (error) {
    console.error("Fehler beim Aktualisieren des Spiels: ", error.message);

    alert("Ein Fehler ist aufgetreten: " + error.message);
  }
}

async function getUserById(userId) {
  const response = await fetch(`${endpoint}/User/${userId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const user = await response.json();
    return user;
  } else {
    console.error("Failed to fetch user:", response.statusText);
    alert("Fehler beim Ermitteln des Users");
  }
}

async function getRobotById(robotId) {
  const response = await fetch(`${endpoint}/Robot/${robotId}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const user = await response.json();
    return user;
  } else {
    console.error("Failed to fetch robot:", response.statusText);
    alert("Fehler beim Ermitteln des Roboters");
  }
}

async function GetKIUser1() {
  try {
    const response = await fetch(`${endpoint}/User`, {
      // Setze hier deinen API-Endpunkt ein
      method: "GET",
      mode: "cors",
    });

    if (response.ok) {
      const users = await response.json();
      const user = users.find((user) => user.email === "KI (Terminator)");
      return user;
    } else {
      console.error("Failed to fetch KI-users:", response.statusText);
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
}

async function GetKIUser2() {
  try {
    const response = await fetch(`${endpoint}/User`, {
      // Setze hier deinen API-Endpunkt ein
      method: "GET",
      mode: "cors",
    });

    if (response.ok) {
      const users = await response.json();
      const user = users.find((user) => user.email === "KI (Agent Smith)");
      return user;
    } else {
      console.error("Failed to fetch KI-users:", response.statusText);
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
}

async function createMove(moveRequest) {
  try {
    const response = await fetch(`${endpoint}/Move`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
      body: JSON.stringify(moveRequest),
    });

    if (response.ok) {
      const result = await response.json();
      console.log(result);
      return result;
    } else {
      const error = await response.json();
      alert("Fehler beim Erstellen des Zuges: " + error.message);
    }
  } catch (error) {
    console.error("Fehler beim Erstellen des Zuges:", error.message);
    alert("Ein Fehler ist aufgetreten: " + error.message);
  }
}

async function getLatestMoveForGame(game) {
  try {
    const response = await fetch(`${endpoint}/Move`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
    });

    if (response.ok) {
      const result = await response.json();

      // Filter und Sortierung
      let filteredMoves = result.sort(
        (a, b) => new Date(b.insertedOn) - new Date(a.insertedOn)
      );

      if (filteredMoves.length > 0) {
        const latestMove = filteredMoves[0];
        console.log(latestMove);
        return latestMove;
      } else {
        alert("Keine Züge gefunden.");
      }
    } else {
      const error = await response.json();
      alert("Fehler beim Erstellen des Zuges: " + error.message);
    }
  } catch (error) {
    console.error("Fehler beim Erstellen des Zuges:", error.message);
    alert("Ein Fehler ist aufgetreten: " + error.message);
  }
}

async function getCurrentGame() {
  try {
    let gameId = localStorage.getItem("game-id");
    const response = await fetch(`${endpoint}/Game/${gameId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
    });

    if (response.ok) {
      const result = await response.json();
      console.log(result);
      return result;
    } else {
      const error = await response.json();
      console.log(error);
      // alert("Fehler beim Laden des Spiels: " + error.Message);
    }
  } catch (error) {
    console.error("Fehler beim Laden des Spiels: ", error.message);
    // alert("Fehler beim Laden des Spiels: " + error.message);
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
