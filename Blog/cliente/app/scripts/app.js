/**
 * @ngdoc overview
 * @name BlogAngularApp
 * @description
 * # BlogAngularApp
 *
 * Main module of the application.
 */
angular
    .module('BlogAngularApp', [
        'ngAnimate',
        'ngCookies',
        'ngMessages',
        'ngRoute',
        'ngSanitize',
        'wiz.markdown'
    ]).config(function ($routeProvider) {
        'use strict';
        $routeProvider
            .when('/', {
                templateUrl: 'views/main.html',
                controller: 'MainCtrl',
                controllerAs: 'main',
                resolve: {
                    check: function ($rootScope) {
                        $rootScope.currentTab = 'index';
                    }
                }
            })
            .when('/createPost', {
                templateUrl: 'views/createPost.html',
                controller: 'CreatePostCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($location, $rootScope) {
                        if (!$rootScope.loged) {
                            $location.path('/');
                        }
                        $rootScope.currentTab = 'newPost';
                    }
                }
            })
            .when('/register', {
                templateUrl: 'views/register.html',
                controller: 'RegisterCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($location, $rootScope) {
                        if ($rootScope.loged) {
                            $location.path('/');
                        }
                        $rootScope.currentTab = 'register';
                    }
                }
            })
            .when('/post/:id', {
                templateUrl: 'views/post.html',
                controller: 'PostCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($rootScope) {
                        $rootScope.currentTab = '';
                    }
                }

            })
            .when('/editPost/:id', {
                templateUrl: 'views/createPost.html',
                controller: 'EditPostCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($location, $rootScope) {
                        if (!$rootScope.loged) {
                            $location.path('/');
                        }
                        $rootScope.currentTab = '';
                    }
                }
            })
            .when('/user', {
                templateUrl: 'views/user.html',
                controller: 'UserCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($location, $rootScope) {
                        if (!$rootScope.loged) {
                            $location.path('/');
                        }
                        $rootScope.currentTab = 'user';
                    }
                }
            })
            .when('/chat', {
                templateUrl: 'views/chat.html',
                controller: 'ChatCtrl',
                controllerAs: 'ctrl',
                resolve: {
                    check: function ($location, $rootScope) {
                        if (!$rootScope.loged) {
                            $location.path('/');
                        }
                        $rootScope.currentTab = 'chat';
                    }
                }
            })
            .otherwise({
                redirectTo: '/'
            });
    });
