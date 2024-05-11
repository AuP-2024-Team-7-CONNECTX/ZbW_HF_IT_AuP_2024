document.addEventListener("DOMContentLoaded", function () {
  const urlParams = new URLSearchParams(window.location.search);
  const email = urlParams.get("email");
  const token = urlParams.get("token");

  if (email && token) {
    document.getElementById("emailInput").value = email; // Automatically fill the email input field
  }

  const confirmButton = document.getElementById("confirmButton");
  confirmButton.addEventListener("click", function () {
    confirmEmail(email, token);
  });
});

function confirmEmail(email, token) {
  const fullEndpoint = `${endpoint}/User/confirmEmail?email=${encodeURIComponent(
    email
  )}&token=${encodeURIComponent(token)}`;
  fetch(fullEndpoint, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((response) => {
      if (response.ok) {
        alert("Email successfully verified!");
        return;
      }
      throw new Error("Failed to verify email.");
    })
    .catch((error) => {
      alert("Verification failed: " + error.message);
    });
}
