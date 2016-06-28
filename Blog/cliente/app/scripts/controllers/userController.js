'use strict';

angular.module('BlogAngularApp')
    .controller('UserCtrl', ['$scope',
        '$rootScope', '$location', '$timeout', '$document', '$http',
        'UserFactory', 'fileUpload', 'Urls',
        function ($scope, $rootScope, $location, $timeout, $document, $http, UserFactory, fileUpload, Urls) {
            var ctrl = this;

            $scope.showPasswordChange = false;

            $scope.changePassword = function () {
                if (ctrl.changePasswordForm.$valid && $scope.newPass.passwordConfirm) {
                    UserFactory.changePassword($rootScope.token.token, $scope.newPass).then(function () {
                        $rootScope.loged = false;
                        $document.find('#modal').modal('show');
                        $timeout(function () {
                            $document.find('#modal').modal('hide');
                            $timeout(function () {
                                $location.path('/');
                            }, 200);
                        }, 1000);
                    }).catch(function (response) {
                        if (response.data.ModelState && response.data.ModelState.token) {
                            $rootScope.loged = false;
                            alert('No se ha podido cambiar la contraseña: Sesion expirada');
                        } else {
                            alert('No se ha podido cambiar la contraseña: Error interno');
                        }
                    });
                }
            }

            $scope.uploadFile = function () {
                var file = $scope.myFile;

                console.log('file is ');
                console.dir(file);
                fileUpload.uploadFileToUrl(file, Urls.avatar + $rootScope.token.token);
            };

            $scope.prepareFile = function (files) {
                $scope.myFile = files[0];
            }
         }
    ]);
