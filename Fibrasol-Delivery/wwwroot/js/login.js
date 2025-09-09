$('._login').on('click', Login);

$('form').on('submit', function (e) {
    e.preventDefault();
    Login();
});/*Fix para permitir al usuario enviar el form con el uso de enter*/

function Login() {
    $('.error-message').empty();
    $('.error-message').addClass('d-none');
    $('._login').empty().append('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Ingresando');
    LockForm();
    let valid = true;
    if (!$('#_email').val()) {
        $('.error-message').append('<span class="mb-1">El correo no puede estar en blanco.</span>');
        valid = false;

    } else {
        if (!($('#_email').val().includes('@') && $('#_email').val().includes('.'))) {
            $('.error-message').append('<span class="mb-1">El correo es invalido.</span>');
            valid = false;
        }
    }

    if (!$('#_password').val()) {
        $('.error-message').append('<span class="mb-1">La contraseña no puede estar en blanco.</span>');
        valid = false;
    }

    if (valid) {
        let bodyObject = JSON.stringify({
            Email: $('#_email').val(),
            Password: $('#_password').val()
        });

        $.ajax({
            type: 'POST',
            url: '/login/',
            data: bodyObject,
            contentType: 'application/json',
            success: function () {
                location.href = '/';
            },
            error: function (error) {
                if (error.status == 401) {
                    $('.error-message').append('<span class="mb-1">Usuario o contraseña invalida.</span>');
                } else {
                    $('.error-message').append('<span class="mb-1">Ocurrio un error iniciando sesión.</span>');
                }

                $('._login').empty().append('Iniciar Sesión');
                UnlockForm();
            }
        })
    } else {
        $('.error-message').removeClass('d-none');
        $('._login').empty().append('Iniciar Sesión');
        UnlockForm();
    }
}