function createNewFieldPhone() {
    const fieldContainer = document.createElement('div');
    fieldContainer.classList.add('field-container');

    const index = document.querySelectorAll('#phoneContainer .field-container').length; // Считаем количество полей

    // Создаем выпадающий список
    const select = document.createElement('select');
    select.id = 'phone_type_' + index;

    const options = ['Корпоративный', 'Персональный'];
    options.forEach(optionText => {
        const option = document.createElement('option');
        option.value = optionText.toLowerCase();
        option.textContent = optionText;
        select.appendChild(option);
    });

    fieldContainer.appendChild(select);

    // Создаем поле ввода номера телефона
    const input = document.createElement('input');
    input.type = 'text';
    input.id = 'phone_' + index;
    input.placeholder = 'Введите номер';
    fieldContainer.appendChild(input);

    return fieldContainer;
}

// Обработка нажатия на кнопку
const addFieldButton = document.getElementById('addPhone');
const fieldsContainer = document.getElementById('phoneContainer');

addFieldButton.addEventListener('click', (event) => {
    event.preventDefault();

    const fieldCount = document.querySelectorAll('#phoneContainer .field-container').length;

    if (fieldCount < 4) {
        const newField = createNewFieldPhone();
        fieldsContainer.appendChild(newField);
    } else {
        alert("Можно добавить не более 4 номеров!");
    }
});
