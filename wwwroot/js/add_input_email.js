function createNewFieldEmail() {
    const fieldContainer = document.createElement('div');
    fieldContainer.classList.add('field-container');

    const index = document.querySelectorAll('#emailsContainer input').length; // Получаем количество полей

    // Создаем выпадающий список
    const select = document.createElement('select');
    select.id = 'email_type_' + index;

    const options = ['Корпоративная', 'Персональная', 'Временная'];
    options.forEach(optionText => {
        const option = document.createElement('option');
        option.value = optionText.toLowerCase();
        option.textContent = optionText;
        select.appendChild(option);
    });

    fieldContainer.appendChild(select);

    // Создаем поле ввода email
    const input = document.createElement('input');
    input.type = 'email';
    input.id = 'email_' + index;
    input.placeholder = 'Введите email';
    fieldContainer.appendChild(input);

    return fieldContainer;
}

// Обработка нажатия на кнопку
const addEmailButton = document.getElementById('addEmail');
const emailsContainer = document.getElementById('emailsContainer');

addEmailButton.addEventListener('click', (event) => {
    event.preventDefault();

    const newField = createNewFieldEmail();
    emailsContainer.appendChild(newField);
});