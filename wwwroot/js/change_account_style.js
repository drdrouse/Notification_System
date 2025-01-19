// Функция для переключения темы
function toggleTheme() {
    const body = document.body;

    // Переключаем класс dark-mode для изменения темы
    body.classList.toggle('light-mode');

    const image = document.getElementById('setting');
    if (body.classList.contains('light-mode')) {
        image.src = '~/content/images/account/setting-lite.png?' + new Date().getTime(); // Путь к изображению для светлой темы
    } else {
        image.src = '~/content/images/account/setting.png?' + new Date().getTime(); // Путь к изображению для тёмной темы
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