document.addEventListener("DOMContentLoaded", function () {
  var user = JSON.parse(localStorage.getItem("user"));
  var userElement = document.getElementById("user-info");

  if (user) {
    if (userElement) {
      userElement.textContent = `Angemeldet als: ${user.name}`;
    }
  } else {
    // Umleiten zur Login-Seite, wenn kein Benutzer angemeldet ist
    window.location.href = "../Login/login.html";
  }
});

document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  window.location.href = "../Login/login.html";
});
