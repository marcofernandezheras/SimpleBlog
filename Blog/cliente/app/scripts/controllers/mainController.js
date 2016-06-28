'use strict';

/**
 * @ngdoc function
 * @name BlogAngularApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the BlogAngularApp
 */
angular.module('BlogAngularApp')
    .controller('MainCtrl', ['$scope', '$rootScope', 'PostFactory', 'PagerFactory', function ($scope, $rootScope, PostFactory, PagerFactory) {
        $scope.loadingPosts = true;
        $scope.posts = [];
        $scope.pager = {};
        var currentPage = 1;


        $scope.goToPost = function (post) {
            PostFactory.currentPost = post;
        };

        $scope.setPage = function (page) {
            if (page < 1 || page > $scope.pager.totalPages) {
                return;
            }

            currentPage = page;
            $scope.loadingPosts = true;
            PostFactory.range(currentPage - 1, 10).then(function (response) {
                $scope.posts = response.data.posts;
                $scope.pager = PagerFactory.GetPager(response.data.total, page);
            }).catch(function () {
                $scope.posts = [];
            }).finally(function () {
                $scope.loadingPosts = false;
            });
        };

        $scope.deletePost = function (post) {
            if (confirm('¿Realmente desea borrar este post?')) {
                PostFactory.delete($rootScope.token.token, post).then(function () {
                    for (var i = 0; i < allPost.length; i++) {
                        if (allPost[i].id === post.id) {
                            allPost.splice(i, 1);
                            $scope.setPage(currentPage);
                        }
                    }
                }).catch(function () {
                    if (response.data.ModelState && response.data.ModelState.token) {
                        $rootScope.loged = false;
                        alert('No ha sido posible borrar el post: Sesion expirada')
                    } else {
                        alert('No ha sido posible borrar el post: Error interno')
                    }
                })
            }
        }

        $scope.setPage(1);
  }]);
