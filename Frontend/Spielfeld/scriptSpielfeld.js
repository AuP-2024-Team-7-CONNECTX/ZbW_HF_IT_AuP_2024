document.addEventListener('DOMContentLoaded', () => {
    // Initialisieren der Sounds für die Spieler
    const redSound = new Audio('../Sounds/red.m4a');
    const blueSound = new Audio('../Sounds/blue.m4a');

    // Initialisierung der Spieler: Zufällige Auswahl wer beginnt
    let currentPlayer = Math.random() < 0.5 ? 'rot' : 'blau';
    let redTotalTime = 0, blueTotalTime = 0;  // Gesamtspielzeit für jeden Spieler
    let redRemaining = 21, blueRemaining = 21; // Verbleibende Steine für jeden Spieler
    let gameStarted = false; // Spielstatus
    let gameTimer; // Timer für Spielzüge

    // DOM-Elemente für die Spielerinformationen
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

    // Initialisierung des Spielzustands: 7x6 Spielfeld
    const gameState = Array.from({ length: 7 }, () => Array(6).fill(null));

    // Aktualisiert die Anzeige der Spielerinformationen
    function updatePlayerInfo() {
        currentPlayerTitle.textContent = `Spieler an der Reihe: ${currentPlayer === 'rot' ? 'Spielername 1' : 'Spielername 2'}`;
        document.title = currentPlayerTitle.textContent;
        updateTimeDisplay();
    }

    // Aktualisiert die Zeit- und Steinanzeigen für beide Spieler
    function updateTimeDisplay() {
        redInfoTimeMove.textContent = redTotalTime.toFixed(1);
        blueInfoTimeMove.textContent = blueTotalTime.toFixed(1);
        redInfoTotalTime.textContent = redTotalTime.toFixed(1);
        blueInfoTotalTime.textContent = blueTotalTime.toFixed(1);
        redInfoRemaining.textContent = redRemaining;
        blueInfoRemaining.textContent = blueRemaining;
    }

    // Startet den Timer bei Beginn eines Zuges
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
            redSound.play();
        } else {
            blueTotalTime += timeElapsed;
            blueRemaining--;
            blueSound.play();
        }
        updateTimeDisplay();
    }

    // Initialisiert die Spalten mit klickbaren Zellen
    function initializeColumn(column) {
        for (let i = 0; i < 6; i++) {
            const cell = document.createElement('div');
            cell.classList.add('cell');
            column.appendChild(cell);
        }
    }

    // Beendet das Spiel und zeigt das Ergebnis an
    function endGame(message) {
        setTimeout(() => {
            alert(message);
            showRestartButton();
            showNewOpponentButton();
            columns.forEach(column => column.removeEventListener('click', handleColumnClick));
        }, 100);
    }

    // Zeigt den Neustart-Button an und deaktiviert die Spielfeld-Interaktion
    function showRestartButton() {
        restartButton.style.display = 'inline-block';
        restartButton.addEventListener('click', handleRestartClick);
        columns.forEach(column => {
            column.style.pointerEvents = 'none';
        });
    }

    // Zeigt den Button für einen neuen Gegner an
    function showNewOpponentButton() {
        newOpponentButton.style.display = 'inline-block';
        newOpponentButton.addEventListener('click', handleNewOpponentClick);
    }

    // Behandelt den Klick auf den Neustart-Button
    function handleRestartClick() {
        window.location.reload();
    }

    // Behandelt Klicks auf eine Spalte und führt Spiellogik durch
    function handleColumnClick(event) {
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

            currentPlayer = currentPlayer === 'rot' ? 'blau' : 'rot';
            updatePlayerInfo();
            if (gameStarted) startGameTimer();
        }
    }

    // Wechselt zur Startseite für ein neues Spiel
    function handleNewOpponentClick() {
        window.location.href = '../Spielstart/spielstart.html';
    }

    // Fügt den Spalten Event Listener hinzu und initialisiert das Spiel
    columns.forEach(column => {
        column.addEventListener('click', handleColumnClick);
    });

    // Initialisiert das Spiel beim Laden der Seite
    function initializeGame() {
        columns.forEach(initializeColumn);
        updateTimeDisplay();
        updatePlayerInfo();
    }

    initializeGame();
});
