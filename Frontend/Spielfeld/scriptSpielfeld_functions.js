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

async function getGame() {
  const response = await fetch(`${endpoint}/Game//${localStorageUser.id}`, {
    method: "GET",
    mode: "cors",
  });

  if (response.ok) {
    const data = await response.json();
    if (data.success) {
      await SetOpponentsForLocalStorage(data.senderId);
      window.location.href = "../Spielfeld/spielfeld.html";
    } else {
      console.log(data.message);
    }
  } else {
    const errorData = await response.json();
    console.error("Error:", errorData.message);
  }
}
