document.addEventListener("DOMContentLoaded", async () => {
  const redSound = new Audio("../Sounds/red.m4a");
  const blueSound = new Audio("../Sounds/blue.m4a");

  var intervalId = "";

  const abortButton = document.getElementById("abort-button");
  if (abortButton) {
    abortButton.addEventListener("click", abortGame);
  }

  // Function to initialize LocalStorage items and create the game
  async function initializeLocalStorageAndGame() {
    const localStorageRobot = JSON.parse(localStorage.getItem("robot"));
    const opponentRobot = JSON.parse(localStorage.getItem("opponent-robot"));
    const gameMode = localStorage.getItem("game-mode");

    if (gameMode === "PlayerVsPlayer") {
      var gameRequest = {
        robotIds: [localStorageRobot.id, opponentRobot.id],
        state: "InProgress",
        gameMode: gameMode,
      };
    } else {
      var gameRequest = {
        robotIds: [localStorageRobot.id],
        state: "InProgress",
        gameMode: gameMode,
      };
    }

    let localStorageUser = JSON.parse(localStorage.getItem("user"));

    let game = await CreateNewGame(gameRequest);

    localStorage.setItem("game-id", game.data.id);
    const playerOne = await getUserById(game.data.user1Id);
    const playerTwo = await getUserById(game.data.user2Id);

    let startingPlayer =
      game.data.startingUserId === playerOne.id ? playerOne : playerTwo;
    const robotOne = await getRobotById(game.data.robot1Id);
    const robotTwo =
      gameMode === "PlayerVsPlayer"
        ? await getRobotById(game.data.robot2Id)
        : robotOne;

    if (startingPlayer.id !== localStorageUser.id) {
      intervalId = setInterval(RegisterNewIncomingTurnFromBackend, 4000);
    }

    return { playerOne, playerTwo, startingPlayer, robotOne, robotTwo };
  }

  // Await the initialization of LocalStorage items and game creation
  const { playerOne, playerTwo, startingPlayer, robotOne, robotTwo } =
    await initializeLocalStorageAndGame();

  // Initialize players based on game mode

  let currentPlayer = startingPlayer;
  let redTotalTime = 0,
    blueTotalTime = 0;
  let redRemaining = 21,
    blueRemaining = 21;
  let gameStarted = false;
  let gameTimer;

  const currentPlayerTitle = document.getElementById("current-player-title");
  const redInfoTimeMove = document.getElementById("red-time-move");
  const blueInfoTimeMove = document.getElementById("blue-time-move");
  const redInfoTotalTime = document.getElementById("red-time-total");
  const blueInfoTotalTime = document.getElementById("blue-time-total");
  const redInfoRemaining = document.getElementById("red-remaining");
  const blueInfoRemaining = document.getElementById("blue-remaining");
  const columns = document.querySelectorAll(".column");
  const redPlayerName = document.getElementById("red-player-name");
  const bluePlayerName = document.getElementById("blue-player-name");

  redPlayerName.textContent = playerOne.name;
  bluePlayerName.textContent = playerTwo.name;

  const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

  async function updatePlayerInfo() {
    currentPlayerTitle.textContent = `Spieler an der Reihe: ${
      currentPlayer.id === playerOne.id ? playerOne.name : playerTwo.name
    }`;
    document.title = currentPlayerTitle.textContent;
    // updateTimeDisplay();

    if (
      currentPlayer.id === startingPlayer.id &&
      !currentPlayer.email.includes("KI_")
    ) {
      columns.forEach((column) => {
        column.style.pointerEvents = "auto";
        column.addEventListener("click", handleColumnClick);
      });
    }
  }

  async function updateTimeDisplay() {
    redInfoTimeMove.textContent = redTotalTime.toFixed(1);
    blueInfoTimeMove.textContent = blueTotalTime.toFixed(1);
    redInfoTotalTime.textContent = redTotalTime.toFixed(1);
    blueInfoTotalTime.textContent = blueTotalTime.toFixed(1);
    redInfoRemaining.textContent = redRemaining;
    blueInfoRemaining.textContent = blueRemaining;
  }

  async function startGameTimer() {
    const startTime = Date.now();
    gameTimer = setInterval(() => {
      const currentTime = Date.now();
      const timeElapsed = (currentTime - startTime) / 1000;
      if (currentPlayer.id === playerOne.id) {
        redInfoTimeMove.textContent = timeElapsed.toFixed(1);
      } else {
        blueInfoTimeMove.textContent = timeElapsed.toFixed(1);
      }
    }, 100);
  }

  async function stopGameTimer() {
    clearInterval(gameTimer);
    var timeElapsed = parseFloat(
      currentPlayer.id === playerOne.id
        ? redInfoTimeMove.textContent
        : blueInfoTimeMove.textContent
    );
    if (currentPlayer.id === playerOne.id) {
      redTotalTime += timeElapsed;
      redRemaining--;
      redSound.play();
    } else {
      blueTotalTime += timeElapsed;
      blueRemaining--;
      blueSound.play();
    }
    await updateTimeDisplay();
    return timeElapsed;
  }

  function initializeColumn(column) {
    for (let i = 0; i < 6; i++) {
      const cell = document.createElement("div");
      cell.classList.add("cell");
      column.appendChild(cell);
    }
  }

  async function handleColumnClick(event) {
    const currentUser = JSON.parse(localStorage.getItem("user"));
    if (currentUser.id !== currentPlayer.id) {
      return;
    }
    displayRobotMoveText("Roboter platziert Stein...");

    if (!gameStarted) {
      await startGameTimer();
      gameStarted = true;
    }
    const column = event.currentTarget;
    if (!column.childElementCount) initializeColumn(column);

    const cells = Array.from(column.children).reverse();
    const rowIndex = cells.findIndex(
      (cell) =>
        !cell.classList.contains("rot") && !cell.classList.contains("blau")
    );
    const emptyCell = cells[rowIndex];

    if (emptyCell) {
      let moveDuration = await stopGameTimer();
      emptyCell.classList.add(
        currentPlayer.id === playerOne.id ? "rot" : "blau"
      );
      gameState[column.dataset.column][rowIndex] =
        currentPlayer.id === playerOne.id ? "rot" : "blau";

      let robot = currentPlayer.id === playerOne.id ? robotOne : robotTwo;
      let game = await getCurrentGame();

      let moveRequest = {
        RobotId: robot.id, // Ersetze durch die tatsächliche Roboter-ID
        MoveDetails: column.dataset.column, // Ersetze durch die tatsächlichen Bewegungsdetails
        Duration: moveDuration,
        GameId: game.id, // Ersetze durch die tatsächliche Spiel-ID,
        TurnWithAlgorithm: false,
      };

      await createMove(moveRequest);

      let gameMode = localStorage.getItem("game-mode");
      let gameRequest = {
        state: "InProgress",
        gameMode: gameMode,
      };

      currentPlayer = currentPlayer.id === playerOne.id ? playerTwo : playerOne;

      await UpdateGame(gameRequest);
      await updatePlayerInfo();

      columns.forEach((column) => {
        column.style.pointerEvents = "none";
        column.removeEventListener("click", handleColumnClick);
      });

      let opponentUser = JSON.parse(localStorage.getItem("opponent-user"));
      if (opponentUser.email.includes("KI_") && gameMode !== "PlayerVsPlayer") {
        let robot = opponentUser.id === playerOne.id ? robotOne : robotTwo;
        let game = await getCurrentGame();

        let moveRequest = {
          RobotId: robot.id,
          MoveDetails: "100",
          Duration: parseFloat(5),
          GameId: game.id,
          TurnWithAlgorithm: true,
        };

        await createMove(moveRequest);
      }
      // Text "Roboter platziert Stein..." anzeigen
      intervalId = setInterval(RegisterNewIncomingTurnFromBackend, 4000);
    }
  }

  async function handleOpponentColumnClick(columnIndex) {
    // Text entfernen
    displayRobotMoveText("");

    let localStorageUser = JSON.parse(localStorage.getItem("user"));

    if (!gameStarted) {
      await startGameTimer();
      gameStarted = true;
    }

    const column = document.querySelector(`[data-column='${columnIndex}']`);
    if (!column.childElementCount) initializeColumn(column);

    const cells = Array.from(column.children).reverse();
    const rowIndex = cells.findIndex(
      (cell) =>
        !cell.classList.contains("rot") && !cell.classList.contains("blau")
    );
    const emptyCell = cells[rowIndex];

    if (emptyCell) {
      emptyCell.classList.add(
        localStorageUser.id === playerOne.id ? "blau" : "rot"
      );
      gameState[column.dataset.column][rowIndex] =
        localStorageUser.id === playerOne.id ? "blau" : "rot";

      currentPlayer = localStorageUser;

      let gameMode = localStorage.getItem("game-mode");
      if (gameMode === "PlayerVsRobot") {
        displayRobotMoveText("Roboter platziert Stein...");

        setTimeout(async () => {
          // Markieren Sie diese Funktion als async
          await updatePlayerInfo();
          clearInterval(intervalId);
          displayRobotMoveText("");
          columns.forEach((column) => {
            column.style.pointerEvents = "auto";
            column.addEventListener("click", handleColumnClick);
          });

          await startGameTimer();
        }, 5);
      } else {
        await updatePlayerInfo();
        clearInterval(intervalId);

        columns.forEach((column) => {
          column.style.pointerEvents = "auto";
          column.addEventListener("click", handleColumnClick);
        });

        await startGameTimer();
      }
    }
  }

  async function initializeGame() {
    columns.forEach(initializeColumn);
    await updateTimeDisplay();
    await updatePlayerInfo();
  }

  await initializeGame();

  async function RegisterNewIncomingTurnFromBackend() {
    let game = await getCurrentGame();

    if (game.state === 1) {
      await abortGame();
    }

    let localStorageUser = JSON.parse(localStorage.getItem("user"));
    if (game.winnerUser && game.winnerUser.id === localStorageUser.id) {
      await endGame();
    }

    if (game.winnerUser && game.winnerUser.id !== localStorageUser.id) {
      let move = await getLatestMoveForGame(game);
      await handleOpponentColumnClick(move.moveDetails);
      await updateOpponentTime(move.duration);
      let localStorageUser = JSON.parse(localStorage.getItem("user"));
      currentPlayer = localStorageUser;

      await endGame();
    }

    if (game.newTurnForFrontend) {
      let localStorageUser = JSON.parse(localStorage.getItem("user"));
      let move = await getLatestMoveForGame(game);

      if (move.playerId !== localStorageUser.id) {
        await handleOpponentColumnClick(move.moveDetails);
        await updateOpponentTime(move.duration);
        let localStorageUser = JSON.parse(localStorage.getItem("user"));
        currentPlayer = localStorageUser;
        await startGameTimer();

        const gameMode = localStorage.getItem("game-mode");
        let gameRequest = {
          state: game.state === "Completed" ? "Completed" : "InProgress",
          gameMode: gameMode,
          newTurnForFrontend: false,
          newTurnForFrontendRowColumn: null,
          ManualTurnIsAllowed: gameMode === "PlayerVsPlayer" ? false : true,
        };

        await UpdateGame(gameRequest);
      }
    }
  }

  async function updateOpponentTime(moveDuration) {
    clearInterval(gameTimer);

    if (currentPlayer.id !== playerOne.id) {
      redTotalTime += moveDuration;
    } else {
      blueTotalTime += moveDuration;
    }
    await updateTimeDisplay();
  }

  async function endGame() {
    clearInterval(intervalId);
    let gameMode = localStorage.getItem("game-mode");

    let gameRequest = {
      state: "Completed",
      gameMode: gameMode,
      currentUserId: null,
    };

    await UpdateGame(gameRequest);

    // Remove specific items from localStorage

    let game = await getCurrentGame();
    let winner = game.winnerUser.name;
    // Show end game message
    const endGameMessage = document.createElement("div");
    endGameMessage.id = "end-game-message";
    endGameMessage.innerHTML = `
  <h1>Spiel beendet!</h1>
  <p>Gewinner: ${winner}</p>
  <button id="back-to-menu">Zurück zum Hauptmenü</button>
`;
    document.body.appendChild(endGameMessage);

    const backToMenuButton = document.getElementById("back-to-menu");
    backToMenuButton.addEventListener("click", async () => {
      window.location.href = "../Hauptmenu/hauptmenu.html";

      let robot = JSON.parse(localStorage.getItem("robot"));
      let robotToDisconnect = await getRobotById(robot.id);
      robotToDisconnect.isConnected = false;
      robotToDisconnect.currentUserId = null;
      await DisconnectFromMqtt(robotToDisconnect);
      await UpdateRobot(robotToDisconnect);

      let opponentUser = JSON.parse(localStorage.getItem("opponent-user"));
      if (opponentUser.email.includes("KI_")) {
        let opponentRobot = JSON.parse(localStorage.getItem("opponent-robot"));
        let robotToDisconnect = await getRobotById(opponentRobot.id);
        robotToDisconnect.isConnected = false;
        robotToDisconnect.currentUserId = null;
        await DisconnectFromMqtt(robotToDisconnect);
        await UpdateRobot(robotToDisconnect);
      }
    });

    columns.forEach((column) => {
      column.style.pointerEvents = "none";
      column.removeEventListener("click", handleColumnClick);
    });

    localStorage.removeItem("robot");
    localStorage.removeItem("opponent-user");
    localStorage.removeItem("opponent-robot");
    localStorage.removeItem("game-mode");
    localStorage.removeItem("game-creator");
    localStorage.removeItem("game-id");
  }

  async function abortGame() {
    clearInterval(intervalId);

    let gameMode = localStorage.getItem("game-mode");

    let gameRequest = {
      state: "Aborted",
      gameMode: gameMode,
      currentUserId: null,
    };

    await UpdateGame(gameRequest);
    let robot = JSON.parse(localStorage.getItem("robot"));
    let robotToDisconnect = await getRobotById(robot.id);
    robotToDisconnect.isConnected = false;
    robotToDisconnect.currentUserId = null;
    await DisconnectFromMqtt(robotToDisconnect);
    await UpdateRobot(robotToDisconnect);
    localStorage.removeItem("robot");
    localStorage.removeItem("opponent-user");
    localStorage.removeItem("opponent-robot");
    localStorage.removeItem("game-mode");
    localStorage.removeItem("game-creator");
    localStorage.removeItem("game-id");

    window.location.href = "../Hauptmenu/hauptmenu.html";
  }

  function displayRobotMoveText(message) {
    const robotMoveText = document.getElementById("robot-move-text");
    if (robotMoveText) {
      robotMoveText.textContent = message;
    } else {
      const newText = document.createElement("div");
      newText.id = "robot-move-text";
      newText.textContent = message;
      newText.style.position = "absolute";
      newText.style.top = "10px";
      newText.style.width = "100%";
      newText.style.textAlign = "center";
      newText.style.backgroundColor = "#ffffff";
      newText.style.padding = "10px";
      newText.style.border = "2px solid #000000";
      newText.style.borderRadius = "5px";
      newText.style.zIndex = "1000";
      document.body.appendChild(newText);
    }
  }
});
