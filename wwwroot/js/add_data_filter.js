// Получаем элементы DOM
const openFormButton_data = document.getElementById('filter_data');
const modalForm_data = document.getElementById('modalForm_data_filter');
const closeFormButton_data = document.getElementById('closeFormButton');

// Открываем форму при нажатии на кнопку
openFormButton_data.addEventListener('click', () => {
    modalForm_data.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_data.addEventListener('click', () => {
    modalForm_data.style.display = 'none';
});
