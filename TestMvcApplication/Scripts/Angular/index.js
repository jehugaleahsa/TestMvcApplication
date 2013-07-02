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
    $scope.selected = newCustomer();
    $scope.customerList = [];

    $scope.orderByField = 'Name';

    repository.load(function (data) {
        $scope.customerList = data;
        $scope.$apply();
    }, handleError);

    function newCustomer() {
        return {
            'CustomerId': null,
            'Name': null,
            'BirthDate': null,
            'Height': null 
        };
    }

    function copyCustomer(customer) {
        return {
            'CustomerId': customer.CustomerId,
            'Name': customer.Name,
            'BirthDate': customer.BirthDate,
            'Height': customer.Height 
        };
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

    $scope.showCreateModal = function () {
        $('#modal-customer-title').text('Create Customer');
        $('#btn-create-customer-save').text('Create');
        $('#form-create-customer').attr('data-mode', 'create');
        $scope.selected = newCustomer();
        $scope.$apply();
        $('#modal-create-customer').modal('show');
    };

    $scope.showEditModal = function (customer) {
        $('#modal-customer-title').text('Edit Customer');
        $('#btn-create-customer-save').text('Save');
        $('#form-create-customer').attr('data-mode', 'edit');
        $scope.selected = copyCustomer(customer);
        $scope.$apply();
        $('#modal-create-customer').modal('show');
    };

    $scope.submit = function () {
        if (!isValid()) {
            handleError('Please correct the fields in error.');
            return;
        }
        var mode = $('#form-create-customer').attr('data-mode');
        if (mode == 'create') {
            repository.create($scope.selected, function (data) {
                $scope.customerList.push(data);
                $scope.selected = newCustomer();
                $scope.$apply();
                $('#modal-create-customer').modal('hide');
            }, handleError);
        } else {
            repository.update($scope.selected, function (data) {
                var index = find($scope.selected.CustomerId);
                var customer = $scope.customerList[index];
                customer.Name = $scope.selected.Name;
                customer.BirthDate = $scope.selected.BirthDate;
                customer.Height = $scope.selected.Height;
                $scope.$apply();
                $('#modal-create-customer').modal('hide');
            }, handleError);
        }
    };

    $scope.deleteCustomer = function (customer) {
        showConfirmationModal({
            content: 'Are you sure you want to delete ' + customer.Name + '?',
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

    $scope.isNameValid = function () {
        if (!$scope.selected.Name) {
            $('#name-validation').text('You must provide a name.');
            return 'error';
        }
        $('#name-validation').text('');
        return 'success';
    };

    $scope.isBirthDateValid = function () {
        if (!$scope.selected.BirthDate) {
            $('#birth-date-validation').text('You must provide a birth date.');
            return 'error';
        }
        if (!moment($scope.selected.BirthDate).isValid()) {
            $('#birth-date-validation').text('You must provide a valid birth date.');
            return 'error';
        }
        $('#birth-date-validation').text('');
        return 'success';
    };

    $scope.isHeightValid = function () {
        if (!$scope.selected.Height) {
            $('#height-validation').text('You must provide a height');
            return 'error';
        }
        var height = parseInt($scope.selected.Height, 10);
        if (isNaN(height)) {
            $('#height-validation').text('You must provide a valid height.');
            return 'error';
        }
        $('#height-validation').text('');
        return 'success';
    };

    function isValid() {
        if ($scope.isNameValid() == 'error') { return false; }
        if ($scope.isBirthDateValid() == 'error') { return false; }
        if ($scope.isHeightValid() == 'error') { return false; }
        return true;
    }
}

application.controller('CustomerController', ['$scope', 'repository', CustomerController]);

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
            type: 'DELETE',
            contentType: 'application/json'
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