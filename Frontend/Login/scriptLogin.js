document.getElementById('login-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;


    // Ladeindikator anzeigen
    displayLoadingIndicator(true);

    // Erstellung des JSON-Objekts mit den Anmeldeinformationen
    var loginData = {
        email: email,
        password: password
    };

    // Konvertierung des JSON-Objekts in einen String und Senden der Daten
    fetch('http://localhost:5260/User', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(loginData),
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        } else {
            response.json().then(data => {
                throw new Error(data.message || 'Login fehlgeschlagen');
            });
        }
    })
    .then(data => {
        console.log('Erfolg:', data);
        // Weiterleitung oder Benachrichtigung des Benutzers nach erfolgreicher Authentifizierung
        window.location.href = '../Spielstart/spielstart.html'; // Anpassen an deine tatsächliche Zielseite
    })
    .catch((error) => {
        console.error('Fehler:', error);
        alert('Login fehlgeschlagen: ' + error.message);  // Benutzern die Fehlermeldung zeigen
    })
    .finally(() => {
        // Ladeindikator ausblenden
        displayLoadingIndicator(false);
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

function displayLoadingIndicator(show) {
    const button = document.querySelector('button[type="submit"]');
    if (show) {
        button.innerText = 'Lädt...';
        button.disabled = true;
    } else {
        button.innerText = 'Login';
        button.disabled = false;
    }
}
