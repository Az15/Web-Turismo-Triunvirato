$(document).on('hidden.bs.modal', function (e) {
    // 1. Revisa si hay otros modales aún abiertos
    const openModals = document.querySelectorAll('.modal.show');

    // 2. Si NO hay modales abiertos...
    if (openModals.length === 0) {
        // ... busca y elimina forzosamente cualquier backdrop restante
        const backdrop = document.querySelector('.modal-backdrop');
        if (backdrop) {
            backdrop.remove();
        }

        // También asegura que el scroll del cuerpo vuelva a funcionar
        document.body.style.overflow = '';
    }
});