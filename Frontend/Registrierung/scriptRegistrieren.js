document
  .getElementById("register-form")
  .addEventListener("submit", async function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;

    // Validierung der Eingaben
    if (!validateEmail(email)) {
      alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
      return;
    } else if (!validatePassword(password)) {
      alert(
        "Das Passwort muss mindestens 8 Zeichen lang sein und mindestens einen Grossbuchstaben, eine Zahl und ein Sonderzeichen enthalten."
      );
      return;
    }

    let hashedPassword = await hashPassword(password);

    // Erstellen des UserRequest-Objekts
    var userRequest = {
      Name: email,
      Email: email,
      Authenticated: false, // Standardwert, da der Benutzer sich gerade registriert.
      Password: hashedPassword,
      isIngame: false,
    };

    // Senden der POST-Anfrage
    try {
      const response = await fetch(`${endpoint}/User`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        mode: "cors",
        body: JSON.stringify(userRequest),
      });

      if (response.ok) {
        alert(
          "Registrierung erfolgreich! Sie erhalten ein Mail zur Verifizierung!"
        );

        await sendRegistrationEmail(email);

        // Aufruf der Funktion zum Senden der Registrierungs-E-Mail
      } else {
        const error = await response.json();
        alert("Fehler bei der Registrierung: " + error.message);
      }
    } catch (error) {
      console.error("Fehler bei der Registrierung: ", error.message);
      alert("Ein Fehler ist aufgetreten: " + error.message);
    }
  });

document
  .getElementById("resend-email")
  .addEventListener("click", async function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Buttons

    var email = document.getElementById("email").value;
    await sendRegistrationEmail(email);
  });

async function sendRegistrationEmail(email) {
  try {
    const response = await fetch(
      `${endpoint}/User/registeremail?email=${email}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "text/plain",
        },
        mode: "cors",
      }
    );
    if (response.ok) {
      console.log("Registrierungs-E-Mail wurde erfolgreich gesendet.");
      alert("Registrierungs-E-Mail wurde erfolgreich gesendet.");
    } else {
      const error = await response.json();
      alert("Fehler beim Email-Versand: " + error.message);
    }
  } catch (error) {
    console.error(
      "Fehler beim Versand der Registrierungs-E-Mail: ",
      error.message
    );
    alert("Ein Fehler ist aufgetreten: " + error.message);
  }
}

function validateEmail(email) {
  var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
  return regex.test(email);
}

function validatePassword(password) {
  var hasUpperCase = /[A-Z]/.test(password);
  var hasNumber = /[0-9]/.test(password);
  var hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  var isValidLength = password.length >= 8;
  return hasUpperCase && hasNumber && hasSpecialChar && isValidLength;
}

async function hashPassword(password) {
  const msgUint8 = new TextEncoder().encode(password); // encode as (utf-8) Uint8Array
  const hashBuffer = await crypto.subtle.digest("SHA-256", msgUint8); // hash the message
  const hashArray = Array.from(new Uint8Array(hashBuffer)); // convert buffer to byte array
  const hashHex = hashArray
    .map((b) => b.toString(16).padStart(2, "0"))
    .join(""); // convert bytes to hex string
  return hashHex;
}
