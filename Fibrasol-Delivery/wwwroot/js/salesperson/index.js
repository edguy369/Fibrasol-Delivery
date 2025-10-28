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
                    return FibrasolUtils.datatables.createActionButtons(row.id, row.name, {
                        editFn: 'editSalesPerson',
                        deleteFn: 'deleteSalesPerson'
                    });
                }
            }
        ],
        ...FibrasolUtils.datatables.getSpanishConfig(),
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

    FibrasolUtils.ui.showModal('salesPersonModal');
}

function deleteSalesPerson(id) {
    if (confirm('¿Está seguro de que desea eliminar este vendedor?')) {
        FibrasolUtils.api.ajax({
            url: '/sales-persons/' + id,
            type: 'DELETE',
            data: JSON.stringify({}),
            success: function(response) {
                FibrasolUtils.ui.showSuccessMessage('Vendedor eliminado correctamente');
                FibrasolUtils.datatables.reloadTable('salesPersonsTable');
            },
            error: function(xhr, status, error) {
                console.error('Error deleting sales person:', error);
                alert('Error al eliminar el vendedor. Por favor, inténtelo de nuevo.');
            }
        });
    }
}

function saveSalesPerson() {
    const id = document.getElementById('salesPersonId').value;
    const name = document.getElementById('salesPersonName').value.trim();

    if (!name) {
        alert('Por favor, ingrese el nombre del vendedor.');
        return;
    }

    const saveButton = document.querySelector('#salesPersonModal .btn-primary-custom');
    FibrasolUtils.ui.setButtonLoading(saveButton);

    const data = { Name: name };
    const isUpdate = !!id;

    FibrasolUtils.api.ajax({
        url: isUpdate ? `/sales-persons/${id}` : '/sales-persons',
        type: isUpdate ? 'PUT' : 'POST',
        data: JSON.stringify(data),
        success: function(response) {
            FibrasolUtils.ui.hideModal('salesPersonModal');
            FibrasolUtils.ui.showSuccessMessage(isUpdate ? 'Vendedor actualizado correctamente' : 'Vendedor creado correctamente');
            FibrasolUtils.datatables.reloadTable('salesPersonsTable');
        },
        error: function(xhr, status, error) {
            console.error(`Error ${isUpdate ? 'updating' : 'creating'} sales person:`, error);
            alert(`Error al ${isUpdate ? 'actualizar' : 'crear'} el vendedor. Por favor, inténtelo de nuevo.`);
        },
        complete: function() {
            FibrasolUtils.ui.setButtonLoading(saveButton, false);
        }
    });
}
