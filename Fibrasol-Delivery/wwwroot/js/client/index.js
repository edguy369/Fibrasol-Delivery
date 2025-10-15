$(document).ready(function() {
    $('#clientsTable').DataTable({
        ajax: {
            url: '/clients',
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
                    return '<button type="button" class="btn btn-edit btn-sm me-1" onclick="editClient(' + row.id + ', \'' + row.name.replace(/'/g, "\\'") + '\')">' +
                           '<i class="bi bi-pencil"></i>' +
                           '</button>' +
                           '<button type="button" class="btn btn-delete btn-sm" onclick="deleteClient(' + row.id + ')">' +
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
    document.getElementById('clientModalLabel').textContent = 'Nuevo Cliente';
    document.getElementById('clientId').value = '';
    document.getElementById('clientName').value = '';
}

function editClient(id, name) {
    document.getElementById('clientModalLabel').textContent = 'Editar Cliente';
    document.getElementById('clientId').value = id;
    document.getElementById('clientName').value = name;

    var modal = new bootstrap.Modal(document.getElementById('clientModal'));
    modal.show();
}

function deleteClient(id) {
    if (confirm('¿Está seguro de que desea eliminar este cliente?')) {
        $.ajax({
            url: '/clients/' + id,
            type: 'DELETE',
            contentType: 'application/json',
            data: JSON.stringify({}),
            success: function(response) {
                // Show success message
                showSuccessMessage('Cliente eliminado correctamente');

                // Reload DataTable
                $('#clientsTable').DataTable().ajax.reload();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting client:', error);
                alert('Error al eliminar el cliente. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveClient() {
    var id = document.getElementById('clientId').value;
    var name = document.getElementById('clientName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del cliente.');
        return;
    }

    // Disable save button to prevent multiple clicks
    var saveButton = document.querySelector('#clientModal .btn-primary-custom');
    var originalText = saveButton.innerHTML;
    saveButton.disabled = true;
    saveButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status"></span>Guardando...';

    if (id) {
        // Update existing client
        updateClient(id, name, saveButton, originalText);
    } else {
        // Create new client
        createClient(name, saveButton, originalText);
    }
}

function createClient(name, saveButton, originalText) {
    $.ajax({
        url: '/clients',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('clientModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Cliente creado correctamente');

            // Reload DataTable
            $('#clientsTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error creating client:', error);
            alert('Error al crear el cliente. Por favor, inténtelo de nuevo.');
        },
        complete: function() {
            // Re-enable save button
            saveButton.disabled = false;
            saveButton.innerHTML = originalText;
        }
    });
}

function updateClient(id, name, saveButton, originalText) {
    $.ajax({
        url: '/clients/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('clientModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Cliente actualizado correctamente');

            // Reload DataTable
            $('#clientsTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error updating client:', error);
            alert('Error al actualizar el cliente. Por favor, inténtelo de nuevo.');
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
