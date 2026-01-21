const pathParts = window.location.pathname.split('/');
const orderId = pathParts[pathParts.length - 2];

async function loadDeliveryOrderData() {
    try {
        const orderData = await FibrasolUtils.api.get(`/delivery-orders/${orderId}`);

        if (orderData) {
            populateOrderData(orderData);
        } else {
            console.error('No order data found');
            alert('No se encontraron datos de la orden');
        }
    } catch (error) {
        console.error('Error loading delivery order:', error);
        alert('Error al cargar los datos de la orden');
    }
}

function populateOrderData(orderData) {
    document.getElementById('orderNumber').textContent = orderData.id;

    const orderDate = new Date(orderData.createdAt || new Date());
    const formattedDate = orderDate.toLocaleDateString('es-GT', {
        day: 'numeric',
        month: 'short'
    });
    document.getElementById('orderDate').textContent = formattedDate;

    populateTable(orderData);

    document.getElementById('totalAmount').textContent = FibrasolUtils.currency.formatWithSymbol(orderData.total, orderData.currency || 'Q');

    // Populate concept if exists
    if (orderData.concept) {
        document.getElementById('orderConcept').textContent = orderData.concept;
    }

    populatePilots(orderData);
}

function populatePilots(orderData) {
    const pilotsContainer = document.getElementById('pilotsNames');

    if (orderData.riders && orderData.riders.length > 0) {
        const riderNames = orderData.riders.map(rider => rider.name.toUpperCase());

        let formattedNames = '';
        if (riderNames.length === 1) {
            formattedNames = `<strong>${riderNames[0]}</strong>`;
        } else if (riderNames.length === 2) {
            formattedNames = `<strong>${riderNames[0]}</strong> &nbsp;&nbsp;&nbsp; Y &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong>${riderNames[1]}</strong>`;
        } else {
            const lastRider = riderNames.pop();
            const otherRiders = riderNames.map(name => `<strong>${name}</strong>`).join(' &nbsp;&nbsp;&nbsp; ');
            formattedNames = `${otherRiders} &nbsp;&nbsp;&nbsp; Y &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong>${lastRider}</strong>`;
        }

        pilotsContainer.innerHTML = formattedNames;
    } else {
        pilotsContainer.innerHTML = '<strong>SIN ASIGNAR</strong>';
    }
}

function populateTable(orderData) {
    const tbody = document.getElementById('deliveryTableBody');
    tbody.innerHTML = '';

    if (!orderData.backorders || orderData.backorders.length === 0) {
        const row = document.createElement('tr');
        row.innerHTML = '<td colspan="7" style="text-align: center;">No hay comandas para mostrar</td>';
        tbody.appendChild(row);
        return;
    }

    orderData.backorders.forEach(backorder => {
        if (!backorder.invoices || backorder.invoices.length === 0) {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td class="client-cell">${backorder.client ? backorder.client.name : 'N/A'}</td>
                <td class="address-cell">N/A</td>
                <td>N/A</td>
                <td>${(orderData.currency || 'Q')} 0.00</td>
                <td></td>
                <td>${backorder.number || 'N/A'}</td>
                <td>${backorder.weight ? backorder.weight.toFixed(1) : '0.0'}</td>
            `;
            tbody.appendChild(row);
        } else {
            const backorderNumber = backorder.number || 'N/A';
            const backorderWeight = backorder.weight ? backorder.weight.toFixed(1) : '0.0';
            const invoiceCount = backorder.invoices.length;

            backorder.invoices.forEach((invoice, index) => {
                const row = document.createElement('tr');
                let cells = '';

                // Use invoice client if available, otherwise fall back to backorder client
                const invoiceClientName = invoice.client ? invoice.client.name : (backorder.client ? backorder.client.name : 'N/A');

                if (index === 0) {
                    cells = `
                        <td class="client-cell">${invoiceClientName}</td>
                        <td class="address-cell">${invoice.address || 'N/A'}</td>
                        <td>${invoice.reference || 'N/A'}</td>
                        <td>${FibrasolUtils.currency.formatWithSymbol(invoice.value || 0, orderData.currency || 'Q')}</td>
                        <td>${invoice.salesPerson ? invoice.salesPerson.name : 'N/A'}</td>
                        <td class="client-cell" rowspan="${invoiceCount}">${backorderNumber}</td>
                        <td class="client-cell" rowspan="${invoiceCount}">${backorderWeight}</td>
                    `;
                } else {
                    cells = `
                        <td class="client-cell">${invoiceClientName}</td>
                        <td class="address-cell">${invoice.address || 'N/A'}</td>
                        <td>${invoice.reference || 'N/A'}</td>
                        <td>${FibrasolUtils.currency.formatWithSymbol(invoice.value || 0, orderData.currency || 'Q')}</td>
                        <td>${invoice.salesPerson ? invoice.salesPerson.name : 'N/A'}</td>
                    `;
                }
                row.innerHTML = cells;
                tbody.appendChild(row);
            });
        }
    });
}

document.addEventListener('DOMContentLoaded', loadDeliveryOrderData);
