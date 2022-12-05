import * as ymaps from "yandex-maps";
import { pxpGlobal } from "../../../globals/pxp";
import { Url } from "../../../utils/Url";
import { DeliveryDisplayType } from "../../controllers/DeliveryTypesSelector";
import pickPointMapItemTemplate from "../../_templates/deliveryTypes/pickpointMapData/pickPointMapItemTemplate.html";
import { IBoundedPointData, IPointData } from "../address/BaseAddressModel";
import { IAddressSuggest } from "../address/suggests/SuggestedStreetItem";
import { CityInfo, ToponimType } from "../city/CityInfo";
import { DeliveryUtils } from "../DeliveryUtils";
import { AddressSelectDeliveryPointMap } from "../types/standartDeliveryType/addressSelectDeliveryType/viewModels/AddressSelectDeliveryPointMap";
import { MapMarkerType, PointDeliveryPointItem } from "../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { PointsDeliveryPointsMap } from "../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap";
import { BaseMapManager, IPxpLatLng } from "./_baseMapManager";
import { AddressService } from "../../_services/AddressService"

export default () => {
    ko.bindingHandlers.ymapDeliveryPoints = {
        update: (element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: PointsDeliveryPointsMap, bindingContext: unknown) => {
            YandexMapPickPointsBinding.init(element, valueAccessor, allBindings, viewModel, bindingContext);
        }
    }

    ko.bindingHandlers.ymapCourierPoint = {
        update: (element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: AddressSelectDeliveryPointMap, bindingContext: unknown) => {
            YandexMapAddressBinding.init(element, valueAccessor, allBindings, viewModel, bindingContext);
        }
    }
}


///
/// Далее часто будет использоваться конвертация под (<any>...)
/// это сделано из-за отсутсвия должного количества полей в интерфейсах index.d.ts для ymaps
///
class YandexMapBindingBase extends BaseMapManager {

    private static readonly _yandexMapUrl: string = "https://api-maps.yandex.ru/2.1/?apikey={0}&lang=ru_RU";
    protected static readonly constToZoomYMapOnSelect: number = 17;
    protected static readonly constToZoomYMapCenterSuggest: number = 15;


    protected static isApiComplete(key: string): Promise<void> {
        if (window.ymaps?.Map == null) { 
            var url = this._yandexMapUrl.replace("{0}", key);
            return Url.loadScriptAsync(url).then(() => {
                return window.ymaps.ready();
            });
        }

        return new Promise<void>((resolve) => { resolve(); });
    }

    // преобразование гео точки яндекса в IPointData
    protected static getAddressDataFromGeoObj(geoObj: ymaps.IGeoObject | null): IPointData | null {
        if (geoObj == null) return null;

        // это объект с GeocoderMetaData
        var metaDataProperty: any = geoObj.properties.get('metaDataProperty', {});
        if (Object.keys(metaDataProperty).length === 0) return null;

        // это GeocoderMetaData
        var geocoderMetaData: any = metaDataProperty.GeocoderMetaData;
        if (geocoderMetaData == null) return null;
            
        // это Address
        var address: any = geocoderMetaData.Address;
        if (address?.Components == null) return null;

        var addressDict: { [kind: string]: string } = {};
        for (var component of address.Components) {
            addressDict[component.kind] = component.name;
        }
        if (Object.keys(addressDict).indexOf('house') === -1) return null;
            
        // 0: { kind: "country", name: "Россия" }
        // 1: { kind: "province", name: "Сибирский федеральный округ" }
        // 2: { kind: "province", name: "Томская область" }
        // 3: { kind: "area", name: "городской округ Томск" } // этого иногда нет, смиритесь
        // 4: { kind: "locality", name: "Томск" }
        // 5: { kind: "street", name: "улица Белинского" }
        // 6: { kind: "house", name: "18" }
        // 7: { kind: "entrance", name: "подъезд 1" }

        // @ts-ignore-start
        var coords = geoObj.geometry.getCoordinates();
        let descs: string[] = address.formatted.split(", ").reverse();
        // @ts-ignore-end
        let description = descs.length>1?descs[1]:"";
        descs.forEach((val, ind) => {if (ind>1) description=`${description}, ${val}`});
        var pointData: IPointData = {
            lat: coords?.[0] ?? null,
            lon: coords?.[1] ?? null,
            city: {
                country: addressDict['country']?? "",
                region: addressDict['province']?? "",
                title: addressDict['locality']?? ""
            },
            street: addressDict['street'],
            house: addressDict['house'],
            flat: null,
            postalCode: address.postal_code,
            description: null
        };
        return pointData;
    }
        ///
}

class YandexMapPickPointsBinding extends YandexMapBindingBase {

    static init(element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: PointsDeliveryPointsMap, bindingContext: unknown) {
        viewModel.isMapRendered(false);
        if (element == null || !(element instanceof HTMLElement)) return;

        this.isApiComplete(viewModel.authData.ApiKey ?? '').then(() => {
            this._render(element, viewModel);
        }).then(() => { 
            setTimeout(() => {
                return viewModel.isMapRendered(true);
            }, 200);
        });
    }


    private static _render(element: HTMLElement, viewModel: PointsDeliveryPointsMap) {

        var points = viewModel.availablePoints();
        if (points.length == 0) return;

        var _map = this.getMapFromCache<ymaps.Map>(viewModel, DeliveryDisplayType.Pickpoint);
        if (_map != null) {
            _map.balloon.close();
            element.appendChild(_map.container.getElement());            
            return;
        }

        this.toggleLoadingState(element);

        _map = _createMap(element, viewModel.currentCity());
        viewModel.callBubbleCallback = _callGeoObjOnMap;
        viewModel.setCenterByAddressName = _setCenterByAddressName;
        viewModel.setPointByPointData = _setPointByPointData;
        viewModel.refreshMap = _refreshMap;
        viewModel.resetMapView = _resetMapView;
        // подписываемся на изменение карты
        _map.events.add('actionend', () => {
            viewModel.currentMapVisibleCoordsBounds(_map?.getBounds() ?? null);
        });

                
        // кластер = город
        const clusterer = new window.ymaps.Clusterer(<any>{
            clusterBalloonItemContentLayout: _getBalloonContent(pickPointMapItemTemplate),
            clusterIconColor: "#64bb46",
        });

        const groups = this.getPointsGroups(points);
        const placemarks = groups.map(group => {
            const placemark = _getPoint(group);
            group.forEach(p => p.geoObjectOnMap = placemark);
            placemark.events.add('click', _handlePointClick, this);
            return placemark;
        });
        
        (<any>clusterer).add(placemarks);
        (<any>_map.geoObjects).add(clusterer);

        const bounds = _map.geoObjects.getBounds();
        if (bounds != null) {
            _map.setBounds(bounds, {
                zoomMargin: [40, 40],
                checkZoomRange: true,
            });
        }

        this.setMapToCache<ymaps.Map>(_map, viewModel, DeliveryDisplayType.Pickpoint);
        this.toggleLoadingState(element);

        function _getBalloonContent(template: string) {
            return window.ymaps.templateLayoutFactory.createClass(
                template,
                {
                    build: function() {
                        const self = this as unknown as ymaps.ILayout;
                        const parent = self.getParentElement();
                        // @ts-ignore
                        const template = self.getTemplate()._text;
                        if (parent != null) {
                            parent.innerHTML = "";
                            ko.cleanNode(parent);
                            parent.appendChild(document.createRange().createContextualFragment(template));
                        }
                        //@ts-ignore
                        const geoObj = self.getData().geoObject;
                        //@ts-ignore
                        const model: PointDeliveryPointItem[] = geoObj.options.get("model").filter(p => p.isSelectedOnList());
                        const finalModel = {
                            data: model,
                            selectPointFromMap: viewModel.selectPointFromMap,
                            resetMapView: viewModel.resetMapView
                        }

                        const closeButton = parent.closest('[class*="balloon__layout"]')?.querySelector('[class*="close-button"]');
                        //@ts-ignore
                        self.onCloseClick = self.onCloseClick.bind(self);
                        //@ts-ignore
                        closeButton?.addEventListener('click', self.onCloseClick)

                        ko.applyBindings(finalModel, parent);
                    },
                    clear: function() {
                        const self = this as unknown as ymaps.ILayout;
                        const parent = self.getParentElement();
                        ko.cleanNode(parent);
                    },
                    onCloseClick: function (e: any) {
                        // убираем подсветку с листа
                        //@ts-ignore
                        const geoObj = this.getData().geoObject;
                        const model: PointDeliveryPointItem[] = geoObj.options.get("model");
                        viewModel.deselectItemsOnList(model);
                    },
                }
            );
        }

        function _handlePointClick(e: ymaps.Event) {
            const geoObj = e.get('target');
            //@ts-ignore
            const points: PointDeliveryPointItem[] = geoObj.options.get("model");
            viewModel.selectItemsOnList(points);
        }

        function _getPoint(points: PointDeliveryPointItem[]): ymaps.GeoObject {
            const firstPoint = points[0];
            const geometry = {
                type: "Point",
                coordinates: [firstPoint.latitude, firstPoint.longitude],
            };
            const properties = {
                hintContent: firstPoint.address,
                //iconContent: `${viewModel.loc_from} ${_getMinPriceString(points)}`,
            };
            const geoOptions = {
                model: points,
                balloonContentLayout: _getBalloonContent(pickPointMapItemTemplate),
                preset: "islands#blueDeliveryIcon"
            }

            function _getMinPriceString(points: PointDeliveryPointItem[]) {
                const minPrice = Math.min.apply(null, points.map((pt) => { return pt.deliveryPrice() }));
                return pxpGlobal.utilities.getPriceString(minPrice);
            }
            return new window.ymaps.GeoObject({ geometry, properties }, geoOptions);
        }

        function _createMap(element: HTMLElement, city: CityInfo | null) {
            const mapOptions: ymaps.IMapOptions = {
                copyrightLogoVisible: false,
                copyrightProvidersVisible: false,
                copyrightUaVisible: false,
                suppressMapOpenBlock: true,
                yandexMapAutoSwitch: false,
                restrictMapArea: true,
                minZoom: 8,
                maxZoom: 20,
            };
            const mapState: ymaps.IMapState = {
                center: city?.coords ?? [0, 0],
                zoom: 8,
                controls: ["geolocationControl", "zoomControl"],
                type: "yandex#map"
            }
            return new window.ymaps.Map(element, mapState, mapOptions);
        }

        function _callGeoObjOnMap(geoObj: MapMarkerType | null) {
            if (geoObj == null || !('balloon' in geoObj)) return;

            var ballon = geoObj.balloon;
            if (ballon == null) return;
            ballon.close();

            _map?.panTo((<any>geoObj.geometry)?.getCoordinates() ?? [], { delay: 0, flying: false })
                .then(() => {
                    return _map?.setZoom(YandexMapBindingBase.constToZoomYMapOnSelect).then(() => {
                        return ballon.open();
                    })
                });
        }
        function _setPlacemarkVisible(geoObj: ymaps.GeoObject | null, value: boolean) {
            // т.к. set не принимает булы мы отдаем 0 и 1, их он хавает отлично
            geoObj?.options.set('visible', +value);
        }

        // крутим до адреса внутри бабла мапы
        function _scrollToAddressInBubble(itemId: number | null) {
            if (itemId == null) return;

            var bubbleEl: HTMLElement | null = document?.getElementById?.('delivery-view-map-container__bubble')?.parentElement ?? null;
            if (bubbleEl == null) return;

            var el: HTMLElement | null = document.getElementById('bubble_address_' + itemId);
            if (el == null) return;

            bubbleEl.scrollTop = el.offsetTop - 20;
        }

        function _setCenterByAddressName(addressName: string | null): void {
            if (addressName == null) return;
            //@ts-ignore
            window.ymaps.geocode(addressName).then((res) => {
                var point =  YandexMapBindingBase.getAddressDataFromGeoObj(res.geoObjects.get(0));
                if (point==null) return;
                console.log(point.lat + " " + point.lon + " [" + point.street + ", " + point.house + "]");
                if (point.lat==null || point.lon==null) return;
                _map?.setCenter([point.lat, point!.lon]);
                _map?.setZoom(YandexMapBindingBase.constToZoomYMapOnSelect);
            });       

        }

        var housePlasemark: ymaps.GeoObject | null = null;
        function _setPointByPointData(data: IBoundedPointData) {

            if (!data.lat || !data.lon)
                return;
            _map?.setCenter([data.lat, data.lon]);
            if (data.bounds !=null)
                _map?.setBounds([[data.bounds.lowLat, data.bounds.lowLon], [data.bounds.upLat, data.bounds.upLon]]);
            else
                _map?.setZoom(YandexMapBindingBase.constToZoomYMapCenterSuggest);
            if (housePlasemark != null)
                _map?.geoObjects.remove(housePlasemark);
            if (data.house == null || data.house == '')
                return;
            const point = _getPoint(data);
            _map?.geoObjects.add(point);
            housePlasemark = point;

            function _getPoint(data: IBoundedPointData) {
                const geometry = {
                    type: "Point",
                    coordinates: [data.lat, data.lon],
                };
                const properties = {
                    hintContent: data.description,
                    //iconContent: `${viewModel.loc_from} ${_getMinPriceString(points)}`,
                };
                const geoOptions = {
                    model: points,
                    preset: "islands#redHomeIcon"
                }          
                return new window.ymaps.GeoObject({ geometry, properties }, geoOptions);
            }
        }

        function _resetMapView(item: PointDeliveryPointItem | null) {
            if (item != null) {
                const baloon = (item.geoObjectOnMap as ymaps.GeoObject)?.balloon;
                if (baloon != null) {
                    if (baloon.isOpen()) baloon.close();
                }
            }
            if (bounds != null) {
                _map!.setBounds(bounds, {
                    zoomMargin: [40, 40],
                    checkZoomRange: true,
                });
            }
        }

        function _refreshMap() {
            clusterer.removeAll();
            //@ts-ignore
            clusterer.add(placemarks.filter(geoObj => geoObj.options.get("model").some(p => p.isVisibleByFilter)));

        }
    }

}

class YandexMapAddressBinding extends YandexMapBindingBase {
    static init(element: HTMLElement, valueAccessor: unknown, allBindings: unknown, viewModel: AddressSelectDeliveryPointMap, bindingContext: unknown) {
        viewModel.isMapRendered(false);
        if (element == null || !(element instanceof HTMLElement)) return;

        this.isApiComplete(viewModel.authData.ApiKey ?? '').then(() => {
            this._render(element, viewModel);
        }).then(() => { 
            setTimeout(() => {
                return viewModel.isMapRendered(true);
            }, 200);
        });
    }

    private static _render(element: HTMLElement, viewModel: AddressSelectDeliveryPointMap) {
        this.toggleLoadingState(element);

        var currentPlacemarker: ymaps.Placemark | null = null;
        var city = viewModel.currentCity();
        const mapState: ymaps.IMapState = {
            center: city?.coords ?? [0, 0],
            zoom: 10,
            controls: ["zoomControl"],
            type: "yandex#map",
        };
        const mapOptions: ymaps.IMapOptions = {
            copyrightLogoVisible: false,
            copyrightProvidersVisible: false,
            copyrightUaVisible: false,
            suppressMapOpenBlock: true,
            yandexMapAutoSwitch: false,
            yandexMapDisablePoiInteractivity: true,
            restrictMapArea: false,
            minZoom: 10,
            maxZoom: 20,
        };
        const _map = new window.ymaps.Map(element, mapState, mapOptions);

        window._map = _map;
        ///
        ///  events
        ///
        _map.events.add('click', (e) => {
            var coords = e.get('coords') as number[];     
            _setAddressByCoords(coords);
        });
        ///


        ///
        /// load map
        ///
        new Promise<void>((resolve) => {
            // нет города - в Москву
            if (city == null) {
                _map.setCenter([55.7522, 37.6156]).then(() => { resolve(); }); // Москва
            } else { // есть город - ставим карту по центру его координат
                Promise.all([
                    _map.setCenter([city.latitude, city.longitude]),
                    _map.setZoom(12)
                ]).then(() => { resolve(); })
            }
        }).then(() => {
            viewModel.createPointOnMap = _createPointOnMap;
            viewModel.setAddressByName = _setAddressByName;
            this.toggleLoadingState(element);
        });
        ///


        ///
        /// funcs
        ///
        function _createPointOnMap(point: IPointData | null, forse: boolean = false): void {
            if (point == null) {
                _map.geoObjects.removeAll();
                currentPlacemarker = null;
                return;
            }

            if (!forse && currentPlacemarker != null && point.lat != null && point.lon != null) {
                var currentPlacemarkerCoords = currentPlacemarker.geometry?.getCoordinates() ?? null;
                if (currentPlacemarkerCoords != null && currentPlacemarkerCoords[0] === point.lat && currentPlacemarkerCoords[1] === point.lon) {
                    return;
                }
            }

            // удаляем прошлый маркер
            _map.geoObjects.removeAll();

            if (point == null || point.lat == null || point.lon == null || (point.street === '' && point.house === '')) return;
            console.log(point.lat + " " + point.lon + " [" + point.street + ", " + point.house + "]");

            var placemarkCoords = [point.lat, point.lon];
            const geometry = {
                type: "Point",
                coordinates: placemarkCoords,
            };
            const properties = {
                iconCaption: point.street + ', ' + point.house
            };
            const geoOptions = {
                preset: "islands#blueHomeIcon",
            };
            currentPlacemarker = new window.ymaps.GeoObject({ geometry, properties }, geoOptions);

            (<any>_map.geoObjects).add(currentPlacemarker);

            // если точка слишком далеко - центруем и зумим
            // если точка вне текущего ректангла - центруем
            var currentZoom = _map.getZoom();
            if (currentZoom < 14) {
                _map.setCenter(placemarkCoords);
                _map.setZoom(YandexMapBindingBase.constToZoomYMapOnSelect);
            } else {
                // текущие видимые границы карты
                var currentMapBounds = _map.getBounds();
                if (!DeliveryUtils.isPointInBounds(currentMapBounds, placemarkCoords)) {
                    _map.setCenter(placemarkCoords);
                }
            }
        }

        //// передача точки большим контролам и отрисовка ее на карте
        //function _selectPoint(point: IPointData | null) {
        //    if (point == null) return;

        //    viewModel.setNewPoint(point);
        //    _createPointOnMap(point);
        //    window.document.getElementById('deliveryAddressFlat')?.focus();
        //    window.document.getElementById('deliveryAddressAddressLine2')?.focus();
        //}

        // поиск точки по названию
        function _setAddressByName(addressName: string | null): void {
            if (addressName == null) return;

            _getAddressByName(addressName)?.then((result) => {
                var pointData = YandexMapBindingBase.getAddressDataFromGeoObj(result);
                viewModel.setNewPoint(pointData);
            });  
        }
        function _getAddressByName(addressName: string | null): Promise<ymaps.GeoObject | null> | null {
            if (addressName == null) return null;
            
            //@ts-ignore
            return window.ymaps.geocode(addressName).then((res) => {
                return res.geoObjects.get(0);
            });
        }

        // поиск точки по координатам
        function _setAddressByCoords(coords: number[] | null): void {
            if (coords == null || coords.length != 2) return;
               
            AddressService.findStreetAddressDataByCoords({lat: coords[0].toString(), lng: coords[1].toString()})
                .then(pointData => {
                    viewModel.setNewPoint(pointData);
                });
        }
    }
}