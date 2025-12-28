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
        const isOngoing = loan.loanStatus === 'O';

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
                        ${isOngoing ? `<a href="javascript:void(0)" onclick="openBadLoanModal('${loan.loanCode}', '${escapeHtml(loan.memberName)}')" class="action-link action-link-bad">Bad Loan</a>` : ''}
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
        case 'O': return 'Ongoing';
        case 'C': return 'Closed';
        case 'R': return 'Rejected';
        case 'S': return 'Pre-Closed';
        case 'b': return 'Bad Loan - Pending';
        case 'B': return 'Bad Loan - Settled';
        default: return status;
    }
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, function (m) { return map[m]; });
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

// Bad Loan Modal Functions
let currentBadLoanCode = '';

function openBadLoanModal(loanCode, memberName) {
    currentBadLoanCode = loanCode;
    document.getElementById('modalLoanCode').textContent = loanCode;
    document.getElementById('modalMemberName').textContent = memberName;
    document.getElementById('statusRemarks').value = '';
    document.getElementById('charCount').textContent = '0';
    document.getElementById('badLoanModal').style.display = 'flex';
}

function closeBadLoanModal() {
    document.getElementById('badLoanModal').style.display = 'none';
    currentBadLoanCode = '';
}

async function confirmBadLoan() {
    const remarks = document.getElementById('statusRemarks').value.trim();

    if (!remarks) {
        alert('Please enter remarks for marking this loan as bad');
        return;
    }

    if (remarks.length > 50) {
        alert('Remarks cannot exceed 50 characters');
        return;
    }

    try {
        const response = await fetch('/Loan/MarkAsBadLoan', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                loanCode: currentBadLoanCode,
                statusRemarks: remarks
            })
        });

        const result = await response.json();

        if (result.success) {
            alert('Loan marked as bad loan successfully');
            closeBadLoanModal();
            // Reload loans to show updated status
            if (currentGroupCode) {
                loadLoans(currentGroupCode);
            }
        } else {
            alert('Error: ' + result.message);
        }
    } catch (error) {
        console.error('Error marking loan as bad:', error);
        alert('An error occurred. Please try again.');
    }
}

// Character counter for remarks textarea
document.addEventListener('DOMContentLoaded', function () {
    const remarksField = document.getElementById('statusRemarks');
    if (remarksField) {
        remarksField.addEventListener('input', function () {
            document.getElementById('charCount').textContent = this.value.length;
        });
    }
});

// Close modal when clicking outside
window.addEventListener('click', function (event) {
    const modal = document.getElementById('badLoanModal');
    if (event.target === modal) {
        closeBadLoanModal();
    }
});