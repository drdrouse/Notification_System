﻿// Получаем элементы DOM
const openFormButton = document.getElementById('change_password');
const modalForm = document.getElementById('modalForm');
const closeFormButton = document.getElementById('closeFormButton');

// Открываем форму при нажатии на кнопку
openFormButton.addEventListener('click', () => {
    modalForm.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton.addEventListener('click', () => {
    modalForm.style.display = 'none';
});

// Закрываем форму при клике вне её области
window.addEventListener('click', (event) => {
    if (event.target === modalForm) {
        modalForm.style.display = 'none';
    }
});