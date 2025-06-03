// Получаем элементы DOM
const openFormButton_photo = document.getElementById('add_photo');
const modalForm_photo = document.getElementById('modalForm_photo');
const closeFormButton_photo = document.getElementById('closeFormButton_phone');

// Открываем форму при нажатии на кнопку
openFormButton_photo.addEventListener('click', () => {
    modalForm_photo.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_photo.addEventListener('click', () => {
    modalForm_photo.style.display = 'none';
});
