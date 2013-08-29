function CustomerData() {
    var self = this;
    self.customerId = ko.observable();
    self.name = ko.observable();
    self.birthDate = ko.observable();
    self.height = ko.observable();
}

function CustomerList() {
    var self = this;
    self.sortField = undefined;
    self.sortType = undefined;
    self.sortDirection = 1;
    var customers = ko.observableArray();

    self.getCustomers = function () {
        return customers;
    }

    self.add = function (customer) {
        customers.push(customer);
    }

    function convert(value) {
        if (self.sortType == 'Date') {
            return $.datepicker.parseDate('m/d/yy', value);
        } else if (self.sortType == 'Number') {
            return parseFloat(value);
        } else {
            return value.toString();
        }
    }

    function compare(left, right) {
        if (left < right) {
            return -1;
        } else if (left > right) {
            return 1;
        } else {
            return 0;
        }
    }

    function byFieldComparer(left, right) {
        var leftField = left[self.sortField]();
        var rightField = right[self.sortField]();
        leftField = convert(leftField);
        rightField = convert(rightField);
        var result = compare(leftField, rightField);
        result *= self.sortDirection;
        return result;
    }

    self.sort = function () {
        if (self.sortField) {
            customers.sort(byFieldComparer);
        }
    }

    self.hasItems = ko.computed(function () {
        return customers().length > 0;
    });

    self.clear = function () {
        customers.removeAll();
    }

    function findIndex(predicate) {
        for (var index = 0; index != customers().length; ++index) {
            var customer = customers()[index];
            if (predicate(customer)) {
                return index;
            }
        }
        return -1;
    }

    self.find = function (customerId) {
        var index = findIndex(function (customer) { return customer.customerId() == customerId });
        return index == -1 ? null : customers()[index];
    }

    self.remove = function (customerId) {
        var index = findIndex(function (customer) { return customer.customerId() == customerId });
        if (index != -1) {
            customers.splice(index, 1);
        }
    }
}

var customerList = new CustomerList();

function ViewModel() {
    var self = this;

    self.list = customerList;
    self.selected = ko.observable(new CustomerData());
    self.setSelected = function (customer) {
        self.selected(customer);
    }
};
var viewModel = new ViewModel();

function loadData() {
    $.ajax({
        url: buildUrl('Knockout', 'Load'),
        type: 'post',
        dataType: 'json',
        cache: false
    })
    .done(function (data, textStatus, handler) {
        loadCustomers(data);
    })
    .fail(function (handler, textStatus, errorThrown) {
        handleError(errorThrown);
    });
}

function loadCustomers(data) {
    customerList.clear();
    for (var index = 0; index != data.length; ++index) {
        var customer = data[index];
        var customerData = new CustomerData();
        customerData.customerId(customer.CustomerId);
        customerData.name(customer.Name);
        customerData.birthDate(customer.BirthDate);
        customerData.height(customer.Height);
        customerList.add(customerData);
    }
    customerList.sort();
}

function handleError(errorMessage) {
    showInformationModal('Errors', errorMessage);
}

function sortByField() {
    var columnHeader = $(this);
    customerList.sortField = columnHeader.attr('data-field-name');
    var direction = columnHeader.attr('data-field-direction') || "-1";
    customerList.sortDirection = -parseInt(direction, 10);
    var type = columnHeader.attr('data-field-type') || 'String';
    customerList.sortType = type;
    columnHeader.attr('data-field-direction', customerList.sortDirection.toString());
    customerList.sort();
    return false;
}

function showCreateModal(event) {
    event.preventDefault();
    $('#modal-customer-title').text('Create Customer');
    $('#btn-create-customer-save').text('Create');
    $('#form-create-customer').attr('data-mode', 'create');
    viewModel.setSelected(new CustomerData());
    $('#modal-create-customer').modal('show');
}

function showEditModal(event) {
    event.preventDefault();
    $('#modal-customer-title').text('Edit Customer');
    $('#btn-create-customer-save').text('Update');
    $('#form-create-customer').attr('data-mode', 'edit');
    var customerId = $(this).attr('data-customer-id');
    var current = customerList.find(customerId);
    if (current != null) {
        viewModel.setSelected(current);
        $('#modal-create-customer').modal('show');
    }
}

function showDeleteModal(event) {
    event.preventDefault();
    var customerId = $(this).attr('data-customer-id');
    var current = customerList.find(customerId);
    if (current != null) {
        viewModel.setSelected(current);
        $('#modal-delete-customer').modal('show');
    }
}

function submitCustomerChanges(event) {
    event.preventDefault();
    var form = $('#form-create-customer');
    var isValid = form.parsley('isValid');
    if (isValid) {
        var mode = form.attr('data-mode');
        if (mode == 'create') {
            createCustomer(form);
        } else {
            updateCustomer(form);
        }
    }    
}

function submitDeleteCustomer(event) {
    event.preventDefault();
    var customerId = $(this).attr('data-customer-id');
    $.ajax({
        url: buildUrl('Knockout', 'Delete'),
        type: 'delete',
        data: { customer_id: customerId },
        dataType: 'text'
    })
    .success(function (data, textStatus, handler) {
        $('#modal-delete-customer').modal('hide');
        customerList.remove(customerId);
    })
    .fail(function (handler, textStatus, errorThrown) {
        handleError(errorThrown);
    });
}

function createCustomer(form) {
    var data = $(form).serialize();
    $.ajax({
        url: buildUrl('Knockout', 'Create'),
        type: 'post',
        data: data,
        dataType: 'json'
    })
    .success(function (data, textStatus, handler) {
        viewModel.selected().customerId(data.CustomerId);
        viewModel.selected().name(data.Name);
        viewModel.selected().birthDate(data.BirthDate);
        viewModel.selected().height(data.Height);
        customerList.add(viewModel.selected());
        customerList.sort();

        $('#modal-create-customer').modal('hide');
    })
    .fail(function (handler, textStatus, errorThrown) {
        handleError(errorThrown);
    });
}

function updateCustomer(form) {
    var data = $(form).serialize();
    $.ajax({
        url: buildUrl('Knockout', 'Edit'),
        type: 'put',
        data: data,
        dataType: 'text'
    })
    .success(function (data, textStatus, handler) {
        $('#modal-create-customer').modal('hide');
    })
    .fail(function (handler, textStatus, errorThrown) {
        handleError(errorThrown);
    });
}

$(function () {
    $('#dpBirthDate').datepicker({ dateFormat: 'm/d/yy' });

    $('#form-create-customer').parsley({
        successClass: 'success',
        errorClass: 'error',
        errors: {
            classHandler: function (el) {
                return $(el).closest('.control-group');
            },
            errorsWrapper: $('<div />').prop('outerHTML'),
            errorElem: $('<span />').addClass('help-inline').prop('outerHTML')
        }
    });

    ko.applyBindings(viewModel);
    loadData();

    $('#btn-create-customer').click(showCreateModal);
    $('#btn-create-customer-save').click(submitCustomerChanges);
    $('#btn-delete-customer-save').click(submitDeleteCustomer);
    $(document).on('click', '.sort-field', sortByField);
    $(document).on('click', '.lnk-edit-customer', showEditModal);
    $(document).on('click', '.lnk-delete-customer', showDeleteModal);
});