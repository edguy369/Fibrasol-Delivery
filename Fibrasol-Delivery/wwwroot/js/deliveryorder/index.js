function getTableColumns() {
    return [
        { data: 'id' },
        {
            data: 'status.name',
            render: function(data, type, row) {
                const badgeClass = data.toLowerCase() === 'finalizado' ? 'status-delivered' : 'status-pending';
                return `<span class="status-badge ${badgeClass}">${data}</span>`;
            }
        },
        {
            data: 'created',
            render: function(data, type, row) {
                return FibrasolUtils.dates.formatDateTime(data);
            }
        },
        {
            data: 'total',
            render: function(data, type, row) {
                return FibrasolUtils.currency.format(data);
            }
        },
        {
            data: null,
            orderable: false,
            render: function(data, type, row) {
                return `<a href="/constancias/${row.id}/impresion" class="btn btn-print btn-sm me-1" target="_blank">
                           <i class="bi bi-printer"></i>
                       </a>
                       <a href="/constancias/${row.id}" class="btn btn-view btn-sm me-1">
                           <i class="bi bi-eye"></i>
                       </a>
                       <button type="button" class="btn btn-delete btn-sm" onclick="deleteDeliveryOrder(${row.id})">
                           <i class="bi bi-trash"></i>
                       </button>`;
            }
        }
    ];
}

$(document).ready(function() {
    $('#deliveryOrdersTable').DataTable({
        ajax: {
            url: '/delivery-orders',
            type: 'GET',
            dataSrc: '',
            error: function(xhr, status, error) {
                console.error('Error loading delivery orders:', error);
                FibrasolUtils.ui.showErrorMessage('Actualmente no hay constancias de entrega');
                $('#deliveryOrdersTable').DataTable().destroy();
                initializeEmptyTable();
            }
        },
        columns: getTableColumns(),
        ...FibrasolUtils.datatables.getSpanishConfig(),
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json',
            loadingRecords: 'Cargando constancias de entrega...',
            emptyTable: 'Actualmente no hay constancias de entrega',
            zeroRecords: 'Actualmente no hay constancias de entrega'
        },
        order: [[0, 'desc']]
    });
});

function initializeEmptyTable() {
    $('#deliveryOrdersTable').DataTable({
        data: [],
        columns: getTableColumns(),
        ...FibrasolUtils.datatables.getSpanishConfig(),
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json',
            emptyTable: 'Actualmente no hay constancias de entrega',
            zeroRecords: 'Actualmente no hay constancias de entrega'
        },
        order: [[0, 'desc']]
    });
}

function deleteDeliveryOrder(id) {
    if (confirm('¿Está seguro de que desea eliminar esta orden de entrega?')) {
        FibrasolUtils.api.ajax({
            url: '/delivery-orders/' + id,
            type: 'DELETE',
            success: function(response) {
                FibrasolUtils.ui.showSuccessMessage('Constancia de entrega eliminada correctamente');
                FibrasolUtils.datatables.reloadTable('deliveryOrdersTable');
            },
            error: function(xhr, status, error) {
                console.error('Error deleting delivery order:', error);
                FibrasolUtils.ui.showErrorMessage('Error al eliminar la constancia de entrega. Por favor, inténtelo de nuevo.');
            }
        });
    }
}
