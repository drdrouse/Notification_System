﻿// Функция для переключения темы
function toggleTheme() {
    const body = document.body;
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках
    const exiteImage = document.querySelector('#exite img');
    const accountImage = document.querySelector('#account img');

    // Переключаем класс light-mode для изменения темы
    body.classList.toggle('light-mode');

    // Меняем изображение в зависимости от выбранной темы
    if (body.classList.contains('light-mode')) {
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
        exiteImage.src = '/content/images/account/exite-lite.png';
        accountImage.src = '/content/images/account/account-lite.png';
        localStorage.setItem('theme', 'light');
    } else {
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
        exiteImage.src = '/content/images/account/exite.png';
        accountImage.src = '/content/images/account/account.png';
        localStorage.setItem('theme', 'dark');
    }
}

// При загрузке страницы устанавливаем сохранённую тему и изображение
window.onload = () => {
    const savedTheme = localStorage.getItem('theme');
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках
    const exiteImage = document.querySelector('#exite img');
    const accountImage = document.querySelector('#account img');

    if (savedTheme === 'light') {
        document.body.classList.add('light-mode');
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
        exiteImage.src = '/content/images/account/exite-lite.png';
        accountImage.src = '/content/images/account/account-lite.png';
    } else if (savedTheme === 'dark') {
        document.body.classList.remove('light-mode');
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
        exiteImage.src = '/content/images/account/exite.png';
        accountImage.src = '/content/images/account/account.png';
    }
};