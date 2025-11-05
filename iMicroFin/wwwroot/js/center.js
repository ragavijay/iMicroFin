// wwwroot/js/center.js

let currentPage = 1;
let searchTerm = '';
let selectedCenterCode = null;

// Load centers on page load
document.addEventListener('DOMContentLoaded', function () {
    loadCenters();
});

// Search centers
function searchCenters() {
    searchTerm = document.getElementById('searchBox').value;
    currentPage = 1;
    loadCenters();
}

// Load centers with pagination
function loadCenters(page = 1) {
    currentPage = page;

    fetch(`/Center/GetCentersList?search=${encodeURIComponent(searchTerm)}&page=${currentPage}&pageSize=10`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                displayCenters(data.centers);
                displayPagination(data.currentPage, data.totalPages);
                displayPageInfo(data.currentPage, data.totalPages, data.totalCount);
            } else {
                document.getElementById('centersList').innerHTML =
                    '<div class="error-message">Error loading centers</div>';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('centersList').innerHTML =
                '<div class="error-message">Error loading centers</div>';
        });
}

// Display centers list
function displayCenters(centers) {
    const centersList = document.getElementById('centersList');

    if (centers.length === 0) {
        centersList.innerHTML = '<div class="loading">No centers found</div>';
        return;
    }

    let html = '';
    centers.forEach(center => {
        const isActive = center.centerCode === selectedCenterCode ? 'active' : '';
        html += `
            <div class="center-item ${isActive}" onclick="selectCenter('${center.centerCode}', event)">
                <div class="center-item-code">${center.centerCode}</div>
                <div class="center-item-name">${center.centerName}</div>
            </div>
        `;
    });

    centersList.innerHTML = html;
}

// Display pagination
function displayPagination(currentPage, totalPages) {
    const pagination = document.getElementById('pagination');

    if (totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    let html = `
        <button onclick="loadCenters(${currentPage - 1})" ${currentPage === 1 ? 'disabled' : ''}>
            Previous
        </button>
    `;

    // Show page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `
                <button onclick="loadCenters(${i})" class="${i === currentPage ? 'active' : ''}">
                    ${i}
                </button>
            `;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<span>...</span>';
        }
    }

    html += `
        <button onclick="loadCenters(${currentPage + 1})" ${currentPage === totalPages ? 'disabled' : ''}>
            Next
        </button>
    `;

    pagination.innerHTML = html;
}

// Display page info
function displayPageInfo(currentPage, totalPages, totalCount) {
    const pageInfo = document.getElementById('pageInfo');
    pageInfo.textContent = `Page ${currentPage} of ${totalPages} (${totalCount} centers)`;
}

// Select center
function selectCenter(centerCode, event) {
    selectedCenterCode = centerCode;

    fetch(`/Center/GetCenter/${centerCode}`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showForm();
                populateForm(data.center, false);

                // Update active state in list
                document.querySelectorAll('.center-item').forEach(item => {
                    item.classList.remove('active');
                });
                event.target.closest('.center-item').classList.add('active');
            }
        })
        .catch(error => console.error('Error:', error));
}

// Create new center
function createNewCenter() {
    selectedCenterCode = null;
    showForm();
    populateForm({
        centerCode: '',
        centerName: '',
        centerId: ''
    }, true);

    // Remove active state from all items
    document.querySelectorAll('.center-item').forEach(item => {
        item.classList.remove('active');
    });
}

// Show form
function showForm() {
    document.getElementById('centerFormPanel').style.display = 'block';
}

// Hide form
function hideForm() {
    document.getElementById('centerFormPanel').style.display = 'none';
    selectedCenterCode = null;

    // Remove active state from all items
    document.querySelectorAll('.center-item').forEach(item => {
        item.classList.remove('active');
    });
}

// Populate form
function populateForm(center, isNew) {
    document.getElementById('centerCode').value = center.centerCode || '';
    document.getElementById('centerName').value = center.centerName || '';
    document.getElementById('centerId').value = center.centerId || '';
    document.getElementById('errorMessage').textContent = '';
    document.getElementById('successMessage').textContent = '';

    if (isNew) {
        document.getElementById('formTitle').textContent = 'Add New Center';
    } else {
        document.getElementById('formTitle').textContent = 'Edit Center';
    }

    // Focus on center name field
    document.getElementById('centerName').focus();
}

// Cancel edit
function cancelEdit() {
    hideForm();
}

// Save center
function saveCenter(event) {
    event.preventDefault();

    const center = {
        centerCode: document.getElementById('centerCode').value,
        centerName: document.getElementById('centerName').value.trim(),
        centerId: parseInt(document.getElementById('centerId').value) || 0
    };

    fetch('/Center/SaveCenter', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(center)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById('successMessage').textContent = data.message;
                document.getElementById('errorMessage').textContent = '';

                // Reload centers list and hide form
                setTimeout(() => {
                    loadCenters(currentPage);
                    hideForm();
                }, 1000);
            } else {
                document.getElementById('errorMessage').textContent = data.message;
                document.getElementById('successMessage').textContent = '';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('errorMessage').textContent = 'Error saving center';
        });
}