'use strict';

angular.module('BlogAngularApp')
    .factory('CategoryData', function () {
        return {
            categories: []
        }
    })
    .factory('CategoryFactory', ['$http', 'Urls', 'CategoryData', function ($http, Urls, CategoryData) {

        var dataFactory = {};

        dataFactory.all = function () {
            return $http.get(Urls.categories);
        };

        dataFactory.create = function (token, category) {
            return $http.post(Urls.categories + token, category);
        }

        return dataFactory;
    }]);
