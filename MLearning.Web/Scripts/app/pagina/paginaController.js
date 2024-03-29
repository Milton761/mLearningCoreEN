
mlearningApp.controller('paginaController', function ($scope,globales,lopageService) {

    $scope.pagina = 'soy controlador de pagina';
    $scope.unidadActual = null;
    $scope.seccionActual = null;
    $scope.loslide = null;
    $scope.isNew = false;
    $scope.getUnidadSeccion = function () {
        $scope.unidadActual = currentLO;
        $scope.seccionActual = currentLOSection;
        if(currentPage != null){
            $scope.currentPage = currentPage;
            try {
                $scope.loslide = JSON.parse(currentPage.content).lopage.loslide;
                currentPageTag.id = currentPageTag.tag_id;
                $scope.loslide[0].pagetag = currentPageTag;
            }
            catch (err) {
                $scope.loslide = [];
                console.log(err.message);
                alert('JSON invalido');
            }
            
        }
        else {
            $scope.isNew = true;
            $scope.currentPage = {};
            $scope.loslide = [{lotype:0}];
        }
        console.log('actual seccion::',$scope.seccionActual);

    };

    ////////////////////funciones////////////////////
    $scope.addPagina = function (pagina) {

        $scope.loslide.push(pagina);
        console.log($scope.loslide);
    }

    $scope.onUploadPageCoverSuccess = function (e) {
        $scope.$apply(function () {
            $scope.currentPage.url_img = e.response.url;
        });
    }

    $scope.addImageItem = function(slide)
    {
        slide.loitemize.loitem.push({});
    }
    /*$scope.onUploadSlideImageSuccess = function (e) {
        $scope.$apply(function () {
            $scope.slide.loimage =  e.response.url;
        });
    }*/

    //botones
    $scope.guardarPagina = function () {
        
        console.log('isNew? =>', $scope.isNew);


        if (!$scope.pageForm.$valid) {
        //    // Submit as normal
            console.log("Invalid fields in form!");
            return;
        }

        $scope.currentPage.title = $scope.loslide[0].lotitle;
        $scope.currentPage.description = $scope.loslide[0].loparagraph;
        $scope.currentPage.url_img = $scope.loslide[0].loimage;
        $scope.currentPage.tag = $scope.loslide[0].pagetag;

        delete $scope.loslide[0].pagetag;

        if ($scope.seccionActual != null)
        {
            $scope.currentPage.lo_id = $scope.seccionActual.LO_id;
            $scope.currentPage.LOsection_id = $scope.seccionActual.id;
        }
        /*else {

        }*/

        var content = {};
        content.lopage = {};
        content.lopage.loslide = $scope.loslide;

        $scope.currentPage.content = JSON.stringify(content);

        if ($scope.isNew)
        {
            $scope.currentPage.id = 0;
            lopageService.createPage($scope.currentPage).success(function (data) {
                console.log(data);
                if (data.errors == null) {
                    $scope.redireccionar(data.url);
                }
            });
        }
        else {
            lopageService.updatePage($scope.currentPage).success(function (data) {
                console.log(data);
                /*if (data.errors != null && isNew) {
                    $scope.redireccionar(data.url);
                }*/
            });
        }

        console.log('saving page:::', $scope.currentPage);
        $scope.loslide[0].pagetag = $scope.currentPage.tag;
    };


    $scope.cancelarPagina= function () {
        $scope.redireccionar('/Publisher/LODetail/'+ $scope.seccionActual.LO_id);
    };




    ////////////funcion q se ejecutan al iniciar /////////

    $scope.getUnidadSeccion();
});

mlearningApp.directive('pgEditor', function () {
    return {
        scope: true,
        templateUrl: '/Scripts/app/directives/pagina-editor.html',
        link: function (scope, element, attrs) {
            //element.text('this is the slidesEditor directive');
        },
        controller: function($scope){
            $scope.saludar = function(){
                console.log("Hola :D");
            }
        }
    };
});
    
mlearningApp.directive('pgSlide', function () {
    return {
        scope: true,
        /*scope: {
            ngModel: '='
        },*/
        templateUrl: function (element, attrs) {
            var tipo = attrs.tipo || 'pagina-slide';
            return '/Scripts/app/directives/' + tipo + '.html';
        },
        link: function (scope, element, attrs) {
            //console.log('#',attrs);
            scope.hola = function () {
                scope.saludar();
            };
            scope.onUploadSlideImageSuccess = function (e)
            {
                scope.$apply(function () {
                    scope.slide.loimage = e.response.url;
                });
            }
        }
    };
});

mlearningApp.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        //console.log(attrs);
        element.bind("keydown keypress", function (event) {

            if(event.which === 13 && scope.newItem.length>0) {
                scope.$apply(function (){
                    scope.$eval(attrs.ngEnter);
                    scope.newItem='';
                });

                event.preventDefault();
            }
        });
    };
});


