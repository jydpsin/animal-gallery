// API endpoints
const API_BASE_URL = '/api/animalpicture';

// UI Elements
const elements = {
    animalType: document.getElementById('animalType'),
    pictureCount: document.getElementById('pictureCount'),
    statusMessage: document.getElementById('statusMessage'),
    imageContainer: document.getElementById('imageContainer'),
    animalImage: document.getElementById('animalImage'),
    imageInfo: document.getElementById('imageInfo')
};

// Helper Functions
function showLoading() {
    elements.imageContainer.classList.add('loading');
    elements.animalImage.classList.add('d-none');
    elements.statusMessage.classList.add('d-none');
    elements.imageInfo.classList.add('d-none');
}

function hideLoading() {
    elements.imageContainer.classList.remove('loading');
}

function showError(message) {
    elements.statusMessage.textContent = message;
    elements.statusMessage.classList.remove('alert-success', 'd-none');
    elements.statusMessage.classList.add('alert-danger');
}

function showSuccess(message) {
    elements.statusMessage.textContent = message;
    elements.statusMessage.classList.remove('alert-danger', 'd-none');
    elements.statusMessage.classList.add('alert-success');
}

function displayImage(imageData) {
    elements.animalImage.src = `data:${imageData.contentType};base64,${imageData.imageData}`;
    elements.animalImage.classList.remove('d-none');
    
    const storedDate = new Date(imageData.storedAt).toLocaleString();
    elements.imageInfo.innerHTML = `
        <strong>Animal Type:</strong> ${imageData.animalType}<br>
        <strong>Stored At:</strong> ${storedDate}
    `;
    elements.imageInfo.classList.remove('d-none');
}

// API Interaction Functions
async function savePictures() {
    const animalType = elements.animalType.value;
    const count = elements.pictureCount.value;

    if (count < 1 || count > 10) {
        showError('Please enter a number between 1 and 10');
        return;
    }

    try {
        showLoading();
        
        const response = await fetch(`${API_BASE_URL}/${animalType}?count=${count}`, {
            method: 'POST'
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Failed to save pictures');
        }

        const data = await response.json();
        displayImage(data);
        showSuccess(`Successfully saved ${count} picture(s)!`);
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

async function getLatestPicture() {
    const animalType = elements.animalType.value;

    try {
        showLoading();
        
        const response = await fetch(`${API_BASE_URL}/${animalType}/latest`);
        
        if (response.status === 404) {
            throw new Error(`No pictures found for ${animalType}`);
        }
        
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Failed to fetch latest picture');
        }

        const data = await response.json();
        displayImage(data);
        showSuccess('Latest picture retrieved successfully!');
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

// Input validation
elements.pictureCount.addEventListener('input', (e) => {
    const value = parseInt(e.target.value);
    if (value < 1) e.target.value = 1;
    if (value > 10) e.target.value = 10;
});
