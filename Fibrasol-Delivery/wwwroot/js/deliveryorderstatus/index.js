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
                    return FibrasolUtils.datatables.createActionButtons(row.id, row.name, {
                        editFn: 'editDeliveryStatus',
                        deleteFn: 'deleteDeliveryStatus'
                    });
                }
            }
        ],
        ...FibrasolUtils.datatables.getSpanishConfig(),
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

    FibrasolUtils.ui.showModal('deliveryStatusModal');
}

function deleteDeliveryStatus(id) {
    if (confirm('¿Está seguro de que desea eliminar este estado de entrega?')) {
        FibrasolUtils.api.ajax({
            url: '/delivery-statuses/' + id,
            type: 'DELETE',
            success: function(response) {
                FibrasolUtils.ui.showSuccessMessage('Estado eliminado correctamente');
                FibrasolUtils.datatables.reloadTable('deliveryStatusesTable');
            },
            error: function(xhr, status, error) {
                console.error('Error deleting delivery status:', error);
                alert('Error al eliminar el estado. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveDeliveryStatus() {
    const id = document.getElementById('deliveryStatusId').value;
    const name = document.getElementById('deliveryStatusName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del estado.');
        return;
    }

    const saveButton = document.querySelector('#deliveryStatusModal .btn-primary-custom');
    FibrasolUtils.ui.setButtonLoading(saveButton);

    const data = { Name: name };
    const isUpdate = !!id;

    FibrasolUtils.api.ajax({
        url: isUpdate ? `/delivery-statuses/${id}` : '/delivery-statuses',
        type: isUpdate ? 'PUT' : 'POST',
        data: JSON.stringify(data),
        success: function(response) {
            FibrasolUtils.ui.hideModal('deliveryStatusModal');
            FibrasolUtils.ui.showSuccessMessage(isUpdate ? 'Estado actualizado correctamente' : 'Estado creado correctamente');
            FibrasolUtils.datatables.reloadTable('deliveryStatusesTable');
        },
        error: function(xhr, status, error) {
            console.error(`Error ${isUpdate ? 'updating' : 'creating'} delivery status:`, error);
            alert(`Error al ${isUpdate ? 'actualizar' : 'crear'} el estado. Por favor, inténtelo de nuevo.`);
        },
        complete: function() {
            FibrasolUtils.ui.setButtonLoading(saveButton, false);
        }
    });
}
