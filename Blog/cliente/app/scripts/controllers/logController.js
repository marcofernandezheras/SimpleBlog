angular.module('BlogAngularApp')
    .controller('LogController', ['$scope', '$location', '$rootScope', 'UserFactory',
                                  function ($scope, $location, $rootScope, UserFactory) {
            'use strict';
            $scope.user = {
                password: '',
                username: ''
            };
            $scope.login = function () {
                UserFactory.login($scope.user).then(function (response) {
                    $rootScope.loged = response.data.valid;
                    $rootScope.token = response.data;

                    $scope.user.password = '';
                    if ($location.path() === '/register') {
                        $location.path('/');
                    }
                }).catch(function () {
                    $rootScope.loged = false;
                });
            };
            $scope.logout = function () {
                UserFactory.logout().then(function (response) {
                    $rootScope.loged = false;
                    $rootScope.token = {};
                    $scope.user = {};
                    $location.path('/');
                    $.connection.hub.stop();
                    $rootScope.chatStarted = false;
                });
            };
        }
        ]);
