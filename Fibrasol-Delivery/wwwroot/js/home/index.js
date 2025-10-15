// Dashboard data loading
document.addEventListener('DOMContentLoaded', function() {
    loadDashboardData();
    updateTimestamp();
});

async function loadDashboardData() {
    try {
        // Load all dashboard counters in parallel
        const [driversData, clientsData, salesPersonsData, ordersData, invoicesData] = await Promise.all([
            fetchData('/dashboards/riders'),
            fetchData('/dashboards/clients'),
            fetchData('/dashboards/sales-persons'),
            fetchData('/dashboards/delivery-orders'),
            fetchData('/dashboards/invoices')
        ]);

        // Update drivers count
        updateCounter('driversCount', driversData);

        // Update clients count
        updateCounter('clientsCount', clientsData);

        // Update sales persons count
        updateCounter('salesPersonsCount', salesPersonsData);

        // Update delivery orders count
        updateCounter('ordersCount', ordersData);

        // Update invoices count with comparison
        updateInvoicesCounter(invoicesData);

    } catch (error) {
        console.error('Error loading dashboard data:', error);
        showErrorState();
    }
}

async function fetchData(url) {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
}

function updateCounter(elementId, count) {
    const element = document.getElementById(elementId);
    if (element) {
        // Add animation effect
        element.style.opacity = '0.5';
        setTimeout(() => {
            element.innerHTML = count.toLocaleString('es-GT');
            element.style.opacity = '1';
        }, 200);
    }
}

function updateInvoicesCounter(invoicesData) {
    const totalInvoices = invoicesData.invoices || 0;
    const signedInvoices = invoicesData.signedInvoices || 0;

    // Update main counter
    updateCounter('invoicesCount', totalInvoices);

    // Update progress bar and text
    const progressBar = document.getElementById('invoicesProgress');
    const progressText = document.getElementById('invoicesText');

    if (progressBar && progressText) {
        const percentage = totalInvoices > 0 ? (signedInvoices / totalInvoices) * 100 : 0;

        setTimeout(() => {
            progressBar.style.width = `${percentage}%`;
            progressText.textContent = `${signedInvoices.toLocaleString('es-GT')}/${totalInvoices.toLocaleString('es-GT')} firmadas`;
        }, 300);
    }
}

function showErrorState() {
    const counters = ['driversCount', 'clientsCount', 'salesPersonsCount', 'ordersCount', 'invoicesCount'];
    counters.forEach(counterId => {
        const element = document.getElementById(counterId);
        if (element) {
            element.innerHTML = '<span class="error-message">Error al cargar</span>';
        }
    });

    // Show error in progress text
    const progressText = document.getElementById('invoicesText');
    if (progressText) {
        progressText.innerHTML = '<span class="error-message">Error</span>';
    }
}

function updateTimestamp() {
    const now = new Date();
    const timestamp = now.toLocaleString('es-GT', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });

    const lastUpdateElement = document.getElementById('lastUpdate');
    if (lastUpdateElement) {
        lastUpdateElement.textContent = timestamp;
    }
}

// Refresh dashboard every 5 minutes
setInterval(function() {
    loadDashboardData();
    updateTimestamp();
}, 300000); // 5 minutes
