document.getElementById('reset-password-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var email = document.getElementById('email').value;

    // Einfache Überprüfung der E-Mail-Adresse auf Gültigkeit
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
        return;
    }

    alert('Ein Link zum Zurücksetzen Ihres Passworts wurde gesendet. Bitte überprüfen Sie Ihr E-Mail-Postfach.');
});

function validateEmail(email) {
    var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return regex.test(email);
}
