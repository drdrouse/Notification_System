function toggleHiddenArea(serviceId) {
    const hiddenArea = document.getElementById(serviceId);
    if (hiddenArea.style.display === 'none') {
        hiddenArea.style.display = 'block';
    } else {
        hiddenArea.style.display = 'none';
    }
}