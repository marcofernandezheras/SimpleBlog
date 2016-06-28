angular.module('BlogAngularApp')
    .factory('Urls', function () {
        var base = 'http://localhost:55416/api/';
        //var base = 'http://mr23421p/BlogAngular/api/';
        return {
            register: base + 'User/',
            login: base + 'log/',
            post: base + 'Post/',
            comments: base + 'Comment/',
            categories: base + 'Category/',
            password: base + 'Password/',
            avatar: base + 'Avatar/',
            chat: base + 'ChatMessage/'
        };
    });
