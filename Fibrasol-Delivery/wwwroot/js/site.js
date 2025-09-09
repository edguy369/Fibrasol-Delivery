function LockForm() {
    $('input').prop('disabled', true);
    $('select').prop('disabled', true);
    $('textarea').prop('disabled', true);
    $('button').prop('disabled', true);
}

function UnlockForm() {
    $('input').prop('disabled', false);
    $('select').prop('disabled', false);
    $('textarea').prop('disabled', false);
    $('button').prop('disabled', false);
}