async function register() {
    const user = {
        AdSoyad: document.getElementById('name').value,
        EPosta: document.getElementById('email').value,
        Sifre: document.getElementById('password').value
    };

    try {
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Kayıt başarısız');
        }

        alert('Kayıt başarılı! Giriş yapabilirsiniz.');
        window.location.href = '/login.html';
    } catch (error) {
        document.getElementById('error-message').textContent = error.message;
    }
}

async function login() {
    const credentials = {
        EPosta: document.getElementById('email').value,
        Sifre: document.getElementById('password').value
    };

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(credentials)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Giriş başarısız');
        }

        const data = await response.json();
        localStorage.setItem('token', data.token);
        window.location.href = '/';
    } catch (error) {
        document.getElementById('error-message').textContent = error.message;
    }
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = '/login.html';
}