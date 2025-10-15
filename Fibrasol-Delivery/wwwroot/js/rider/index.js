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
                    return FibrasolUtils.datatables.createActionButtons(row.id, row.name, {
                        editFn: 'editRider',
                        deleteFn: 'deleteRider'
                    });
                }
            }
        ],
        ...FibrasolUtils.datatables.getSpanishConfig(),
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

    FibrasolUtils.ui.showModal('riderModal');
}

function deleteRider(id) {
    if (confirm('¿Está seguro de que desea eliminar este conductor?')) {
        FibrasolUtils.api.ajax({
            url: '/riders/' + id,
            type: 'DELETE',
            data: JSON.stringify({}),
            success: function(response) {
                FibrasolUtils.ui.showSuccessMessage('Conductor eliminado correctamente');
                FibrasolUtils.datatables.reloadTable('ridersTable');
            },
            error: function(xhr, status, error) {
                console.error('Error deleting rider:', error);
                alert('Error al eliminar el conductor. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveRider() {
    const id = document.getElementById('riderId').value;
    const name = document.getElementById('riderName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del conductor.');
        return;
    }

    const saveButton = document.querySelector('#riderModal .btn-primary-custom');
    FibrasolUtils.ui.setButtonLoading(saveButton);

    const data = { Name: name };
    const isUpdate = !!id;

    FibrasolUtils.api.ajax({
        url: isUpdate ? `/riders/${id}` : '/riders',
        type: isUpdate ? 'PUT' : 'POST',
        data: JSON.stringify(data),
        success: function(response) {
            FibrasolUtils.ui.hideModal('riderModal');
            FibrasolUtils.ui.showSuccessMessage(isUpdate ? 'Conductor actualizado correctamente' : 'Conductor creado correctamente');
            FibrasolUtils.datatables.reloadTable('ridersTable');
        },
        error: function(xhr, status, error) {
            console.error(`Error ${isUpdate ? 'updating' : 'creating'} rider:`, error);
            alert(`Error al ${isUpdate ? 'actualizar' : 'crear'} el conductor. Por favor, inténtelo de nuevo.`);
        },
        complete: function() {
            FibrasolUtils.ui.setButtonLoading(saveButton, false);
        }
    });
}
