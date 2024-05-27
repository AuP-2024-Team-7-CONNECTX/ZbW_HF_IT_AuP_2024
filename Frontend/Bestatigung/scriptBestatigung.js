document.addEventListener("DOMContentLoaded", function () {
  const urlParams = new URLSearchParams(window.location.search);
  const email = urlParams.get("email");

  if (email) {
    document.getElementById("emailInput").value = email;
  }

  const confirmButton = document.getElementById("confirmButton");
  confirmButton.addEventListener("click", function () {
    confirmEmail(email || document.getElementById("emailInput").value);
  });
});

async function confirmEmail(email) {
  const fullEndpoint = `${endpoint}/User/confirmEmail?email=${email}`;

  try {
    const response = await fetch(fullEndpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
    });

    if (response.ok) {
      const data = await response.json(); // Antwort als JSON parsen

      alert("Email wurde verifiziert. Sie können sich jetzt einloggen!");
      window.location.href = "../Login/login.html";
    } else {
      const error = await response.json();
      throw new Error("Failed to verify email: " + error.message);
    }
  } catch (error) {
    alert("Verification failed: " + error.message);
  }
}
