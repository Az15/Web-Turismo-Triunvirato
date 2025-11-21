document.addEventListener('DOMContentLoaded', function () {
    const iconButtons = document.querySelectorAll('.icon-button');

    iconButtons.forEach(button => {
        button.addEventListener('click', function (event) {
            iconButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            // Opcional: Si quieres persistir el estado al navegar
            const section = this.getAttribute('data-section');
            if (section) {
                // Modificar la URL para incluir el parámetro 'activeSection'
                const url = new URL(this.href, window.location.origin);
                url.searchParams.set('activeSection', section);
                this.href = url.toString(); // Actualizar el href antes de navegar
            }
        });
    });

    // Comprobar si hay un parámetro 'activeSection' en la URL al cargar la página
    const urlParams = new URLSearchParams(window.location.search);
    const activeSection = urlParams.get('activeSection');

    if (activeSection) {
        iconButtons.forEach(button => {
            if (button.getAttribute('data-section') === activeSection) {
                button.classList.add('active');
            }
        });
    }

    // Usamos jQuery (ya que está incluido en el layout)
    $('#toggleNavButton').click(function () {
        var navBarContainer = $('#iconBarContainer');
        var button = $(this);

        // Alterna la visibilidad del contenedor de la botonera
        navBarContainer.toggle(400); // Oculta/Muestra con una pequeña animación (400ms)

        // Cambia el texto del botón
        if (navBarContainer.is(':visible')) {
            button.text('Ocultar Botones');
        } else {
            button.text('Mostrar Botones');
        }
    });

    // ***** FIN DE LÓGICA AGREGADA *****
});


});