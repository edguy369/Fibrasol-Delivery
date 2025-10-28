$(document).ready(function() {
    $('#deliveryStatusesTable').DataTable({
        ajax: {
            url: '/delivery-statuses',
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
                    return '<button type="button" class="btn btn-edit btn-sm me-1" onclick="editDeliveryStatus(' + row.id + ', \'' + row.name.replace(/'/g, "\\'") + '\')">' +
                           '<i class="bi bi-pencil"></i>' +
                           '</button>' +
                           '<button type="button" class="btn btn-delete btn-sm" onclick="deleteDeliveryStatus(' + row.id + ')">' +
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
    document.getElementById('deliveryStatusModalLabel').textContent = 'Nuevo Estado';
    document.getElementById('deliveryStatusId').value = '';
    document.getElementById('deliveryStatusName').value = '';
}

function editDeliveryStatus(id, name) {
    document.getElementById('deliveryStatusModalLabel').textContent = 'Editar Estado';
    document.getElementById('deliveryStatusId').value = id;
    document.getElementById('deliveryStatusName').value = name;

    var modal = new bootstrap.Modal(document.getElementById('deliveryStatusModal'));
    modal.show();
}

function deleteDeliveryStatus(id) {
    if (confirm('¿Está seguro de que desea eliminar este estado de entrega?')) {
        $.ajax({
            url: '/delivery-statuses/' + id,
            type: 'DELETE',
            contentType: 'application/json',
            success: function(response) {
                // Show success message
                showSuccessMessage('Estado eliminado correctamente');

                // Reload DataTable
                $('#deliveryStatusesTable').DataTable().ajax.reload();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting delivery status:', error);
                alert('Error al eliminar el estado. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveDeliveryStatus() {
    var id = document.getElementById('deliveryStatusId').value;
    var name = document.getElementById('deliveryStatusName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del estado.');
        return;
    }

    // Disable save button to prevent multiple clicks
    var saveButton = document.querySelector('#deliveryStatusModal .btn-primary-custom');
    var originalText = saveButton.innerHTML;
    saveButton.disabled = true;
    saveButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status"></span>Guardando...';

    if (id) {
        // Update existing delivery status
        updateDeliveryStatus(id, name, saveButton, originalText);
    } else {
        // Create new delivery status
        createDeliveryStatus(name, saveButton, originalText);
    }
}

function createDeliveryStatus(name, saveButton, originalText) {
    $.ajax({
        url: '/delivery-statuses',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('deliveryStatusModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Estado creado correctamente');

            // Reload DataTable
            $('#deliveryStatusesTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error creating delivery status:', error);
            alert('Error al crear el estado. Por favor, inténtelo de nuevo.');
        },
        complete: function() {
            // Re-enable save button
            saveButton.disabled = false;
            saveButton.innerHTML = originalText;
        }
    });
}

function updateDeliveryStatus(id, name, saveButton, originalText) {
    $.ajax({
        url: '/delivery-statuses/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('deliveryStatusModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Estado actualizado correctamente');

            // Reload DataTable
            $('#deliveryStatusesTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error updating delivery status:', error);
            alert('Error al actualizar el estado. Por favor, inténtelo de nuevo.');
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
