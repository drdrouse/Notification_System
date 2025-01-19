// Функция для переключения темы
function toggleTheme() {
    const body = document.body;

    // Переключаем класс dark-mode для изменения темы
    body.classList.toggle('light-mode');

    const settingImage = document.querySelector('#settings img');
    if (body.classList.contains('light-mode')) {
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
    } else {
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
    }

    // Запоминаем выбранную тему в localStorage
    if (body.classList.contains('light-mode')) {
        localStorage.setItem('theme', 'light');
    } else {
        localStorage.setItem('theme', 'dark');
    }
}

// При загрузке страницы устанавливаем сохранённую тему
window.onload = () => {
    if (localStorage.getItem('theme') === 'light') {
        document.body.classList.add('light-mode');
    }
};