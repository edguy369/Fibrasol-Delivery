document.getElementById('togglePassword').addEventListener('click', function () {
    const password = document.getElementById('_password');
    const toggleIcon = document.getElementById('toggleIcon');

    if (password.type === 'password') {
        password.type = 'text';
        toggleIcon.classList.remove('bi-eye');
        toggleIcon.classList.add('bi-eye-slash');
    } else {
        password.type = 'password';
        toggleIcon.classList.remove('bi-eye-slash');
        toggleIcon.classList.add('bi-eye');
    }
});
