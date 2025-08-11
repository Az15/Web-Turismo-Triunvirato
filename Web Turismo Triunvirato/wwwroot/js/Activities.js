document.addEventListener('DOMContentLoaded', function () {
    const locationFilter = document.getElementById('locationFilter');
    const activitySearch = document.getElementById('activitySearch');
    const activitiesList = document.getElementById('activitiesList');
    const activityCards = document.querySelectorAll('.activity-card');

    function filterActivities() {
        const selectedLocation = locationFilter.value.toLowerCase();
        const searchTerm = activitySearch.value.toLowerCase().trim();

        activityCards.forEach(card => {
            const cardLocation = card.dataset.location.toLowerCase();
            const cardTitle = card.querySelector('.card-title').textContent.toLowerCase();

            const matchesLocation = !selectedLocation || cardLocation === selectedLocation;
            const matchesSearch = !searchTerm || cardTitle.includes(searchTerm);

            if (matchesLocation && matchesSearch) {
                card.classList.remove('hidden');
            } else {
                card.classList.add('hidden');
            }
        });
    }

    // Eventos para los filtros
    locationFilter.addEventListener('change', filterActivities);
    activitySearch.addEventListener('keyup', filterActivities);
});