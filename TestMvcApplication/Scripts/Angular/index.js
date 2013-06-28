var application = angular.module('TestMVCApplication', []);

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

    function newCustomer() {
        return { 'CustomerId': null, 'Name': null, 'BirthDate': null, 'Height': null };
    }

    repository.load(function (data) {
        $scope.customerList = data;
        $scope.$apply();
    }, handleError);

    $scope.showCreateModal = function () {
        $('#modal-customer-title').text('Create Customer');
        $('#btn-create-customer-save').text('Create');
        $('#form-create-customer').attr('data-mode', 'create');
        $scope.selected = newCustomer();
        $scope.$apply();
        $('#modal-create-customer').modal('show');
    };

    $scope.addCustomer = function () {
        repository.create($scope.selected, function (data) {
            $scope.customerList.push(data);
            $scope.selected = newCustomer();
            $scope.$apply();
            $('#modal-create-customer').modal('hide');
        }, handleError);
    };
}

application.controller('CustomerController', ['$scope', 'repository', CustomerController]);

function CustomerRepository(baseUrl) {

    function buildUrl() {
        var array = $.makeArray(arguments);
        var path = baseUrl + array.join('/');
        return path;
    }

    this.load = function (success, error) {
        var url = buildUrl('Knockout', 'Load');
        $.ajax({
            url: url,
            type: 'post',
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
        var url = buildUrl('Knockout', 'Create');
        $.ajax({
            url: url,
            type: 'post',
            data: customer,
            dataType: 'json'
        })
        .success(function (data, textStatus, handler) {
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
    showModal('Errors', errorMessage);
}