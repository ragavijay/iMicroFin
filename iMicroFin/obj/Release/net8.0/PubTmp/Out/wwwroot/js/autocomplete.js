// wwwroot/js/autocomplete.js - Modern Autocomplete Implementation

class AutoComplete {
    constructor(config) {
        this.inputElement = document.getElementById(config.inputId);
        this.hiddenElement = document.getElementById(config.hiddenId);
        this.serviceUrl = config.serviceUrl;
        this.minChars = config.minChars || 1;
        this.debounceTime = config.debounceTime || 300;
        this.onSelect = config.onSelect || null;

        this.currentRequest = null;
        this.debounceTimer = null;
        this.currentFocus = -1;
        this.listContainer = null;

        this.init();
    }

    init() {
        // Create autocomplete container
        this.createContainer();

        // Add event listeners
        this.inputElement.addEventListener('input', (e) => this.handleInput(e));
        this.inputElement.addEventListener('keydown', (e) => this.handleKeydown(e));
        this.inputElement.addEventListener('focus', () => {
            const value = this.inputElement.value.trim();
            // Show all results when clicking on empty field OR show filtered results
            if (value.length === 0) {
                this.fetchSuggestions(''); // Fetch all centers
            } else if (value.length >= this.minChars) {
                this.fetchSuggestions(value);
            }
        });

        // Close on click outside
        document.addEventListener('click', (e) => {
            if (e.target !== this.inputElement) {
                this.closeList();
            }
        });
    }

    createContainer() {
        // Remove existing container if any
        const existing = this.inputElement.parentElement.querySelector('.autocomplete-dropdown');
        if (existing) {
            existing.remove();
        }

        // Create new container
        this.listContainer = document.createElement('div');
        this.listContainer.className = 'autocomplete-dropdown';
        this.listContainer.style.display = 'none';

        // Insert after input or its wrapper
        const wrapper = this.inputElement.closest('.input-group') || this.inputElement.parentElement;
        wrapper.style.position = 'relative';
        wrapper.appendChild(this.listContainer);
    }

    handleInput(e) {
        const value = e.target.value.trim();

        // Clear hidden field when input changes
        if (this.hiddenElement) {
            this.hiddenElement.value = '';
        }

        // Clear previous timer
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }

        if (value.length < this.minChars) {
            this.closeList();
            return;
        }

        // Debounce the API call
        this.debounceTimer = setTimeout(() => {
            this.fetchSuggestions(value);
        }, this.debounceTime);
    }

    handleKeydown(e) {
        const items = this.listContainer.querySelectorAll('.autocomplete-item');

        if (e.keyCode === 40) { // Down arrow
            e.preventDefault();
            this.currentFocus++;
            this.setActive(items);
        } else if (e.keyCode === 38) { // Up arrow
            e.preventDefault();
            this.currentFocus--;
            this.setActive(items);
        } else if (e.keyCode === 13) { // Enter
            e.preventDefault();
            if (this.currentFocus > -1 && items[this.currentFocus]) {
                items[this.currentFocus].click();
            }
        } else if (e.keyCode === 27) { // Escape
            this.closeList();
        }
    }

    setActive(items) {
        if (!items || items.length === 0) return;

        // Remove active class from all
        items.forEach(item => item.classList.remove('autocomplete-active'));

        // Handle wrap around
        if (this.currentFocus >= items.length) this.currentFocus = 0;
        if (this.currentFocus < 0) this.currentFocus = items.length - 1;

        // Add active class to current
        items[this.currentFocus].classList.add('autocomplete-active');

        // Scroll into view if needed
        items[this.currentFocus].scrollIntoView({ block: 'nearest' });
    }

    async fetchSuggestions(searchText) {
        // Cancel previous request
        if (this.currentRequest) {
            this.currentRequest.abort();
        }

        const controller = new AbortController();
        this.currentRequest = controller;

        try {
            const response = await fetch(`${this.serviceUrl}/${encodeURIComponent(searchText)}`, {
                signal: controller.signal
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const data = await response.json();
            this.displaySuggestions(data.centers || data);
        } catch (error) {
            if (error.name !== 'AbortError') {
                console.error('Autocomplete error:', error);
                this.showError('Failed to load suggestions');
            }
        }
    }

    displaySuggestions(items) {
        this.closeList();
        this.currentFocus = -1;

        if (!items || items.length === 0) {
            this.showNoResults();
            return;
        }

        items.forEach(item => {
            const itemDiv = document.createElement('div');
            itemDiv.className = 'autocomplete-item';

            // Store data in dataset
            Object.keys(item).forEach(key => {
                itemDiv.dataset[key.toLowerCase()] = item[key];
            });

            // Create display content
            const displayText = this.formatItem(item);
            itemDiv.innerHTML = displayText;

            // Add click handler
            itemDiv.addEventListener('click', () => this.selectItem(item));

            this.listContainer.appendChild(itemDiv);
        });

        this.listContainer.style.display = 'block';
    }

    formatItem(item) {
        // Override this method for custom formatting
        // Default: show centerName (centerCode)
        const code = item.centerCode || item.CenterCode || '';
        const name = item.centerName || item.CenterName || '';

        return `<strong>${name}</strong> <span class="autocomplete-code">(${code})</span>`;
    }

    selectItem(item) {
        const code = item.centerCode || item.CenterCode || '';
        const name = item.centerName || item.CenterName || '';

        this.inputElement.value = name;

        if (this.hiddenElement) {
            this.hiddenElement.value = code;
        }

        this.closeList();

        // Trigger custom callback if provided
        if (this.onSelect) {
            this.onSelect(item);
        }

        // Trigger change event
        this.inputElement.dispatchEvent(new Event('change', { bubbles: true }));
    }

    showNoResults() {
        this.listContainer.innerHTML = '<div class="autocomplete-no-results">No results found</div>';
        this.listContainer.style.display = 'block';
    }

    showError(message) {
        this.listContainer.innerHTML = `<div class="autocomplete-error">${message}</div>`;
        this.listContainer.style.display = 'block';
    }

    closeList() {
        this.listContainer.innerHTML = '';
        this.listContainer.style.display = 'none';
        this.currentFocus = -1;
    }

    destroy() {
        if (this.listContainer) {
            this.listContainer.remove();
        }
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }
    }
}

// Multi-field AutoComplete (for 3+ fields)
class MultiFieldAutoComplete extends AutoComplete {
    constructor(config) {
        super(config);
        this.hiddenElements = config.hiddenIds.map(id => document.getElementById(id));
        this.fieldNames = config.fieldNames;
    }

    handleInput(e) {
        const value = e.target.value.trim();

        // Clear all hidden fields when input changes
        this.hiddenElements.forEach(el => {
            if (el) el.value = '';
        });

        // Clear previous timer
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }

        if (value.length < this.minChars) {
            this.closeList();
            return;
        }

        // Debounce the API call
        this.debounceTimer = setTimeout(() => {
            this.fetchSuggestions(value);
        }, this.debounceTime);
    }

    formatItem(item) {
        // Override for custom formatting
        // Default: show main field with additional info
        const displayText = item[this.fieldNames[this.fieldNames.length - 1]] || '';
        const code = item[this.fieldNames[0]] || '';

        return `<strong>${displayText}</strong> <span class="autocomplete-code">(${code})</span>`;
    }

    selectItem(item) {
        // Set visible input (last field in fieldNames)
        const displayField = this.fieldNames[this.fieldNames.length - 1];
        this.inputElement.value = item[displayField] || '';

        // Set all hidden fields
        this.fieldNames.forEach((fieldName, index) => {
            if (this.hiddenElements[index]) {
                this.hiddenElements[index].value = item[fieldName] || '';
            }
        });

        this.closeList();

        if (this.onSelect) {
            this.onSelect(item);
        }

        this.inputElement.dispatchEvent(new Event('change', { bubbles: true }));
    }

    async fetchSuggestions(searchText) {
        // Cancel previous request
        if (this.currentRequest) {
            this.currentRequest.abort();
        }

        const controller = new AbortController();
        this.currentRequest = controller;

        try {
            const response = await fetch(`${this.serviceUrl}/${encodeURIComponent(searchText)}`, {
                signal: controller.signal
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const data = await response.json();
            // Handle both direct array and wrapped response
            const items = data.groups || data.centers || data;
            this.displaySuggestions(items);
        } catch (error) {
            if (error.name !== 'AbortError') {
                console.error('Autocomplete error:', error);
                this.showError('Failed to load suggestions');
            }
        }
    }
}

// Convenience functions for backward compatibility
function createAutoComplete(serviceUrl, hiddenId, inputId) {
    return new AutoComplete({
        serviceUrl: serviceUrl,
        hiddenId: hiddenId,
        inputId: inputId
    });
}

function createAutoComplete3(serviceUrl, hiddenId1, hiddenId2, hiddenId3, inputId) {
    return new MultiFieldAutoComplete({
        serviceUrl: serviceUrl,
        hiddenIds: [hiddenId1, hiddenId2, hiddenId3],
        inputId: inputId,
        fieldNames: ['field1', 'field2', 'field3', 'displayField']
    });
}