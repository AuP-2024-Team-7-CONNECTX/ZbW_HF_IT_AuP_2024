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

    var gameRequest = {
      robotIds: [localStorageRobot.id, opponentRobot.id],
      state: "InProgress",
      gameMode: gameMode,
    };

    let localStorageUser = JSON.parse(localStorage.getItem("user"));

    let game = await CreateNewGame(gameRequest);

    localStorage.setItem("game-id", game.data.id);
    const playerOne = await getUserById(game.data.user1Id);
    const playerTwo = await getUserById(game.data.user2Id);

    let startingPlayer =
      game.data.startingUserId === playerOne.id ? playerOne : playerTwo;
    const robotOne = await getRobotById(game.data.robot1Id);
    const robotTwo = await getRobotById(game.data.robot2Id);

    if (startingPlayer.id !== localStorageUser.id) {
      intervalId = setInterval(RegisterNewIncomingTurnFromBackend, 2000);
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

    if (currentPlayer.id === startingPlayer.id) {
      columns.forEach((column) => {
        column.style.pointerEvents = "auto";
        column.addEventListener("click", handleColumnClick);
      });
    }
  }

  function updateTimeDisplay() {
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
    updateTimeDisplay();
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

      intervalId = setInterval(RegisterNewIncomingTurnFromBackend, 2000);
    }
  }

  async function handleOpponentColumnClick(columnIndex) {
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

      await updatePlayerInfo();
      clearInterval(intervalId);

      columns.forEach((column) => {
        column.style.pointerEvents = "auto";
        column.addEventListener("click", handleColumnClick);
      });

      await startGameTimer();
    }
  }

  async function initializeGame() {
    columns.forEach(initializeColumn);
    updateTimeDisplay();
    updatePlayerInfo();
  }

  await initializeGame();

  async function RegisterNewIncomingTurnFromBackend() {
    let game = await getCurrentGame();

    if (game.state === 1) {
      await abortGame();
    }

    if (game.newTurnForFrontend) {
      let localStorageUser = JSON.parse(localStorage.getItem("user"));
      let move = await getLatestMoveForGame(game);

      if (move.playerId !== localStorageUser.id) {
        await handleOpponentColumnClick(game.newTurnForFrontendRowColumn);
        await updateOpponentTime(move.duration);
        const gameMode = localStorage.getItem("game-mode");
        let gameRequest = {
          state: "InProgress",
          gameMode: gameMode,
          newTurnForFrontend: false,
          newTurnForFrontendRowColumn: null,
          ManualTurnIsAllowed: false,
        };

        await UpdateGame(gameRequest);
      }
    }

    if (game.state === 0) {
      await endGame();
    }
  }

  async function updateOpponentTime(moveDuration) {
    clearInterval(gameTimer);

    if (currentPlayer.id !== playerOne.id) {
      redTotalTime += moveDuration;
    } else {
      blueTotalTime += moveDuration;
    }
    updateTimeDisplay();
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
    let robot = JSON.parse(localStorage.getItem("robot"));
    let robotToDisconnect = await getRobotById(robot.id);
    robotToDisconnect.isConnected = false;
    robotToDisconnect.currentUserId = null;
    await DisconnectFromMqtt(robotToDisconnect);
    await UpdateRobot(robotToDisconnect);
    // Remove specific items from localStorage
    // localStorage.removeItem("robot");
    // localStorage.removeItem("opponent-user");
    // localStorage.removeItem("opponent-robot");
    // localStorage.removeItem("game-mode");
    // localStorage.removeItem("game-creator");
    // localStorage.removeItem("game-id");

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
    backToMenuButton.addEventListener("click", () => {
      window.location.href = "../Hauptmenu/hauptmenu.html";
    });

    columns.forEach((column) => {
      column.style.pointerEvents = "none";
      column.removeEventListener("click", handleColumnClick);
    });
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
    // localStorage.removeItem("robot");
    // localStorage.removeItem("opponent-user");
    // localStorage.removeItem("opponent-robot");
    // localStorage.removeItem("game-mode");
    // localStorage.removeItem("game-creator");
    // localStorage.removeItem("game-id");

    window.location.href = "../Hauptmenu/hauptmenu.html";
  }
});
