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

// function confirmEmail(email) {
//   const fullEndpoint = `${endpoint}/User/confirmEmail?email=${encodeURIComponent(
//     email
//   )}&token=${encodeURIComponent(token)}`;
//   fetch(fullEndpoint, {
//     method: "POST",
//     headers: {
//       "Content-Type": "text/plain",
//     },
//   })
//     .then((response) => {
//       if (response.ok) {
//         alert("Email successfully verified!");
//         window.location.href = "../Login/login.html";
//       } else {
//         throw new Error("Failed to verify email.");
//       }
//     })
//     .catch((error) => {
//       alert("Verification failed: " + error.message);
//     });
// }

function confirmEmail(email) {
  // const fullEndpoint = `${endpoint}/User/confirmEmail?email=${encodeURIComponent(
  //   email
  // )}`;
  const fullEndpoint = `${endpoint}/User/confirmEmail?email=${email}`;

  fetch(fullEndpoint, {
    method: "POST",
    headers: {
      "Content-Type": "text/plain",
    },
  })
    .then((response) => {
      if (response.ok) {
        alert("Email wurde verifiziert. Sie können sich jetzt einloggen!");
        window.location.href = "../Login/login.html"; // Adjust to your actual target page
      } else {
        throw new Error("Failed to verify email.");
      }
    })
    .catch((error) => {
      alert("Verification failed: " + error.message);
    });
}
