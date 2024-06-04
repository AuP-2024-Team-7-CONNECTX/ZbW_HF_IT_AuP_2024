document.addEventListener("DOMContentLoaded", async () => {
  const redSound = new Audio("../Sounds/red.m4a");
  const blueSound = new Audio("../Sounds/blue.m4a");

  // Function to initialize LocalStorage items and create the game
  async function initializeLocalStorageAndGame() {
    const localStorageRobot = JSON.parse(localStorage.getItem("robot"));
    const opponentRobot = JSON.parse(localStorage.getItem("opponent-robot"));
    const gameMode = localStorage.getItem("game-mode");

    var gameRequest = {
      robotIds: [localStorageRobot.id, opponentRobot.id],
      currentMoveId: null,
      state: "Active",
      gameMode: gameMode,
    };

    let game = await CreateNewGame(gameRequest);
    const playerOne = await getUserById(game.data.user1Id);
    const playerTwo = await getUserById(game.data.user2Id);

    const robotOne = await getRobotById(game.data.robot1Id);
    const robotTwo = await getRobotById(game.data.robot2Id);

    return { playerOne, playerTwo, robotOne, robotTwo };
  }

  // Await the initialization of LocalStorage items and game creation
  const { playerOne, playerTwo, robotOne, robotTwo } =
    await initializeLocalStorageAndGame();

  // Initialize players based on game mode

  let currentPlayer = Math.random() < 0.5 ? playerOne : playerTwo;
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

  function updatePlayerInfo() {
    currentPlayerTitle.textContent = `Spieler an der Reihe: ${
      currentPlayer === playerOne ? playerOne.name : playerTwo.name
    }`;
    document.title = currentPlayerTitle.textContent;
    updateTimeDisplay();
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

  function endGame(message) {
    setTimeout(() => {
      alert(message);
      showRestartButton();
      showNewOpponentButton();
      columns.forEach((column) =>
        column.removeEventListener("click", handleColumnClick)
      );
    }, 100);
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

  function handleColumnClick(event) {
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

      currentPlayer = currentPlayer === playerOne ? playerTwo : playerOne;
      updatePlayerInfo();
      if (gameStarted) startGameTimer();
    }
  }

  function handleNewOpponentClick() {
    window.location.href = "../Spielstart/spielstart.html";
  }

  columns.forEach((column) => {
    column.addEventListener("click", handleColumnClick);
  });

  async function initializeGame() {
    columns.forEach(initializeColumn);
    updateTimeDisplay();
    updatePlayerInfo();
  }

  await initializeGame();
});
