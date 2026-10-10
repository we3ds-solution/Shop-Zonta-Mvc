// Toaster Notifications
class ToasterManager {
    constructor() {
        this.createContainer();
    }

    createContainer() {
        if (!document.querySelector('.toast-container')) {
            const container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }
    }

    show(message, type = 'success', duration = 5000) {
        const toastId = 'toast-' + Date.now();
        const iconMap = {
            success: 'fas fa-check-circle',
            error: 'fas fa-exclamation-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };

        const titleMap = {
            success: 'Success',
            error: 'Error',
            warning: 'Warning',
            info: 'Information'
        };

        const toastHtml = `
            <div id="${toastId}" class="toast toast-custom toast-${type}" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-header">
                    <i class="${iconMap[type]} me-2"></i>
                    <strong class="me-auto">${titleMap[type]}</strong>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>
        `;

        const container = document.querySelector('.toast-container');
        container.insertAdjacentHTML('beforeend', toastHtml);

        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: duration });
        toast.show();

        // Remove toast element after it's hidden
        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });
    }

    success(message, duration = 5000) {
        this.show(message, 'success', duration);
    }

    error(message, duration = 7000) {
        this.show(message, 'error', duration);
    }

    warning(message, duration = 6000) {
        this.show(message, 'warning', duration);
    }

    info(message, duration = 5000) {
        this.show(message, 'info', duration);
    }
}

// Global toaster instance
window.toaster = new ToasterManager();

// Search and Filter Manager
class SearchFilterManager {
    constructor(options = {}) {
        this.searchUrl = options.searchUrl || window.location.pathname;
        this.searchInput = options.searchInput || '#searchInput';
        this.filterSelects = options.filterSelects || [];
        this.resultsContainer = options.resultsContainer || '#resultsContainer';
        this.loadingSpinner = options.loadingSpinner || '#loadingSpinner';
        this.resultsInfo = options.resultsInfo || '#resultsInfo';
        this.paginationContainer = options.paginationContainer || '#paginationContainer';
        
        this.currentPage = 1;
        this.pageSize = 10;
        this.totalItems = 0;
        this.totalPages = 0;
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadUrlParams();
    }

    bindEvents() {
        // Search input with debounce
        let searchTimeout;
        $(this.searchInput).on('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.currentPage = 1;
                this.performSearch();
            }, 500);
        });

        // Filter selects
        this.filterSelects.forEach(selector => {
            $(selector).on('change', () => {
                this.currentPage = 1;
                this.performSearch();
            });
        });

        // Clear button
        $('#clearFilters').on('click', () => {
            this.clearFilters();
        });

        // Page size change
        $('#pageSize').on('change', () => {
            this.pageSize = parseInt($('#pageSize').val());
            this.currentPage = 1;
            this.performSearch();
        });
    }

    loadUrlParams() {
        const urlParams = new URLSearchParams(window.location.search);
        
        // Load search term
        const searchTerm = urlParams.get('search');
        if (searchTerm) {
            $(this.searchInput).val(searchTerm);
        }

        // Load filters
        this.filterSelects.forEach(selector => {
            const filterName = $(selector).attr('name');
            const filterValue = urlParams.get(filterName);
            if (filterValue) {
                $(selector).val(filterValue);
            }
        });

        // Load pagination
        this.currentPage = parseInt(urlParams.get('page')) || 1;
        this.pageSize = parseInt(urlParams.get('pageSize')) || 10;
        $('#pageSize').val(this.pageSize);
    }

    performSearch() {
        const searchData = this.getSearchData();
        this.showLoading();
        this.updateUrl(searchData);

        $.ajax({
            url: this.searchUrl,
            type: 'GET',
            data: searchData,
            success: (response) => {
                this.handleSearchSuccess(response);
            },
            error: (xhr, status, error) => {
                this.handleSearchError(error);
            },
            complete: () => {
                this.hideLoading();
            }
        });
    }

    getSearchData() {
        const data = {
            search: $(this.searchInput).val(),
            page: this.currentPage,
            pageSize: this.pageSize
        };

        // Add filter values
        this.filterSelects.forEach(selector => {
            const filterName = $(selector).attr('name');
            const filterValue = $(selector).val();
            if (filterValue) {
                data[filterName] = filterValue;
            }
        });

        return data;
    }

    updateUrl(searchData) {
        const url = new URL(window.location);
        Object.keys(searchData).forEach(key => {
            if (searchData[key]) {
                url.searchParams.set(key, searchData[key]);
            } else {
                url.searchParams.delete(key);
            }
        });
        window.history.replaceState({}, '', url);
    }

    handleSearchSuccess(response) {
        if (response.html) {
            $(this.resultsContainer).html(response.html);
        }
        
        if (response.totalItems !== undefined) {
            this.totalItems = response.totalItems;
            this.totalPages = Math.ceil(this.totalItems / this.pageSize);
            this.updateResultsInfo();
            this.updatePagination();
        }
    }

    handleSearchError(error) {
        window.toaster.error('An error occurred while searching. Please try again.');
        console.error('Search error:', error);
    }

    updateResultsInfo() {
        const startItem = ((this.currentPage - 1) * this.pageSize) + 1;
        const endItem = Math.min(this.currentPage * this.pageSize, this.totalItems);
        
        const infoHtml = `
            <div class="results-info">
                <span class="results-count">${this.totalItems}</span> results found
                ${this.totalItems > 0 ? `(showing ${startItem}-${endItem})` : ''}
            </div>
        `;
        
        $(this.resultsInfo).html(infoHtml);
    }

    updatePagination() {
        if (this.totalPages <= 1) {
            $(this.paginationContainer).hide();
            return;
        }

        let paginationHtml = '<nav><ul class="pagination pagination-custom">';
        
        // Previous button
        const prevDisabled = this.currentPage === 1 ? 'disabled' : '';
        paginationHtml += `
            <li class="page-item ${prevDisabled}">
                <a class="page-link" href="#" data-page="${this.currentPage - 1}">
                    <i class="fas fa-chevron-left"></i> Previous
                </a>
            </li>
        `;

        // Page numbers
        const startPage = Math.max(1, this.currentPage - 2);
        const endPage = Math.min(this.totalPages, this.currentPage + 2);

        if (startPage > 1) {
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`;
            if (startPage > 2) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            const activeClass = i === this.currentPage ? 'active' : '';
            paginationHtml += `
                <li class="page-item ${activeClass}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `;
        }

        if (endPage < this.totalPages) {
            if (endPage < this.totalPages - 1) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="${this.totalPages}">${this.totalPages}</a></li>`;
        }

        // Next button
        const nextDisabled = this.currentPage === this.totalPages ? 'disabled' : '';
        paginationHtml += `
            <li class="page-item ${nextDisabled}">
                <a class="page-link" href="#" data-page="${this.currentPage + 1}">
                    Next <i class="fas fa-chevron-right"></i>
                </a>
            </li>
        `;

        paginationHtml += '</ul></nav>';
        
        $(this.paginationContainer).html(paginationHtml).show();

        // Bind pagination click events
        $(this.paginationContainer).find('.page-link').on('click', (e) => {
            e.preventDefault();
            const page = parseInt($(e.target).closest('.page-link').data('page'));
            if (page && page !== this.currentPage && !$(e.target).closest('.page-item').hasClass('disabled')) {
                this.currentPage = page;
                this.performSearch();
            }
        });
    }

    showLoading() {
        $(this.loadingSpinner).show();
        $(this.resultsContainer).css('opacity', '0.5');
    }

    hideLoading() {
        $(this.loadingSpinner).hide();
        $(this.resultsContainer).css('opacity', '1');
    }

    clearFilters() {
        $(this.searchInput).val('');
        this.filterSelects.forEach(selector => {
            $(selector).val('');
        });
        this.currentPage = 1;
        this.performSearch();
    }
}

// Auto-show toaster messages from TempData
$(document).ready(function() {
    // Check for TempData messages and show toasters
    if (window.tempDataMessages) {
        Object.keys(window.tempDataMessages).forEach(key => {
            const message = window.tempDataMessages[key];
            if (message) {
                switch(key.toLowerCase()) {
                    case 'successmessage':
                        window.toaster.success(message);
                        break;
                    case 'errormessage':
                        window.toaster.error(message);
                        break;
                    case 'warningmessage':
                        window.toaster.warning(message);
                        break;
                    case 'infomessage':
                        window.toaster.info(message);
                        break;
                }
            }
        });
    }
});