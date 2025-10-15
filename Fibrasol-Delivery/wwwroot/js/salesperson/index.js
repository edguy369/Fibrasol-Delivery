$(document).ready(function() {
    $('#salesPersonsTable').DataTable({
        ajax: {
            url: '/sales-persons',
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
                    return '<button type="button" class="btn btn-edit btn-sm me-1" onclick="editSalesPerson(' + row.id + ', \'' + row.name.replace(/'/g, "\\'") + '\')">' +
                           '<i class="bi bi-pencil"></i>' +
                           '</button>' +
                           '<button type="button" class="btn btn-delete btn-sm" onclick="deleteSalesPerson(' + row.id + ')">' +
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
    document.getElementById('salesPersonModalLabel').textContent = 'Nuevo Vendedor';
    document.getElementById('salesPersonId').value = '';
    document.getElementById('salesPersonName').value = '';
}

function editSalesPerson(id, name) {
    document.getElementById('salesPersonModalLabel').textContent = 'Editar Vendedor';
    document.getElementById('salesPersonId').value = id;
    document.getElementById('salesPersonName').value = name;

    var modal = new bootstrap.Modal(document.getElementById('salesPersonModal'));
    modal.show();
}

function deleteSalesPerson(id) {
    if (confirm('¿Está seguro de que desea eliminar este vendedor?')) {
        $.ajax({
            url: '/sales-persons/' + id,
            type: 'DELETE',
            contentType: 'application/json',
            data: JSON.stringify({}),
            success: function(response) {
                // Show success message
                showSuccessMessage('Vendedor eliminado correctamente');

                // Reload DataTable
                $('#salesPersonsTable').DataTable().ajax.reload();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting sales person:', error);
                alert('Error al eliminar el vendedor. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveSalesPerson() {
    var id = document.getElementById('salesPersonId').value;
    var name = document.getElementById('salesPersonName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del vendedor.');
        return;
    }

    // Disable save button to prevent multiple clicks
    var saveButton = document.querySelector('#salesPersonModal .btn-primary-custom');
    var originalText = saveButton.innerHTML;
    saveButton.disabled = true;
    saveButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status"></span>Guardando...';

    if (id) {
        // Update existing sales person
        updateSalesPerson(id, name, saveButton, originalText);
    } else {
        // Create new sales person
        createSalesPerson(name, saveButton, originalText);
    }
}

function createSalesPerson(name, saveButton, originalText) {
    $.ajax({
        url: '/sales-persons',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('salesPersonModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Vendedor creado correctamente');

            // Reload DataTable
            $('#salesPersonsTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error creating sales person:', error);
            alert('Error al crear el vendedor. Por favor, inténtelo de nuevo.');
        },
        complete: function() {
            // Re-enable save button
            saveButton.disabled = false;
            saveButton.innerHTML = originalText;
        }
    });
}

function updateSalesPerson(id, name, saveButton, originalText) {
    $.ajax({
        url: '/sales-persons/' + id,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name
        }),
        success: function(response) {
            // Close modal and reload page
            var modal = bootstrap.Modal.getInstance(document.getElementById('salesPersonModal'));
            modal.hide();

            // Show success message
            showSuccessMessage('Vendedor actualizado correctamente');

            // Reload DataTable
            $('#salesPersonsTable').DataTable().ajax.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error updating sales person:', error);
            alert('Error al actualizar el vendedor. Por favor, inténtelo de nuevo.');
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
