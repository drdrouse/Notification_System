// Функция для переключения темы
function toggleTheme() {
    const body = document.body;
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках
    const exiteImage = document.querySelector('#exite img');
    const accountImage = document.querySelector('#account img');
    const usercreateImage = document.querySelector('#usercreate img');
    const photoImage = document.querySelector('#photo img');
    const logImage = document.querySelector('#log img');
    const messengersImage = document.querySelector('#messengers img');
    const passwordImage = document.querySelector('#change_password img');
    const change_themeImage = document.querySelector('#change_theme img');
    const phoneImage = document.querySelector('#add_phone img');
    const mailImage = document.querySelector('#add_mail img');
    const undisableImage = document.querySelector('#undisable img');
    const disableImage = document.querySelector('#disable img');
    const addPhotoImage = document.querySelector('#add_photo img');
    // Переключаем класс light-mode для изменения темы
    body.classList.toggle('light-mode');

    // Меняем изображение в зависимости от выбранной темы
    if (body.classList.contains('light-mode')) {
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
        exiteImage.src = '/content/images/account/exite-lite.png';
        accountImage.src = '/content/images/account/account-lite.png';
        if (photoImage) photoImage.src = '/content/images/account/avatar-placeholder-lite.png';
        if (usercreateImage) usercreateImage.src = '/content/images/account/users-lite.png';
        if (logImage) logImage.src = '/content/images/account/log-lite.png';
        if (messengersImage) messengersImage.src = '/content/images/account/messenger-lite.png';
        if (passwordImage) passwordImage.src = '/content/images/account/password-lite.png';
        if (change_themeImage) change_themeImage.src = '/content/images/account/change-theme-lite.png';
        if (phoneImage) phoneImage.src = '/content/images/account/phone-lite.png';
        if (mailImage) mailImage.src = '/content/images/account/mail-lite.png'; 
        if (undisableImage) undisableImage.src = '/content/images/account/undisable-lite.png';
        if (disableImage) disableImage.src = '/content/images/account/disable-lite.png';
        if (addPhotoImage) addPhotoImage.src = 'content/images/account/add_photo-lite.png';
        localStorage.setItem('theme', 'light');
    } else {
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
        exiteImage.src = '/content/images/account/exite.png';
        accountImage.src = '/content/images/account/account.png';
        if (photoImage) photoImage.src = '/content/images/account/avatar-placeholder.png';
        if (usercreateImage) usercreateImage.src = '/content/images/account/users.png';
        if (logImage) logImage.src = '/content/images/account/log.png';
        if (messengersImage) messengersImage.src = '/content/images/account/messenger.png';
        if (localStorage) localStorage.setItem('theme', 'dark');
        if (passwordImage) passwordImage.src = '/content/images/account/password.png';
        if (change_themeImage) change_themeImage.src = '/content/images/account/change-theme.png';
        if (phoneImage) phoneImage.src = '/content/images/account/phone.png';
        if (mailImage) mailImage.src = '/content/images/account/mail.png'; 
        if (undisableImage) undisableImage.src = '/content/images/account/undisable.png';
        if (disableImage) disableImage.src = '/content/images/account/disable.png';
        if (addPhotoImage) addPhotoImage.src = 'content/images/account/add_photo.png';
    }
}

// При загрузке страницы устанавливаем сохранённую тему и изображение
window.onload = () => {
    const savedTheme = localStorage.getItem('theme');
    const settingImage = document.querySelector('#settings img'); // Изображение в настройках
    const exiteImage = document.querySelector('#exite img');
    const accountImage = document.querySelector('#account img');
    const photoImage = document.querySelector('#photo img');
    const usercreateImage = document.querySelector('#usercreate img');
    const logImage = document.querySelector('#log img');
    const messengersImage = document.querySelector('#messengers img');
    const passwordImage = document.querySelector('#change_password img');
    const change_themeImage = document.querySelector('#change_theme img');
    const phoneImage = document.querySelector('#add_phone img');
    const mailImage = document.querySelector('#add_mail img');
    const undisableImage = document.querySelector('#undisable img');
    const disableImage = document.querySelector('#disable img');
    const addPhotoImage = document.querySelector('#add_photo img');
    if (savedTheme === 'light') {
        document.body.classList.add('light-mode');
        settingImage.src = '/content/images/account/setting-lite.png'; // Путь к изображению для светлой темы
        exiteImage.src = '/content/images/account/exite-lite.png';
        accountImage.src = '/content/images/account/account-lite.png'; 
        if (photoImage) photoImage.src = '/content/images/account/avatar-placeholder-lite.png';
        if (usercreateImage) usercreateImage.src = '/content/images/account/users-lite.png';
        if (logImage) logImage.src = '/content/images/account/log-lite.png';
        if (messengersImage) messengersImage.src = '/content/images/account/messenger-lite.png';
        if (passwordImage) passwordImage.src = '/content/images/account/password-lite.png';
        if (change_themeImage) change_themeImage.src = '/content/images/account/change-theme-lite.png';
        if (phoneImage) phoneImage.src = '/content/images/account/phone-lite.png';
        if (mailImage) mailImage.src = '/content/images/account/mail-lite.png';
        if (undisableImage) undisableImage.src = '/content/images/account/undisable-lite.png';
        if (disableImage) disableImage.src = '/content/images/account/disable-lite.png'
        if (addPhotoImage) addPhotoImage.src = 'content/images/account/add_photo-lite.png';
    } else if (savedTheme === 'dark') {
        document.body.classList.remove('light-mode');
        settingImage.src = '/content/images/account/setting.png'; // Путь к изображению для тёмной темы
        exiteImage.src = '/content/images/account/exite.png';
        accountImage.src = '/content/images/account/account.png';  
        if (photoImage) photoImage.src = '/content/images/account/avatar-placeholder.png';
        if (usercreateImage) usercreateImage.src = '/content/images/account/users.png';
        if (logImage) logImage.src = '/content/images/account/log.png';
        if (messengersImage) messengersImage.src = '/content/images/account/messenger.png';
        if (passwordImage) passwordImage.src = '/content/images/account/password.png';
        if (change_themeImage) change_themeImage.src = '/content/images/account/change-theme.png';
        if (phoneImage) phoneImage.src = '/content/images/account/phone.png';
        if (mailImage) mailImage.src = '/content/images/account/mail.png'; 
        if (undisableImage) undisableImage.src = '/content/images/account/undisable.png';
        if (disableImage) disableImage.src = '/content/images/account/disable.png';
        if (addPhotoImage) addPhotoImage.src = 'content/images/account/add_photo.png';
    }
};