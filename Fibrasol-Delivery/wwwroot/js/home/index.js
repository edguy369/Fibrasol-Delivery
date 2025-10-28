document.addEventListener('DOMContentLoaded', function() {
    loadDashboardData();
    updateTimestamp();
});

async function loadDashboardData() {
    try {
        const [driversData, clientsData, salesPersonsData, ordersData, invoicesData] = await Promise.all([
            FibrasolUtils.api.get('/dashboards/riders'),
            FibrasolUtils.api.get('/dashboards/clients'),
            FibrasolUtils.api.get('/dashboards/sales-persons'),
            FibrasolUtils.api.get('/dashboards/delivery-orders'),
            FibrasolUtils.api.get('/dashboards/invoices')
        ]);

        FibrasolUtils.ui.updateCounter('driversCount', driversData);
        FibrasolUtils.ui.updateCounter('clientsCount', clientsData);
        FibrasolUtils.ui.updateCounter('salesPersonsCount', salesPersonsData);
        FibrasolUtils.ui.updateCounter('ordersCount', ordersData);
        updateInvoicesCounter(invoicesData);

    } catch (error) {
        console.error('Error loading dashboard data:', error);
        showErrorState();
    }
}

function updateInvoicesCounter(invoicesData) {
    const totalInvoices = invoicesData.invoices || 0;
    const signedInvoices = invoicesData.signedInvoices || 0;

    FibrasolUtils.ui.updateCounter('invoicesCount', totalInvoices);

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

setInterval(function() {
    loadDashboardData();
    updateTimestamp();
}, 300000);
