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
      console.error("Error:", error);
    }
  });
});

async function hashPassword(password) {
  const msgUint8 = new TextEncoder().encode(password); // encode as (utf-8) Uint8Array
  const hashBuffer = await crypto.subtle.digest("SHA-256", msgUint8); // hash the message
  const hashArray = Array.from(new Uint8Array(hashBuffer)); // convert buffer to byte array
  const hashHex = hashArray
    .map((b) => b.toString(16).padStart(2, "0"))
    .join(""); // convert bytes to hex string
  return hashHex;
}
