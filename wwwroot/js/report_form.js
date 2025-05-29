// Получаем элементы DOM
const openFormButton_report = document.getElementById('report');
const modalForm_report = document.getElementById('modalForm_create_report');
const closeFormButton_report = document.getElementById('closeFormButton');

// Открываем форму при нажатии на кнопку
openFormButton_report.addEventListener('click', () => {
    modalForm_report.style.display = 'block';
});

// Закрываем форму при нажатии на крестик
closeFormButton_report.addEventListener('click', () => {
    modalForm_report.style.display = 'none';
});
