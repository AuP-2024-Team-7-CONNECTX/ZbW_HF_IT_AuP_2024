document
  .getElementById("reset-password-form")
  .addEventListener("submit", async function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    const newPassword = document.getElementById("password").value;

    try {
      let user = JSON.parse(localStorage.getItem("user"));

      if (!user) {
        alert("User information not found. Please log in again.");
        return;
      }

      let hashedPassword = await hashPassword(newPassword);
      const changePasswordResponse = await fetch(
        `${endpoint}/User/changepassword/?email=${user.email}&newPassword=${hashedPassword}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          mode: "cors",
        }
      );

      if (changePasswordResponse.ok) {
        console.log("Passwort konnte erfolgreich zurückgesetzt werden");
        alert(
          "Passwort konnte erfolgreich zurückgesetzt werden. Sie werden nun abgemeldet!"
        );
        localStorage.removeItem("user");
        localStorage.removeItem("robot");
        window.location.href = "../Login/login.html";
      } else {
        const error = await changePasswordResponse.json();
        alert(error.message);
      }
    } catch (error) {
      console.error("Error:", error.message);
      alert("Ein Fehler ist aufgetreten: " + error.message);
    }
  });

document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  localStorage.removeItem("robot");
  window.location.href = "../Login/login.html";
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
