// Функция для переключения темы
function toggleTheme() {
    const body = document.body;
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках

    // Переключаем класс light-mode для изменения темы
    body.classList.toggle('light-mode');

    // Меняем изображение в зависимости от выбранной темы
    if (body.classList.contains('light-mode')) {
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
        localStorage.setItem('theme', 'light');
    } else {
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
        localStorage.setItem('theme', 'dark');
    }
}

// При загрузке страницы устанавливаем сохранённую тему и изображение
window.onload = () => {
    const savedTheme = localStorage.getItem('theme');
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках

    if (savedTheme === 'light') {
        document.body.classList.add('light-mode');
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
    } else if (savedTheme === 'dark') {
        document.body.classList.remove('light-mode');
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
    }
};