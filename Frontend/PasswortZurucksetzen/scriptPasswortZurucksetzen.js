document
  .getElementById("reset-password-form")
  .addEventListener("submit", async function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var email = document.getElementById("email").value;

    try {
      const response = await fetch(`${endpoint}/User/${email}`, {
        // Setze hier deinen API-Endpunkt ein
        method: "GET",
        mode: "cors",
      });

      if (response.ok) {
        const users = await response.json();
        console.log("Fetched users:", users);

        const hashedPassword = await hashPassword(password);

        const user = users.find(
          (user) => user.email === email && user.password === hashedPassword
        );

        if (user && user.authenticated) {
          console.log("Erfolg:", user);
          window.location.href = "../Hauptmenu/hauptmenu.html"; // Anpassen an Ihre tatsächliche Zielseite
        } else if (user && !user.authenticated) {
          alert("Login fehlgeschlagen: Benutzer nicht authentifiziert");
        } else {
          alert(
            "Login fehlgeschlagen: Benutzer nicht gefunden oder falsches Passwort"
          );
        }
      } else {
        console.error("Failed to fetch users:", response.statusText);
      }
    } catch (error) {
      console.error("Error:", error.message);
    }
  });
