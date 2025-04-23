// Получаем элементы DOM
const openFormButton_phone = document.getElementById('add_phone');
const modalForm_phone = document.getElementById('modalForm_phone');
const closeFormButton_phone = document.getElementById('closeFormButton_phone');

// Открываем форму при нажатии на кнопку
openFormButton_phone.addEventListener('click', () => {
    modalForm_phone.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_phone.addEventListener('click', () => {
    modalForm_phone.style.display = 'none';
});
