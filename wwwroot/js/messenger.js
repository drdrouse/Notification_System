// Получим все кнопки
const buttons = document.querySelectorAll('button[data-target]');

// Проходимся по каждой кнопке и добавляем обработчики событий
buttons.forEach(button => {
    button.addEventListener('click', () => {
        // Получаем ID целевой области
        const targetId = button.dataset.target;

        // Находим целевую область
        const targetArea = document.getElementById(targetId);

        // Меняем класс области 
        if (targetArea.classList.contains('messanger-data-hidden')) {
            targetArea.classList.remove('messanger-data-hidden');
            targetArea.classList.add('messanger-data-visible');
        } else {
            targetArea.classList.remove('messanger-data-visible');
            targetArea.classList.add('messanger-data-hidden');
        }
    });
});