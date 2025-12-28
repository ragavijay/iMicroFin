// wwwroot/js/loan-form.js

document.addEventListener('DOMContentLoaded', function () {
    // Initialize date pickers
    initializeDatePickers();

    // Add event listeners
    setupEventListeners();

    // Check if editing existing loan
    const loanCode = document.getElementById('LoanId')?.value;
    if (loanCode) {
        document.getElementById('loanInfo').style.display = 'block';
    }
});

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
    const memberCodeField = document.getElementById('MemberCode');
    const loanAmountField = document.getElementById('LoanAmount');
    const processingFeeRateField = document.getElementById('ProcessingFeeRate');
    const insuranceRateField = document.getElementById('InsuranceRate');
    const interestRateField = document.getElementById('InterestRate');
    const tenureField = document.getElementById('Tenure');

    if (memberCodeField) {
        memberCodeField.addEventListener('keydown', function (e) {
            const loanCode = document.getElementById('LoanId')?.value;
            if (!loanCode || loanCode === '') {
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

// Check if member exists and can take loan
function checkMember() {
    const memberCode = document.getElementById('MemberCode').value.trim();
    const errorElement = document.getElementById('errMemberCode');

    if (memberCode === '') {
        errorElement.textContent = 'Enter Member Id';
        document.getElementById('loanInfo').style.display = 'none';
        return;
    }

    errorElement.textContent = '';

    fetch(`/Loan/CheckMember/${memberCode}`)
        .then(response => response.text())
        .then(memberName => {
            if (memberName === 'Not found') {
                errorElement.textContent = 'Member not found';
                document.getElementById('loanInfo').style.display = 'none';
            } else if (memberName === 'Loan exists') {
                errorElement.textContent = 'Active loan already exists for this member';
                document.getElementById('loanInfo').style.display = 'none';
            } else {
                document.getElementById('MemberName').value = memberName;
                document.getElementById('loanInfo').style.display = 'block';
                document.getElementById('LoanPurpose').focus();
            }
        })
        .catch(error => {
            console.error('Error:', error);
            errorElement.textContent = 'Error checking member';
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

        // ✔ Correct weekly interest rate
        const weeklyRate = interestRate / 100 / 52;

        // Amortization formula
        const factor = Math.pow(1 + weeklyRate, tenure);

        // ✔ Correct EWI calculation
        const ewi = Math.round(loanAmount * weeklyRate * factor / (factor - 1));

        // Total repayment
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

// Show loan repayment schedule
function showSchedule() {
    const repaymentAmount = document.getElementById('RepaymentAmount').value;
    const errorElement = document.getElementById('ErrEwi');

    if (repaymentAmount === '' || repaymentAmount === 'NaN' || repaymentAmount === '0') {
        errorElement.textContent = 'Fill all data';
        return;
    }

    errorElement.textContent = '';

    // Populate schedule header
    const loanId = document.getElementById('LoanId').value || 'N/A';
    const memberCode = document.getElementById('MemberCode').value;
    const memberName = document.getElementById('MemberName').value;
    const loanAmount = document.getElementById('LoanAmount').value;
    const interestRate = document.getElementById('InterestRate').value;
    const ewi = Number(document.getElementById('Ewi').value);

    document.getElementById('scheduleLoanId').textContent = loanId;
    document.getElementById('scheduleMemberId').textContent = memberCode;
    document.getElementById('scheduleMemberName').textContent = memberName;
    document.getElementById('scheduleLoanAmount').textContent = loanAmount;
    document.getElementById('scheduleInterestRate').textContent = interestRate + '%';
    document.getElementById('scheduleEwi').textContent = ewi;

    // Generate schedule table
    const tenure = Number(document.getElementById('Tenure').value);
    const rate = Number(interestRate);
    let balance = Number(loanAmount);

    const scheduleTable = document.getElementById('scheduleTable');
    const tbody = scheduleTable.querySelector('tbody');

    // Clear existing rows
    tbody.innerHTML = '';

    for (let i = 1; i <= tenure; i++) {

        let interest = balance * rate / 100 / 52;
        let principal = ewi - interest;

        // ✔ FIX: Adjust last installment so balance becomes exactly 0
        if (i === tenure) {
            principal = balance;                    // pay remaining balance
            interest = balance * rate / 100 / 52;   // recompute accurate interest
        }

        const row = tbody.insertRow();
        row.insertCell(0).textContent = i;
        row.insertCell(1).textContent = balance.toFixed(2);
        row.insertCell(2).textContent = principal.toFixed(2);
        row.insertCell(3).textContent = interest.toFixed(2);

        balance = balance - principal;

        // ✔ Prevent negative balance due to floating-point errors
        if (balance < 0.01) balance = 0;

        row.insertCell(4).textContent = balance.toFixed(2);
    }

    // Show schedule, hide form
    document.getElementById('LoanForm').style.display = 'none';
    document.getElementById('schedule').style.display = 'block';
}


// Close schedule and return to form
function closeSchedule() {
    document.getElementById('LoanForm').style.display = 'block';
    document.getElementById('schedule').style.display = 'none';
}

// Print schedule
function printSchedule() {
    window.print();
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