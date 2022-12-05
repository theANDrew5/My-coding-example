//@ts-ignore
import { } from 'google.maps';
import { pxpGlobal } from '../../../globals/pxp';
import pickPointMapItemTemplate from "../../_templates/deliveryTypes/pickpointMapData/pickPointMapItemTemplate.html";
import { DeliveryDisplayType } from '../../controllers/DeliveryTypesSelector';
import { BaseMapManager, IPxpLatLng } from './_baseMapManager';
import { IBoundedPointData, IPointData } from '../address/BaseAddressModel';
import { Url } from "../../../utils/Url";
import { PointsDeliveryPointsMap } from '../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap';
import { MapMarkerType, PointDeliveryPointItem } from '../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem';
import { AddressSelectDeliveryPointMap } from '../types/standartDeliveryType/addressSelectDeliveryType/viewModels/AddressSelectDeliveryPointMap';
import { IAddressSuggest } from '../address/suggests/SuggestedStreetItem';
import { MarkerClusterer } from '@googlemaps/markerclusterer';


export default () => {
    ko.bindingHandlers.gmapDeliveryPoints = {
        update: (element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: PointsDeliveryPointsMap, bindingContext: unknown) => {
            GoogleMapPointsBinding.init(element, valueAccessor, allBindings, viewModel, bindingContext);
        }
    }

    ko.bindingHandlers.gmapCourierPoint = {
        update: (element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: AddressSelectDeliveryPointMap, bindingContext: unknown) => {
            GoogleMapSelectedPointBinding.init(element, valueAccessor, allBindings, viewModel, bindingContext);
        }
    }
}

class GoogleMapBindingBase extends BaseMapManager {
    private static readonly _googleMapsUrl = "https://maps.googleapis.com/maps/api/js?key={0}&language={1}";
    protected static readonly constToZoomGMapOnSelect: number = 17;
    protected static readonly constToZoomYMapCenterSuggest: number = 15;

    protected static readonly pickpointIcon = '/Content/Images/delivery/pickpoint.png';
    protected static readonly houseIcon = '/Content/Images/delivery/house.png';

    protected static isApiComplete(key: string): Promise<void> {
        if (window.google?.maps?.Map == null) {
            const langCode = pxpGlobal.frontend.languageCode;
            var url = this._googleMapsUrl.replace("{0}", key).replace("{1}", langCode);
            return Url.loadScriptAsync(url);
        }
        return new Promise<void>((resolve) => { resolve(); });
    }
}

class GoogleMapPointsBinding extends GoogleMapBindingBase {
    private static readonly _linkToClusterImages: string = "https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m";
    
    static init(element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: PointsDeliveryPointsMap, bindingContext: unknown) {
        viewModel.isMapRendered(false);
        if (element == null || !(element instanceof HTMLElement)) return;

        this.isApiComplete(viewModel.authData.ApiKey ?? '').then(() => {
            this._render(element, viewModel);
        }).then(() => { 
            return viewModel.isMapRendered(true);
        });
    }

    private static _render(element: HTMLElement, viewModel: PointsDeliveryPointsMap) {
        var self = this;
        var points = viewModel.availablePoints();
        if (points.length == 0) return;

        const _geocoder = new google.maps.Geocoder();

        //var _map = this.getMapFromCache<google.maps.Map>(viewModel, DeliveryDisplayType.Pickpoint);
        //if (_map != null) {
        //    element.appendChild(_map.getDiv());
        //    return;
        //}

        this.toggleLoadingState(element);

        var _map = _createMap(element);
        var bounds = new google.maps.LatLngBounds();
        viewModel.callBubbleCallback = _callGeoObjOnMap;
        viewModel.refreshMap = _refreshMap;
        viewModel.setPointByPointData = _setPointByPointData;

        // подписываемся на изменение карты
        _map.addListener("bounds_changed", () => {
            var bounds = _map?.getBounds();
            if (bounds == null) return;

            var ne = bounds.getNorthEast();
            var sw = bounds.getSouthWest();

            var finalCoords = [[sw.lat(), sw.lng()], [ne.lat(), ne.lng()]];
            viewModel.currentMapVisibleCoordsBounds(finalCoords);
        });

        // создаем информационное окно
        const infowindow = new google.maps.InfoWindow();
        // по кнопке метод close не вызывается
        google.maps.event.addListener(infowindow,'closeclick',function(){
            viewModel.selectItemsOnList([]);
        });
        // тут мы переопределяем методы, чтобы во время открытия/закрытия 
        // поля информации творить свои штуки >:-)
        var _iwCloseOrig = infowindow.close;
        _iwCloseOrig = _iwCloseOrig.bind(infowindow);
        infowindow.close = (...args) => {
            let content = infowindow.getContent();
            if (content instanceof Node) {
                ko.cleanNode(content);
            }
            _iwCloseOrig(...args);
        }
        var _iwOpenOrig = infowindow.open;
        _iwOpenOrig = _iwOpenOrig.bind(infowindow);
        infowindow.open = (...args) => {
            let content = document.createRange().createContextualFragment(pickPointMapItemTemplate)?.firstElementChild;
            if (content == null) return;

            var currentMarker = (<PxpExtendedGoogleMarker>args[1]);
            var data = currentMarker.__getDataModel();
            var pxpDelId = currentMarker.__pxpDId;

            var bubbleEl: HTMLElement | null = document?.getElementById?.('delivery-view-map-container__bubble') ?? null;
            if (bubbleEl != null) {
                var bubbleContent = infowindow.getContent() as Node;
                if (bubbleContent != null) {
                    var bubbleModel = ko.contextFor(bubbleContent)?.$data?.data as PointDeliveryPointItem[];
                    if (bubbleModel != null) {
                        if (bubbleModel.filter((d) => { return d.addressData.Id === pxpDelId }).length > 0) {
                            
                            //var pos = currentMarker.getPosition();
                            //if (pos != null) infowindow?.setPosition(pos);
                            infowindow.close();
                            _iwOpenOrig(...args);
                            setTimeout(() => { _scrollToAddressInBubble(pxpDelId); }, 100);
                            
                            return;
                        }
                    }
                }
            }

            infowindow.close();
            infowindow.setContent(content);
            var finalModel = {
                data: data,
                selectPointFromMap: viewModel.selectPointFromMap
            }

            _iwOpenOrig(...args);

            ko.applyBindings(finalModel, content);
        }

        //const markers: google.maps.Marker[] = [];
        //const groups = this.getPointsGroups(points);
        //for (let gPoints of groups) {
        //    const marker = _getMarker(gPoints);
        //    markers.push(marker);
        //    gPoints.forEach((p: PointDeliveryPointItem) => {
        //        p.geoObjectOnMap = marker;
        //        //p.isVisibleByFilter.subscribe((v) => {
        //        //    // TODO
        //        //});
        //    });

        //    const markerBounds = marker.getPosition();
        //    if (markerBounds != null) {
        //        bounds.extend(markerBounds);
        //    }
        //}

        // кластер хранит город
        const _markers = _getMarkers(points);
        const clusterer = new MarkerClusterer({map: _map, markers: _markers});

        // ставим границы для карты
        _map.fitBounds(bounds);

        this.setMapToCache<google.maps.Map>(_map, viewModel, DeliveryDisplayType.Pickpoint);
        this.toggleLoadingState(element);

        function _getMarker(points: PointDeliveryPointItem[]): google.maps.Marker {
            const point = points[0];
            var marker = new window.google.maps.Marker({
                position: { lat: point.latitude ?? 0, lng: point.longitude ?? 0 },
                map: _map!,
                title: point.address,
                icon: GoogleMapBindingBase.pickpointIcon
            }) as PxpExtendedGoogleMarker;

            // методы расширения модели
            marker.__openInfoWindow = () => {
                infowindow.open(_map!, marker);
            };
            marker.__getDataModel = () => {
                return points;
            };

            // клик по самому маркеру
            marker.addListener("click", () => {
                viewModel.selectItemsOnList(points);
                marker.__openInfoWindow();
            });

            return marker;
        }

        function _createMap(blockEl: HTMLElement): google.maps.Map {
            var mapOptions: google.maps.MapOptions = {
                center: { lat: 0, lng: 0 },
                zoom: 15,
                minZoom: 8,
                maxZoom: 18,
                gestureHandling: 'greedy',
                disableDoubleClickZoom: true,
                fullscreenControl: false,
                keyboardShortcuts: false,
                mapTypeControl: false
            };

            return new window.google.maps.Map(blockEl, mapOptions);
        }

        function _callGeoObjOnMap(geoObj: MapMarkerType | null) {
            if (geoObj == null || !('__openInfoWindow' in geoObj)) return;
            const marker = geoObj as PxpExtendedGoogleMarker;
            
            var markerCoords = marker.getPosition();
            if (markerCoords == null) return;

            _map?.panTo(markerCoords);
            _map?.setZoom(GoogleMapBindingBase.constToZoomGMapOnSelect);
            marker.__openInfoWindow();
        }

        // крутим до адреса внутри бабла мапы
        function _scrollToAddressInBubble(itemId?: number) {
            if (itemId == null) return;

            var bubbleEl: HTMLElement | null = document?.getElementById?.('delivery-view-map-container__bubble')?.parentElement ?? null;
            if (bubbleEl == null) return;

            var el: HTMLElement | null = document.getElementById('bubble_address_' + itemId);
            if (el == null) return;

            bubbleEl.scrollTop = el.offsetTop - 20;
        }

        function _refreshMap() {
            _markers.forEach(marker => {
                //@ts-ignore
                const points: PointDeliveryPointItem[] = marker.__getDataModel();
                if (points.some(p => p.isVisibleByFilter)) {
                    marker.setVisible(true);
                    return;
                };
                marker.setVisible(false);
            });
            
        }


        function _getMarkers(points: PointDeliveryPointItem []): google.maps.Marker[] {
            let markers: google.maps.Marker[] = [];
            let groups = self.getPointsGroups(points);
            for (let gPoints of groups) {
                const marker = _getMarker(gPoints);
                markers.push(marker);
                gPoints.forEach((p: PointDeliveryPointItem) => {
                    p.geoObjectOnMap = marker;
                });

                const markerBounds = marker.getPosition();
                if (markerBounds != null) {
                    bounds.extend(markerBounds);
                }
            }
            return markers;
        }

        var _currentMarker: google.maps.Marker | null = null;
        function _setPointByPointData(data: IBoundedPointData) {
            
            if (data.lat == null || data.lon == null)
                return;
            // удаляем прошлый маркер
            _currentMarker?.setMap(null);
            
            var curentMarkerCoords = new google.maps.LatLng({lat: data.lat, lng: data.lon});

            _map.setCenter(curentMarkerCoords);
            if (data.bounds != null)
                _map.fitBounds({east: data.bounds.upLon, north: data.bounds.upLat, south: data.bounds.lowLat, west: data.bounds.lowLon});
            else
                _map.setZoom(GoogleMapBindingBase.constToZoomYMapCenterSuggest);

            if (data.house == null || data.house == '')
                return;

            _currentMarker = new window.google.maps.Marker({
                position: curentMarkerCoords,
                map: _map,
                icon: GoogleMapBindingBase.houseIcon
            });
        }
    }
}

class GoogleMapSelectedPointBinding extends GoogleMapBindingBase {
    static init(element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: AddressSelectDeliveryPointMap, _bindingContext: unknown) {
        viewModel.isMapRendered(false);
        if (element == null || !(element instanceof HTMLElement)) return;

        this.isApiComplete(viewModel.authData.ApiKey ?? '').then(() => {
            this._render(element, viewModel);
        }).then(() => { 
            return viewModel.isMapRendered(true);
        });
    }
    private static _render(element: HTMLElement, viewModel: AddressSelectDeliveryPointMap) {
        this.toggleLoadingState(element);

        var _currentMarker: google.maps.Marker | null = null;
        var city = viewModel.currentCity();
        var coords = { lat: city?.coords?.[0] ?? 0, lng: city?.coords?.[1] ?? 0 }
        var _map = new window.google.maps.Map(element, <google.maps.MapOptions>{
            center: coords,
            zoom: 15,
            gestureHandling: 'greedy',
            disableDoubleClickZoom: true,
            fullscreenControl: false,
            keyboardShortcuts: false,
            mapTypeControl: false
        });


        ///
        /// events
        ///
        google.maps.event.addListener(_map, "click", (event: any) => { 
            var coords = event.latLng as google.maps.LatLng;
            if (coords == null) return;

            viewModel.setNewPointCoords(<IPxpLatLng>{
                lat: coords.lat().toString(), 
                lng: coords.lng().toString()
            });
        });
        ///


        ///
        /// load map
        ///
        // нет города - в Москву
        if (city == null) {
            _map.panTo({ lat: 55.7522, lng: 37.6156 }); // Москва
        } else { // есть город - ставим карту по центру его координат
            _map.panTo({ lat: city.latitude, lng: city.longitude });
            _map.setZoom(12);
        }
        ///

        viewModel.createPointOnMap = _createPointOnMap;
        this.toggleLoadingState(element);


        ///
        /// funcs
        ///
        function _createPointOnMap(point: IPointData | null, forse: boolean = false): void {
            if (point == null) {
                _currentMarker?.setMap(null);
                return;
            }
            if (point.lat == null || point.lon == null || point.street === '' || point.house === '') return;
            console.log(point.lat + " " + point.lon + " [" + point.street + ", " + point.house + "]");

            // удаляем прошлый маркер
            _currentMarker?.setMap(null);
            
            var curentMarkerCoords = new google.maps.LatLng({ lat: point.lat, lng: point.lon });
            _currentMarker = new window.google.maps.Marker({
                position: curentMarkerCoords,
                map: _map,
                icon: GoogleMapBindingBase.houseIcon
            });

            if (_currentMarker == null) return;

            // если точка слишком далеко - центруем и зумим
            // если точка вне текущего ректангла - центруем
            var currentZoom = _map.getZoom();
            if (currentZoom! < 14) {
                var mcoords = _currentMarker.getPosition();
                if (mcoords != null) _map.setCenter(mcoords);
                _map.setZoom(GoogleMapBindingBase.constToZoomGMapOnSelect);
            } else {
                // текущие видимые границы карты
                var currentBounds = _map.getBounds();
                if (currentBounds != null && !currentBounds.contains(curentMarkerCoords)) {
                    _map.setCenter(curentMarkerCoords);
                }
            }
        }
        ///
    }

}


interface PxpExtendedGoogleMarker extends google.maps.Marker {
    __openInfoWindow: () => void;
    __getDataModel: () => PointDeliveryPointItem[];
    __pxpDId?: number;
}