function goToMainMenu() {
  window.location.href = "../Hauptmenu/hauptmenu.html";
}

function navigateToSpielfeld() {
  window.location.href = "../Spielfeld/spielfeld.html";
}

function goToLobby() {
  window.location.href = "../Lobby/lobby.html";
}

function setLocalStorageGameMode(gameMode) {
  localStorage.setItem("game-mode", gameMode);
}

function removeLocalStorageGameMode() {
  localStorage.removeItem("game-mode");
}
