document.getElementById("logout-button").addEventListener("click", function () {
  localStorage.removeItem("user");
  localStorage.removeItem("robot");
  window.location.href = "../Login/login.html";
});
