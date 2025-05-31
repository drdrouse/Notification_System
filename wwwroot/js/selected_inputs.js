// Функция для загрузки полей в зависимости от выбранного сервиса
function loadServiceFields() {
    const serviceType = document.getElementById('service_name').value.toLowerCase();
    const container = document.getElementById('fieldsContainer');

    if (serviceType === 'email') {
        container.innerHTML = `
                <h4>Настройки Email-сервиса</h4>
                <div class="form-group">
                    <label>Период отправки:</label>
                    <select name="SendPeriod" class="form-control">
                        <option value="1" selected>Минута</option>
                        <option value="5">Пять минут</option>
                        <option value="10">Десять минут</option>
                        <option value="30">Пол часа</option>
                        <option value="60">Час</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>SMTP Сервер:</label>
                    <input type="text" name="SmtpServer" class="form-control" required>
                </div>
                <div class="form-group">
                    <label>SMTP Порт:</label>
                    <input type="number" name="SmtpPort" class="form-control" value="587" required>
                </div>
                <div class="form-group">
                    <label>Логин:</label>
                    <input type="text" name="Username" class="form-control" required>
                </div>
                <div class="form-group">
                    <label>Пароль:</label>
                    <input type="password" name="Password" class="form-control" required>
                </div>
                <div class="form-group">
                    <label>Email отправителя:</label>
                    <input type="email" name="FromEmail" class="form-control" required>
                </div>
                <div class="form-group">
                    <label>Использовать SSL:</label>
                    <select name="EnableSsl" class="form-control">
                        <option value="true">Да</option>
                        <option value="false">Нет</option>
                    </select>
                </div>
            `;
    } else {
        container.innerHTML = `
                <div class="form-group">
                    <p class="text-muted">Дополнительные настройки не требуются</p>
                </div>
            `;
    }
}

// Загружаем поля при загрузке страницы, если значение уже выбрано
document.addEventListener('DOMContentLoaded', function () {
    const serviceSelect = document.getElementById('service_name');
    if (serviceSelect.value) {
        loadServiceFields();
    }
});