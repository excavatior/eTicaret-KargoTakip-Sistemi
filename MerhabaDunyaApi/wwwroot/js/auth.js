// Oturum durumunu kontrol et
document.addEventListener('DOMContentLoaded', function () {
    checkAuthState();
    setupFormHandlers();
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
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    const errorElement = document.getElementById('login-error');
    const submitBtn = document.querySelector('.sign-in-form .btn');

    // Validasyon
    if (!email || !password) {
        showError(errorElement, 'Lütfen tüm alanları doldurunuz');
        return;
    }

    try {
        // Butonu disable et
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Giriş Yapılıyor...';

        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                EPosta: email,
                Sifre: password
            })
        });

        const data = await parseResponse(response);

        // Başarılı giriş
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        showSuccess('Giriş başarılı! Yönlendiriliyorsunuz...');
        setTimeout(() => window.location.href = '/', 1000);

    } catch (error) {
        showError(errorElement, error.message || 'Geçersiz e-posta veya şifre');
    } finally {
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = 'Giriş Yap';
        }
    }
}

// Kayıt işlemi
async function handleRegister() {
    const name = document.getElementById('register-name').value.trim();
    const email = document.getElementById('register-email').value.trim();
    const password = document.getElementById('register-password').value;
    const errorElement = document.getElementById('register-error');
    const submitBtn = document.querySelector('.sign-up-form .btn');

    // Validasyon
    if (!name || !email || !password) {
        showError(errorElement, 'Lütfen tüm alanları doldurunuz');
        return;
    }

    if (password.length < 6) {
        showError(errorElement, 'Şifre en az 6 karakter olmalıdır');
        return;
    }

    try {
        // Butonu disable et
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Kayıt Olunuyor...';

        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                AdSoyad: name,
                EPosta: email,
                Sifre: password
            })
        });

        const data = await parseResponse(response);

        showSuccess('Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...');

        // Kayıt sonrası giriş formuna geç
        setTimeout(() => {
            document.querySelector('.container').classList.remove('sign-up-mode');
            document.getElementById('login-email').value = email;
            document.getElementById('login-password').value = '';
        }, 1500);

    } catch (error) {
        showError(errorElement, error.message || 'Kayıt işlemi başarısız oldu');
    } finally {
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = 'Kayıt Ol';
        }
    }
}

// API yanıtlarını parse et
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
        throw error;
    }

    return data;
}

// Hata mesajını göster
function showError(element, message) {
    if (!element) return;

    element.textContent = message;
    element.style.display = 'block';

    setTimeout(() => {
        element.style.display = 'none';
    }, 5000);
}

// Başarı mesajını göster
function showSuccess(message) {
    // Toastify veya benzeri bir kütüphane ekleyebilirsiniz
    const successDiv = document.createElement('div');
    successDiv.className = 'success-message';
    successDiv.innerHTML = `
        <i class="fas fa-check-circle"></i>
        <span>${message}</span>
    `;

    document.body.appendChild(successDiv);

    setTimeout(() => {
        successDiv.classList.add('fade-out');
        setTimeout(() => successDiv.remove(), 500);
    }, 3000);
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

// Global fonksiyonlar
window.handleLogin = handleLogin;
window.handleRegister = handleRegister;