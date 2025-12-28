// wwwroot/js/member-form.js (Optimized Version with API)

let groupAutoComplete;

document.addEventListener('DOMContentLoaded', function () {
    // Initialize date pickers
    initializeDatePickers();

    // Initialize group autocomplete
    initializeGroupAutoComplete();

    // Initialize bank lookup
    getBank();

    // Load existing images if in edit mode
    loadExistingImagesViaAPI();
});

// Initialize Flatpickr date pickers
// Initialize Flatpickr date pickers
function initializeDatePickers() {
    const dobField = document.getElementById("DOB");
    const nomineeDobField = document.getElementById("NomineeDOB");

    // Get existing value
    const dobValue = dobField.value.trim();
    const nomineeDobValue = nomineeDobField.value.trim();

    // Parse date from multiple formats
    function parseDate(dateStr) {
        if (!dateStr || dateStr === '') return null;

        // Format 1: DD/MM/YYYY
        if (dateStr.includes('/')) {
            const parts = dateStr.split('/');
            if (parts.length === 3) {
                const day = parseInt(parts[0], 10);
                const month = parseInt(parts[1], 10) - 1;
                const year = parseInt(parts[2], 10);
                return new Date(year, month, day);
            }
        }

        // Format 2: "01-Oct-80 12:00:00 AM" or similar
        if (dateStr.includes('-')) {
            const parsed = new Date(dateStr);
            if (!isNaN(parsed.getTime())) {
                return parsed;
            }
        }

        return null;
    }

    // Set defaults ONLY if empty
    let dobDate = parseDate(dobValue);
    let nomineeDobDate = parseDate(nomineeDobValue);

    if (!dobDate) {
        dobDate = new Date(1980, 0, 1);
    } else {
        // Format the date properly back to DD/MM/YYYY
        const day = String(dobDate.getDate()).padStart(2, '0');
        const month = String(dobDate.getMonth() + 1).padStart(2, '0');
        const year = dobDate.getFullYear();
        dobField.value = `${day}/${month}/${year}`;
    }

    if (!nomineeDobDate) {
        nomineeDobDate = new Date(1980, 0, 1);
    } else {
        // Format the date properly back to DD/MM/YYYY
        const day = String(nomineeDobDate.getDate()).padStart(2, '0');
        const month = String(nomineeDobDate.getMonth() + 1).padStart(2, '0');
        const year = nomineeDobDate.getFullYear();
        nomineeDobField.value = `${day}/${month}/${year}`;
    }

    // Member DOB
    flatpickr("#DOB", {
        dateFormat: "d/m/Y",
        maxDate: "today",
        defaultDate: dobDate,
        allowInput: true,
        onChange: function (selectedDates, dateStr, instance) {
            document.getElementById("errDob").innerHTML = "";
        }
    });

    // Nominee DOB
    flatpickr("#NomineeDOB", {
        dateFormat: "d/m/Y",
        maxDate: "today",
        defaultDate: nomineeDobDate,
        allowInput: true,
        onChange: function (selectedDates, dateStr, instance) {
            document.getElementById("errNomineeDob").innerHTML = "";
        }
    });
}

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
            document.getElementById('GroupCodeDisplay').value = group.groupCode || '';
        }
    });

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

    const existingGroupCode = document.getElementById('GroupCode').value;
    if (existingGroupCode) {
        document.getElementById('GroupCodeDisplay').value = existingGroupCode;
    }
}

// Load existing images via API (Better performance)
function loadExistingImagesViaAPI() {
    const memberCode = document.getElementById('MemberCode').value;

    if (memberCode) {
        fetch(`/Member/GetMemberImages/${memberCode}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    if (data.photoExists) {
                        showExistingImage(
                            data.photoUrl,
                            'photoPreviewContainer',
                            'photoPreview'
                        );
                    }
                    if (data.aadharExists) {
                        showExistingImage(
                            data.aadharUrl,
                            'aadharPreviewContainer',
                            'aadharPreview'
                        );
                    }
                }
            })
            .catch(error => console.error('Error loading images:', error));
    }
}

// Show existing image
function showExistingImage(imageUrl, containerID, imgID) {
    const container = document.getElementById(containerID);
    const preview = document.getElementById(imgID);

    preview.src = imageUrl + '?t=' + new Date().getTime();
    container.style.display = 'block';
}

// Image preview function for new uploads
function previewImage(input, containerID, imgID) {
    const container = document.getElementById(containerID);
    const preview = document.getElementById(imgID);

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            container.style.display = 'block';
        };

        reader.readAsDataURL(input.files[0]);
    }
}

// Clear image function
function clearImage(inputID, containerID) {
    const input = document.getElementById(inputID);
    const container = document.getElementById(containerID);

    input.value = '';
    container.style.display = 'none';
}

// Form validation
function validateForm() {
    if (document.getElementById("AccountNumber").value != document.getElementById("RAccountNumber").value) {
        document.getElementById("ErrAccountNumber").innerHTML = "Account Number Mismatch";
        return false;
    } else {
        document.getElementById("ErrAccountNumber").innerHTML = "";
    }

    if (document.getElementById("MemberAadharNumber").value != document.getElementById("RMemberAadharNumber").value) {
        document.getElementById("ErrRMemberAadharNumber").innerHTML = "Aadhar Number Mismatch";
        return false;
    } else {
        document.getElementById("ErrRMemberAadharNumber").innerHTML = "";
    }

    document.getElementById("PAN").value = document.getElementById("PAN").value.toUpperCase();
    document.getElementById("IFSC").value = document.getElementById("IFSC").value.toUpperCase();
    document.getElementById("MemberName").value = document.getElementById("MemberName").value.toUpperCase();
    document.getElementById("FName").value = document.getElementById("FName").value.toUpperCase();
    document.getElementById("HName").value = document.getElementById("HName").value.toUpperCase();
    document.getElementById("NomineeName").value = document.getElementById("NomineeName").value.toUpperCase();

    return true;
}

// Get bank details from IFSC
function getBank() {
    var ifsc = document.getElementById("IFSC").value;
    var bankName = document.getElementById("BankName");
    var bankBranch = document.getElementById("BankBranch");

    if (ifsc == '') {
        bankName.value = '';
        bankBranch.value = '';
        return;
    }

    var ajaxReq = new XMLHttpRequest();
    ajaxReq.onreadystatechange = function () {
        if (this.readyState == 1) {
            bankName.value = "Loading....";
            bankBranch.value = "Loading....";
        } else if (this.readyState == 4) {
            if (this.status == 200) {
                var bankInfo = JSON.parse(this.responseText);
                bankName.value = bankInfo.BANK;
                bankBranch.value = bankInfo.BRANCH;
            } else {
                bankName.value = "NA";
                bankBranch.value = "NA";
            }
        }
    };
    ajaxReq.open("GET", "https://ifsc.razorpay.com/" + ifsc, true);
    ajaxReq.send();
}