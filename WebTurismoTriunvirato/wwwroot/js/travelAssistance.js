document.addEventListener('DOMContentLoaded', function () {
    // Maneja el scroll suave al hacer clic en el botón "Ver Coberturas"
    const scrollToDetailsBtn = document.getElementById('scrollToDetailsBtn');
    const detailsSection = document.getElementById('travel-assistance-details');

    if (scrollToDetailsBtn && detailsSection) {
        scrollToDetailsBtn.addEventListener('click', function (e) {
            e.preventDefault();
            detailsSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    }

    // Funcionalidad extra: puedes agregar animaciones al hacer scroll
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate__animated', 'animate__fadeInUp');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1 // 10% del elemento visible
    });

    // Observar las secciones que quieres animar
    const sections = document.querySelectorAll('#travel-assistance-details, #contact-section');
    sections.forEach(section => {
        observer.observe(section);
    });
});