'use strict';

angular.module('BlogAngularApp')
    .filter('Emoji', function () {
        return function (input) {
            var emojis = input.match(/(:.*?:)/);
            if (emojis) {
                for (var i = 0; i < emojis.length; i++) {
                    input = input.split(emojis[i]).join('<img class="emoji" src="images/emojis/' + emojis[i].split(':').join('') + '.png" />');
                }
            }
            return input;
        };
    });
