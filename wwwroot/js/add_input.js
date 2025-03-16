function createNewField() {
    // Создаем контейнер для одного набора полей
    const fieldContainer = document.createElement('div');
    fieldContainer.classList.add('field-container');

    // Создаем метку
    const label = document.createElement('label');
    label.htmlFor = 'srvice_setting' + fieldsContainer.childNodes.length; // Уникальное имя для каждого поля
    label.textContent = 'Дополнительные данные:';
    fieldContainer.appendChild(label);

    // Создаем поле ввода
    const input = document.createElement('input');
    input.type = 'text';
    input.id = 'srvice_setting' + fieldsContainer.childNodes.length; // Уникальное ID для каждого поля
    input.placeholder = 'Наименование: значение';
    fieldContainer.appendChild(input);

    return fieldContainer;
}

// Обработка нажатия на кнопку
const addFieldButton = document.getElementById('addFieldButton');
const fieldsContainer = document.getElementById('fieldsContainer');

addFieldButton.addEventListener('click', () => {
    // Создаем новое поле и добавляем его в контейнер
    const newField = createNewField();
    fieldsContainer.appendChild(newField);
});