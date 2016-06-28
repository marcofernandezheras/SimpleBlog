'use strict';

angular.module('BlogAngularApp')
    .factory('PostFactory', ['$http', 'Urls', function ($http, Urls) {

        var dataFactory = {};

        dataFactory.currentPost = null;

        dataFactory.range = function (startPage, amount) {
            return $http.get(Urls.post + startPage + '/' + amount);
        }

        dataFactory.get = function (id) {
            return $http.get(Urls.post + id);
        }

        dataFactory.create = function (token, post) {
            return $http.post(Urls.post + token, post);
        }

        dataFactory.edit = function (token, post) {
            return $http.put(Urls.post + token + '/' + post.id, post);
        }

        dataFactory.delete = function (token, post) {
            return $http.delete(Urls.post + token + '/' + post.id);
        }

        return dataFactory;
    }]);
