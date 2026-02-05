window.monthlyChartInstance = null;

window.renderMonthlyChart = (data) => {
    const ctx = document.getElementById('monthlyChart');

    // Destroy previous chart if it exists
    if (window.monthlyChartInstance) {
        window.monthlyChartInstance.destroy();
    }

    window.monthlyChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(x => `${x.month}/${x.year}`),
            datasets: [{
                label: 'Monthly Spend',
                data: data.map(x => x.total),
                fill: false
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Expense Breakdown by Monthly'
                }
            },
            responsive: false,
            maintainAspectRatio: false,
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Amount Spent ($)'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'By Month'
                    }
                }
            }
        }
    });
};


window.storeChartInstance = null;

window.renderStoreChart = (data) => {
    const ctx = document.getElementById('storeChart');

    // Destroy old chart if it exists
    if (window.storeChartInstance) {
        window.storeChartInstance.destroy();
    }

    window.storeChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(x => x.store.toLowerCase()),
            datasets: [{
                label: 'Spend by Store',
                data: data.map(x => x.total),
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Expense Breakdown by Store'
                }
            },
            responsive: false,
            maintainAspectRatio: false,
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Store Names'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Amount Spent ($)'
                    }
                }
            }
        }
    });
};
/*

window.categoryChartInstance = null;

window.renderCategoryChart = (data) => {
    const ctx = document.getElementById('categoryChart');

    if (window.categoryChartInstance) {
        window.categoryChartInstance.destroy();
    }

    window.categoryChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: data.map(x => x.category.toLowerCase()),
            datasets: [{
                data: data.map(x => x.total)
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Expense Breakdown by Category'
                }
            },
            responsive: false
        }
    });
};

*/
window.categoryChartInstance = null;

window.renderCategoryChart = (data) => {
    const ctx = document.getElementById('categoryChart');
    if (!ctx || !data || data.length === 0) return;

    if (window.categoryChartInstance) {
        window.categoryChartInstance.destroy();
    }

    window.categoryChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: data.map(x => (x.category ?? 'uncategorized').toLowerCase()),
            datasets: [{
                data: data.map(x => x.total)
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Expense Breakdown by Item'
                }
            },
            responsive: false
        }
    });
};

