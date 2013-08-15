var application = angular.module('Customer', []);

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
    $scope.customerModal = {
        Title: '',
        Action: '',
        Mode: ''
    };

    resetCustomer($scope.selected);

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

    function setDirty(form, isDirty) {
        var elements = [form.txtName, form.dpBirthDate, form.txtHeight]
        for (var index = 0; index != elements.length; ++index) {
            elements[index].$dirty = isDirty;
        }
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

    $scope.showCreateModal = function (selected, modal, form) {
        modal.Title = 'Create Customer';
        modal.Action = 'Create';
        modal.Mode = 'Create';

        resetCustomer(selected);
        setDirty(form, false);
        $('#modal-create-customer').modal('show');
    };

    $scope.showEditModal = function (modal, selected, customer, form) {
        modal.Title = 'Edit Customer';
        modal.Action = 'Save';
        modal.Mode = 'Edit'

        copyCustomer(customer, selected);
        setDirty(form, false);
        $('#modal-create-customer').modal('show');
    };

    $scope.submit = function (modal, selected, form) {
        setDirty(form, true);
        if (form.$invalid) {
            return;
        }
        if (modal.Mode == 'Create') {
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

    $scope.getStatusClass = function (control) {
        return {
            error: control.$dirty && control.$invalid,
            success: !control.$invalid
        };
    };

    $scope.showErrorMessage = function (control, check) {
        return control.$dirty && control.$error[check];
    };
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
        }).done(function (data, textStatus, handler) {
            success(data);
        }).fail(function (handler, textStatus, errorThrown) {
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
        }).done(function (data, textStatus, handler) {
            success(data);
        }).fail(function (handler, textStatus, errorThrown) {
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
        }).done(function (data, textStatus, handler) {
            success(data);
        }).fail(function (handler, textStatus, errorThrown) {
            error(errorThrown);
        });
    };

    this.remove = function (customer, success, error) {
        var url = buildUrl('api', 'Customers', customer.CustomerId);
        $.ajax({
            url: url,
            type: 'DELETE'
        }).done(function (data, textStatus, handler) {
            success(data);
        }).fail(function (handler, textStatus, errorThrown) {
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