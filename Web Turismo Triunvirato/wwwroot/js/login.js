$(document).ready(function () {
    // Mostrar el modal de login solo si el botón existe (usuario no autenticado)
    if ($("#showLoginBtn").length) {
        $("#showLoginBtn").click(function () {
            // Asegúrate de que tienes cargado Bootstrap y su JS
            var loginModal = new bootstrap.Modal(document.getElementById('loginModal'));
            loginModal.show();
        });
    }

    // El dropdown de usuario debería funcionar automáticamente con las clases de Bootstrap
});
