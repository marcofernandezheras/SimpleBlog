'use strict';

angular.module('BlogAngularApp')
    .factory('UserFactory', ['$rootScope', '$http', 'Urls', function ($rootScope, $http, Urls) {
        var dataFactory = {};

        dataFactory.register = function (user) {
            return $http.post(Urls.register, user);
        };

        dataFactory.login = function (user) {
            return $http.post(Urls.login, user);
        };

        dataFactory.logout = function () {
            return $http.delete(Urls.login + $rootScope.token.token);
        };


        dataFactory.changePassword = function (token, password) {
            return $http.put(Urls.password + token, password);
        }
        return dataFactory;
    }]);
