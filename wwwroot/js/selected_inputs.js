// Функция для загрузки полей в зависимости от выбранного сервиса
function loadServiceFields() {
    const serviceType = document.getElementById('service_name').value.toLowerCase();
    const container = document.getElementById('fieldsContainer');

    // Общие поля для всех сервисов
    const commonFields = `
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
    `;

    if (serviceType === 'email') {
        container.innerHTML = `
            <h4>Настройки Email-сервиса</h4>
            ${commonFields}
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
    } else if (serviceType === 'sms') {
        container.innerHTML = `
            <h4>Настройки SMS-сервиса</h4>
            ${commonFields}
            <div class="form-group">
                <label>API ключ:</label>
                <input type="text" name="ApiKey" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Имя отправителя:</label>
                <input type="text" name="SenderName" class="form-control" required>
            </div>
            <div class="form-group">
                <label>URL API:</label>
                <input type="text" name="ApiUrl" class="form-control" value="https://api.sms-provider.com" required>
            </div>
        `;
    } else if (serviceType === 'telegram') {
        container.innerHTML = `
            <h4>Настройки Telegram-бота</h4>
            ${commonFields}
            <div class="form-group">
                <label>Токен бота:</label>
                <input type="text" name="BotToken" class="form-control" required>
            </div>
            <div class="form-group">
                <label>ID чата:</label>
                <input type="text" name="ChatId" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Имя бота:</label>
                <input type="text" name="BotName" class="form-control" required>
            </div>
        `;
    } else if (serviceType === 'vk') {
        container.innerHTML = `
            <h4>Настройки VK-бота</h4>
            ${commonFields}
            <div class="form-group">
                <label>Токен сообщества:</label>
                <input type="text" name="AccessToken" class="form-control" required>
            </div>
            <div class="form-group">
                <label>ID группы:</label>
                <input type="text" name="GroupId" class="form-control" required>
            </div>
            <div class="form-group">
                <label>Версия API:</label>
                <input type="text" name="ApiVersion" class="form-control" value="5.131" required>
            </div>
            <div class="form-group">
                <label>Подтверждающий ключ:</label>
                <input type="text" name="ConfirmationKey" class="form-control" required>
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