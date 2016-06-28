angular.module('BlogAngularApp')
    .controller('PostCtrl', ['$scope', '$rootScope', '$routeParams', '$location', 'PostFactory', 'CommentFactory',
                             function ($scope, $rootScope, $routeParams, $location, PostFactory, CommentFactory) {
            'use strict';
            $scope.comments = [];
            $scope.loadingComments = true;

            function getComments() {
                CommentFactory.all($scope.post).then(function (response) {
                    $scope.comments = response.data;
                }).finally(function () {
                    $scope.loadingComments = false;
                });
            }

            if (PostFactory.currentPost) {
                $scope.post = PostFactory.currentPost;
                getComments();
            } else {
                PostFactory.get($routeParams.id).then(function (response) {
                    $scope.post = response.data;
                    getComments();
                }).catch(function () {
                    $location.path("/");
                });
                PostFactory.currentPost = $scope.post;
            }

            $scope.addComent = function () {
                if ($scope.newComent.comment1.length) {
                    $scope.newComent.idPost = $routeParams.id;
                    CommentFactory.comment($rootScope.token.token, $scope.newComent).then(function (response) {
                        $scope.comments.push(response.data);
                        $scope.newComent = {};
                    }).catch(function (response) {
                        if (response.data.ModelState && response.data.ModelState.token) {
                            $rootScope.loged = false;
                            alert('No se ha podido crear el post: Sesion expirada');
                        } else {
                            alert('No se ha podido crear el post: Error interno');
                        }
                    });
                }
            }

            var commentToDelete = null;
            $scope.prepareDelete = function (comment) {
                commentToDelete = comment;
            };

            $scope.deleteComment = function () {
                if (commentToDelete) {
                    CommentFactory.delete($rootScope.token.token, commentToDelete).then(function () {
                        for (var i = 0; i < $scope.comments.length; i++) {
                            if ($scope.comments[i].id === commentToDelete.id) {
                                $scope.comments.splice(i, 1);
                            }
                        }

                    }).catch(function (response) {
                        if (response.data.ModelState && response.data.ModelState.token) {
                            $rootScope.loged = false;
                            alert('No se ha borrar el comentario: Sesion expirada');
                        } else {
                            alert('No se a podido borrar el comentario: Error interno');
                        }
                    }).finally(function () {
                        commentToDelete = null;
                    });
                }
            }

            $scope.commentToEdit = {};
            $scope.newComentcontent = '';
            $scope.prepareEdit = function (comment) {
                $scope.commentToEdit = comment;
                $scope.newComentcontent = comment.comment1;
            };
            $scope.editComment = function () {
                if ($scope.commentToEdit.comment1 && $scope.newComentcontent.length) {
                    $scope.commentToEdit.comment1 = $scope.newComentcontent;
                    CommentFactory.edit($rootScope.token.token, $scope.commentToEdit).then(function () {

                    }).catch(function (response) {
                        if (response.data.ModelState && response.data.ModelState.token) {
                            $rootScope.loged = false;
                            alert('No se ha editar el comentario: Sesion expirada');
                        } else {
                            alert('No se a podido editar el comentario: Error interno');
                        }
                    }).finally(function () {
                        $scope.commentToEdit = {};
                    });
                } else {
                    $scope.commentToEdit.comment1 = oldComent;
                }
            }

             }]);
