async function CreateNewGame() {
  try {
    let robotOne = JSON.parse(localStorage.getItem("robot"));
    let robotTwo = JSON.parse(localStorage.getItem("opponent-robot"));

    let gameMode = localStorage.getItem("game-mode");

    // Assuming robotOne and robotTwo have an "Id" field
    var gameRequest = {
      robotIds: [robotOne.id, robotTwo.id],
      currentMoveId: null,
      state: "Active",
      gameMode: gameMode,
    };

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
    } else {
      const error = await response.json();

      alert("Fehler beim Erstellen des Spiels: " + error.message);
    }
  } catch (error) {
    console.error("Fehler beim Erstellen des Spiels: ", error.message);

    alert("Ein Fehler ist aufgetreten: " + error.message);
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
