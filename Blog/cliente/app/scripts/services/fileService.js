angular.module('BlogAngularApp')
    .directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;

                element.bind('change', function () {
                    scope.$apply(function () {
                        model
                        Setter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }])
    .directive('upload', ['$http', function ($http) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                onDrop: '&'
            },
            template: '<div class="asset-upload">Drag here to upload</div>',
            link: function (scope, element, attrs, ngModel) {

                element.on('dragover', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                });
                element.on('dragenter', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                });

                element.on('drop', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (e.originalEvent.dataTransfer) {
                        if (e.originalEvent.dataTransfer.files.length > 0) {
                            scope.onDrop({
                                files: e.originalEvent.dataTransfer.files
                            });

                            var fd = new FormData();
                            fd.append('file', e.originalEvent.dataTransfer.files[0]);

                            //var status = new createStatusbar(obj); //Using this we can set progress.
                            //status.setFileNameSize(files[i].name, files[i].size);
                            //sendFileToServer(fd, status);

                            var img = document.createElement("img");

                            img.classList.add("obj");
                            img.file = e.originalEvent.dataTransfer.files[0];

                            var reader = new FileReader();
                            reader.onload = (function (aImg) {
                                return function (e) {
                                    aImg.src = e.target.result;
                                    element[0].innerHTML = '';
                                    element[0].appendChild(aImg);
                                };
                            })(img);
                            reader.readAsDataURL(e.originalEvent.dataTransfer.files[0]);
                        }
                    }
                    return false;
                });

            }
        };
    }])
    .service('fileUpload', ['$http', function ($http) {
        this.uploadFileToUrl = function (file, uploadUrl) {
            var fd = new FormData();
            fd.append('file', file);

            $http.post(uploadUrl, fd, {
                transformRequest: angular.identity,
                headers: {
                    'Content-Type': undefined
                }
            })

            .success(function () {})

            .error(function () {});
        }
    }]);
