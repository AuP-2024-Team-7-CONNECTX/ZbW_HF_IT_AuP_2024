document.addEventListener('DOMContentLoaded', () => {
    // Initialize sounds for players
    const redSound = new Audio('../Sounds/red.m4a');
    const blueSound = new Audio('../Sounds/blue.m4a');

    // Initialize first move status
    let firstMoveMade = false;

    // Randomize starting player
    let currentPlayer = Math.random() < 0.5 ? 'rot' : 'blau';
    const computerPlayer = currentPlayer === 'rot' ? 'blau' : 'rot';

    let redRemaining = 21, blueRemaining = 21;
    let gameStarted = true;

    // Timing variables
    let redMoveStartTime, blueMoveStartTime;
    let redTotalTime = 0, blueTotalTime = 0;
    let currentMoveStartTime;
    let timerInterval;

    // DOM elements
    const currentPlayerTitle = document.getElementById('current-player-title');
    const redInfoRemaining = document.getElementById('red-remaining');
    const blueInfoRemaining = document.getElementById('blue-remaining');
    const redTimeMove = document.getElementById('red-time-move');
    const blueTimeMove = document.getElementById('blue-time-move');
    const redTimeTotal = document.getElementById('red-time-total');
    const blueTimeTotal = document.getElementById('blue-time-total');
    const columns = document.querySelectorAll('.column');
    const restartButton = document.getElementById('restart-button');
    const newOpponentButton = document.getElementById('new-opponent-button');

    // Game state: 7 columns x 6 rows
    const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

    // Update player information
    function updatePlayerInfo() {
        currentPlayerTitle.textContent = `Spieler an der Reihe: ${currentPlayer === 'rot' ? 'Spielername 1' : 'Spielername 2'}`;
        redInfoRemaining.textContent = redRemaining;
        blueInfoRemaining.textContent = blueRemaining;
    }

    // Start a new move timer
    function startMoveTimer() {
        currentMoveStartTime = Date.now();
        if (currentPlayer === 'rot') {
            redMoveStartTime = currentMoveStartTime;
        } else {
            blueMoveStartTime = currentMoveStartTime;
        }

        clearInterval(timerInterval);
        timerInterval = setInterval(updateMoveTimeDisplay, 100); // Update every 100 ms
    }

    // Update the move time display in tenths of a second
    function updateMoveTimeDisplay() {
        const elapsedMilliseconds = Date.now() - currentMoveStartTime;
        const elapsedTenths = (elapsedMilliseconds / 1000).toFixed(1);

        if (currentPlayer === 'rot') {
            redTimeMove.textContent = elapsedTenths;
        } else {
            blueTimeMove.textContent = elapsedTenths;
        }
    }

    // Stop the move timer and add the time to the total
    function stopMoveTimer() {
        const elapsedMilliseconds = Date.now() - currentMoveStartTime;
        const elapsedSeconds = (elapsedMilliseconds / 1000).toFixed(1);
        clearInterval(timerInterval);

        if (currentPlayer === 'rot') {
            redTotalTime += parseFloat(elapsedSeconds);
            redTimeTotal.textContent = redTotalTime.toFixed(1);
        } else {
            blueTotalTime += parseFloat(elapsedSeconds);
            blueTimeTotal.textContent = blueTotalTime.toFixed(1);
        }
    }

    // Check for a winner
    function checkForWinner(col, row, player) {
        return checkDirection(player, col, row, 0, 1) || // Horizontal
               checkDirection(player, col, row, 1, 0) || // Vertical
               checkDirection(player, col, row, 1, 1) || // Diagonal /
               checkDirection(player, col, row, 1, -1);  // Diagonal \
    }

    // Check a particular direction for a winning line
    function checkDirection(player, startX, startY, stepX, stepY) {
        let count = 1;

        let col = startX + stepX;
        let row = startY + stepY;
        while (col >= 0 && col < 7 && row >= 0 && row < 6 && gameState[col][row] === player) {
            count++;
            col += stepX;
            row += stepY;
        }

        col = startX - stepX;
        row = startY - stepY;
        while (col >= 0 && col < 7 && row >= 0 && row < 6 && gameState[col][row] === player) {
            count++;
            col -= stepX;
            row -= stepY;
        }

        return count >= 4;
    }

    // Process a move in the given column
    function handleMove(columnIndex, player) {
        const column = document.querySelector(`.column[data-column="${columnIndex}"]`);
        const cells = Array.from(column.children).reverse();
        const rowIndex = cells.findIndex(cell => !cell.classList.contains('rot') && !cell.classList.contains('blau'));

        if (rowIndex !== -1) {
            const emptyCell = cells[rowIndex];
            emptyCell.classList.add(player);
            gameState[columnIndex][rowIndex] = player;

            // Play sound based on player
            if (player === 'rot') {
                redSound.play();
            } else {
                blueSound.play();
            }

            // Begin the timer if it's the first move
            if (!firstMoveMade) {
                firstMoveMade = true;
                startMoveTimer();
            }

            // Check for a win or draw and apply a delay before displaying messages or disabling the board
            const playerName = player === 'rot' ? 'Spielername 1' : 'Spielername 2';

            if (checkForWinner(columnIndex, rowIndex, player)) {
                stopMoveTimer();
                setTimeout(() => {
                    alert(`Spieler ${playerName} hat gewonnen!`);
                    showRestartButton();
                    showNewOpponentButton();
                    gameStarted = false;
                }, 100);
                return;
            }

            if (gameState.every(col => col.every(cell => cell !== null))) {
                stopMoveTimer();
                setTimeout(() => {
                    alert("Unentschieden!");
                    showRestartButton();
                    showNewOpponentButton();
                    gameStarted = false;
                }, 100);
                return;
            }

            stopMoveTimer();

            // Switch to the next player
            if (player === 'rot') {
                redRemaining--;
                currentPlayer = 'blau';
                updatePlayerInfo();
                setTimeout(makeComputerMove, 500);
            } else {
                blueRemaining--;
                currentPlayer = 'rot';
                updatePlayerInfo();
                startMoveTimer();
            }
        }
    }

    // Process the computer's move
    function makeComputerMove() {
        if (currentPlayer !== computerPlayer || !gameStarted) {
            return;
        }

        startMoveTimer(); // Start the timer for the computer's move

        let columnIndex;
        do {
            columnIndex = Math.floor(Math.random() * 7);
        } while (gameState[columnIndex].every(cell => cell !== null)); // Ensure the column isn't full

        setTimeout(() => {
            handleMove(columnIndex, computerPlayer);
            stopMoveTimer(); // Stop the timer for the computer's move
        }, 500);
    }

    // Column click event (for human player)
    function handleColumnClick(event) {
        if (currentPlayer === 'rot' && gameStarted) {
            const columnIndex = parseInt(event.currentTarget.dataset.column);
            handleMove(columnIndex, 'rot');
        }
    }

    // Show the restart button and disable game board interaction
    function showRestartButton() {
        restartButton.style.display = 'inline-block';
        restartButton.addEventListener('click', handleRestartClick);
        columns.forEach(column => {
            column.style.pointerEvents = 'none';
        });
    }

    // Show the new opponent button
    function showNewOpponentButton() {
        newOpponentButton.style.display = 'inline-block';
        newOpponentButton.addEventListener('click', handleNewOpponentClick);
    }

    // Restart game handler
    function handleRestartClick() {
        resetGame();
        restartButton.style.display = 'none';
        newOpponentButton.style.display = 'none';
    }

    // New opponent handler
    function handleNewOpponentClick() {
        window.location.href = 'spielstart.html';
    }

    // Reset the game board
    function resetGame() {
        gameState.forEach(col => col.fill(null));
        columns.forEach(column => {
            column.style.pointerEvents = 'auto';
            Array.from(column.children).forEach(cell => cell.classList.remove('rot', 'blau'));
        });

        redRemaining = 21;
        blueRemaining = 21;
        redTotalTime = 0;
        blueTotalTime = 0;
        redTimeTotal.textContent = '0.0';
        blueTimeTotal.textContent = '0.0';
        currentPlayer = Math.random() < 0.5 ? 'rot' : 'blau';
        firstMoveMade = false;
        gameStarted = true;
        updatePlayerInfo();

        // If the computer player starts the game, make the first move
        if (currentPlayer === computerPlayer) {
            setTimeout(makeComputerMove, 500);
        }
    }

    // Initialize the columns
    columns.forEach(column => {
        column.addEventListener('click', handleColumnClick);
        for (let i = 0; i < 6; i++) {
            const cell = document.createElement('div');
            cell.classList.add('cell');
            column.appendChild(cell);
        }
    });

    updatePlayerInfo();

    // If the computer player starts the game, make the first move
    if (currentPlayer === computerPlayer) {
        setTimeout(makeComputerMove, 500);
    }
});
