const allCheckboxes = document.querySelectorAll('.checkbox-group input[type="checkbox"]');

const specialCheckbox = document.querySelector('#Admin');

// Обработчик для всех чекбоксов
allCheckboxes.forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        // Если меняется состояние специального чекбокса
        if (this === specialCheckbox) {
            // Обнуляем все остальные
            allCheckboxes.forEach(cb => {
                if (cb !== specialCheckbox) {
                    cb.checked = false;
                }
            });
        } else {
            // Если меняется обычный чекбокс
            if (specialCheckbox.checked) {
                specialCheckbox.checked = false;
            }
        }
    });
});