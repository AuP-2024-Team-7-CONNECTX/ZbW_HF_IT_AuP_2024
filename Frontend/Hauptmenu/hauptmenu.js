document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  window.location.href = "../Login/login.html";
});
