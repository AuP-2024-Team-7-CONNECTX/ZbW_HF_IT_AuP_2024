document.addEventListener('DOMContentLoaded', () => {
    let currentPlayer = Math.random() < 0.5 ? 'rot' : 'blau';
    let redTotalTime = 0, blueTotalTime = 0;
    let redRemaining = 21, blueRemaining = 21;
    let gameStarted = false;
    let gameTimer;

    const currentPlayerTitle = document.getElementById('current-player-title');
    const redInfoTimeMove = document.getElementById('red-time-move');
    const blueInfoTimeMove = document.getElementById('blue-time-move');
    const redInfoTotalTime = document.getElementById('red-time-total');
    const blueInfoTotalTime = document.getElementById('blue-time-total');
    const redInfoRemaining = document.getElementById('red-remaining');
    const blueInfoRemaining = document.getElementById('blue-remaining');
    const columns = document.querySelectorAll('.column');
    const restartButton = document.getElementById('restart-button');
    const newOpponentButton = document.getElementById('new-opponent-button');

    const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

    function updatePlayerInfo() {
        currentPlayerTitle.textContent = `Spieler an der Reihe: ${currentPlayer === 'rot' ? 'Spielername 1' : 'Spielername 2'}`;
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
            if (currentPlayer === 'rot') {
                redInfoTimeMove.textContent = timeElapsed.toFixed(1);
            } else {
                blueInfoTimeMove.textContent = timeElapsed.toFixed(1);
            }
        }, 100);
    }

    function stopGameTimer() {
        clearInterval(gameTimer);
        const timeElapsed = parseFloat(currentPlayer === 'rot' ? redInfoTimeMove.textContent : blueInfoTimeMove.textContent);
        if (currentPlayer === 'rot') {
            redTotalTime += timeElapsed;
            redRemaining--;
        } else {
            blueTotalTime += timeElapsed;
            blueRemaining--;
        }
        updateTimeDisplay();
    }

    function checkDraw() {
        return [...gameState].flat().every(cell => cell !== null);
    }

    function checkForWinner(col, row, player) {
        return checkLine(player, col, row, 0, 1) || 
               checkLine(player, col, row, 1, 0) ||
               checkLine(player, col, row, 1, 1) ||
               checkLine(player, col, row, 1, -1);
    }

    function checkLine(player, startX, startY, stepX, stepY) {
        let count = 0;
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
        return count >= 3;
    }

    function initializeColumn(column) {
        for (let i = 0; i < 6; i++) {
            const cell = document.createElement('div');
            cell.classList.add('cell');
            column.appendChild(cell);
        }
    }

    function endGame(message) {
        alert(message);
        showRestartButton();
        showNewOpponentButton();
        columns.forEach(column => column.removeEventListener('click', handleColumnClick));
    }

    function showRestartButton() {
        restartButton.style.display = 'inline-block';
        restartButton.addEventListener('click', handleRestartClick);
        columns.forEach(column => {
            column.style.pointerEvents = 'none';
        });
    }

    function showNewOpponentButton() {
        newOpponentButton.style.display = 'inline-block';
        newOpponentButton.addEventListener('click', handleNewOpponentClick);
    }

    function handleRestartClick() {
        window.location.reload();
    }

    function handleColumnClick(event) {
        // Diese Funktion wird als Event-Handler für Klickereignisse verwendet.
        if (!gameStarted) {
            startGameTimer();
            gameStarted = true;
        }
        const column = event.currentTarget;
        if (!column.childElementCount) initializeColumn(column);

        const cells = Array.from(column.children).reverse();
        const rowIndex = cells.findIndex(cell => !cell.classList.contains('rot') && !cell.classList.contains('blau'));
        const emptyCell = cells[rowIndex];

        if (emptyCell) {
            stopGameTimer();
            emptyCell.classList.add(currentPlayer);
            gameState[column.dataset.column][rowIndex] = currentPlayer;

            if (checkForWinner(parseInt(column.dataset.column), rowIndex, currentPlayer)) {
                emptyCell.classList.add('win');
                endGame(`Spieler ${currentPlayer} hat gewonnen!`);
                gameStarted = false;
                return;
            }
            
            if (checkDraw()) {
                endGame("Unentschieden!");
                gameStarted = false;
                return;
            }

            currentPlayer = currentPlayer === 'rot' ? 'blau' : 'rot';
            updatePlayerInfo();
            if (gameStarted) startGameTimer();
        }
    }

    function handleNewOpponentClick() {
        window.location.href = '../Spielstart/spielstart.html'; // Ändern Sie die URL entsprechend der Seite, auf die Sie zurückkehren möchten
    }

    columns.forEach(column => {
        column.addEventListener('click', handleColumnClick);
    });

    function initializeGame() {
        columns.forEach(initializeColumn);
        updateTimeDisplay();
        updatePlayerInfo();
    }

    initializeGame();
});
