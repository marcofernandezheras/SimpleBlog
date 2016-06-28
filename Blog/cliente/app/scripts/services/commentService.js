'use strict';

angular.module('BlogAngularApp')
    .factory('CommentFactory', ['$http', 'Urls', function ($http, Urls) {

        var dataFactory = {};

        dataFactory.all = function (post) {
            return $http.get(Urls.comments + post.id);
        }

        dataFactory.comment = function (token, comment) {
            return $http.post(Urls.comments + token, comment);
        }

        dataFactory.edit = function (token, comment) {
            return $http.put(Urls.comments + token + '/' + comment.id, comment);
        }

        dataFactory.delete = function (token, comment) {
            return $http.delete(Urls.comments + token + '/' + comment.id);
        }

        return dataFactory;
    }]);
