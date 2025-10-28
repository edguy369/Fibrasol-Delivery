let salesChart = null;
let salesDataTable = null;
let currentReportData = [];

$(document).ready(function() {
    initializePage();
    generateReport();
});

function initializePage() {
    const firstDay = FibrasolUtils.dates.getFirstDayOfMonth();
    const lastDay = FibrasolUtils.dates.getLastDayOfMonth();

    document.getElementById('startDate').value = FibrasolUtils.dates.toInputFormat(firstDay);
    document.getElementById('endDate').value = FibrasolUtils.dates.toInputFormat(lastDay);

    salesDataTable = $('#salesTable').DataTable({
        ...FibrasolUtils.datatables.getSpanishConfig(),
        order: [[0, 'asc']],
        columnDefs: [
            {
                targets: [2],
                render: function(data, type, row) {
                    return type === 'display' ? FibrasolUtils.currency.format(data) : data;
                }
            },
            {
                targets: [3],
                render: function(data, type, row) {
                    return type === 'display' ? data + '%' : data;
                }
            },
            {
                targets: [4],
                render: function(data, type, row) {
                    if (type === 'display') {
                        const percentage = parseFloat(data);
                        let badgeClass = 'bg-secondary';
                        let icon = 'bi-dash';

                        if (percentage > 20) {
                            badgeClass = 'bg-success';
                            icon = 'bi-arrow-up';
                        } else if (percentage > 10) {
                            badgeClass = 'bg-warning';
                            icon = 'bi-arrow-right';
                        } else if (percentage > 0) {
                            badgeClass = 'bg-info';
                            icon = 'bi-arrow-down-right';
                        }

                        return `<span class="badge ${badgeClass}"><i class="bi ${icon} me-1"></i>${percentage}%</span>`;
                    }
                    return data;
                }
            }
        ]
    });
}

function applyQuickPeriod() {
    const period = document.getElementById('quickPeriod').value;
    const now = new Date();
    let startDate, endDate;

    switch(period) {
        case 'thisMonth':
            startDate = FibrasolUtils.dates.getFirstDayOfMonth();
            endDate = FibrasolUtils.dates.getLastDayOfMonth();
            break;
        case 'lastMonth':
            startDate = new Date(now.getFullYear(), now.getMonth() - 1, 1);
            endDate = new Date(now.getFullYear(), now.getMonth(), 0);
            break;
        case 'thisQuarter':
            const quarter = Math.floor(now.getMonth() / 3);
            startDate = new Date(now.getFullYear(), quarter * 3, 1);
            endDate = new Date(now.getFullYear(), quarter * 3 + 3, 0);
            break;
        case 'thisYear':
            startDate = new Date(now.getFullYear(), 0, 1);
            endDate = new Date(now.getFullYear(), 11, 31);
            break;
        case 'lastYear':
            startDate = new Date(now.getFullYear() - 1, 0, 1);
            endDate = new Date(now.getFullYear() - 1, 11, 31);
            break;
        default:
            return;
    }

    document.getElementById('startDate').value = FibrasolUtils.dates.toInputFormat(startDate);
    document.getElementById('endDate').value = FibrasolUtils.dates.toInputFormat(endDate);
}

function generateReport() {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    if (!startDate || !endDate) {
        alert('Por favor, seleccione las fechas de inicio y fin.');
        return;
    }

    if (new Date(startDate) > new Date(endDate)) {
        alert('La fecha de inicio no puede ser mayor que la fecha de fin.');
        return;
    }

    showLoading();

    const startFormatted = FibrasolUtils.dates.formatDate(startDate);
    const endFormatted = FibrasolUtils.dates.formatDate(endDate);
    document.getElementById('reportPeriod').textContent = `${startFormatted} - ${endFormatted}`;

    const requestData = {
        startDate: startDate,
        endDate: endDate
    };

    $.ajax({
        url: '/sales-persons/report',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function(data) {
            currentReportData = data;
            updateStatistics(data);
            updateChart(data);
            updateDataTable(data);
            hideLoading();
        },
        error: function(xhr, status, error) {
            console.error('Error generating report:', error);
            alert('Error al generar el reporte. Por favor, inténtelo de nuevo.');
            hideLoading();
        }
    });
}

function updateStatistics(data) {
    const totalSales = data.reduce((sum, item) => sum + item.totalSales, 0);
    const avgSales = data.length > 0 ? totalSales / data.length : 0;
    const topSeller = data.length > 0 ? data[0].salesPerson.name : '-';
    const totalSellers = data.length;

    document.getElementById('totalSales').textContent = FibrasolUtils.currency.format(totalSales);
    document.getElementById('avgSales').textContent = FibrasolUtils.currency.format(avgSales);
    document.getElementById('topSeller').textContent = topSeller;
    document.getElementById('totalSellers').textContent = totalSellers;
}

function updateChart(data) {
    const ctx = document.getElementById('salesChart').getContext('2d');

    if (salesChart) {
        salesChart.destroy();
    }

    const labels = data.map(item => item.salesPerson.name);
    const salesData = data.map(item => item.totalSales);

    const backgroundColors = data.map((_, index) => {
        const hue = (index * 137.5) % 360;
        return `hsla(${hue}, 70%, 60%, 0.8)`;
    });

    const borderColors = backgroundColors.map(color => color.replace('0.8', '1'));

    salesChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Ventas Totales',
                data: salesData,
                backgroundColor: backgroundColors,
                borderColor: borderColors,
                borderWidth: 2,
                borderRadius: 8,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Ventas por Vendedor',
                    font: {
                        size: 16,
                        weight: 'bold'
                    },
                    padding: 20
                },
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return 'Ventas: ' + FibrasolUtils.currency.format(context.parsed.y);
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return 'Q ' + value.toLocaleString('es-GT');
                        }
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        maxRotation: 45,
                        minRotation: 0
                    }
                }
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            }
        }
    });
}

function updateDataTable(data) {
    const totalSales = data.reduce((sum, item) => sum + item.totalSales, 0);

    const tableData = data.map((item, index) => {
        const percentage = totalSales > 0 ? ((item.totalSales / totalSales) * 100).toFixed(1) : 0;
        return [
            index + 1,
            item.salesPerson.name,
            item.totalSales,
            percentage,
            percentage
        ];
    });

    salesDataTable.clear();
    salesDataTable.rows.add(tableData);
    salesDataTable.draw();
}

function showLoading() {
    document.getElementById('chartLoading').style.display = 'flex';
}

function hideLoading() {
    document.getElementById('chartLoading').style.display = 'none';
}

function refreshReport() {
    generateReport();
}

function exportReport() {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    let csvContent = "data:text/csv;charset=utf-8,";
    csvContent += "Reporte de Ventas - " + startDate + " a " + endDate + "\n\n";
    csvContent += "Ranking,Vendedor,Total Ventas,Porcentaje\n";

    currentReportData.forEach((item, index) => {
        const totalSales = currentReportData.reduce((sum, i) => sum + i.totalSales, 0);
        const percentage = totalSales > 0 ? ((item.totalSales / totalSales) * 100).toFixed(1) : 0;
        csvContent += `${index + 1},${item.salesPerson.name},${item.totalSales},${percentage}%\n`;
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", `reporte-ventas-${startDate}-${endDate}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
