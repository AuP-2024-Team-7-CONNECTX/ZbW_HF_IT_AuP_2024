document.getElementById('login-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    // Validierung der Eingaben
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
    } else if (!validatePassword(password)) {
        alert("Das Passwort muss mindestens 8 Zeichen lang sein und mindestens einen Großbuchstaben, eine Zahl und ein Sonderzeichen enthalten.");
    } else {
        // Erstellung des JSON-Objekts mit den Anmeldeinformationen
        var loginData = {
            email: email,
            password: password
        };

        // Konvertierung des JSON-Objekts in einen String und Senden der Daten
        fetch('http://example.com/api/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginData),
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Netzwerkantwort war nicht ok.');
            }
        })
        .then(data => {
            console.log('Erfolg:', data);
            // Weiterleitung oder Benachrichtigung des Benutzers nach erfolgreicher Authentifizierung
        })
        .catch((error) => {
            console.error('Fehler:', error);
        });
    }
});

// Validierungsfunktionen (bereits vorhanden)
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
