// wwwroot/js/group-loan-form.js

let groupAutoComplete;

document.addEventListener('DOMContentLoaded', function () {
    // Initialize date pickers
    initializeDatePickers();

    // Initialize group autocomplete
    initializeGroupAutoComplete();

    // Add event listeners
    setupEventListeners();
});

// Initialize group autocomplete
function initializeGroupAutoComplete() {
    groupAutoComplete = new MultiFieldAutoComplete({
        inputId: 'GroupName',
        hiddenIds: ['GroupCode', 'CenterName', 'LeaderName'],
        fieldNames: ['groupCode', 'centerName', 'leaderName', 'groupName'],
        serviceUrl: '/Group/GetGroupListByPattern',
        minChars: 1,
        debounceTime: 300,
        onSelect: function (group) {
            console.log('Selected group:', group);

            // Manually set all fields to ensure they're populated
            document.getElementById('GroupCode').value = group.groupCode || '';
            document.getElementById('GroupCodeDisplay').value = group.groupCode || '';
            document.getElementById('CenterName').value = group.centerName || '';
            document.getElementById('LeaderName').value = group.leaderName || '';

            // Hide loan info until group is validated
            document.getElementById('loanInfo').style.display = 'none';
        }
    });

    // Custom formatting for group items
    groupAutoComplete.formatItem = function (item) {
        const groupCode = item.groupCode || '';
        const groupName = item.groupName || '';
        const centerName = item.centerName || '';
        const leaderName = item.leaderName || '';

        return `
            <div style="display: flex; flex-direction: column; gap: 4px;">
                <div style="display: flex; justify-content: space-between;">
                    <strong>${groupName}</strong>
                    <span class="autocomplete-code">${groupCode}</span>
                </div>
                <div style="font-size: 0.85em; color: #666;">
                    Center: ${centerName}${leaderName ? ' | Leader: ' + leaderName : ''}
                </div>
            </div>
        `;
    };
}
// Initialize date pickers
function initializeDatePickers() {
    const loanDateField = document.getElementById('LoanDate');
    const disposalDateField = document.getElementById('LoanDisposalDate');

    if (loanDateField) {
        const loanDateValue = loanDateField.value.trim();
        let loanDate = null;

        if (loanDateValue && loanDateValue !== '') {
            const parts = loanDateValue.split('/');
            if (parts.length === 3) {
                loanDate = new Date(parts[2], parts[1] - 1, parts[0]);
            }
        } else {
            loanDate = new Date();
            loanDateField.value = formatDate(loanDate);
        }

        flatpickr('#LoanDate', {
            dateFormat: 'd/m/Y',
            defaultDate: loanDate,
            allowInput: true
        });
    }

    if (disposalDateField) {
        const disposalDateValue = disposalDateField.value.trim();
        let disposalDate = null;

        if (disposalDateValue && disposalDateValue !== '') {
            const parts = disposalDateValue.split('/');
            if (parts.length === 3) {
                disposalDate = new Date(parts[2], parts[1] - 1, parts[0]);
            }
        } else {
            disposalDate = new Date();
            disposalDateField.value = formatDate(disposalDate);
        }

        flatpickr('#LoanDisposalDate', {
            dateFormat: 'd/m/Y',
            defaultDate: disposalDate,
            allowInput: true
        });
    }
}

// Format date as DD/MM/YYYY
function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

// Setup event listeners
function setupEventListeners() {
    const groupNameField = document.getElementById('GroupName');
    const loanAmountField = document.getElementById('LoanAmount');
    const processingFeeRateField = document.getElementById('ProcessingFeeRate');
    const insuranceRateField = document.getElementById('InsuranceRate');
    const interestRateField = document.getElementById('InterestRate');
    const tenureField = document.getElementById('Tenure');

    if (groupNameField) {
        groupNameField.addEventListener('keydown', function (e) {
            const groupCode = document.getElementById('GroupCode')?.value;
            if (!groupCode || groupCode === '') {
                document.getElementById('loanInfo').style.display = 'none';
            }
        });
    }

    if (loanAmountField) {
        loanAmountField.addEventListener('keyup', function () {
            updateLoanFee();
            updateEwi();
        });
    }

    if (processingFeeRateField) {
        processingFeeRateField.addEventListener('keyup', updateLoanFee);
    }

    if (insuranceRateField) {
        insuranceRateField.addEventListener('keyup', updateLoanFee);
    }

    if (interestRateField) {
        interestRateField.addEventListener('keyup', updateEwi);
    }

    if (tenureField) {
        tenureField.addEventListener('keyup', updateEwi);
    }
}

// Check if group exists and can take loan
function checkGroup() {
    const groupCode = document.getElementById('GroupCode').value.trim();
    const errorElement = document.getElementById('errGroupName');

    if (groupCode === '') {
        errorElement.textContent = 'Select a valid group';
        document.getElementById('loanInfo').style.display = 'none';
        return;
    }

    errorElement.textContent = '';

    fetch(`/Loan/CheckGroup/${groupCode}`)
        .then(response => response.text())
        .then(result => {
            if (result === 'Not found') {
                errorElement.textContent = 'Group not found';
                document.getElementById('loanInfo').style.display = 'none';
            } else if (result === 'Success') {
                document.getElementById('loanInfo').style.display = 'block';
                document.getElementById('LoanPurpose').focus();
            } else {
                errorElement.textContent = 'Loan already exists for one or more members';
                document.getElementById('loanInfo').style.display = 'none';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            errorElement.textContent = 'Error checking group';
        });
}

// Update loan fees (processing fee and insurance)
function updateLoanFee() {
    try {
        const loanAmount = Number(document.getElementById('LoanAmount').value) || 0;
        const processingFeeRate = Number(document.getElementById('ProcessingFeeRate').value) || 0;
        const insuranceRate = Number(document.getElementById('InsuranceRate').value) || 0;

        const processingFee = Math.round(loanAmount * processingFeeRate / 100);
        const insurance = Math.round(loanAmount * insuranceRate / 100);

        document.getElementById('ProcessingFee').value = processingFee;
        document.getElementById('Insurance').value = insurance;
    } catch (e) {
        console.error('Error updating loan fee:', e);
    }
}

// Calculate and update EWI (Equal Weekly Installment)
function updateEwi() {
    try {
        const loanAmount = Number(document.getElementById('LoanAmount').value) || 0;
        const tenure = Number(document.getElementById('Tenure').value) || 0;
        const interestRate = Number(document.getElementById('InterestRate').value) || 0;

        if (loanAmount <= 0 || tenure <= 0 || interestRate <= 0) {
            return;
        }

        const weeklyRate = interestRate / 5200; // Convert annual rate to weekly
        const factor = Math.pow(1 + weeklyRate, tenure);
        const ewi = Math.round(loanAmount * weeklyRate * factor / (factor - 1));
        const repaymentAmount = ewi * tenure;

        document.getElementById('Ewi').value = ewi;
        document.getElementById('RepaymentAmount').value = repaymentAmount;
    } catch (e) {
        console.error('Error updating EWI:', e);
    }
}

// Calculate interest rate to achieve target EWI
function calculateInterestRate() {
    try {
        const loanAmount = Number(document.getElementById('LoanAmount').value) || 0;
        const tenure = Number(document.getElementById('Tenure').value) || 0;

        if (loanAmount <= 0 || tenure <= 0) {
            alert('Please enter loan amount and tenure first');
            return;
        }

        const targetEwi = prompt('Enter Target Weekly Due Amount', '');
        if (!targetEwi || targetEwi === '') return;

        const ewi = Number(targetEwi);
        if (isNaN(ewi) || ewi <= 0) {
            alert('Invalid amount');
            return;
        }

        let interestRate = 40.0;
        let currentEwi = 0.0;
        let weeklyRate = 0.0;
        let factor = 0.0;

        // Iterate to find the interest rate
        while (currentEwi < ewi && interestRate < 200) {
            interestRate += 0.01;
            weeklyRate = interestRate / 5200;
            factor = Math.pow(1 + weeklyRate, tenure);
            currentEwi = loanAmount * weeklyRate * factor / (factor - 1);
        }

        document.getElementById('InterestRate').value = Math.round(interestRate * 100) / 100;
        updateEwi();
    } catch (e) {
        console.error('Error calculating interest rate:', e);
        alert('Error calculating interest rate');
    }
}

// Validate form before submission
function validateOnSubmit() {
    const repaymentAmount = document.getElementById('RepaymentAmount').value;
    const errorElement = document.getElementById('ErrEwi');

    if (repaymentAmount === '' || repaymentAmount === 'NaN' || repaymentAmount === '0') {
        errorElement.textContent = 'Fill all data and calculate EWI';
        return false;
    }

    errorElement.textContent = '';
    return true;
}