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
                    return FibrasolUtils.datatables.createActionButtons(row.id, row.name, {
                        editFn: 'editClient',
                        deleteFn: 'deleteClient'
                    });
                }
            }
        ],
        ...FibrasolUtils.datatables.getSpanishConfig(),
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

    FibrasolUtils.ui.showModal('clientModal');
}

function deleteClient(id) {
    if (confirm('¿Está seguro de que desea eliminar este cliente?')) {
        FibrasolUtils.api.ajax({
            url: '/clients/' + id,
            type: 'DELETE',
            data: JSON.stringify({}),
            success: function(response) {
                FibrasolUtils.ui.showSuccessMessage('Cliente eliminado correctamente');
                FibrasolUtils.datatables.reloadTable('clientsTable');
            },
            error: function(xhr, status, error) {
                console.error('Error deleting client:', error);
                alert('Error al eliminar el cliente. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveClient() {
    const id = document.getElementById('clientId').value;
    const name = document.getElementById('clientName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del cliente.');
        return;
    }

    const saveButton = document.querySelector('#clientModal .btn-primary-custom');
    FibrasolUtils.ui.setButtonLoading(saveButton);

    const data = { Name: name };
    const isUpdate = !!id;

    FibrasolUtils.api.ajax({
        url: isUpdate ? `/clients/${id}` : '/clients',
        type: isUpdate ? 'PUT' : 'POST',
        data: JSON.stringify(data),
        success: function(response) {
            FibrasolUtils.ui.hideModal('clientModal');
            FibrasolUtils.ui.showSuccessMessage(isUpdate ? 'Cliente actualizado correctamente' : 'Cliente creado correctamente');
            FibrasolUtils.datatables.reloadTable('clientsTable');
        },
        error: function(xhr, status, error) {
            console.error(`Error ${isUpdate ? 'updating' : 'creating'} client:`, error);
            alert(`Error al ${isUpdate ? 'actualizar' : 'crear'} el cliente. Por favor, inténtelo de nuevo.`);
        },
        complete: function() {
            FibrasolUtils.ui.setButtonLoading(saveButton, false);
        }
    });
}
