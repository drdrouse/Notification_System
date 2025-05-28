// Получаем элементы DOM
const openFormButton_action = document.getElementById('filter_action');
const modalForm_action = document.getElementById('modalForm_action_filter');
const closeFormButton_action = document.getElementById('closeFormButton');

// Открываем форму при нажатии на кнопку
openFormButton_action.addEventListener('click', () => {
    modalForm_action.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_action.addEventListener('click', () => {
    modalForm_action.style.display = 'none';
});
