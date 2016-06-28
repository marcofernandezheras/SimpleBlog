'use strict';

angular.module('BlogAngularApp')
    .controller('EditPostCtrl', ['$scope', '$rootScope', '$routeParams',
                                 '$location', 'PostFactory', 'CategoryData', 'CategoryFactory',
                                 function ($scope, $rootScope, $routeParams, $location, PostFactory, CategoryData, CategoryFactory) {
            var ctrl = this;
            $scope.categories = [];
            $scope.userName = $rootScope.token.userName;
            if (!CategoryData.categories.length) {
                CategoryFactory.all().then(function (response) {
                    $scope.categories = response.data;
                    CategoryData.categories = response.data;
                });
            } else {
                $scope.categories = CategoryData.categories;
            }

            if (PostFactory.currentPost) {
                $scope.post = PostFactory.currentPost;
            } else {
                PostFactory.get($routeParams.id).then(function (response) {
                    $scope.post = response.data;
                }).catch(function () {
                    $location.path("/");
                });
                PostFactory.currentPost = $scope.post;
            }

            $scope.createPost = function () {
                if (ctrl.newPostForm.$valid) {
                    PostFactory.edit($rootScope.token.token, $scope.post).then(function (response) {
                        PostFactory.currentPost = response.data;
                        $location.path('/post/' + $scope.post.id);
                    }).catch(function (response) {
                        if (response.data.ModelState && response.data.ModelState.token) {
                            $rootScope.loged = false;
                            alert('No se ha podido crear el post: Sesion expirada')
                        } else {
                            alert('No se ha podido crear el post: Error interno')
                        }
                    });
                }
            }

            $scope.saveCategory = function () {
                CategoryFactory.create($rootScope.token.token, $scope.newCategory).then(function (response) {
                    CategoryData.categories.push(response.data);
                    $scope.newCategory = {};
                }).catch(function (response) {
                    if (response.data.ModelState && response.data.ModelState.token) {
                        $rootScope.loged = false;
                        alert('No se ha podido crear la categori: Sesion expirada');
                    } else {
                        alert('No se ha podido crear la categori: Error interno');
                    }
                });
            }
                                   }]);
