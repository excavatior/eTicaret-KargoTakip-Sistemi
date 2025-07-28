// auth.js (güncellenmiş)
// Oturum durumunu kontrol et ve form/slider işlevlerini başlat
document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    setupFormHandlers();

    // --- Slider (kaydırmalı Giriş/Kayıt) ---
    const wrapper = document.getElementById('container') || document.querySelector('.auth-wrapper');
    const signUpBtn = document.getElementById('signUp');
    const signInBtn = document.getElementById('signIn');

    if (signUpBtn && wrapper) {
        signUpBtn.addEventListener('click', () => {
            wrapper.classList.add('right-panel-active');
        });
    }
    if (signInBtn && wrapper) {
        signInBtn.addEventListener('click', () => {
            wrapper.classList.remove('right-panel-active');
        });
    }

    // URL hash ile direkt kayıt paneline geçiş
    if (location.hash === '#signup' && wrapper) {
        wrapper.classList.add('right-panel-active');
    }
});

// Form event listener'larını kur
function setupFormHandlers() {
    const loginForm = document.querySelector('.sign-in-form');
    const registerForm = document.querySelector('.sign-up-form');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();
            handleLogin();
        });
    }

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            handleRegister();
        });
    }
}

// Giriş işlemi
async function handleLogin() {
    clearMessages();

    const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            EPosta: get('#login-email').value,
            Sifre: get('#login-password').value
        })
    });

    const data = await res.json();

    if (!data.success) {
        showError('#login-error', data.message);
        return;
    }

    // başarılı
    localStorage.setItem('isLoggedIn', '1');
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
    location.href = 'index.html#profile';
}


// Kayıt işlemi
async function handleRegister() {
    const name = document.getElementById('register-name').value.trim();
    const email = document.getElementById('register-email').value.trim();
    const password = document.getElementById('register-password').value;
    const errorElement = document.getElementById('register-error');
    const submitBtn = document.querySelector('.sign-up-form .btn');

    if (!name || !email || !password) {
        showError(errorElement, 'Lütfen tüm alanları doldurunuz');
        return;
    }
    if (password.length < 6) {
        showError(errorElement, 'Şifre en az 6 karakter olmalıdır');
        return;
    }

    try {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Kayıt Olunuyor...';

        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ AdSoyad: name, EPosta: email, Sifre: password })
        });

        const data = await parseResponse(response);

        showSuccess('Kayıt başarılı! Giriş paneline yönlendiriliyorsunuz...');
        setTimeout(() => document.getElementById('signIn').click(), 1200);

    } catch (error) {
        showError(errorElement, error.message || 'Kayıt işlemi başarısız oldu');
    } finally {
        submitBtn.disabled = false;
        submitBtn.innerHTML = 'Kayıt Ol';
    }
}

// API yanıtlarını parse et
async function parseResponse(response) {
    const ct = response.headers.get('content-type') || '';
    let data = ct.includes('application/json')
        ? await response.json()
        : await response.text();

    if (!response.ok) {
        const msg = (typeof data === 'object' ? data.message : data) || 'İşlem başarısız';
        throw new Error(msg);
    }
    return data;
}

// Hata mesajını göster
function showError(element, message) {
    if (!element) return;
    element.textContent = message;
    element.style.display = 'block';
    setTimeout(() => element.style.display = 'none', 5000);
}

// Başarı mesajını göster
function showSuccess(message) {
    const div = document.createElement('div');
    div.className = 'success-message';
    div.innerHTML = `<i class='fas fa-check-circle'></i> ${message}`;
    document.body.append(div);
    setTimeout(() => {
        div.classList.add('fade-out');
        setTimeout(() => div.remove(), 500);
    }, 1600);
}

// Oturum durumunu kontrol et
function checkAuthState() {
    const token = localStorage.getItem('token');
    if (token && !isTokenExpired(token)) {
        window.location.href = '/';
    }
}

// Token süresi kontrolü
function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.exp < Date.now() / 1000;
    } catch {
        return true;
    }
}

// Global fonksiyonlar (gerekirse)
window.handleLogin = handleLogin;
window.handleRegister = handleRegister;
