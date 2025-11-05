// wwwroot/js/group.js

let currentPage = 1;
let searchTerm = '';
let selectedGroupCode = null;
let centerAutoComplete = null;

// Load groups on page load
document.addEventListener('DOMContentLoaded', function () {
    loadGroups();
    initializeCenterAutoComplete();
});

// Initialize center autocomplete
function initializeCenterAutoComplete() {
    centerAutoComplete = new AutoComplete({
        inputId: 'centerName',
        hiddenId: 'centerCode',
        serviceUrl: '/Center/GetCentersListByPattern',
        minChars: 1,
        debounceTime: 300,
        onSelect: function (center) {
            console.log('Selected center:', center);
        }
    });

    // Custom formatting for center items
    centerAutoComplete.formatItem = function (item) {
        const code = item.centerCode || '';
        const name = item.centerName || '';
        return `
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <strong>${name}</strong>
                <span class="autocomplete-code">${code}</span>
            </div>
        `;
    };
}

// Search groups
function searchGroups() {
    searchTerm = document.getElementById('searchBox').value;
    currentPage = 1;
    loadGroups();
}

// Load groups with pagination
function loadGroups(page = 1) {
    currentPage = page;

    fetch(`/Group/GetGroupsList?search=${encodeURIComponent(searchTerm)}&page=${currentPage}&pageSize=10`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                displayGroups(data.groups);
                displayPagination(data.currentPage, data.totalPages);
                displayPageInfo(data.currentPage, data.totalPages, data.totalCount);
            } else {
                document.getElementById('groupsList').innerHTML =
                    '<div class="error-message">Error loading groups</div>';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('groupsList').innerHTML =
                '<div class="error-message">Error loading groups</div>';
        });
}

// Display groups list
function displayGroups(groups) {
    const groupsList = document.getElementById('groupsList');

    if (groups.length === 0) {
        groupsList.innerHTML = '<div class="loading">No groups found</div>';
        return;
    }

    let html = '';
    groups.forEach(group => {
        const isActive = group.groupCode === selectedGroupCode ? 'active' : '';
        html += `
            <div class="group-item ${isActive}" onclick="selectGroup('${group.groupCode}', event)">
                <div class="group-item-header">
                    <div class="group-item-code">${group.groupCode}</div>
                </div>
                <div class="group-item-name">${group.groupName}</div>
                <div class="group-item-center">Center: ${group.centerName} (${group.centerCode})</div>
            </div>
        `;
    });

    groupsList.innerHTML = html;
}

// Display pagination
function displayPagination(currentPage, totalPages) {
    const pagination = document.getElementById('pagination');

    if (totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    let html = `
        <button onclick="loadGroups(${currentPage - 1})" ${currentPage === 1 ? 'disabled' : ''}>
            Previous
        </button>
    `;

    // Show page numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `
                <button onclick="loadGroups(${i})" class="${i === currentPage ? 'active' : ''}">
                    ${i}
                </button>
            `;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<span>...</span>';
        }
    }

    html += `
        <button onclick="loadGroups(${currentPage + 1})" ${currentPage === totalPages ? 'disabled' : ''}>
            Next
        </button>
    `;

    pagination.innerHTML = html;
}

// Display page info
function displayPageInfo(currentPage, totalPages, totalCount) {
    const pageInfo = document.getElementById('pageInfo');
    pageInfo.textContent = `Page ${currentPage} of ${totalPages} (${totalCount} groups)`;
}

// Select group
function selectGroup(groupCode, event) {
    selectedGroupCode = groupCode;

    fetch(`/Group/GetGroup/${groupCode}`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showForm();
                populateForm(data.group, false);

                // Update active state in list
                document.querySelectorAll('.group-item').forEach(item => {
                    item.classList.remove('active');
                });
                event.target.closest('.group-item').classList.add('active');
            }
        })
        .catch(error => console.error('Error:', error));
}

// Create new group
function createNewGroup() {
    selectedGroupCode = null;
    showForm();
    populateForm({
        groupCode: '',
        groupName: '',
        groupId: 0,
        centerCode: '',
        centerName: '',
        leaderName: ''
    }, true);

    // Remove active state from all items
    document.querySelectorAll('.group-item').forEach(item => {
        item.classList.remove('active');
    });
}

// Show form
function showForm() {
    document.getElementById('groupFormPanel').style.display = 'block';
}

// Hide form
function hideForm() {
    document.getElementById('groupFormPanel').style.display = 'none';
    selectedGroupCode = null;

    // Remove active state from all items
    document.querySelectorAll('.group-item').forEach(item => {
        item.classList.remove('active');
    });
}

// Populate form
function populateForm(group, isNew) {
    document.getElementById('groupCode').value = group.groupCode || '';
    document.getElementById('groupName').value = group.groupName || '';
    document.getElementById('groupId').value = group.groupId || 0;
    document.getElementById('centerCode').value = group.centerCode || '';
    document.getElementById('centerName').value = group.centerName || '';
    document.getElementById('leaderName').value = group.leaderName || '';
    document.getElementById('errorMessage').textContent = '';
    document.getElementById('successMessage').textContent = '';

    const centerNameInput = document.getElementById('centerName');

    if (isNew) {
        document.getElementById('formTitle').textContent = 'Add New Group';
        // Enable center selection for new groups
        centerNameInput.readOnly = false;
        centerNameInput.classList.remove('readonly-field');
        // Re-enable autocomplete
        if (centerAutoComplete) {
            centerAutoComplete.inputElement.style.pointerEvents = 'auto';
        }
    } else {
        document.getElementById('formTitle').textContent = 'Edit Group';
        // Disable center change for existing groups
        centerNameInput.readOnly = true;
        centerNameInput.classList.add('readonly-field');
        // Disable autocomplete dropdown
        if (centerAutoComplete) {
            centerAutoComplete.inputElement.style.pointerEvents = 'none';
            centerAutoComplete.closeList();
        }
    }

    // Focus on group name field
    document.getElementById('groupName').focus();
}

// Cancel edit
function cancelEdit() {
    hideForm();
}

// Save group
function saveGroup(event) {
    event.preventDefault();

    const group = {
        groupCode: document.getElementById('groupCode').value,
        groupName: document.getElementById('groupName').value.trim(),
        groupId: parseInt(document.getElementById('groupId').value) || 0,
        centerCode: document.getElementById('centerCode').value.trim(),
        centerName: document.getElementById('centerName').value.trim(),
        leaderName: document.getElementById('leaderName').value.trim()
    };

    // Validate center selection
    if (!group.centerCode) {
        document.getElementById('errorMessage').textContent = 'Please select a center';
        return;
    }

    fetch('/Group/SaveGroup', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(group)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById('successMessage').textContent = data.message;
                document.getElementById('errorMessage').textContent = '';

                // Reload groups list and hide form
                setTimeout(() => {
                    loadGroups(currentPage);
                    hideForm();
                }, 1000);
            } else {
                document.getElementById('errorMessage').textContent = data.message;
                document.getElementById('successMessage').textContent = '';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('errorMessage').textContent = 'Error saving group';
        });
}