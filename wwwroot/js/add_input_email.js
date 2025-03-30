function createNewFieldEmail() {
    // Создаем контейнер для одного набора полей
    const fieldContainer = document.createElement('div');
    fieldContainer.classList.add('field-container');

    // Создаем метку
    const label = document.createElement('label');
    label.htmlFor = 'srvice_setting' + fieldsContainer.childNodes.length; // Уникальное имя для каждого поля
    label.textContent = 'Почта';
    fieldContainer.appendChild(label);

    // Создаем поле ввода
    const input = document.createElement('input');
    input.type = 'text';
    input.id = 'srvice_setting' + fieldsContainer.childNodes.length; // Уникальное ID для каждого поля
    fieldContainer.appendChild(input);

    return fieldContainer;
}

// Обработка нажатия на кнопку
const addFieldButton = document.getElementById('addEmail');
const fieldsContainer = document.getElementById('emailsContainer');

addFieldButton.addEventListener('click', () => {
    // Создаем новое поле и добавляем его в контейнер
    const newField = createNewFieldEmail();
    fieldsContainer.appendChild(newField);
});