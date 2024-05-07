document.getElementById('register-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var username = document.getElementById('username').value;
    var firstname = document.getElementById('firstname').value;
    var lastname = document.getElementById('lastname').value;
    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    // Validierung der Eingaben
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
        return;
    } else if (!validatePassword(password)) {
        alert("Das Passwort muss mindestens 8 Zeichen lang sein und mindestens einen Großbuchstaben, eine Zahl und ein Sonderzeichen enthalten.");
        return;
    }

    // Erstellung des JSON-Objekts mit den Registrierungsinformationen
    var registrationData = {
        username: username,
        firstname: firstname,
        lastname: lastname,
        email: email,
        password: password
    };

    // Senden der Anforderung an das Backend
    fetch('https://example.com/api/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(registrationData),
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        } else {
            throw new Error('Anmeldung fehlgeschlagen. Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut.');
        }
    })
    .then(data => {
        console.log('Registrierung erfolgreich:', data);
        alert('Registrierung erfolgreich! Sie können sich jetzt einloggen.');
        window.location.href = '../Login/Login.html'; // Weiterleitung zur Login-Seite
    })
    .catch(error => {
        console.error('Fehler:', error);
        alert(error.message);
    });
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
