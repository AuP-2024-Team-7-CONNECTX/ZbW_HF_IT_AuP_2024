document
  .getElementById("login-form")
  .addEventListener("submit", function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars

    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;

    // Ladeindikator anzeigen
    displayLoadingIndicator(true);

    // Der vollständige Endpoint, der die Basis-URL aus der Config-Datei verwendet
    var apiUrl = `${endpoint}/User`;

    // Abrufen aller Benutzerdaten vom Server
    fetch(apiUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => response.json())
      .then((users) => {
        // Überprüfung der Anmeldedaten gegen die Benutzerliste
        const user = users.find(
          (user) => user.email === email && user.password === password
        );
        if (user && user.authenticated) {
          console.log("Erfolg:", user);
          window.location.href = "../Spielstart/spielstart.html"; // Anpassen an Ihre tatsächliche Zielseite
        } else if (user && !user.authenticated) {
          throw new Error(
            "Login fehlgeschlagen: Benutzer nicht authentifiziert"
          );
        } else {
          throw new Error(
            "Login fehlgeschlagen: Benutzer nicht gefunden oder falsches Passwort"
          );
        }
      })
      .catch((error) => {
        console.error("Fehler:", error);
        alert(error.message); // Benutzern die Fehlermeldung zeigen
      })
      .finally(() => {
        // Ladeindikator ausblenden
        displayLoadingIndicator(false);
      });
  });

function displayLoadingIndicator(show) {
  const button = document.querySelector('button[type="submit"]');
  if (show) {
    button.innerText = "Lädt...";
    button.disabled = true;
  } else {
    button.innerText = "Login";
    button.disabled = false;
  }
}
