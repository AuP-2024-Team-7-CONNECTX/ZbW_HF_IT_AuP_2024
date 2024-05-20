document.addEventListener('DOMContentLoaded', () => {
    // Sounds für die Spieler initialisieren
    const redSound = new Audio('../Sounds/red.m4a');
    const blueSound = new Audio('../Sounds/blue.m4a');

    // Status des Spiels initialisieren
    let currentPlayer = 'rot'; // Roter Spieler beginnt immer

    let redRemaining = 21, blueRemaining = 21;
    let gameStarted = true;

    // Variablen für das Timing
    let redMoveStartTime, blueMoveStartTime;
    let redTotalTime = 0, blueTotalTime = 0;
    let currentMoveStartTime;
    let timerInterval;

    // DOM-Elemente
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

    // Spielzustand: 7 Spalten x 6 Reihen
    const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

    // Spielerinformationen aktualisieren
    function updatePlayerInfo() {
        currentPlayerTitle.textContent = `Spieler an der Reihe: ${currentPlayer === 'rot' ? 'Spielername 1' : 'Spielername 2'}`;
        redInfoRemaining.textContent = redRemaining;
        blueInfoRemaining.textContent = blueRemaining;
    }

    // Einen neuen Zug-Timer starten
    function startMoveTimer() {
        currentMoveStartTime = Date.now();
        if (currentPlayer === 'rot') {
            redMoveStartTime = currentMoveStartTime;
        } else {
            blueMoveStartTime = currentMoveStartTime;
        }

        clearInterval(timerInterval);
        timerInterval = setInterval(updateMoveTimeDisplay, 100); // Alle 100 ms aktualisieren
    }

    // Die Anzeige der Zugzeit in Zehntelsekunden aktualisieren
    function updateMoveTimeDisplay() {
        const elapsedMilliseconds = Date.now() - currentMoveStartTime;
        const elapsedTenths = (elapsedMilliseconds / 1000).toFixed(1);

        if (currentPlayer === 'rot') {
            redTimeMove.textContent = elapsedTenths;
        } else {
            blueTimeMove.textContent = elapsedTenths;
        }
    }

    // Den Zug-Timer stoppen und die Zeit zur Gesamtzeit hinzufügen
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

    // Einen Zug in der angegebenen Spalte verarbeiten
    function handleMove(columnIndex, player) {
        const column = document.querySelector(`.column[data-column="${columnIndex}"]`);
        const cells = Array.from(column.children).reverse();
        const rowIndex = cells.findIndex(cell => !cell.classList.contains('rot') && !cell.classList.contains('blau'));

        if (rowIndex !== -1) {
            const emptyCell = cells[rowIndex];
            emptyCell.classList.add(player);
            gameState[columnIndex][rowIndex] = player;

            // Sound basierend auf dem Spieler abspielen
            if (player === 'rot') {
                redSound.play();
            } else {
                blueSound.play();
            }

            stopMoveTimer();

            // Zum nächsten Spieler wechseln
            if (player === 'rot') {
                redRemaining--;
                currentPlayer = 'blau';
                updatePlayerInfo();
                startMoveTimer();
            } else {
                blueRemaining--;
                currentPlayer = 'rot';
                updatePlayerInfo();
                startMoveTimer();
            }
        }
    }

    // Spalten-Klick-Ereignis (für menschliche Spieler)
    function handleColumnClick(event) {
        if (gameStarted) {
            const columnIndex = parseInt(event.currentTarget.dataset.column);
            handleMove(columnIndex, currentPlayer);
        }
    }

    // Den Neustart-Button anzeigen und die Interaktion mit dem Spielfeld deaktivieren
    function showRestartButton() {
        restartButton.style.display = 'inline-block';
        restartButton.addEventListener('click', handleRestartClick);
        columns.forEach(column => {
            column.style.pointerEvents = 'none';
        });
    }

    // Den Button für einen neuen Gegner anzeigen
    function showNewOpponentButton() {
        newOpponentButton.style.display = 'inline-block';
        newOpponentButton.addEventListener('click', handleNewOpponentClick);
    }

    // Neustart-Spiel-Handler
    function handleRestartClick() {
        resetGame();
        restartButton.style.display = 'none';
        newOpponentButton.style.display = 'none';
    }

    // Handler für einen neuen Gegner
    function handleNewOpponentClick() {
        window.location.href = 'spielstart.html';
    }

    // Das Spielfeld zurücksetzen
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
        currentPlayer = 'rot'; // Immer roter Spieler beginnt
        gameStarted = true;
        updatePlayerInfo();
        startMoveTimer(); // Timer für den roten Spieler starten
    }

    // Die Spalten initialisieren
    columns.forEach(column => {
        column.addEventListener('click', handleColumnClick);
        for (let i = 0; i < 6; i++) {
            const cell = document.createElement('div');
            cell.classList.add('cell');
            column.appendChild(cell);
        }
    });

    updatePlayerInfo();

    // Initiales Spiel starten
    resetGame();
});
