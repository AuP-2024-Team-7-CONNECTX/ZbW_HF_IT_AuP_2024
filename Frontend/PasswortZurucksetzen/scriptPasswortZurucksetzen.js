document.getElementById("reset-password-form").addEventListener("submit", async function (event) {
  event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

  const email = document.getElementById("email").value;
  const newPassword = document.getElementById("password").value;

  try {
    // Anfrage zur Änderung des Passworts
    const changePasswordResponse = await fetch(`${endpoint}/User/changepassword`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
        newPassword: newPassword,
      }),
    });

    if (changePasswordResponse.ok) {
      const result = await changePasswordResponse.json();
      alert(result.message);
    } else {
      const error = await changePasswordResponse.json();
      alert(error.message);
    }
  } catch (error) {
    console.error("Error:", error.message);
    alert("Ein Fehler ist aufgetreten: " + error.message);
    return; // Bei einem Fehler hier, führen wir die Benutzerüberprüfung nicht aus.
  }

  try {
    // Benutzerüberprüfung
    const response = await fetch(`${endpoint}/User/${email}`, {
      method: "GET",
      mode: "cors",
    });

    if (response.ok) {
      const users = await response.json();
      console.log("Fetched users:", users);

      const user = users.find(
        (user) => user.email === email && user.password === newPassword 
      );

      if (user && user.authenticated) {
        console.log("Erfolg:", user);
        window.location.href = "../Hauptmenu/hauptmenu.html"; 
      } else if (user && !user.authenticated) {
        alert("Login fehlgeschlagen: Benutzer nicht authentifiziert");
      } else {
        alert("Login fehlgeschlagen: Benutzer nicht gefunden oder falsches Passwort");
      }
    } else {
      console.error("Failed to fetch users:", response.statusText);
    }
  } catch (error) {
    console.error("Error:", error.message);
  }
});

document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  localStorage.removeItem("robot");
  window.location.href = "../Login/login.html";
});
