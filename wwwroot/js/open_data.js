function toggleHiddenArea() {
    const hiddenArea = document.getElementById('hiddenArea');
    if (hiddenArea.style.display === 'none') {
        hiddenArea.style.display = 'block';
    } else {
        hiddenArea.style.display = 'none';
    }
}