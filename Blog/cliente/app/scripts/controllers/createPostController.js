'use strict';

angular.module('BlogAngularApp')
    .controller('CreatePostCtrl', ['$scope', '$rootScope', '$location', 'PostFactory', 'CategoryData', 'CategoryFactory',
                                   function ($scope, $rootScope, $location, PostFactory, CategoryData, CategoryFactory) {
            var ctrl = this;
            $scope.categories = [];
            $scope.post = {};
            $scope.userName = $rootScope.token.userName;
            if (!CategoryData.categories.length) {
                CategoryFactory.all().then(function (response) {
                    $scope.categories = response.data;
                    CategoryData.categories = response.data;
                });
            } else {
                $scope.categories = CategoryData.categories;
            }

            $scope.createPost = function () {
                if (ctrl.newPostForm.$valid) {
                    PostFactory.create($rootScope.token.token, $scope.post).then(function (response) {
                        PostFactory.currentPost = response.data;
                        PostFactory.currentPost.UserName = $rootScope.token.userName;
                        $location.path('/post/' + response.data.id);
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
