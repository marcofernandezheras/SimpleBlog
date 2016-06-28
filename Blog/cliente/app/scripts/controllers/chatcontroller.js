angular.module('BlogAngularApp')
    .controller('ChatCtrl', ['$scope', '$location', '$rootScope', '$http', '$document', 'Urls',
            function ($scope, $location, $rootScope, $http, $document, Urls) {
            'use strict';
            var ctrl = this;
            var chatBody = $('#chatBody');
            $scope.connecting = true;
            $scope.loading = true;
            $scope.status = 'Connecting...'
            $scope.chats = [];
            $scope.usersInChat = [];
            $scope.multiline = false;
            $scope.viewAvatars = true;
            $scope.notifycations = Notification.permission === 'granted';

            $scope.$watch(function () {
                return $scope.notifycations;
            }, function (newValue, oldValue) {
                if (newValue) {
                    if (Notification.permission === 'default') {
                        Notification.requestPermission();
                    }
                }
            });
            $scope.$watch(
                function () {
                    return $scope.chats.length;
                },
                function (newValue, oldValue) {
                    chatBody.scrollTop(0);
                    chatBody.scrollTop(document.getElementById('chatUL').offsetHeight + 100);
                }
            );

            var chatBody = $('#chatBody');
            var chat = $.connection.chatHub;
            chat.client.broadcastMessage = function (message) {
                $scope.chats.push(message);
                $scope.$digest();
                chatBody.scrollTop(0);
                chatBody.scrollTop(document.getElementById('chatUL').offsetHeight + 100);

                if ($scope.notifycations && message.Username !== $rootScope.token.userName) {
                    if (!('Notification' in window)) {
                        return;
                    }

                    Notification.requestPermission(function (permission) {
                        var notification = new Notification(message.Username, {
                            body: message.Message,
                            icon: 'images/avatars/' + message.Avatar,
                            dir: 'auto'
                        });
                        setTimeout(function () {
                            notification.close();
                        }, 3000);
                    });
                }
            };

            chat.client.userEnter = function (user) {
                $scope.usersInChat.push(user);
                $scope.$digest();
            }

            chat.client.userLeft = function (user) {
                for (var i = 0; i < $scope.usersInChat.length; i++) {
                    if ($scope.usersInChat[i].Username === user.Username) {
                        $scope.usersInChat.splice(i, 1);
                    }
                }
                $scope.$digest();
            }

            if (!$rootScope.chatStarted) {
                $.connection.hub.start().done(function () {
                    $scope.connecting = false;
                    $scope.status = 'Loading chat...'
                    chat.server.enterChat($rootScope.token.token);
                    $rootScope.chatStarted = true;
                    $http.get(Urls.chat + $rootScope.token.token).then(function (response) {
                        $scope.chats = response.data;
                    }).finally(function () {
                        $scope.loading = false;
                        chatBody.scrollTop(0);
                        chatBody.scrollTop(document.getElementById('chatUL').offsetHeight + 100);
                    });
                });
            }
            $scope.send = function () {
                if (ctrl.newChat.$valid) {
                    chat.server.send($rootScope.token.token, $scope.message);
                    $scope.message = '';
                }
            };
    }]);
