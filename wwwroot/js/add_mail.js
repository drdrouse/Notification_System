// Получаем элементы DOM
const openFormButton_mail = document.getElementById('add_mail');
const modalForm_mail = document.getElementById('modalForm_mail');
const closeFormButton_mail = document.getElementById('closeFormButton_phone');

// Открываем форму при нажатии на кнопку
openFormButton_mail.addEventListener('click', () => {
    modalForm_mail.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_mail.addEventListener('click', () => {
    modalForm_mail.style.display = 'none';
});
