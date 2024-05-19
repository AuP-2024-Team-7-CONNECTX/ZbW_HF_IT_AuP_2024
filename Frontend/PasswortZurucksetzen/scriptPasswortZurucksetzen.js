document.getElementById('reset-password-form').addEventListener('submit', function(event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars, also das Neuladen der Seite.

    var email = document.getElementById('email').value;

    // Einfache Überprüfung der E-Mail-Adresse auf Gültigkeit
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
        return;
    }

    // Erstellung des JSON-Objekts mit der E-Mail-Adresse
    var requestData = {
        email: email
    };

    // Senden der Anforderung an das Backend
    fetch('https://example.com/api/reset-password', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(requestData),
    })
    .then(response => response.json())
    .then(data => {
        // Hier könntest du eine Bestätigungsnachricht anzeigen oder den Benutzer weiterleiten
        console.log('Erfolg:', data.message);
        alert('Ein Link zum Zurücksetzen Ihres Passworts wurde gesendet. Bitte überprüfen Sie Ihr E-Mail-Postfach.');
    })
    .catch(error => {
        console.error('Fehler:', error);
        alert('Ein Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.');
    });
});

function validateEmail(email) {
    var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return regex.test(email);
}
