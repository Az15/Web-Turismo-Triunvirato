document.addEventListener('DOMContentLoaded', function () {
    const titulosSeccion = document.querySelectorAll('.titulo-seccion');

    titulosSeccion.forEach(titulo => {
        titulo.addEventListener('click', function () {
            const seccionId = this.getAttribute('data-seccion');
            const contenido = document.getElementById(seccionId);

            if (contenido) {
                contenido.classList.toggle('oculto');
                this.classList.toggle('activo');
            }
        });
    });

    // Asegurar que la primera sección esté desplegada al cargar la página
    //const primeraSeccionTitulo = document.querySelector('.titulo-seccion.activo');
    //if (primeraSeccionTitulo) {
    //    const primeraSeccionId = primeraSeccionTitulo.getAttribute('data-seccion');
    //    const primeraSeccionContenido = document.getElementById(primeraSeccionId);
    //    if (primeraSeccionContenido && primeraSeccionContenido.classList.contains('oculto')) {
    //        primeraSeccionContenido.classList.remove('oculto');
    //    }
    //}

});