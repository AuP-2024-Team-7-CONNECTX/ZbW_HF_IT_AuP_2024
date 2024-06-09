document.addEventListener("DOMContentLoaded", async () => {
  const redSound = new Audio("../Sounds/red.m4a");
  const blueSound = new Audio("../Sounds/blue.m4a");

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
      currentMoveId: null,
      state: "InProgress",
      gameMode: gameMode,
    };

    let localStorageUser = JSON.parse(localStorage.getItem("user"));
    let localStorageGameCreator = JSON.parse(
      localStorage.getItem("game-creator")
    );

    if (localStorageUser === localStorageGameCreator) {
    }

    let game = await CreateNewGame(gameRequest);

    localStorage.setItem("game-id", game.data.id);
    const playerOne = await getUserById(game.data.user1Id);
    const playerTwo = await getUserById(game.data.user2Id);

    let startingPlayer =
      game.data.startingUserId === playerOne.Id ? playerOne : playerTwo;
    const robotOne = await getRobotById(game.data.robot1Id);
    const robotTwo = await getRobotById(game.data.robot2Id);

    if (startingPlayer.id !== localStorageUser.id) {
      setInterval(RegisterNewIncomingTurnFromBackend, 2000);
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
  const restartButton = document.getElementById("restart-button");
  const newOpponentButton = document.getElementById("new-opponent-button");
  const redPlayerName = document.getElementById("red-player-name");
  const bluePlayerName = document.getElementById("blue-player-name");

  redPlayerName.textContent = playerOne.name;
  bluePlayerName.textContent = playerTwo.name;

  const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

  async function updatePlayerInfo() {
    currentPlayerTitle.textContent = `Spieler an der Reihe: ${
      currentPlayer === playerOne ? playerOne.name : playerTwo.name
    }`;
    document.title = currentPlayerTitle.textContent;
    updateTimeDisplay();
    updateColumnEvents();
  }

  function updateTimeDisplay() {
    redInfoTimeMove.textContent = redTotalTime.toFixed(1);
    blueInfoTimeMove.textContent = blueTotalTime.toFixed(1);
    redInfoTotalTime.textContent = redTotalTime.toFixed(1);
    blueInfoTotalTime.textContent = blueTotalTime.toFixed(1);
    redInfoRemaining.textContent = redRemaining;
    blueInfoRemaining.textContent = blueRemaining;
  }

  function startGameTimer() {
    const startTime = Date.now();
    gameTimer = setInterval(() => {
      const currentTime = Date.now();
      const timeElapsed = (currentTime - startTime) / 1000;
      if (currentPlayer === playerOne) {
        redInfoTimeMove.textContent = timeElapsed.toFixed(1);
      } else {
        blueInfoTimeMove.textContent = timeElapsed.toFixed(1);
      }
    }, 100);
  }

  function stopGameTimer() {
    clearInterval(gameTimer);
    const timeElapsed = parseFloat(
      currentPlayer === playerOne
        ? redInfoTimeMove.textContent
        : blueInfoTimeMove.textContent
    );
    if (currentPlayer === playerOne) {
      redTotalTime += timeElapsed;
      redRemaining--;
      redSound.play();
    } else {
      blueTotalTime += timeElapsed;
      blueRemaining--;
      blueSound.play();
    }
    updateTimeDisplay();
  }

  function initializeColumn(column) {
    for (let i = 0; i < 6; i++) {
      const cell = document.createElement("div");
      cell.classList.add("cell");
      column.appendChild(cell);
    }
  }

  async function abortGame() {
    let gameMode = localStorage.getItem("game-mode");
    // Remove specific items from localStorage
    localStorage.removeItem("opponent-user");
    localStorage.removeItem("opponent-robot");
    localStorage.removeItem("game-mode");
    localStorage.removeItem("game-creator");

    let gameRequest = {
      currentMoveId: null,
      state: "Aborted",
      gameMode: gameMode,
    };

    await UpdateGame(gameRequest);

    window.location.href = "../Hauptmenu/hauptmenu.html";
  }

  function showRestartButton() {
    restartButton.style.display = "inline-block";
    restartButton.addEventListener("click", handleRestartClick);
    columns.forEach((column) => {
      column.style.pointerEvents = "none";
    });
  }

  function showNewOpponentButton() {
    newOpponentButton.style.display = "inline-block";
    newOpponentButton.addEventListener("click", handleNewOpponentClick);
  }

  function handleRestartClick() {
    window.location.reload();
  }

  async function handleColumnClick(event) {
    const currentUser = JSON.parse(localStorage.getItem("user"));
    if (currentUser.id !== currentPlayer.id) {
      return;
    }

    if (!gameStarted) {
      startGameTimer();
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
      stopGameTimer();
      emptyCell.classList.add(currentPlayer === playerOne ? "rot" : "blau");
      gameState[column.dataset.column][rowIndex] =
        currentPlayer === playerOne ? "rot" : "blau";

      let robot = currentPlayer === playerOne ? robotOne : robotTwo;
      let game = await getCurrentGame();

      let moveRequest = {
        RobotId: robot.id, // Ersetze durch die tatsächliche Roboter-ID
        MoveDetails: column.dataset.column, // Ersetze durch die tatsächlichen Bewegungsdetails
        Duration: 10.5,
        GameId: game.id, // Ersetze durch die tatsächliche Spiel-ID
      };

      let move = await createMove(moveRequest);
      currentPlayer = currentPlayer === playerOne ? playerTwo : playerOne;

      let gameMode = localStorage.getItem("game-mode");
      let gameRequest = {
        currentMoveId: move.data.id,
        state: "InProgress",
        gameMode: gameMode,
      };

      await UpdateGame(gameRequest);
      updatePlayerInfo();

      if (gameStarted) startGameTimer();
    }
  }

  async function handleOpponentColumnClick(columnIndex) {
    const currentUser = JSON.parse(localStorage.getItem("user"));

    if (!gameStarted) {
      startGameTimer();
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
      stopGameTimer();
      emptyCell.classList.add(currentPlayer === playerOne ? "rot" : "blau");
      gameState[column.dataset.column][rowIndex] =
        currentPlayer === playerOne ? "rot" : "blau";

      let robot = currentPlayer === playerOne ? robotOne : robotTwo;

      let game = await getCurrentGame();

      let moveRequest = {
        RobotId: robot.id, // Ersetze durch die tatsächliche Roboter-ID
        MoveDetails: column.dataset.column, // Ersetze durch die tatsächlichen Bewegungsdetails
        Duration: 10.5,
        GameId: game.id, // Ersetze durch die tatsächliche Spiel-ID
      };

      await createMove(moveRequest);
      currentPlayer = currentPlayer === playerOne ? playerTwo : currentPlayer;

      updatePlayerInfo();

      if (gameStarted) startGameTimer();
    }
  }

  function handleNewOpponentClick() {
    window.location.href = "../Spielstart/spielstart.html";
  }

  function updateColumnEvents() {
    const currentUser = JSON.parse(localStorage.getItem("user"));
    columns.forEach((column) => {
      if (currentUser.id === currentPlayer.id) {
        column.style.pointerEvents = "auto";
        column.addEventListener("click", handleColumnClick);
      } else {
        column.style.pointerEvents = "none";
        column.removeEventListener("click", handleColumnClick);
      }
    });
  }

  async function initializeGame() {
    columns.forEach(initializeColumn);
    updateTimeDisplay();
    updatePlayerInfo();

    // let localStorageUser = JSON.parse(localStorage.getItem("user"));
    // if (currentPlayer !== localStorageUser) {
    //   setInterval(RegisterNewIncomingTurnFromBackend, 2000);
    // }
  }

  await initializeGame();

  async function RegisterNewIncomingTurnFromBackend() {
    let game = await getCurrentGame();

    if (game.state === 1) {
      abortGame();
    }

    if (game.state === 0) {
      // endGame();
    }

    if (game.newTurnForFrontend) {
      await handleOpponentColumnClick(game.newTurnForFrontendRowColumn);
    }
  }
});
