// financial-report.js

document.addEventListener('DOMContentLoaded', function () {
    loadFinancialYears();
});

async function loadFinancialYears() {
    const fySelector = document.getElementById('fySelector');

    try {
        const response = await fetch('/Report/GetFinancialYears');
        const data = await response.json();

        if (data.success && data.financialYears.length > 0) {
            // Clear loading option
            fySelector.innerHTML = '';

            // Add financial years to dropdown
            data.financialYears.forEach(fy => {
                const option = document.createElement('option');
                option.value = fy.fyCode;
                option.textContent = fy.fyDisplay;
                option.dataset.startDate = fy.startDate;
                option.dataset.endDate = fy.endDate;
                fySelector.appendChild(option);
            });

            // Load report for first option (Overall)
            loadFinancialReport();
        } else {
            fySelector.innerHTML = '<option value="">No data available</option>';
        }
    } catch (error) {
        console.error('Error loading financial years:', error);
        fySelector.innerHTML = '<option value="">Error loading years</option>';
    }
}

async function loadFinancialReport() {
    const fySelector = document.getElementById('fySelector');
    const selectedFY = fySelector.value;

    if (!selectedFY) {
        return;
    }

    // Show loading
    document.getElementById('loadingMessage').style.display = 'block';
    document.getElementById('reportContent').style.display = 'none';
    document.getElementById('noDataMessage').style.display = 'none';

    try {
        const response = await fetch(`/Report/GetFinancialReport/${selectedFY}`);
        const data = await response.json();

        document.getElementById('loadingMessage').style.display = 'none';

        if (data.success && data.report) {
            displayReport(data.report);
            document.getElementById('reportContent').style.display = 'block';
        } else {
            document.getElementById('noDataMessage').style.display = 'block';
        }
    } catch (error) {
        console.error('Error loading financial report:', error);
        document.getElementById('loadingMessage').style.display = 'none';
        document.getElementById('noDataMessage').style.display = 'block';
    }
}

function displayReport(report) {
    // Summary Cards
    document.getElementById('totalLoansCount').textContent = report.totalLoansCount;
    document.getElementById('totalLoansDisbursed').textContent = formatCurrency(report.totalLoansDisbursed);
    document.getElementById('totalEWIReceived').textContent = formatCurrency(report.totalEWIReceived);
    document.getElementById('futureEWIExpected').textContent = formatCurrency(report.futureEWIExpected);
    document.getElementById('badLoanPending').textContent = formatCurrency(report.badLoanPendingAmount);

    // Loan Disbursement Section
    document.getElementById('loansCountDetail').textContent = report.totalLoansCount;
    document.getElementById('loanAmountDetail').textContent = formatCurrency(report.totalLoansDisbursed);

    // Collections Section
    document.getElementById('ewiReceivedDetail').textContent = formatCurrency(report.totalEWIReceived);
    document.getElementById('futureEWIDetail').textContent = formatCurrency(report.futureEWIExpected);

    // Losses & Discounts Section
    document.getElementById('badLoanPendingDetail').textContent = formatCurrency(report.badLoanPendingAmount);
    document.getElementById('badLoanDiscountDetail').textContent = formatCurrency(report.badLoanDiscountProvided);
    document.getElementById('preClosureDiscountDetail').textContent = formatCurrency(report.preClosureDiscountProvided);

    // Interest Income Section
    document.getElementById('actualInterest').textContent = formatCurrency(report.actualInterestIncome);
    document.getElementById('anticipatedInterest').textContent = formatCurrency(report.anticipatedInterestIncome);
    document.getElementById('totalInterest').textContent = formatCurrency(report.totalInterestIncome);

    // Net Profit Section
    document.getElementById('netActualInterest').textContent = formatCurrency(report.actualInterestIncome);
    document.getElementById('netBadLoanPending').textContent = '(' + formatCurrency(report.badLoanPendingAmount) + ')';
    document.getElementById('netBadLoanDiscount').textContent = '(' + formatCurrency(report.badLoanDiscountProvided) + ')';

    const netProfit = report.netProfit;
    const netProfitElement = document.getElementById('netProfit');
    netProfitElement.textContent = formatCurrency(Math.abs(netProfit));

    // Color code net profit
    if (netProfit >= 0) {
        netProfitElement.style.color = '#28a745';
        netProfitElement.innerHTML = '<strong>' + formatCurrency(netProfit) + '</strong>';
    } else {
        netProfitElement.style.color = '#dc3545';
        netProfitElement.innerHTML = '<strong>(' + formatCurrency(Math.abs(netProfit)) + ')</strong>';
    }
}

function formatCurrency(amount) {
    if (amount === null || amount === undefined) {
        return '₹0';
    }

    const num = parseFloat(amount);
    if (isNaN(num)) {
        return '₹0';
    }

    return '₹' + num.toLocaleString('en-IN', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });
}