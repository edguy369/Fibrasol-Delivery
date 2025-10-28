// Get order ID from URL
const pathParts = window.location.pathname.split('/');
const orderId = pathParts[pathParts.length - 2]; // Gets the ID before 'impresion'

// Load the delivery order data
async function loadDeliveryOrderData() {
    try {
        const response = await fetch(`/delivery-orders/${orderId}`);
        const orderData = await response.json();

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
    // Set header information
    document.getElementById('orderNumber').textContent = orderData.id;

    // Format date
    const orderDate = new Date(orderData.createdAt || new Date());
    const formattedDate = orderDate.toLocaleDateString('es-GT', {
        day: 'numeric',
        month: 'short'
    });
    document.getElementById('orderDate').textContent = formattedDate;

    // Populate table
    populateTable(orderData);

    // Set total
    document.getElementById('totalAmount').textContent = `Q${orderData.total.toLocaleString('es-GT', { minimumFractionDigits: 2 })}`;

    // Populate pilots names
    populatePilots(orderData);
}

function populatePilots(orderData) {
    const pilotsContainer = document.getElementById('pilotsNames');

    if (orderData.riders && orderData.riders.length > 0) {
        // Get all rider names
        const riderNames = orderData.riders.map(rider => rider.name.toUpperCase());

        // Format names based on count
        let formattedNames = '';
        if (riderNames.length === 1) {
            formattedNames = `<strong>${riderNames[0]}</strong>`;
        } else if (riderNames.length === 2) {
            formattedNames = `<strong>${riderNames[0]}</strong> &nbsp;&nbsp;&nbsp; Y &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong>${riderNames[1]}</strong>`;
        } else {
            // For more than 2 riders, show all with proper spacing
            const lastRider = riderNames.pop();
            const otherRiders = riderNames.map(name => `<strong>${name}</strong>`).join(' &nbsp;&nbsp;&nbsp; ');
            formattedNames = `${otherRiders} &nbsp;&nbsp;&nbsp; Y &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong>${lastRider}</strong>`;
        }

        pilotsContainer.innerHTML = formattedNames;
    } else {
        // No riders assigned
        pilotsContainer.innerHTML = '<strong>SIN ASIGNAR</strong>';
    }
}

function populateTable(orderData) {
    const tbody = document.getElementById('deliveryTableBody');
    tbody.innerHTML = '';

    if (!orderData.backorders || orderData.backorders.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7">No hay comandas para mostrar</td></tr>';
        return;
    }

    orderData.backorders.forEach(backorder => {
        if (!backorder.invoices || backorder.invoices.length === 0) {
            // If no invoices, show just the backorder info
            const row = document.createElement('tr');
            row.innerHTML = `
                <td class="client-cell">${backorder.client ? backorder.client.name : 'N/A'}</td>
                <td class="address-cell">N/A</td>
                <td>N/A</td>
                <td>Q0.00</td>
                <td></td>
                <td>${backorder.number || 'N/A'}</td>
                <td>${backorder.weight ? backorder.weight.toFixed(1) : '0.0'}</td>
            `;
            tbody.appendChild(row);
        } else {
            // Show each invoice as a separate row
            backorder.invoices.forEach((invoice, index) => {
                const row = document.createElement('tr');

                // For the first invoice, show client name and backorder info
                if (index === 0) {
                    row.innerHTML = `
                        <td class="client-cell" rowspan="${backorder.invoices.length}">${backorder.client ? backorder.client.name : 'N/A'}</td>
                        <td class="address-cell">${invoice.address || 'N/A'}</td>
                        <td>${invoice.reference || 'N/A'}</td>
                        <td>Q${invoice.value ? invoice.value.toLocaleString('es-GT', { minimumFractionDigits: 2 }) : '0.00'}</td>
                        <td></td>
                        <td class="client-cell" rowspan="${backorder.invoices.length}">${backorder.number || 'N/A'}</td>
                        <td class="client-cell" rowspan="${backorder.invoices.length}">${backorder.weight ? backorder.weight.toFixed(1) : '0.0'}</td>
                    `;
                } else {
                    // For subsequent invoices, don't repeat client name and backorder info
                    row.innerHTML = `
                        <td class="address-cell">${invoice.address || 'N/A'}</td>
                        <td>${invoice.reference || 'N/A'}</td>
                        <td>Q${invoice.value ? invoice.value.toLocaleString('es-GT', { minimumFractionDigits: 2 }) : '0.00'}</td>
                        <td></td>
                    `;
                }

                tbody.appendChild(row);
            });
        }
    });
}

// Load data when page loads
document.addEventListener('DOMContentLoaded', loadDeliveryOrderData);
