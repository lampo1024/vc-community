﻿//angular.module('platformWebApp')
//.controller('platformWebApp.settingsListController', ['$injector', '$scope', 'platformWebApp.settings', 'platformWebApp.bladeNavigationService',
//function ($injector, $scope, settings, bladeNavigationService) {
//    var selectedNode = null;

//    $scope.blade.refresh = function () {
//        $scope.blade.isLoading = true;

//        settings.getModules({}, function (results) {
//            $scope.blade.isLoading = false;

//            $scope.currentEntities = results;

//            if (selectedNode != null) {
//                //select the node in the new list
//                angular.forEach(results, function (node) {
//                    if (selectedNode.id === node.id) {
//                        selectedNode = node;
//                    }
//                });
//            }
//        }, function (error) {
//bladeNavigationService.setError('Error ' + error.status, $scope.blade);
//});
//    };

//    $scope.selectNode = function (node) {
//        selectedNode = node;
//        $scope.selectedNodeId = selectedNode.id;

//        var newBlade = {
//            id: 'settingsSection',
//            title: selectedNode.title + ' settings',
//            moduleId: selectedNode.id,
//            controller: 'platformWebApp.settingsDetailController',
//            template: 'Scripts/app/settings/blades/settings-detail.tpl.html'
//        };

//        bladeNavigationService.showBlade(newBlade, $scope.blade);
//    };

//    $scope.blade.headIcon = 'fa fa-wrench';


//    // actions on load
//    $scope.blade.refresh();
//}]);