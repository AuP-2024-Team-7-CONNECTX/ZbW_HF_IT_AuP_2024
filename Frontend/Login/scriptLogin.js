document.addEventListener("DOMContentLoaded", () => {
  const loginForm = document.getElementById("login-form");
  const loginButton = document.getElementById("login-button");

  loginForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    // Überprüfen, ob die Eingabefelder nicht leer sind (einfache Validierung)
    if (!email || !password) {
      alert("Bitte E-Mail und Passwort eingeben.");
      return;
    }

    try {
      const response = await fetch(`${endpoint}/User`, {
        // Setze hier deinen API-Endpunkt ein
        method: "GET",
        mode: "cors", // oder 'no-cors', falls notwendig
      });

      if (response.ok) {
        const users = await response.json();
        console.log("Fetched users:", users);

        const user = users.find(
          (user) => user.email === email && user.password === password
        );

        if (user && user.authenticated) {
          console.log("Erfolg:", user);
          window.location.href = "../Spielstart/spielstart.html"; // Anpassen an Ihre tatsächliche Zielseite
        } else if (user && !user.authenticated) {
          alert("Login fehlgeschlagen: Benutzer nicht authentifiziert");
        } else {
          alert(
            "LLogin fehlgeschlagen: Benutzer nicht gefunden oder falsches Passwort"
          );
        }
      } else {
        console.error("Failed to fetch users:", response.statusText);
      }
    } catch (error) {
      console.error("Error:", error);
    }
  });
});
