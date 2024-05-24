document.getElementById('register-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    // Validierung der Eingaben
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
        return;
    } else if (!validatePassword(password)) {
        alert("Das Passwort muss mindestens 8 Zeichen lang sein und mindestens einen Grossbuchstaben, eine Zahl und ein Sonderzeichen enthalten.");
        return;
    }

    // Registrierung erfolgreich
    alert('Registrierung erfolgreich! Sie können sich jetzt einloggen.');
    window.location.href = '../Login/Login.html'; // Weiterleitung zur Login-Seite
});

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
