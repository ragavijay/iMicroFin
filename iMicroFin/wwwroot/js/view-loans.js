// wwwroot/js/view-loans.js

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
        hiddenIds: ['groupCode', 'centerName'],
        fieldNames: ['groupCode', 'centerName', 'groupName'],
        serviceUrl: '/Group/GetGroupListByPattern',
        minChars: 1,
        debounceTime: 300,
        onSelect: function (group) {
            console.log('Selected group:', group);
            currentGroupCode = group.groupCode;

            // Update display fields
            document.getElementById('groupCodeDisplay').value = group.groupCode || '';

            // Load loans for selected group
            loadLoans(group.groupCode);
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
    document.getElementById('centerName').value = initialCenterName;

    currentGroupCode = initialGroupCode;

    // Load loans
    loadLoans(initialGroupCode);
}

// Load loans for a group
function loadLoans(groupCode) {
    if (!groupCode) {
        return;
    }

    // Show loading
    document.getElementById('loadingMessage').style.display = 'block';
    document.getElementById('loansTableContainer').style.display = 'none';

    fetch(`/Loan/GetLoansByGroup/${groupCode}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('loadingMessage').style.display = 'none';

            if (data.success) {
                displayLoans(data.loans);

                // Show action buttons
                document.getElementById('addLoanBtn').style.display = 'inline-block';
                document.getElementById('addGroupLoanBtn').style.display = 'inline-block';
                document.getElementById('exportBtn').style.display = 'inline-block';
            } else {
                alert('Error loading loans: ' + data.message);
            }
        })
        .catch(error => {
            document.getElementById('loadingMessage').style.display = 'none';
            console.error('Error:', error);
            alert('Error loading loans');
        });
}

// Display loans in table
function displayLoans(loans) {
    const tableBody = document.getElementById('loansTableBody');
    const tableContainer = document.getElementById('loansTableContainer');
    const noLoansMessage = document.getElementById('noLoansMessage');

    tableContainer.style.display = 'block';

    if (!loans || loans.length === 0) {
        tableBody.innerHTML = '';
        noLoansMessage.style.display = 'block';
        return;
    }

    noLoansMessage.style.display = 'none';

    let html = '';
    loans.forEach(loan => {
        const statusText = getLoanStatusText(loan.loanStatus);
        const canEdit = (loan.loanStatus === 'P' || loan.loanStatus === 'A');

        html += `
            <tr>
                <td>${loan.loanCode}</td>
                <td>${loan.memberCode}</td>
                <td>${loan.memberName}</td>
                <td>₹${loan.loanAmount.toLocaleString()}</td>
                <td><span class="loan-status-${loan.loanStatus}">${statusText}</span></td>
                <td>
                    <div class="action-links">
                        <a href="/Loan/ViewLoan/${loan.loanCode}" class="action-link action-link-view">View</a>
                        ${canEdit ? `<a href="/Loan/LoanForm/${loan.loanCode}" class="action-link action-link-edit">Edit</a>` : ''}
                        ${canEdit ? `<a href="/Loan/LoanStatusForm/${loan.loanCode}" class="action-link action-link-approve">Approve</a>` : ''}
                    </div>
                </td>
            </tr>
        `;
    });

    tableBody.innerHTML = html;
}

// Get loan status text
function getLoanStatusText(status) {
    switch (status) {
        case 'P': return 'Pending';
        case 'A': return 'Approved';
        case 'O': return 'Outstanding';
        case 'C': return 'Closed';
        case 'R': return 'Rejected';
        default: return status;
    }
}

// Add new loan
function addNewLoan() {
    if (!currentGroupCode) {
        alert('Please select a group first');
        return;
    }

    // Redirect to loan form
    window.location.href = `/Loan/LoanForm`;
}

// Add new group loan
function addNewGroupLoan() {
    if (!currentGroupCode) {
        alert('Please select a group first');
        return;
    }

    // Redirect to group loan form
    window.location.href = `/Loan/GroupLoanForm`;
}

// Export loans
function exportLoans() {
    if (!currentGroupCode) {
        alert('Please select a group first');
        return;
    }

    window.location.href = `/Loan/ExportLoans`;
}