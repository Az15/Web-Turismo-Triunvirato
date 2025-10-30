/**
 * Script para manejar el envío AJAX del formulario de consulta de correo.
 * Se ejecuta al cargar el DOM.
 */
document.addEventListener('DOMContentLoaded', function () {

    // Seleccionamos todos los formularios que tengan el ID 'consultaMailForm'
    const forms = document.querySelectorAll('#consultaMailForm');

    forms.forEach(form => {
        // Usamos un listener de 'submit' en cada formulario
        form.addEventListener('submit', function (e) {
            e.preventDefault(); // Detiene el envío estándar del formulario

            const formData = new FormData(this);
            // Obtenemos el ID del paquete desde el atributo data-id del formulario
            const itemId = this.getAttribute('data-id');
            const messageDiv = document.getElementById(`formMessage-${itemId}`);

            // 1. Mostrar estado de envío
            messageDiv.innerHTML = '<span class="text-info"><i class="fas fa-spinner fa-spin"></i> Enviando consulta...</span>';
            messageDiv.style.display = 'block';

            // 2. Envío de datos mediante Fetch API
            fetch(this.action, {
                method: 'POST',
                // Para enviar datos de formulario usando Fetch, usamos URLSearchParams
                body: new URLSearchParams(formData),
                // Es importante incluir el token de validación (si lo estás usando)
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => {
                    // Verificar si la respuesta es JSON antes de parsear
                    if (!response.ok) {
                        throw new Error(`Error HTTP: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    // Asumo que el Controller devuelve { success: true, message: "Mensaje de éxito" }
                    if (data.success) {
                        messageDiv.innerHTML = `<span class="text-success"><i class="fas fa-check-circle"></i> ${data.message}</span>`;

                        // Cierra ambos modales después de un breve retraso
                        setTimeout(() => {
                            // Necesitamos obtener la instancia de Bootstrap Modal para cerrarlo
                            // Es importante que Bootstrap ya esté cargado
                            const mailModalElement = document.getElementById(`mailModal-${itemId}`);
                            const mainModalElement = document.getElementById(`packageModal-${itemId}`);

                            // Cierra el segundo modal
                            if (mailModalElement) {
                                bootstrap.Modal.getInstance(mailModalElement)?.hide();
                            }
                            // Cierra el modal principal
                            if (mainModalElement) {
                                bootstrap.Modal.getInstance(mainModalElement)?.hide();
                            }
                        }, 1500);

                    } else {
                        // Muestra errores de validación o del servidor
                        messageDiv.innerHTML = `<span class="text-danger"><i class="fas fa-times-circle"></i> Error: ${data.message || 'Error en el envío.'}</span>`;
                    }
                })
                .catch(error => {
                    console.error('Error de conexión o de servidor:', error);
                    messageDiv.innerHTML = '<span class="text-danger">Ocurrió un error de conexión o el servidor no respondió.</span>';
                });
        });
    });
});