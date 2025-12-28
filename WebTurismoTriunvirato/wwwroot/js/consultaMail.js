window.agregarPasajero = function (itemId) {
    const container = document.getElementById(`pasajeros-container-${itemId}`);
    if (!container) return;

    const index = container.querySelectorAll('.pasajero-row').length;
    const div = document.createElement('div');
    div.className = 'pasajero-row card shadow-sm p-3 mb-3 border-start border-primary border-4 w-100';

    div.innerHTML = `
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h6 class="text-primary small fw-bold mb-0">Pasajero #${index + 1}</h6>
            <button type="button" class="btn btn-sm btn-link text-danger p-0 text-decoration-none" onclick="eliminarPasajero(this, ${itemId})">
                <i class="fas fa-trash"></i> Quitar
            </button>
        </div>
        <div class="row">
            <div class="col-12 mb-2">
                <label class="ultra-small text-muted">Nombre y Apellido</label>
                <input type="text" name="Pasajeros[${index}].NombreCompleto" class="form-control form-control-sm" required />
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mb-2">
                <label class="ultra-small text-muted">DNI / Pasaporte</label>
                <input type="text" name="Pasajeros[${index}].Dni" class="form-control form-control-sm" required />
            </div>
            <div class="col-md-6 mb-2">
                <label class="ultra-small text-muted">Fecha de Nacimiento</label>
                <input type="date" name="Pasajeros[${index}].FechaNacimiento" class="form-control form-control-sm" required />
            </div>
        </div>
    `;
    container.appendChild(div);
};

window.eliminarPasajero = function (btn, itemId) {
    btn.closest('.pasajero-row').remove();
    const container = document.getElementById(`pasajeros-container-${itemId}`);
    const rows = container.querySelectorAll('.pasajero-row');

    rows.forEach((r, i) => {
        r.querySelector('h6').innerText = `Pasajero #${i + 1}`;
        r.querySelectorAll('input').forEach(input => {
            const name = input.name;
            if (name.includes('NombreCompleto')) input.name = `Pasajeros[${i}].NombreCompleto`;
            if (name.includes('Dni')) input.name = `Pasajeros[${i}].Dni`;
            if (name.includes('FechaNacimiento')) input.name = `Pasajeros[${i}].FechaNacimiento`;
        });
    });
};



document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('#consultaMailForm');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const formData = new FormData(this);
            const itemId = this.getAttribute('data-id');
            const messageDiv = document.getElementById(`formMessage-${itemId}`);

            messageDiv.innerHTML = '<span class="text-info"><i class="fas fa-spinner fa-spin"></i> Procesando reserva...</span>';
            messageDiv.style.display = 'block';

            fetch(this.action, {
                method: 'POST',
                body: new URLSearchParams(formData),
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        messageDiv.innerHTML = `<span class="text-success"><i class="fas fa-check-circle"></i> ${data.message}</span>`;
                        setTimeout(() => { location.reload(); }, 1500);
                    } else {
                        messageDiv.innerHTML = `<span class="text-danger"><i class="fas fa-times-circle"></i> Error: ${data.message}</span>`;
                    }
                })
                .catch(error => {
                    messageDiv.innerHTML = '<span class="text-danger">Error de conexión.</span>';
                });
        });
    });
});