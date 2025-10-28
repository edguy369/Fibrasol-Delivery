function getTableColumns() {
    return [
        { data: 'id' },
        {
            data: 'status.name',
            render: function(data, type, row) {
                console.log(data);
                let badgeClass = '';
                switch(data.toLowerCase()) {
                    case 'ingresado':
                        badgeClass = 'status-pending';
                        break;
                    case 'finalizado':
                        badgeClass = 'status-delivered';
                        break;
                    default:
                        badgeClass = 'status-pending';
                }
                return '<span class="status-badge ' + badgeClass + '">' + data + '</span>';
            }
        },
        {
            data: 'created',
            render: function(data, type, row) {
                const date = new Date(data);
                return date.toLocaleDateString('es-ES') + ' ' + date.toLocaleTimeString('es-ES', {hour: '2-digit', minute: '2-digit'});
            }
        },
        {
            data: 'total',
            render: function(data, type, row) {
                return '$' + data.toFixed(2);
            }
        },
        {
            data: null,
            orderable: false,
            render: function(data, type, row) {
                return '<a href="/constancias/' + row.id + '/impresion" class="btn btn-print btn-sm me-1" target="_blank">' +
                       '<i class="bi bi-printer"></i>' +
                       '</a>' +
                       '<a href="/constancias/' + row.id + '" class="btn btn-view btn-sm me-1">' +
                       '<i class="bi bi-eye"></i>' +
                       '</a>' +
                       '<button type="button" class="btn btn-delete btn-sm" onclick="deleteDeliveryOrder(' + row.id + ')">' +
                       '<i class="bi bi-trash"></i>' +
                       '</button>';
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
                // Show error message instead of dummy data
                showErrorMessage('Actualmente no hay constancias de entrega');

                // Initialize empty table
                $('#deliveryOrdersTable').DataTable().destroy();
                initializeEmptyTable();
            }
        },
        columns: getTableColumns(),
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json',
            loadingRecords: 'Cargando constancias de entrega...',
            emptyTable: 'Actualmente no hay constancias de entrega',
            zeroRecords: 'Actualmente no hay constancias de entrega'
        },
        pageLength: 10,
        responsive: true,
        order: [[0, 'desc']]
    });
});

function initializeEmptyTable() {
    $('#deliveryOrdersTable').DataTable({
        data: [],
        columns: getTableColumns(),
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json',
            emptyTable: 'Actualmente no hay constancias de entrega',
            zeroRecords: 'Actualmente no hay constancias de entrega'
        },
        pageLength: 10,
        responsive: true,
        order: [[0, 'desc']]
    });
}

function deleteDeliveryOrder(id) {
    if (confirm('¿Está seguro de que desea eliminar esta orden de entrega?')) {
        // Make AJAX call to delete the order
        $.ajax({
            url: '/delivery-orders/' + id,
            type: 'DELETE',
            contentType: 'application/json',
            success: function(response) {
                // Show success message
                showSuccessMessage('Constancia de entrega eliminada correctamente');

                // Reload DataTable from server
                $('#deliveryOrdersTable').DataTable().ajax.reload();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting delivery order:', error);
                showErrorMessage('Error al eliminar la constancia de entrega. Por favor, inténtelo de nuevo.');
            }
        });
    }
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

function showErrorMessage(message) {
    // Create and show a temporary error alert
    var alertHtml = '<div class="alert alert-warning alert-dismissible fade show" role="alert">' +
                   '<i class="bi bi-exclamation-triangle me-2"></i>' + message +
                   '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                   '</div>';

    $('.page-header').after(alertHtml);

    // Auto-dismiss after 5 seconds
    setTimeout(function() {
        $('.alert-warning').fadeOut();
    }, 5000);
}
