// wwwroot/js/view-members.js

let groupAutoComplete;
let currentGroupCode = '';

document.addEventListener('DOMContentLoaded', function () {
    // Initialize group autocomplete
    initializeGroupAutoComplete();

    // Load initial data if group code is provided
    if (initialGroupCode && initialGroupCode !== '') {
        loadInitialGroupData();
    }
});

// Initialize group autocomplete
function initializeGroupAutoComplete() {
    groupAutoComplete = new MultiFieldAutoComplete({
        inputId: 'groupName',
        hiddenIds: ['groupCode', 'leaderName'],
        fieldNames: ['groupCode', 'leaderName', 'groupName'],
        serviceUrl: '/Group/GetGroupListByPattern',
        minChars: 1,
        debounceTime: 300,
        onSelect: function (group) {
            console.log('Selected group:', group);
            currentGroupCode = group.groupCode;

            // Update display fields
            document.getElementById('groupCodeDisplay').value = group.groupCode || '';

            // Load members for selected group
            loadMembers(group.groupCode);
        }
    });

    // Custom formatting for group items
    groupAutoComplete.formatItem = function (item) {
        const groupCode = item.groupCode || '';
        const groupName = item.groupName || '';
        const centerName = item.centerName || '';

        return `
            <div style="display: flex; flex-direction: column; gap: 4px;">
                <div style="display: flex; justify-content: space-between;">
                    <strong>${groupName}</strong>
                    <span class="autocomplete-code">${groupCode}</span>
                </div>
                <div style="font-size: 0.85em; color: #666;">
                    Center: ${centerName}
                </div>
            </div>
        `;
    };
}

// Load initial group data (when group code is passed as parameter)
function loadInitialGroupData() {
    document.getElementById('groupName').value = initialGroupName;
    document.getElementById('groupCode').value = initialGroupCode;
    document.getElementById('groupCodeDisplay').value = initialGroupCode;
    document.getElementById('leaderName').value = initialLeaderName;

    currentGroupCode = initialGroupCode;

    // Load members
    loadMembers(initialGroupCode);
}

// Load members for a group
function loadMembers(groupCode) {
    if (!groupCode) {
        return;
    }

    // Show loading
    document.getElementById('loadingMessage').style.display = 'block';
    document.getElementById('membersTableContainer').style.display = 'none';

    fetch(`/Member/GetMembersByGroup/${groupCode}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('loadingMessage').style.display = 'none';

            if (data.success) {
                displayMembers(data.members);

                // Show action buttons
                document.getElementById('addMemberBtn').style.display = 'inline-block';
                document.getElementById('exportBtn').style.display = 'inline-block';
            } else {
                alert('Error loading members: ' + data.message);
            }
        })
        .catch(error => {
            document.getElementById('loadingMessage').style.display = 'none';
            console.error('Error:', error);
            alert('Error loading members');
        });
}

// Display members in table
function displayMembers(members) {
    const tableBody = document.getElementById('membersTableBody');
    const tableContainer = document.getElementById('membersTableContainer');
    const noMembersMessage = document.getElementById('noMembersMessage');

    tableContainer.style.display = 'block';

    if (!members || members.length === 0) {
        tableBody.innerHTML = '';
        noMembersMessage.style.display = 'block';
        return;
    }

    noMembersMessage.style.display = 'none';

    let html = '';
    members.forEach(member => {
        html += `
            <tr>
                <td>${member.memberCode}</td>
                <td>${member.memberName}</td>
                <td>${member.groupName}</td>
                <td>${member.centerName}</td>
                <td>${member.leaderName || '-'}</td>
                <td>
                    <div class="action-links">
                        <a href="/Member/ViewMember/${member.memberCode}" class="action-link action-link-view">View</a>
                        <a href="/Member/MemberForm/${member.memberCode}" class="action-link action-link-edit">Edit</a>
                    </div>
                </td>
            </tr>
        `;
    });

    tableBody.innerHTML = html;
}

// Add new member
function addNewMember() {
    if (!currentGroupCode) {
        alert('Please select a group first');
        return;
    }

    // Redirect to member form with group pre-selected
    // You'll need to modify your MemberForm to accept group code as parameter
    window.location.href = `/Member/MemberForm?groupCode=${currentGroupCode}`;
}

// Export members
function exportMembers() {
    if (!currentGroupCode) {
        alert('Please select a group first');
        return;
    }

    window.location.href = `/Member/ExportMembers/${currentGroupCode}`;
}