$(document).ready(function() {
    $('#ridersTable').DataTable({
        ajax: {
            url: '/riders',
            type: 'GET',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            {
                data: null,
                orderable: false,
                render: function(data, type, row) {
                    return '<button type="button" class="btn btn-edit btn-sm me-1" onclick="editRider(' + row.id + ', \'' + row.name.replace(/'/g, "\\'") + '\')">' +
                           '<i class="bi bi-pencil"></i>' +
                           '</button>' +
                           '<button type="button" class="btn btn-delete btn-sm" onclick="deleteRider(' + row.id + ')">' +
                           '<i class="bi bi-trash"></i>' +
                           '</button>';
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
        },
        pageLength: 10,
        responsive: true,
        order: [[0, 'asc']]
    });
});

function openCreateModal() {
    document.getElementById('riderModalLabel').textContent = 'Nuevo Conductor';
    document.getElementById('riderId').value = '';
    document.getElementById('riderName').value = '';
}

function editRider(id, name) {
    document.getElementById('riderModalLabel').textContent = 'Editar Conductor';
    document.getElementById('riderId').value = id;
    document.getElementById('riderName').value = name;

    var modal = new bootstrap.Modal(document.getElementById('riderModal'));
    modal.show();
}

function deleteRider(id) {
    if (confirm('¿Está seguro de que desea eliminar este conductor?')) {
        $.ajax({
            url: '/riders/' + id,
            type: 'DELETE',
            contentType: 'application/json',
            data: JSON.stringify({}),
            success: function(response) {
                // Show success message
                showSuccessMessage('Conductor eliminado correctamente');

                // Reload DataTable
                $('#ridersTable').DataTable().ajax.reload();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting rider:', error);
                alert('Error al eliminar el conductor. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveRider() {
    var id = document.getElementById('riderId').value;
    var name = document.getElementById('riderName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del conductor.');
        return;
    }

    // Disable save button to prevent multiple clicks
    var saveButton = document.querySelector('#riderModal .btn-primary-custom');
    var originalText = saveButton.innerHTML;
    saveButton.disabled = true;
    saveButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status"></span>Guardando...';

    if (id) {
        // Update existing rider
        updateRider(id, name, saveButton, originalText);
    } else {
        // Create new rider
        createRider(name, saveButton, originalText);
    }
}

function createRider(name, saveButton, originalText) {
    $.ajax({
        url: '/riders',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('riderModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Conductor creado correctamente');

            // Reload DataTable
            $('#ridersTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error creating rider:', error);
            alert('Error al crear el conductor. Por favor, inténtelo de nuevo.');
        },
        complete: function() {
            // Re-enable save button
            saveButton.disabled = false;
            saveButton.innerHTML = originalText;
        }
    });
}

function updateRider(id, name, saveButton, originalText) {
    $.ajax({
        url: '/riders/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('riderModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Conductor actualizado correctamente');

            // Reload DataTable
            $('#ridersTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error updating rider:', error);
            alert('Error al actualizar el conductor. Por favor, inténtelo de nuevo.');
        },
        complete: function() {
            // Re-enable save button
            saveButton.disabled = false;
            saveButton.innerHTML = originalText;
        }
    });
}

function showSuccessMessage(message) {
    // Create and show a temporary success alert
    var alertHtml = '<div class="alert alert-success alert-dismissible fade show" role="alert">' +
                   '<i class="bi bi-check-circle me-2"></i>' + message +
                   '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                   '</div>';

    $('.page-header').after(alertHtml);

    // Auto-dismiss after 3 seconds
    setTimeout(function() {
        $('.alert-success').fadeOut();
    }, 3000);
}
