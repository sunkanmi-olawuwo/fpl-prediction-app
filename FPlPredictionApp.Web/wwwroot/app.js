// Chart.js rendering for Players and Teams pages
window.renderTopScorersChart = (labels, data) => {
    if (!window.Chart) {
        var script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/chart.js';
        script.onload = () => renderTopScorersChart(labels, data);
        document.head.appendChild(script);
        return;
    }
    const ctx = document.getElementById('topScorersChart').getContext('2d');
    if (window.topScorersChart) window.topScorersChart.destroy();
    window.topScorersChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Goals',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.7)'
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } }
        }
    });
};

window.renderTeamStrengthChart = (labels, data) => {
    if (!window.Chart) {
        var script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/chart.js';
        script.onload = () => renderTeamStrengthChart(labels, data);
        document.head.appendChild(script);
        return;
    }
    const ctx = document.getElementById('teamStrengthChart').getContext('2d');
    if (window.teamStrengthChart) window.teamStrengthChart.destroy();
    window.teamStrengthChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Strength',
                data: data,
                backgroundColor: 'rgba(255, 99, 132, 0.7)'
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } }
        }
    });
};
