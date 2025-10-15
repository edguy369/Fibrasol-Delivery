/**
 * Fibrasol Delivery - Site-wide JavaScript Utilities
 * Common helper functions used across the application
 */

const FibrasolUtils = (function() {
    'use strict';

    // ============================================
    // AJAX & API Helpers
    // ============================================

    const api = {
        async get(url) {
            const response = await fetch(url);
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return await response.json();
        },

        async post(url, data) {
            return await this.request(url, 'POST', data);
        },

        async put(url, data) {
            return await this.request(url, 'PUT', data);
        },

        async delete(url, data = {}) {
            return await this.request(url, 'DELETE', data);
        },

        async request(url, method, data) {
            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return await response.json();
        },

        ajax(options) {
            return $.ajax({
                contentType: 'application/json',
                ...options
            });
        }
    };

    // ============================================
    // UI Helpers
    // ============================================

    const ui = {
        showSuccessMessage(message) {
            const html = `
                <div class="alert alert-success alert-dismissible fade show" role="alert">
                    <i class="bi bi-check-circle me-2"></i>${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>`;
            $('.page-header').after(html);
            setTimeout(() => $('.alert-success').fadeOut(), 3000);
        },

        showErrorMessage(message) {
            const html = `
                <div class="alert alert-warning alert-dismissible fade show" role="alert">
                    <i class="bi bi-exclamation-triangle me-2"></i>${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>`;
            $('.page-header').after(html);
            setTimeout(() => $('.alert-warning').fadeOut(), 5000);
        },

        setButtonLoading(button, loading = true) {
            if (loading) {
                button.dataset.originalText = button.innerHTML;
                button.disabled = true;
                button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Guardando...';
            } else {
                button.disabled = false;
                button.innerHTML = button.dataset.originalText || 'Guardar';
            }
        },

        showModal(modalId) {
            const modal = new bootstrap.Modal(document.getElementById(modalId));
            modal.show();
        },

        hideModal(modalId) {
            const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
            if (modal) modal.hide();
        },

        updateCounter(elementId, count, animated = true) {
            const el = document.getElementById(elementId);
            if (!el) return;

            if (animated) {
                el.style.opacity = '0.5';
                setTimeout(() => {
                    el.textContent = count.toLocaleString('es-GT');
                    el.style.opacity = '1';
                }, 200);
            } else {
                el.textContent = count.toLocaleString('es-GT');
            }
        }
    };

    // ============================================
    // DataTables Helpers
    // ============================================

    const datatables = {
        getSpanishConfig() {
            return {
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
                },
                pageLength: 10,
                responsive: true
            };
        },

        createActionButtons(id, name, config) {
            const { editFn, deleteFn } = config;
            const escapedName = name.replace(/'/g, "\\'");
            return `
                <button type="button" class="btn btn-edit btn-sm me-1" onclick="${editFn}(${id}, '${escapedName}')">
                    <i class="bi bi-pencil"></i>
                </button>
                <button type="button" class="btn btn-delete btn-sm" onclick="${deleteFn}(${id})">
                    <i class="bi bi-trash"></i>
                </button>`;
        },

        reloadTable(tableId) {
            $(`#${tableId}`).DataTable().ajax.reload();
        }
    };

    // ============================================
    // Form Helpers
    // ============================================

    const forms = {
        lockForm() {
            $('input, select, textarea, button').prop('disabled', true);
        },

        unlockForm() {
            $('input, select, textarea, button').prop('disabled', false);
        },

        resetModal(fields) {
            fields.forEach(field => {
                const el = document.getElementById(field.id);
                if (el) el.value = field.value || '';
            });
        },

        validateRequired(fields) {
            for (const field of fields) {
                const value = document.getElementById(field)?.value.trim();
                if (!value) {
                    alert(`Por favor, complete el campo: ${field}`);
                    return false;
                }
            }
            return true;
        },

        getFormData(fields) {
            const data = {};
            fields.forEach(field => {
                const el = document.getElementById(field.id);
                if (el) data[field.name] = el.value.trim();
            });
            return data;
        }
    };

    // ============================================
    // CRUD Operations Helper
    // ============================================

    const crud = {
        async create(config) {
            const { url, data, tableId, successMessage, errorMessage } = config;

            try {
                await api.post(url, data);
                ui.hideModal(config.modalId);
                ui.showSuccessMessage(successMessage || 'Creado correctamente');
                if (tableId) datatables.reloadTable(tableId);
            } catch (error) {
                console.error('Error creating:', error);
                ui.showErrorMessage(errorMessage || 'Error al crear. Inténtelo de nuevo.');
            }
        },

        async update(config) {
            const { url, id, data, tableId, successMessage, errorMessage } = config;

            try {
                await api.put(`${url}/${id}`, data);
                ui.hideModal(config.modalId);
                ui.showSuccessMessage(successMessage || 'Actualizado correctamente');
                if (tableId) datatables.reloadTable(tableId);
            } catch (error) {
                console.error('Error updating:', error);
                ui.showErrorMessage(errorMessage || 'Error al actualizar. Inténtelo de nuevo.');
            }
        },

        async delete(config) {
            const { url, id, tableId, confirmMessage, successMessage, errorMessage } = config;

            if (!confirm(confirmMessage || '¿Está seguro de eliminar este elemento?')) return;

            try {
                await api.delete(`${url}/${id}`);
                ui.showSuccessMessage(successMessage || 'Eliminado correctamente');
                if (tableId) datatables.reloadTable(tableId);
            } catch (error) {
                console.error('Error deleting:', error);
                ui.showErrorMessage(errorMessage || 'Error al eliminar. Inténtelo de nuevo.');
            }
        }
    };

    // ============================================
    // Date Helpers
    // ============================================

    const dates = {
        formatDate(date, locale = 'es-GT') {
            return new Date(date).toLocaleDateString(locale);
        },

        formatDateTime(date, locale = 'es-GT') {
            const d = new Date(date);
            return d.toLocaleDateString(locale) + ' ' +
                   d.toLocaleTimeString(locale, {hour: '2-digit', minute: '2-digit'});
        },

        getFirstDayOfMonth() {
            const now = new Date();
            return new Date(now.getFullYear(), now.getMonth(), 1);
        },

        getLastDayOfMonth() {
            const now = new Date();
            return new Date(now.getFullYear(), now.getMonth() + 1, 0);
        },

        toInputFormat(date) {
            return date.toISOString().split('T')[0];
        }
    };

    // ============================================
    // Currency Helpers
    // ============================================

    const currency = {
        format(amount, locale = 'es-GT') {
            return 'Q ' + parseFloat(amount).toLocaleString(locale, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        },

        formatWithSymbol(amount, symbol = 'Q', locale = 'es-GT') {
            return `${symbol} ${parseFloat(amount).toLocaleString(locale, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            })}`;
        },

        parse(value) {
            return parseFloat(value) || 0;
        }
    };

    // ============================================
    // Utility Functions
    // ============================================

    const utils = {
        debounce(func, wait) {
            let timeout;
            return function(...args) {
                clearTimeout(timeout);
                timeout = setTimeout(() => func.apply(this, args), wait);
            };
        },

        getUrlParam(param) {
            const urlParams = new URLSearchParams(window.location.search);
            return urlParams.get(param);
        },

        getIdFromUrl(index = -1) {
            const parts = window.location.pathname.split('/');
            return parts[parts.length + index];
        },

        escapeHtml(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
    };

    // ============================================
    // Export Public API
    // ============================================

    return {
        api,
        ui,
        datatables,
        forms,
        crud,
        dates,
        currency,
        utils
    };
})();

// Make it globally available
window.FibrasolUtils = FibrasolUtils;

// Legacy function support for backward compatibility
function LockForm() {
    FibrasolUtils.forms.lockForm();
}

function UnlockForm() {
    FibrasolUtils.forms.unlockForm();
}