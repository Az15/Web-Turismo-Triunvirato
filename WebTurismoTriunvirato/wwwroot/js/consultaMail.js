/**
 * Script para manejar el env�o AJAX del formulario de consulta de correo.
 * Se ejecuta al cargar el DOM.
 */
document.addEventListener('DOMContentLoaded', function () {

    // Seleccionamos todos los formularios que tengan el ID 'consultaMailForm'
    const forms = document.querySelectorAll('#consultaMailForm');

    forms.forEach(form => {
        // Usamos un listener de 'submit' en cada formulario
        form.addEventListener('submit', function (e) {
            e.preventDefault(); // Detiene el env�o est�ndar del formulario

            const formData = new FormData(this);
            // Obtenemos el ID del paquete desde el atributo data-id del formulario
            const itemId = this.getAttribute('data-id');
            const messageDiv = document.getElementById(`formMessage-${itemId}`);

            // 1. Mostrar estado de env�o
            messageDiv.innerHTML = '<span class="text-info"><i class="fas fa-spinner fa-spin"></i> Enviando consulta...</span>';
            messageDiv.style.display = 'block';

            // 2. Env�o de datos mediante Fetch API
            fetch(this.action, {
                method: 'POST',
                // Para enviar datos de formulario usando Fetch, usamos URLSearchParams
                body: new URLSearchParams(formData),
                // Es importante incluir el token de validaci�n (si lo est�s usando)
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
                    // Asumo que el Controller devuelve { success: true, message: "Mensaje de �xito" }
                    if (data.success) {
                        messageDiv.innerHTML = `<span class="text-success"><i class="fas fa-check-circle"></i> ${data.message}</span>`;

                        // Cierra ambos modales despu�s de un breve retraso
                        setTimeout(() => {
                            // Necesitamos obtener la instancia de Bootstrap Modal para cerrarlo
                            // Es importante que Bootstrap ya est� cargado
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
                        // Muestra errores de validaci�n o del servidor
                        messageDiv.innerHTML = `<span class="text-danger"><i class="fas fa-times-circle"></i> Error: ${data.message || 'Error en el env�o.'}</span>`;
                    }
                })
                .catch(error => {
                    console.error('Error de conexi�n o de servidor:', error);
                    messageDiv.innerHTML = '<span class="text-danger">Ocurri� un error de conexi�n o el servidor no respondi�.</span>';
                });
        });
    });
});