'use strict';

angular.module('BlogAngularApp')
    /* Directiva para comparar dos inputs */
    .directive('compareTo', function () {
        return {
            require: "ngModel",
            scope: {
                otherModelValue: "=compareTo"
            },
            link: function (scope, element, attributes, ngModel) {

                ngModel.$validators.compareTo = function (modelValue) {
                    return modelValue == scope.otherModelValue;
                };

                scope.$watch("otherModelValue", function () {
                    ngModel.$validate();
                });
            }
        };
    })
    .controller('RegisterCtrl', ['$scope', 'UserFactory', function ($scope, UserFactory) {
        var ctrl = this;
        $scope.newUser = {};
        $scope.register = function () {
            if ($scope.newUser.passwordConfirm && ctrl.registerForm.$valid) {
                UserFactory.register($scope.newUser).then(function (response) {
                    alert('Registro completado con éxito');
                    $scope.newUser = {};
                    ctrl.registerForm.$setPristine();
                }).catch(function (response) {
                    alert(response.statusText);
                })
            }
        }
    }]);
