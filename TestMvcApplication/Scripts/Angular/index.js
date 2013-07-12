﻿var application = angular.module('Customer', []);

application.filter('date', function () {
    return function (date) {
        return moment(date).format('MM/DD/YYYY');
    };
});

application.directive('datepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, control) {
            $(function () {
                element.datepicker({
                    dateFormat: 'm/d/yy',
                    onSelect: function (date) {
                        control.$setViewValue(date);
                        scope.$apply();
                    }
                });
            });
        }
    };
});

function CustomerController($scope, repository) {
    $scope.selected = {};
    $scope.customerList = [];
    $scope.orderByField = 'Name';
    $scope.CustomerModal = {
        Title: '',
        Action: '',
        Mode: '',
        Validation: {}
    };

    resetCustomer($scope.selected);
    resetValidation($scope.CustomerModal);

    repository.load(function (data) {
        $scope.customerList = data;
        $scope.$apply();
    }, handleError);

    function resetCustomer(customer) {
        customer.CustomerId = null;
        customer.Name = null;
        customer.BirthDate = null;
        customer.Height = null;
    }

    function resetValidation(customerModal) {
        customerModal.Validation = {
            Name: {
                Status: '',
                Message: ''
            },
            BirthDate: {
                Status: '',
                Message: ''
            },
            Height: {
                Status: '',
                Message: ''
            }
        };
    }

    function copyCustomer(from, to) {
        to.CustomerId = from.CustomerId;
        to.Name = from.Name;
        to.BirthDate = from.BirthDate;
        to.Height = from.Height;
    }

    function find(customerId) {
        for (var i = 0; i != $scope.customerList.length; ++i) {
            var customer = $scope.customerList[i];
            if (customer.CustomerId == customerId) {
                return i;
            }
        }
        return -1;
    }

    $scope.showCreateModal = function (selected, customerModal) {
        customerModal.Title = 'Create Customer';
        customerModal.Action = 'Create';
        customerModal.Mode = 'Create';

        resetValidation(customerModal);
        resetCustomer(selected);
        $('#modal-create-customer').modal('show');
    };

    $scope.showEditModal = function (customerModal, selected, customer) {
        customerModal.Title = 'Edit Customer';
        customerModal.Action = 'Save';
        customerModal.Mode = 'Edit'

        resetValidation(customerModal);
        copyCustomer(customer, selected);
        $('#modal-create-customer').modal('show');
    };

    $scope.submit = function (customerModal, selected) {
        if (!isValid()) {
            return;
        }
        if (customerModal.Mode == 'Create') {
            repository.create(selected, function (data) {
                $scope.customerList.push(data);
                resetCustomer(selected);
                $scope.$apply();
                $('#modal-create-customer').modal('hide');
            }, handleError);
        } else {
            repository.update(selected, function (data) {
                var index = find(selected.CustomerId);
                var customer = $scope.customerList[index];
                customer.Name = selected.Name;
                customer.BirthDate = selected.BirthDate;
                customer.Height = selected.Height;
                $scope.$apply();
                $('#modal-create-customer').modal('hide');
            }, handleError);
        }
    };

    $scope.deleteCustomer = function (customer) {
        showConfirmationModal({
            content: 'Are you sure you want to delete ' + customer.Name + '?',
            text: 'Delete',
            style: 'btn-danger',
            success: function () {
                repository.remove(customer, function () {
                    var index = find(customer.CustomerId);
                    $scope.customerList.splice(index, 1);
                    $scope.$apply();
                }, handleError);
            }
        });
    };

    $scope.isNameValid = function (modal, selected) {
        if (!selected.Name) {
            modal.Validation.Name.Status = 'error';
            modal.Validation.Name.Message = 'You must provide a name.';
            return false;
        }
        modal.Validation.Name.Status = 'success';
        modal.Validation.Name.Message = '';
        return true;
    };

    $scope.isBirthDateValid = function (modal, selected) {
        if (!selected.BirthDate) {
            modal.Validation.BirthDate.Status = 'error';
            modal.Validation.BirthDate.Message = 'You must provide a birth date.';
            return false;
        }
        if (!moment(selected.BirthDate).isValid()) {
            modal.Validation.BirthDate.Status = 'error';
            modal.Validation.BirthDate.Message = 'You must provide a valid birth date.';
            return false;
        }
        modal.Validation.BirthDate.Status = 'success';
        modal.Validation.BirthDate.Message = '';
        return true;
    };

    $scope.isHeightValid = function (modal, selected) {
        if (!selected.Height) {
            modal.Validation.Height.Status = 'error';
            modal.Validation.Height.Message = 'You must provide a height.';
            return false;
        }
        var height = parseInt(selected.Height, 10);
        if (isNaN(height)) {
            modal.Validation.Height.Status = 'error';
            modal.Validation.Height.Message = 'You must provide a valid height.';
            return false;
        }
        modal.Validation.Height.Status = 'success';
        modal.Validation.Height.Message = '';
        return true;
    };

    function isValid() {
        var isNameValid = $scope.isNameValid($scope.CustomerModal, $scope.selected);
        var isBirthDateValid = $scope.isBirthDateValid($scope.CustomerModal, $scope.selected);
        var isHeightValid = $scope.isHeightValid($scope.CustomerModal, $scope.selected);
        return isNameValid && isBirthDateValid && isHeightValid;
    }
}

function CustomerRepository(baseUrl) {

    function buildUrl() {
        var array = $.makeArray(arguments);
        var path = baseUrl + array.join('/');
        return path;
    }

    this.load = function (success, error) {
        var url = buildUrl('api', 'Customers');
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'JSON',
            cache: false
        })
        .done(function (data, textStatus, handler) {
            success(data);
        })
        .fail(function (handler, textStatus, errorThrown) {
            error(errorThrown);
        });
    };

    this.create = function (customer, success, error) {
        var url = buildUrl('api', 'Customers');
        var data = JSON.stringify(customer);
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            contentType: 'application/json'
        })
        .done(function (data, textStatus, handler) {
            success(data);
        })
        .fail(function (handler, textStatus, errorThrown) {
            error(errorThrown);
        });
    };

    this.update = function (customer, success, error) {
        var url = buildUrl('api', 'Customers');
        var data = JSON.stringify(customer);
        $.ajax({
            url: url,
            type: 'PUT',
            data: data,
            contentType: 'application/json'
        })
        .done(function (data, textStatus, handler) {
            success(data);
        })
        .fail(function (handler, textStatus, errorThrown) {
            error(errorThrown);
        });
    };

    this.remove = function (customer, success, error) {
        var url = buildUrl('api', 'Customers', customer.CustomerId);
        $.ajax({
            url: url,
            type: 'DELETE'
        })
        .done(function (data, textStatus, handler) {
            success(data);
        })
        .fail(function (handler, textStatus, errorThrown) {
            error(errorThrown);
        });
    };
}

application.factory('repository', [function () {
    var baseUrl = $('#base-url').attr('data-base-url');
    return new CustomerRepository(baseUrl);
}]);

function handleError(errorMessage) {
    showInformationModal('Errors', errorMessage);
}