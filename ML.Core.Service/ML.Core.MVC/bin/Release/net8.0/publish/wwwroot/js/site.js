// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const dropArea = document.getElementById('drop-area');
const fileInput = document.getElementById('file-input');
const browseButton = document.getElementById('browse-button');
const fileList = document.getElementById('file-list');

// Prevent default drag behaviors
['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});

// Highlight drop area when dragging over
['dragenter', 'dragover'].forEach(eventName => {
    dropArea.addEventListener(eventName, () => dropArea.classList.add('drag-over'), false);
});

['dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, () => dropArea.classList.remove('drag-over'), false);
});

// Handle dropped files
dropArea.addEventListener('drop', handleDrop, false);

// Handle file input change
fileInput.addEventListener('change', handleFiles, false);

// Trigger file input when browse button is clicked
dropArea.addEventListener('click', () => fileInput.click(), false);



function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

function handleDrop(e) {
    const files = e.dataTransfer.files;

    handleFiles(files);
}

function handleFiles(files) {

    /*    fileList.innerHTML = '';*/
    debugger;
    if (!files || files.length === undefined) {
        files = fileInput.files;
    }


    fileList.innerHTML = ''; // Clear the existing list
    for (const file of files) {
      /*  uploadFile(file);*/
        displayFile(file);
    }
}

function displayFile(file) {
    const li = document.createElement('li');
    li.textContent = `${file.name} (${file.size} bytes)`;
    fileList.appendChild(li);
}

function uploadFile(file) {
    const formData = new FormData();
    formData.append('file', file);

    fetch('/Home/UploadFile', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            console.log('File uploaded:', data);
        })
        .catch(error => {
            console.error('Error uploading file:', error);
        });
}