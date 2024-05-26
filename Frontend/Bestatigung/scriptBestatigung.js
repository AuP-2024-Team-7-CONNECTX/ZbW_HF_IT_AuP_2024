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

function confirmEmail(email) {
  const fullEndpoint = `${endpoint}/User/confirmEmail?email=${email}`;

  fetch(fullEndpoint, {
    method: "POST",
    headers: {
      "Content-Type": "text/plain",
    },
    mode: "cors",
  })
    .then((response) => {
      if (response.ok) {
        alert("Email wurde verifiziert. Sie können sich jetzt einloggen!");
        window.location.href = "../Login/login.html";
      } else {
        throw new Error("Failed to verify email.");
      }
    })
    .catch((error) => {
      alert("Verification failed: " + error.message);
    });
}
