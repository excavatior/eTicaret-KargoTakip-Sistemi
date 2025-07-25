document.addEventListener('DOMContentLoaded', function () {
    // Form event listener'ları
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    if (loginForm) {
        loginForm.addEventListener('submit', handleFormSubmit(login));
    }

    if (registerForm) {
        registerForm.addEventListener('submit', handleFormSubmit(register));
    }

    // Oturum durumunu kontrol et
    updateAuthState();
});

// Form submit handler factory
function handleFormSubmit(handler) {
    return async function (e) {
        e.preventDefault();
        const form = e.target;
        const submitButton = form.querySelector('button[type="submit"]');

        try {
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> İşleniyor...';
            await handler();
        } catch (error) {
            console.error('İşlem hatası:', error);
        } finally {
            submitButton.disabled = false;
            submitButton.innerHTML = form.id === 'loginForm'
                ? '<i class="fas fa-sign-in-alt"></i> Giriş Yap'
                : '<i class="fas fa-user-plus"></i> Kayıt Ol';
        }
    };
}

// Kullanıcı kayıt fonksiyonu (Gelişmiş versiyon)
async function register() {
    const user = {
        AdSoyad: document.getElementById('name').value.trim(),
        EPosta: document.getElementById('email').value.trim().toLowerCase(),
        Sifre: document.getElementById('password').value
    };

    // Client-side validation
    if (!user.AdSoyad || !user.EPosta || !user.Sifre) {
        throw new Error('Tüm alanlar zorunludur!');
    }

    if (user.Sifre.length < 6) {
        throw new Error('Şifre en az 6 karakter olmalıdır');
    }

    try {
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(user)
        });

        const data = await parseResponse(response);

        // Başarılı kayıt sonrası
        showSuccess('Kayıt başarılı! Giriş yapabilirsiniz.');
        setTimeout(() => window.location.href = '/login.html', 1500);

        return data;
    } catch (error) {
        showError(error.message);
        throw error;
    }
}

// Kullanıcı giriş fonksiyonu (Gelişmiş versiyon)
async function login() {
    const credentials = {
        EPosta: document.getElementById('email').value.trim().toLowerCase(),
        Sifre: document.getElementById('password').value
    };

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            credentials: 'include', // Cookie-based auth için
            body: JSON.stringify(credentials)
        });

        const data = await parseResponse(response);

        // Token'i sakla ve yönlendir
        if (data.token) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data.user));
        }

        showSuccess('Giriş başarılı! Yönlendiriliyorsunuz...');
        updateAuthState();
        setTimeout(() => window.location.href = '/', 1000);

        return data;
    } catch (error) {
        showError(error.message);
        throw error;
    }
}

// Çıkış yap fonksiyonu (Gelişmiş versiyon)
async function logout() {
    try {
        // Sunucu tarafında logout işlemi için istek gönder
        await fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'include'
        });
    } finally {
        // Client-side temizlik
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        updateAuthState();
        window.location.href = '/login.html';
    }
}

// API yanıtlarını parse etme
async function parseResponse(response) {
    const contentType = response.headers.get('content-type');
    let data;

    if (contentType?.includes('application/json')) {
        data = await response.json();
    } else {
        data = await response.text();
    }

    if (!response.ok) {
        const error = new Error(data.message || 'İşlem başarısız');
        error.response = response;
        error.data = data;
        throw error;
    }

    return data;
}

// Oturum durumuna göre arayüzü güncelle (Gelişmiş versiyon)
function updateAuthState() {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));
    const authButtons = document.getElementById('authButtons');
    const userMenu = document.getElementById('userMenu');

    if (authButtons) {
        if (token) {
            authButtons.innerHTML = `
                <div class="dropdown">
                    <button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        <i class="fas fa-user-circle"></i> ${user?.AdSoyad || 'Profil'}
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="/profile.html"><i class="fas fa-user"></i> Profilim</a></li>
                        <li><a class="dropdown-item" href="/orders.html"><i class="fas fa-box"></i> Siparişlerim</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><button class="dropdown-item" onclick="logout()"><i class="fas fa-sign-out-alt"></i> Çıkış Yap</button></li>
                    </ul>
                </div>
            `;
        } else {
            authButtons.innerHTML = `
                <a href="/login.html" class="btn btn-login me-2"><i class="fas fa-sign-in-alt"></i> Giriş Yap</a>
                <a href="/register.html" class="btn btn-outline"><i class="fas fa-user-plus"></i> Kayıt Ol</a>
            `;
        }
    }

    // Kullanıcı menüsü varsa güncelle
    if (userMenu) {
        userMenu.style.display = token ? 'block' : 'none';
    }
}

// Başarı mesajlarını göster
function showSuccess(message) {
    const successElement = document.getElementById('success-message') || createMessageElement('success');
    successElement.textContent = message;
    successElement.style.display = 'block';

    setTimeout(() => {
        successElement.style.display = 'none';
    }, 5000);
}

// Hata mesajlarını göster
function showError(message) {
    const errorElement = document.getElementById('error-message') || createMessageElement('error');
    errorElement.textContent = message;
    errorElement.style.display = 'block';

    setTimeout(() => {
        errorElement.style.display = 'none';
    }, 5000);
}

// Mesaj elementi oluştur (yoksa)
function createMessageElement(type) {
    const element = document.createElement('div');
    element.id = `${type}-message`;
    element.className = `alert alert-${type} fixed-top mx-auto mt-3`;
    element.style.display = 'none';
    element.style.maxWidth = '500px';
    element.style.zIndex = '1100';
    document.body.appendChild(element);
    return element;
}

// Global erişim için fonksiyonları window'a ekle
window.register = register;
window.login = login;
window.logout = logout;
window.updateAuthState = updateAuthState;